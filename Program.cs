using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using WebBanHang.Data;
using WebBanHang.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Email =====
var smtpServer     = builder.Configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
var smtpPortStr    = builder.Configuration["EmailSettings:SmtpPort"];
var senderEmail    = builder.Configuration["EmailSettings:SenderEmail"] ?? "";
var senderPassword = builder.Configuration["EmailSettings:SenderPassword"] ?? "";
builder.Services.AddSingleton(new EmailService(
    smtpServer,
    int.TryParse(smtpPortStr, out var smtpPort) ? smtpPort : 587,
    senderEmail,
    senderPassword
));

// ===== Session =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// ===== App services =====
builder.Services.AddScoped<VnPayService>();
builder.Services.AddScoped<CartService>();

// ===== Connection string: ưu tiên ENV (Render), fallback appsettings =====
var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException(
        "Missing DB connection string. Set ENV 'ConnectionStrings__DefaultConnection' or appsettings.json."
    );

// Log ngắn gọn (ẩn pass)
var csLog = connectionString.Length > 80 ? connectionString[..80] + "..." : connectionString;
Console.WriteLine($"[DEBUG] Connection string detected: {csLog}");

// ===== Chọn provider: cho phép ép bằng ENV DB_PROVIDER=postgres =====
var provider = Environment.GetEnvironmentVariable("DB_PROVIDER");
var usePostgres =
    (provider?.Equals("postgres", StringComparison.OrdinalIgnoreCase) ?? false) ||
    connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase) ||
    connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase);

builder.Services.AddDbContext<ApplicationDbContext>(opt => {
    if (usePostgres)
    {
        Console.WriteLine("[DEBUG] Using PostgreSQL provider");
        opt.UseNpgsql(connectionString);
    }
    else
    {
        Console.WriteLine("[DEBUG] Using SQL Server provider");
        opt.UseSqlServer(connectionString);
    }
    // Suppress warning về pending model changes để migrations có thể chạy
    opt.ConfigureWarnings(warnings => 
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Cấu hình để background service không làm crash app khi có lỗi
builder.Services.Configure<Microsoft.Extensions.Hosting.HostOptions>(opts =>
{
    opts.BackgroundServiceExceptionBehavior = Microsoft.Extensions.Hosting.BackgroundServiceExceptionBehavior.Ignore;
});

// Tạm thời disable background service - sẽ enable lại sau khi migrations chạy xong
// XÓA HOÀN TOÀN để đảm bảo service không chạy
// builder.Services.AddHostedService<AutoVoucherService>();
Console.WriteLine("[CONFIG] AutoVoucherService is DISABLED - will enable after migrations complete");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Không ép HTTPS redirect ở Production vì Render tự terminate TLS
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ===== Migrate DB với CHÍNH connectionString & provider ở trên =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Kiểm tra xem có pending migrations không
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();
        if (pendingMigrations.Any())
        {
            Console.WriteLine($"[MIGRATION] Applying {pendingMigrations.Count} pending migrations...");
            try
            {
                db.Database.Migrate();
                Console.WriteLine("[MIGRATION] Migrations applied successfully");
            }
            catch (Exception migrateEx)
            {
                // Nếu migrate fail do pending changes, thử tạo database và tables thủ công
                Console.WriteLine($"[MIGRATION] Migrate failed: {migrateEx.Message}");
                Console.WriteLine("[MIGRATION] Attempting to ensure database exists...");
                try
                {
                    db.Database.EnsureCreated();
                    Console.WriteLine("[MIGRATION] Database ensured (tables may need manual creation)");
                }
                catch (Exception ensureEx)
                {
                    Console.WriteLine($"[MIGRATION] EnsureCreated also failed: {ensureEx.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("[MIGRATION] No pending migrations");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
        // Chỉ log lỗi và tiếp tục - app vẫn có thể chạy nếu tables đã tồn tại
    }

    // Seed admin
    try
    {
        var existingAdmin = db.Users.FirstOrDefault(u => u.Username == "admin");
        if (existingAdmin == null)
        {
            db.Users.Add(new WebBanHang.Models.User {
                Username = "admin",
                Password = "123456",
                Role = "Admin",
                Gender = "Admin",
                PhoneNumber = "0123456789",
                Address = "Trụ sở chính"
            });
            db.SaveChanges();
        }
        else
        {
            existingAdmin.Password = "123456";
            existingAdmin.Role = "Admin";
            db.Users.Update(existingAdmin);
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing admin user: {ex.Message}");
    }
}

app.Run();
