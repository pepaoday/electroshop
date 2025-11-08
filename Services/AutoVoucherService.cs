using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebBanHang.Data;
using WebBanHang.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace WebBanHang.Services
{
    public class AutoVoucherService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AutoVoucherService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Đợi một chút để đảm bảo migrations đã chạy xong
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        
                        // Kiểm tra xem database đã sẵn sàng chưa
                        if (!await IsDatabaseReadyAsync(context))
                        {
                            Console.WriteLine("[AutoVoucherService] Database not ready yet, waiting...");
                            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            continue;
                        }

                        await AssignVouchersToEligibleUsers(context);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AutoVoucherService] Error: {ex.Message}");
                    // Nếu có lỗi, đợi 5 phút trước khi thử lại
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                    continue;
                }

                // Chờ 1 giờ rồi chạy lại
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task<bool> IsDatabaseReadyAsync(ApplicationDbContext context)
        {
            try
            {
                // Thử query một bảng đơn giản để kiểm tra database đã sẵn sàng
                await context.Database.ExecuteSqlRawAsync("SELECT 1");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task AssignVouchersToEligibleUsers(ApplicationDbContext context)
        {
            try
            {
                const int pointThreshold = 500; // Ngưỡng điểm để nhận voucher
                const string templateCode = "REWARD_50K"; // Mã của voucher mẫu

                // Kiểm tra xem bảng Vouchers có tồn tại không
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SELECT 1 FROM \"Vouchers\" LIMIT 1");
                }
                catch (PostgresException pgEx) when (pgEx.SqlState == "42P01") // Table does not exist
                {
                    Console.WriteLine("[AutoVoucherService] Vouchers table does not exist yet, skipping...");
                    return;
                }
                catch (Exception ex)
                {
                    // Kiểm tra nếu là lỗi table không tồn tại (cho SQL Server)
                    if (ex.Message.Contains("does not exist") || ex.Message.Contains("Invalid object name"))
                    {
                        Console.WriteLine("[AutoVoucherService] Vouchers table does not exist yet, skipping...");
                        return;
                    }
                    throw;
                }

                // Tìm voucher mẫu - wrap trong try-catch để handle table not found
                Voucher? voucherTemplate = null;
                try
                {
                    voucherTemplate = await context.Vouchers.FirstOrDefaultAsync(v => v.IsTemplate && v.Code == templateCode);
                }
                catch (PostgresException pgEx) when (pgEx.SqlState == "42P01")
                {
                    Console.WriteLine("[AutoVoucherService] Vouchers table does not exist yet, skipping...");
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("does not exist") || ex.Message.Contains("Invalid object name"))
                    {
                        Console.WriteLine("[AutoVoucherService] Vouchers table does not exist yet, skipping...");
                        return;
                    }
                    throw;
                }

                if (voucherTemplate == null)
                {
                    return;
                }

                // Tìm tất cả user đủ điểm - wrap trong try-catch
                List<User> eligibleUsers;
                try
                {
                    eligibleUsers = await context.Users.Where(u => u.Points >= pointThreshold).ToListAsync();
                }
                catch (PostgresException pgEx) when (pgEx.SqlState == "42P01")
                {
                    Console.WriteLine("[AutoVoucherService] Users table does not exist yet, skipping...");
                    return;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("does not exist") || ex.Message.Contains("Invalid object name"))
                    {
                        Console.WriteLine("[AutoVoucherService] Users table does not exist yet, skipping...");
                        return;
                    }
                    throw;
                }

                foreach (var user in eligibleUsers)
                {
                    // Tạo một voucher mới từ mẫu
                    var newVoucher = new Voucher
                    {
                        Code = $"REWARD_{Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8)}",
                        DiscountType = voucherTemplate.DiscountType,
                        DiscountValue = voucherTemplate.DiscountValue,
                        MinAmount = voucherTemplate.MinAmount,
                        ExpiryDate = DateTime.Now.AddDays(30), // Hạn dùng 30 ngày
                        IsActive = true,
                        IsTemplate = false,
                        UserId = user.Id
                    };

                    context.Vouchers.Add(newVoucher);
                    var notification = new Notification
                    {
                        UserId = user.Id,
                        Message = $"Chúc mừng! Bạn nhận được voucher thưởng do tích đủ {pointThreshold} điểm.",
                        Url = "/Account/MyVouchers"
                    };
                    context.Notifications.Add(notification);

                    // Trừ điểm của user
                    user.Points -= pointThreshold;
                    context.Users.Update(user);
                }

                if (eligibleUsers.Any())
                {
                    await context.SaveChangesAsync();
                }
            }
            catch (PostgresException pgEx) when (pgEx.SqlState == "42P01") // Table does not exist
            {
                Console.WriteLine("[AutoVoucherService] Database table does not exist yet, will retry later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AutoVoucherService] Error in AssignVouchersToEligibleUsers: {ex.Message}");
                // Không throw để service tiếp tục chạy
            }
        }
    }
}