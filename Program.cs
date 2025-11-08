using Microsoft.EntityFrameworkCore;
using WebBanHang.Data;
using WebBanHang.Helpers;
using WebBanHang.Services;
using WebBanHang.Models;
using System.Linq;
using System.Security.Cryptography; 
using System.Text;                
using System.Net.Mail;
using System.Net;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ EmailService
var smtpServer = builder.Configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
var smtpPort = builder.Configuration["EmailSettings:SmtpPort"];
var senderEmail = builder.Configuration["EmailSettings:SenderEmail"] ?? "";
var senderPassword = builder.Configuration["EmailSettings:SenderPassword"] ?? "";

builder.Services.AddSingleton(new EmailService(
    smtpServer,
    int.TryParse(smtpPort, out var port) ? port : 587,
    senderEmail,
    senderPassword
));

// Thêm dịch vụ cho Session và HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<CartService>();

// Đảm bảo rằng AddDbContext được gọi và cấu hình đúng
// Hỗ trợ cả SQL Server và PostgreSQL (cho Render.com)
// Ưu tiên đọc từ environment variable (cho Render), sau đó mới từ appsettings
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' is not configured. " +
        "Please set the 'ConnectionStrings__DefaultConnection' environment variable or " +
        "configure it in appsettings.json");
}

// Log connection string (ẩn password) để debug
var connectionStringForLog = connectionString.Length > 50 
    ? connectionString.Substring(0, 50) + "..." 
    : connectionString;
Console.WriteLine($"[DEBUG] Connection string detected: {connectionStringForLog}");

// Kiểm tra nhiều pattern để detect PostgreSQL - ưu tiên check Host= trước
// Render PostgreSQL thường dùng format: Host=dpg-xxx;Port=5432;Database=xxx;...
bool isPostgreSQL = connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) || // PostgreSQL key-value format
                    connectionString.Contains("postgresql://", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("npgsql", StringComparison.OrdinalIgnoreCase) ||
                    connectionString.Contains("dpg-", StringComparison.OrdinalIgnoreCase) || // Render PostgreSQL pattern
                    connectionString.Contains(".postgres.render.com", StringComparison.OrdinalIgnoreCase);

if (isPostgreSQL)
{
    // Sử dụng PostgreSQL (cho Render.com)
    Console.WriteLine("[DEBUG] Using PostgreSQL database provider");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Sử dụng SQL Server (mặc định)
    Console.WriteLine("[DEBUG] Using SQL Server database provider");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Thêm dịch vụ cho Controller và View (MVC) và Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); 

//dịch vụ cho AutoVoucherService
builder.Services.AddHostedService<AutoVoucherService>();



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Tắt HTTPS redirection trong production vì Cloud Run và Render tự xử lý HTTPS
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();


app.UseSession();

app.UseAuthorization();


app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Chỉ khởi tạo database nếu connection string đã được cấu hình
var connectionStringForInit = app.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionStringForInit))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        // Đảm bảo database đã được tạo và migrations đã chạy
        try
        {
            // Sử dụng Migrate() thay vì EnsureCreated() để hỗ trợ migrations
            dbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            // Nếu migrations thất bại, thử EnsureCreated() (fallback)
            Console.WriteLine($"Migration error: {ex.Message}. Trying EnsureCreated()...");
            try
            {
                dbContext.Database.EnsureCreated();
            }
            catch (Exception ex2)
            {
                Console.WriteLine($"EnsureCreated error: {ex2.Message}");
                // Không throw exception ở đây để app vẫn có thể start
                // nhưng sẽ fail khi có request đến database
            }
        }

        // Đồng bộ tài khoản admin
        try
        {
            var existingAdmin = dbContext.Users.FirstOrDefault(u => u.Username == "admin");
            if (existingAdmin == null)
            {
                dbContext.Users.Add(new User
                {
                    Username = "admin",
                    Password = "123456",
                    Role = "Admin",
                    Gender = "Admin",
                    PhoneNumber = "0123456789",
                    Address = "Trụ sở chính"
                });
                dbContext.SaveChanges();
            }
            else
            {
                existingAdmin.Password = "123456"; // đảm bảo mật khẩu plain để khớp logic Login
                existingAdmin.Role = "Admin"; // đảm bảo đúng vai trò
                dbContext.Users.Update(existingAdmin);
                dbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing admin user: {ex.Message}");
            // Không throw exception ở đây để app vẫn có thể start
        }
    }
}
else
{
    Console.WriteLine("WARNING: Connection string is not configured. Database operations will fail.");
}

app.Run();