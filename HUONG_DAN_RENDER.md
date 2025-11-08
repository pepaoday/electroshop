# 🚀 Hướng Dẫn Deploy ElectroShop lên Render.com (FREE)

Render.com cung cấp free tier cho web services và PostgreSQL database - hoàn toàn miễn phí!

## 📋 Mục Lục
1. [Giới thiệu Render.com](#giới-thiệu-rendercom)
2. [Chuẩn bị](#chuẩn-bị)
3. [Bước 1: Tạo tài khoản Render.com](#bước-1-tạo-tài-khoản-rendercom)
4. [Bước 2: Setup PostgreSQL Database (Free)](#bước-2-setup-postgresql-database-free)
5. [Bước 3: Migrate sang PostgreSQL (Nếu cần)](#bước-3-migrate-sang-postgresql-nếu-cần)
6. [Bước 4: Deploy Website lên Render](#bước-4-deploy-website-lên-render)
7. [Bước 5: Cấu hình Environment Variables](#bước-5-cấu-hình-environment-variables)
8. [Bước 6: Kiểm tra và Test](#bước-6-kiểm-tra-và-test)
9. [Troubleshooting](#troubleshooting)

---

## 🎯 Giới thiệu Render.com

### Ưu điểm:
- ✅ **Free tier** cho Web Services
- ✅ **Free PostgreSQL** database
- ✅ **Auto-deploy** từ Git (GitHub, GitLab, Bitbucket)
- ✅ **HTTPS** tự động
- ✅ **Custom domain** miễn phí
- ✅ Không cần credit card để bắt đầu

### Lưu ý:
- ⚠️ Free tier sẽ **sleep sau 15 phút** không có traffic (wake up khi có request)
- ⚠️ Cần migrate từ SQL Server sang PostgreSQL (Render không hỗ trợ SQL Server free)

---

## 📦 Chuẩn bị

### Yêu cầu:
- ✅ Tài khoản GitHub/GitLab/Bitbucket
- ✅ Code đã push lên Git repository
- ✅ Tài khoản Render.com (sẽ tạo ở bước 1)

### Thời gian ước tính:
- **Lần đầu:** 20-30 phút
- **Lần sau:** 5-10 phút (auto-deploy)

---

## Bước 1: Tạo tài khoản Render.com

### 1.1. Đăng ký tài khoản

1. Vào: **https://render.com/**
2. Click **"Get Started for Free"** hoặc **"Sign Up"**
3. Chọn đăng ký bằng:
   - **GitHub** (khuyến nghị - dễ nhất)
   - **GitLab**
   - **Bitbucket**
   - **Email**

### 1.2. Xác thực tài khoản

1. Nếu dùng GitHub/GitLab/Bitbucket, cho phép Render truy cập repositories
2. Nếu dùng Email, kiểm tra email để xác thực

**✅ Hoàn thành Bước 1!**

---

## Bước 2: Setup PostgreSQL Database (Free)

### 2.1. Tạo PostgreSQL Database

1. Vào Dashboard Render: **https://dashboard.render.com/**
2. Click **"New +"** ở góc trên bên trái
3. Chọn **"PostgreSQL"**
4. Điền thông tin:
   - **Name:** `electroshop-db` (hoặc tên bạn muốn)
   - **Database:** `DoAnWebNCDB` (hoặc để mặc định)
   - **User:** `electroshop_user` (hoặc để mặc định)
   - **Region:** Chọn region gần nhất (ví dụ: Singapore)
   - **PostgreSQL Version:** Chọn version mới nhất
   - **Plan:** Chọn **"Free"** (Free tier)
5. Click **"Create Database"**

### 2.2. Lưu thông tin kết nối

Sau khi database được tạo:

1. Vào trang database vừa tạo
2. Tìm phần **"Connection"** hoặc **"Connections"**
3. **Lưu lại các thông tin sau:**
   - **Internal Database URL:** (sẽ dùng cho app)
   - **External Database URL:** (để kết nối từ local nếu cần)
   - **Host:** 
   - **Port:** 
   - **Database:** 
   - **User:** 
   - **Password:** (Render sẽ tự tạo)

**Ví dụ Internal Database URL:**
```
postgresql://electroshop_user:password123@dpg-xxxxx-a.singapore-postgres.render.com/doanwebncdb
```

**✅ Hoàn thành Bước 2!**

---

## Bước 3: Migrate sang PostgreSQL (Nếu cần)

Ứng dụng hiện tại đang dùng SQL Server. Để deploy lên Render (free), cần migrate sang PostgreSQL.

### 3.1. Thêm PostgreSQL Package

Mở file `DoAnWebNC.csproj` và thêm package:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.7" />
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.7" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.7">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
</ItemGroup>
```

### 3.2. Cập nhật Program.cs

Mở file `Program.cs` và cập nhật để hỗ trợ cả SQL Server và PostgreSQL:

```csharp
// Thay dòng này:
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thành:
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString.Contains("PostgreSQL") || connectionString.Contains("postgresql") || connectionString.StartsWith("postgresql://"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
```

Và thêm using:
```csharp
using Npgsql.EntityFrameworkCore.PostgreSQL;
```

### 3.3. Tạo Migration cho PostgreSQL

```bash
# Xóa migrations cũ (nếu cần)
# dotnet ef migrations remove

# Tạo migration mới cho PostgreSQL
dotnet ef migrations add InitialPostgreSQL --context ApplicationDbContext
```

### 3.4. Cập nhật appsettings.json

Thêm connection string cho PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DoAnWebNCDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSQLConnection": "Host=localhost;Database=DoAnWebNCDB;Username=postgres;Password=your_password"
  }
}
```

**✅ Hoàn thành Bước 3!**

---

## Bước 4: Deploy Website lên Render

### 4.1. Push code lên Git (Nếu chưa)

1. Đảm bảo code đã được push lên GitHub/GitLab/Bitbucket
2. Commit các thay đổi:
```bash
git add .
git commit -m "Prepare for Render deployment"
git push
```

### 4.2. Tạo Web Service trên Render

1. Vào Dashboard Render: **https://dashboard.render.com/**
2. Click **"New +"** > **"Web Service"**
3. Chọn repository của bạn (GitHub/GitLab/Bitbucket)
4. Điền thông tin:
   - **Name:** `electroshop` (hoặc tên bạn muốn)
   - **Region:** Chọn region gần nhất
   - **Branch:** `main` (hoặc branch bạn muốn deploy)
   - **Root Directory:** (Để trống nếu code ở root)
   - **Runtime:** `Docker` (Render sẽ tự detect Dockerfile)
   - **Instance Type:** Chọn **"Free"**
   - **Auto-Deploy:** `Yes` (tự động deploy khi có thay đổi)
5. Click **"Create Web Service"**

### 4.3. Cấu hình Build Settings

Render sẽ tự động detect Dockerfile. Nếu không, cấu hình thủ công:

- **Build Command:** (Để trống - Docker sẽ build)
- **Start Command:** (Để trống - Docker sẽ chạy)

**✅ Hoàn thành Bước 4!**

---

## Bước 5: Cấu hình Environment Variables

### 5.1. Thêm Environment Variables

Trong trang Web Service, vào tab **"Environment"**:

1. Click **"Add Environment Variable"**
2. Thêm các biến sau:

#### Database Connection:
```
Key: ConnectionStrings__DefaultConnection
Value: [Internal Database URL từ Bước 2.2]
```

**Ví dụ:**
```
postgresql://electroshop_user:password123@dpg-xxxxx-a.singapore-postgres.render.com/doanwebncdb
```

#### ASP.NET Core Environment:
```
Key: ASPNETCORE_ENVIRONMENT
Value: Production
```

#### VnPay Settings:
```
Key: VnPay__TmnCode
Value: SJBLAJF0

Key: VnPay__HashSecret
Value: 3BY72RWVVTO43M9JEYSHVG9KHA1MA5TU

Key: VnPay__BaseUrl
Value: https://sandbox.vnpayment.vn/paymentv2/vpcpay.html

Key: VnPay__ReturnUrl
Value: https://your-app-name.onrender.com/Order/PaymentCallBack
```

**Lưu ý:** Thay `your-app-name.onrender.com` bằng URL thực tế của bạn (sẽ có sau khi deploy)

#### Email Settings:
```
Key: EmailSettings__SmtpServer
Value: smtp.gmail.com

Key: EmailSettings__SmtpPort
Value: 587

Key: EmailSettings__SenderEmail
Value: nguyenminh01060210@gmail.com

Key: EmailSettings__SenderPassword
Value: dseh xfyl eplj uuxg
```

### 5.2. Lưu và Deploy

1. Click **"Save Changes"**
2. Render sẽ tự động rebuild và deploy

**✅ Hoàn thành Bước 5!**

---

## Bước 6: Kiểm tra và Test

### 6.1. Xem Logs

1. Vào trang Web Service
2. Click tab **"Logs"**
3. Xem logs để kiểm tra có lỗi không

### 6.2. Truy cập Website

1. Sau khi deploy xong, bạn sẽ thấy URL:
   ```
   https://your-app-name.onrender.com
   ```
2. Truy cập URL này trong trình duyệt

### 6.3. Chạy Migrations

Render không tự động chạy migrations. Bạn cần:

**Cách 1: Chạy migrations trong code (Khuyến nghị)**

Trong `Program.cs`, đã có `dbContext.Database.EnsureCreated()`, nhưng tốt hơn là dùng migrations:

```csharp
// Thay EnsureCreated() bằng:
dbContext.Database.Migrate();
```

**Cách 2: Chạy migrations thủ công**

1. Kết nối với database từ local
2. Chạy: `dotnet ef database update`

### 6.4. Test các chức năng

- [ ] Trang chủ load được
- [ ] Đăng ký/Đăng nhập hoạt động
- [ ] Xem sản phẩm được
- [ ] Thêm vào giỏ hàng được
- [ ] Thanh toán hoạt động

**✅ Hoàn thành Bước 6!**

---

## 🔧 Troubleshooting

### Lỗi: "Database connection failed"
**Giải pháp:**
- Kiểm tra Internal Database URL đúng chưa
- Kiểm tra database đã được tạo chưa
- Kiểm tra environment variable `ConnectionStrings__DefaultConnection`

### Lỗi: "Build failed"
**Giải pháp:**
- Kiểm tra Dockerfile có đúng không
- Kiểm tra logs để xem lỗi cụ thể
- Đảm bảo tất cả dependencies đã được cài đặt

### Lỗi: "Application error"
**Giải pháp:**
- Xem logs trong Render dashboard
- Kiểm tra environment variables đã được set đúng chưa
- Kiểm tra database migrations đã chạy chưa

### Website sleep (Free tier)
**Giải pháp:**
- Free tier sẽ sleep sau 15 phút không có traffic
- Request đầu tiên sau khi sleep sẽ mất vài giây để wake up
- Nếu cần không sleep, upgrade lên paid plan

### Lỗi: "Migration failed"
**Giải pháp:**
- Đảm bảo đã chạy migrations trước khi deploy
- Hoặc sử dụng `Database.Migrate()` trong code
- Kiểm tra connection string đúng chưa

---

## 📝 Lưu ý quan trọng

### Free Tier Limitations:
- ⚠️ **Sleep sau 15 phút:** Website sẽ sleep nếu không có traffic
- ⚠️ **Wake up time:** Request đầu tiên sau khi sleep sẽ mất vài giây
- ⚠️ **Resource limits:** CPU và RAM có giới hạn
- ⚠️ **Bandwidth:** Có giới hạn bandwidth

### Database:
- ✅ PostgreSQL free tier có 1GB storage
- ✅ Có thể upgrade lên paid plan nếu cần
- ✅ Backup tự động (với paid plan)

### Custom Domain:
- ✅ Có thể thêm custom domain miễn phí
- ✅ HTTPS tự động với Let's Encrypt

---

## 🎉 Hoàn Thành!

Website của bạn đã được deploy lên Render.com!

### URL Website:
```
https://your-app-name.onrender.com
```

### Các bước tiếp theo:
1. ✅ Deploy website (Đã xong)
2. ⏭️ Đăng ký Google Search Console (Xem `CHECKLIST_NHANH.md`)
3. ⏭️ Submit sitemap (Xem `CHECKLIST_NHANH.md`)
4. ⏭️ Request indexing (Xem `CHECKLIST_NHANH.md`)

---

## 📚 Tài liệu tham khảo

- Render.com Docs: https://render.com/docs
- PostgreSQL on Render: https://render.com/docs/databases
- .NET on Render: https://render.com/docs/deploy-aspnet-core

---

**Chúc bạn deploy thành công! 🚀**

Nếu gặp vấn đề, xem phần Troubleshooting hoặc hỏi trong Render community.

