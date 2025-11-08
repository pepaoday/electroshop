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
builder.Services.AddSingleton(new EmailService(
    builder.Configuration["EmailSettings:SmtpServer"],
    int.Parse(builder.Configuration["EmailSettings:SmtpPort"]),
    builder.Configuration["EmailSettings:SenderEmail"],
    builder.Configuration["EmailSettings:SenderPassword"]
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
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString) && 
    (connectionString.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase) || 
     connectionString.Contains("postgresql://", StringComparison.OrdinalIgnoreCase) ||
     connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
{
    // Sử dụng PostgreSQL (cho Render.com)
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Sử dụng SQL Server (mặc định)
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
        }
    }

    // Đồng bộ tài khoản admin
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

app.Run();