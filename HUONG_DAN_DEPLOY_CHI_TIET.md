# 🚀 Hướng Dẫn Deploy Chi Tiết Từng Bước - ElectroShop lên Google Cloud Run

## 📋 Mục Lục
1. [Chuẩn bị](#chuẩn-bị)
2. [Bước 1: Tạo Project Google Cloud](#bước-1-tạo-project-google-cloud)
3. [Bước 2: Cài đặt Google Cloud SDK](#bước-2-cài-đặt-google-cloud-sdk)
4. [Bước 3: Setup Database (Cloud SQL)](#bước-3-setup-database-cloud-sql)
5. [Bước 4: Cấu hình appsettings.Production.json](#bước-4-cấu-hình-appsettingsproductionjson)
6. [Bước 5: Deploy Website](#bước-5-deploy-website)
7. [Bước 6: Kiểm tra và Test](#bước-6-kiểm-tra-và-test)
8. [Troubleshooting](#troubleshooting)

---

## 📦 Chuẩn bị

### Yêu cầu:
- ✅ Tài khoản Google (Gmail)
- ✅ Thẻ tín dụng (để kích hoạt Google Cloud - có free trial $300)
- ✅ Máy tính Windows/Mac/Linux
- ✅ Kết nối Internet

### Thời gian ước tính:
- **Lần đầu:** 30-60 phút
- **Lần sau:** 10-15 phút

---

## Bước 1: Tạo Project Google Cloud

### 1.1. Truy cập Google Cloud Console

1. Mở trình duyệt và vào: **https://console.cloud.google.com/**
2. Đăng nhập bằng tài khoản Google của bạn
3. Nếu lần đầu tiên, Google sẽ yêu cầu đồng ý với điều khoản → Click **"Đồng ý"**

### 1.2. Kích hoạt Free Trial (Nếu chưa)

1. Google sẽ hỏi có muốn dùng thử miễn phí không
2. Click **"Dùng thử miễn phí"** hoặc **"Free Trial"**
3. Chọn tài khoản thanh toán (có thể dùng thẻ tín dụng - Google sẽ không tính phí trong 90 ngày đầu, và bạn có $300 credit miễn phí)
4. Điền thông tin thanh toán (Google sẽ không charge nếu bạn không vượt quá free tier)
5. Click **"Bắt đầu dùng thử miễn phí"**

### 1.3. Tạo Project Mới

1. Ở góc trên cùng bên trái, click vào dropdown project (có thể hiển thị "My First Project" hoặc tên project khác)
2. Click **"New Project"** hoặc **"Tạo dự án"**
3. Điền thông tin:
   - **Project name:** `ElectroShop` (hoặc tên bạn muốn)
   - **Organization:** Để mặc định (nếu có)
   - **Location:** Để mặc định
4. Click **"Create"** hoặc **"Tạo"**
5. Đợi vài giây để project được tạo
6. Chọn project vừa tạo (click vào dropdown project và chọn project mới)

### 1.4. Lưu Project ID

1. Click vào dropdown project một lần nữa
2. Bạn sẽ thấy **Project ID** (ví dụ: `electroshop-123456`)
3. **Lưu lại Project ID này** - bạn sẽ cần dùng sau

**✅ Hoàn thành Bước 1!**

---

## Bước 2: Cài đặt Google Cloud SDK

### 2.1. Tải Google Cloud SDK

**Windows:**
1. Vào: **https://cloud.google.com/sdk/docs/install**
2. Tìm phần **"Windows"**
3. Tải file installer: **GoogleCloudSDKInstaller.exe**
4. Chạy file installer
5. Làm theo hướng dẫn (chọn "Install" và "Finish")

**Mac:**
```bash
# Cài đặt bằng Homebrew (nếu có)
brew install --cask google-cloud-sdk

# Hoặc tải từ website
# Vào: https://cloud.google.com/sdk/docs/install
```

**Linux:**
```bash
# Ubuntu/Debian
echo "deb [signed-by=/usr/share/keyrings/cloud.google.gpg] https://packages.cloud.google.com/apt cloud-sdk main" | sudo tee -a /etc/apt/sources.list.d/google-cloud-sdk.list
curl https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo apt-key --keyring /usr/share/keyrings/cloud.google.gpg add -
sudo apt-get update && sudo apt-get install google-cloud-sdk
```

### 2.2. Cài đặt Docker (Nếu chưa có)

**Windows:**
1. Tải Docker Desktop: **https://www.docker.com/products/docker-desktop**
2. Cài đặt và khởi động lại máy
3. Mở Docker Desktop và đợi nó chạy

**Mac:**
1. Tải Docker Desktop: **https://www.docker.com/products/docker-desktop**
2. Cài đặt và mở Docker Desktop

**Linux:**
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install docker.io
sudo systemctl start docker
sudo systemctl enable docker
```

### 2.3. Đăng nhập Google Cloud SDK

1. Mở **Terminal** (Mac/Linux) hoặc **PowerShell/CMD** (Windows)
2. Chạy lệnh:
```bash
gcloud auth login
```
3. Trình duyệt sẽ mở ra, chọn tài khoản Google của bạn
4. Cho phép Google Cloud SDK truy cập
5. Quay lại terminal, bạn sẽ thấy thông báo thành công

### 2.4. Set Project

1. Chạy lệnh (thay `YOUR_PROJECT_ID` bằng Project ID bạn đã lưu ở Bước 1.4):
```bash
gcloud config set project YOUR_PROJECT_ID
```

Ví dụ:
```bash
gcloud config set project electroshop-123456
```

2. Kiểm tra:
```bash
gcloud config get-value project
```
Bạn sẽ thấy Project ID của bạn

### 2.5. Bật các API cần thiết

Chạy các lệnh sau (từng lệnh một):

```bash
# Bật Cloud Run API
gcloud services enable run.googleapis.com

# Bật Artifact Registry API (để lưu Docker images)
gcloud services enable artifactregistry.googleapis.com

# Bật Cloud SQL Admin API (để tạo database)
gcloud services enable sqladmin.googleapis.com

# Bật Cloud Build API (nếu cần)
gcloud services enable cloudbuild.googleapis.com
```

Đợi mỗi lệnh chạy xong (sẽ mất vài giây mỗi lệnh)

### 2.6. Cấu hình Docker

Chạy lệnh:
```bash
gcloud auth configure-docker asia-southeast1-docker.pkg.dev
```

**✅ Hoàn thành Bước 2!**

---

## Bước 3: Setup Database (Cloud SQL)

### 3.1. Tạo Cloud SQL Instance

**Lưu ý:** Cloud SQL SQL Server có phí (~$50/tháng). Nếu muốn tiết kiệm, bạn có thể:
- Dùng PostgreSQL (rẻ hơn)
- Hoặc dùng SQL Server LocalDB tạm thời (không khuyến nghị cho production)

**Tạo SQL Server Instance:**

Chạy lệnh (thay `YOUR_STRONG_PASSWORD` bằng mật khẩu mạnh - ít nhất 8 ký tự, có chữ hoa, chữ thường, số):

```bash
gcloud sql instances create electroshop-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-f1-micro \
  --region=asia-southeast1 \
  --root-password=YOUR_STRONG_PASSWORD
```

**Ví dụ:**
```bash
gcloud sql instances create electroshop-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-f1-micro \
  --region=asia-southeast1 \
  --root-password=MyStrongPass123!
```

**Lưu ý:**
- `db-f1-micro` là tier nhỏ nhất (phù hợp cho test)
- `asia-southeast1` là Singapore (gần Việt Nam)
- Quá trình này sẽ mất **5-10 phút** → Đợi cho đến khi thấy thông báo "Created"

### 3.2. Tạo Database

Chạy lệnh:
```bash
gcloud sql databases create DoAnWebNCDB --instance=electroshop-db
```

Đợi vài giây để database được tạo

### 3.3. Tạo User cho Database

Chạy lệnh (thay `YOUR_DB_USER_PASSWORD` bằng mật khẩu khác với root password):

```bash
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=YOUR_DB_USER_PASSWORD
```

**Ví dụ:**
```bash
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=DbUserPass123!
```

### 3.4. Lấy Connection Name

Chạy lệnh:
```bash
gcloud sql instances describe electroshop-db --format="value(connectionName)"
```

**Lưu lại kết quả** - sẽ có dạng: `PROJECT_ID:asia-southeast1:electroshop-db`

### 3.5. Lấy Public IP (Nếu cần)

Chạy lệnh:
```bash
gcloud sql instances describe electroshop-db --format="value(ipAddresses[0].ipAddress)"
```

**Lưu lại Public IP** (nếu có)

### 3.6. Cho phép kết nối từ bên ngoài (Nếu cần)

Nếu muốn kết nối từ máy local để test:

```bash
# Lấy IP máy của bạn
# Windows: ipconfig
# Mac/Linux: curl ifconfig.me

# Thêm IP vào authorized networks (thay YOUR_IP bằng IP của bạn)
gcloud sql instances patch electroshop-db --authorized-networks=YOUR_IP/32
```

**✅ Hoàn thành Bước 3!**

---

## Bước 4: Cấu hình appsettings.Production.json

### 4.1. Tạo file appsettings.Production.json

1. Mở thư mục project của bạn
2. Copy file `appsettings.Production.example.json` thành `appsettings.Production.json`
3. Hoặc tạo file mới `appsettings.Production.json`

### 4.2. Cập nhật Connection String

Mở file `appsettings.Production.json` và cập nhật:

**Cách 1: Dùng Cloud SQL Proxy (Khuyến nghị)**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=/cloudsql/PROJECT_ID:asia-southeast1:electroshop-db;Database=DoAnWebNCDB;User Id=dbuser;Password=YOUR_DB_USER_PASSWORD;TrustServerCertificate=True;"
  }
}
```

**Thay:**
- `PROJECT_ID` → Project ID của bạn (ví dụ: `electroshop-123456`)
- `YOUR_DB_USER_PASSWORD` → Mật khẩu bạn đã tạo ở Bước 3.3

**Ví dụ:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=/cloudsql/electroshop-123456:asia-southeast1:electroshop-db;Database=DoAnWebNCDB;User Id=dbuser;Password=DbUserPass123!;TrustServerCertificate=True;"
  }
}
```

**Cách 2: Dùng Public IP**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PUBLIC_IP;Database=DoAnWebNCDB;User Id=dbuser;Password=YOUR_DB_USER_PASSWORD;TrustServerCertificate=True;"
  }
}
```

**Thay:**
- `YOUR_PUBLIC_IP` → Public IP bạn đã lưu ở Bước 3.5
- `YOUR_DB_USER_PASSWORD` → Mật khẩu bạn đã tạo ở Bước 3.3

### 4.3. Cập nhật VnPay ReturnUrl

Trong file `appsettings.Production.json`, cập nhật `ReturnUrl`:

```json
{
  "VnPay": {
    "TmnCode": "SJBLAJF0",
    "HashSecret": "3BY72RWVVTO43M9JEYSHVG9KHA1MA5TU",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://YOUR_DOMAIN.run.app/Order/PaymentCallBack"
  }
}
```

**Lưu ý:** Bạn sẽ cập nhật `YOUR_DOMAIN` sau khi deploy xong (ở Bước 5)

### 4.4. Kiểm tra file

File `appsettings.Production.json` của bạn sẽ giống như sau:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=/cloudsql/electroshop-123456:asia-southeast1:electroshop-db;Database=DoAnWebNCDB;User Id=dbuser;Password=DbUserPass123!;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "VnPay": {
    "TmnCode": "SJBLAJF0",
    "HashSecret": "3BY72RWVVTO43M9JEYSHVG9KHA1MA5TU",
    "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
    "ReturnUrl": "https://YOUR_DOMAIN.run.app/Order/PaymentCallBack"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "nguyenminh01060210@gmail.com",
    "SenderPassword": "dseh xfyl eplj uuxg"
  }
}
```

**✅ Hoàn thành Bước 4!**

---

## Bước 5: Deploy Website

### 5.1. Tạo Artifact Registry Repository

Chạy lệnh:

```bash
gcloud artifacts repositories create electroshop-repo \
  --repository-format=docker \
  --location=asia-southeast1 \
  --description="ElectroShop Docker repository"
```

### 5.2. Build Docker Image

**Windows (PowerShell):**
```powershell
# Thay YOUR_PROJECT_ID bằng Project ID của bạn
$PROJECT_ID = "YOUR_PROJECT_ID"
$REGION = "asia-southeast1"
$IMAGE_NAME = "electroshop"

# Build image
docker build -t "$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME`:latest" .
```

**Mac/Linux:**
```bash
# Thay YOUR_PROJECT_ID bằng Project ID của bạn
export PROJECT_ID="YOUR_PROJECT_ID"
export REGION="asia-southeast1"
export IMAGE_NAME="electroshop"

# Build image
docker build -t $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest .
```

**Ví dụ:**
```bash
export PROJECT_ID="electroshop-123456"
export REGION="asia-southeast1"
export IMAGE_NAME="electroshop"

docker build -t $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest .
```

**Lưu ý:** Quá trình build sẽ mất **2-5 phút** lần đầu tiên

### 5.3. Push Docker Image

Sau khi build xong, push image:

**Windows (PowerShell):**
```powershell
docker push "$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME`:latest"
```

**Mac/Linux:**
```bash
docker push $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest
```

**Lưu ý:** Quá trình push sẽ mất **3-10 phút** tùy vào kích thước image

### 5.4. Deploy lên Cloud Run

Chạy lệnh (thay `YOUR_PROJECT_ID` và các biến khác):

**Windows (PowerShell):**
```powershell
$PROJECT_ID = "YOUR_PROJECT_ID"
$REGION = "asia-southeast1"
$IMAGE_NAME = "electroshop"
$SERVICE_NAME = "electroshop"

gcloud run deploy $SERVICE_NAME `
  --image "$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME`:latest" `
  --platform managed `
  --region $REGION `
  --allow-unauthenticated `
  --memory 512Mi `
  --cpu 1 `
  --min-instances 0 `
  --max-instances 10 `
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production `
  --port 8080 `
  --add-cloudsql-instances "$PROJECT_ID`:asia-southeast1:electroshop-db"
```

**Mac/Linux:**
```bash
export PROJECT_ID="YOUR_PROJECT_ID"
export REGION="asia-southeast1"
export IMAGE_NAME="electroshop"
export SERVICE_NAME="electroshop"

gcloud run deploy $SERVICE_NAME \
  --image $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest \
  --platform managed \
  --region $REGION \
  --allow-unauthenticated \
  --memory 512Mi \
  --cpu 1 \
  --min-instances 0 \
  --max-instances 10 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production \
  --port 8080 \
  --add-cloudsql-instances $PROJECT_ID:asia-southeast1:electroshop-db
```

**Ví dụ:**
```bash
export PROJECT_ID="electroshop-123456"
export REGION="asia-southeast1"
export IMAGE_NAME="electroshop"
export SERVICE_NAME="electroshop"

gcloud run deploy $SERVICE_NAME \
  --image $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest \
  --platform managed \
  --region $REGION \
  --allow-unauthenticated \
  --memory 512Mi \
  --cpu 1 \
  --min-instances 0 \
  --max-instances 10 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production \
  --port 8080 \
  --add-cloudsql-instances $PROJECT_ID:asia-southeast1:electroshop-db
```

**Lưu ý:** 
- Quá trình deploy sẽ mất **2-5 phút**
- Khi deploy xong, bạn sẽ thấy URL website (ví dụ: `https://electroshop-xxxxx-xx.a.run.app`)
- **Lưu lại URL này!**

### 5.5. Lấy URL Website

Sau khi deploy xong, bạn sẽ thấy dòng:
```
Service URL: https://electroshop-xxxxx-xx.a.run.app
```

**Lưu lại URL này!**

Hoặc chạy lệnh để lấy URL:
```bash
gcloud run services describe electroshop --region=asia-southeast1 --format="value(status.url)"
```

### 5.6. Cập nhật VnPay ReturnUrl (Nếu chưa)

1. Mở file `appsettings.Production.json`
2. Cập nhật `ReturnUrl` với URL thực tế:
```json
"ReturnUrl": "https://electroshop-xxxxx-xx.a.run.app/Order/PaymentCallBack"
```
3. Rebuild và redeploy (lặp lại Bước 5.2, 5.3, 5.4)

**✅ Hoàn thành Bước 5!**

---

## Bước 6: Kiểm tra và Test

### 6.1. Truy cập Website

1. Mở trình duyệt
2. Truy cập URL website của bạn (ví dụ: `https://electroshop-xxxxx-xx.a.run.app`)
3. Kiểm tra website có load không

### 6.2. Kiểm tra Sitemap

Truy cập: `https://your-domain.run.app/sitemap.xml`

Bạn sẽ thấy XML sitemap

### 6.3. Kiểm tra Robots.txt

Truy cập: `https://your-domain.run.app/robots.txt`

Bạn sẽ thấy nội dung robots.txt

### 6.4. Test các chức năng

1. **Trang chủ:** Kiểm tra có hiển thị sản phẩm không
2. **Đăng ký/Đăng nhập:** Test đăng ký tài khoản mới
3. **Xem sản phẩm:** Click vào sản phẩm bất kỳ
4. **Thêm vào giỏ hàng:** Test thêm sản phẩm vào giỏ
5. **Thanh toán:** Test flow thanh toán (dùng VnPay sandbox)

### 6.5. Kiểm tra Database

1. Vào Google Cloud Console
2. Vào **SQL** > **electroshop-db**
3. Kiểm tra database có được tạo và có data không

### 6.6. Chạy Migrations (Nếu cần)

Nếu database chưa có schema, bạn cần chạy migrations:

**Cách 1: Dùng Cloud SQL Proxy (Khuyến nghị)**

1. Tải Cloud SQL Proxy: https://cloud.google.com/sql/docs/mysql/connect-admin-proxy
2. Chạy proxy:
```bash
cloud_sql_proxy -instances=PROJECT_ID:asia-southeast1:electroshop-db=tcp:5432
```
3. Trong terminal khác, chạy migrations:
```bash
dotnet ef database update
```

**Cách 2: Deploy một container tạm thời**

Tạo script và deploy như một Cloud Run job (tạm thời)

**✅ Hoàn thành Bước 6!**

---

## 🎉 Hoàn Thành!

Website của bạn đã được deploy lên Google Cloud Run!

### URL Website:
```
https://your-domain.run.app
```

### Các bước tiếp theo:
1. ✅ Deploy website (Đã xong)
2. ⏭️ Đăng ký Google Search Console (Xem `CHECKLIST_NHANH.md`)
3. ⏭️ Submit sitemap (Xem `CHECKLIST_NHANH.md`)
4. ⏭️ Request indexing (Xem `CHECKLIST_NHANH.md`)

---

## 🔧 Troubleshooting

### Lỗi: "Permission denied"
**Giải pháp:** Chạy `gcloud auth login` lại

### Lỗi: "Project not found"
**Giải pháp:** Kiểm tra Project ID đúng chưa: `gcloud config get-value project`

### Lỗi: "Docker not running"
**Giải pháp:** Mở Docker Desktop và đợi nó chạy xong

### Lỗi: "Connection string invalid"
**Giải pháp:** Kiểm tra lại connection string trong `appsettings.Production.json`

### Lỗi: "Database not found"
**Giải pháp:** Kiểm tra database đã được tạo chưa: `gcloud sql databases list --instance=electroshop-db`

### Lỗi: "Image not found"
**Giải pháp:** Kiểm tra image đã được push chưa: `gcloud artifacts docker images list asia-southeast1-docker.pkg.dev/PROJECT_ID/electroshop-repo`

### Website không load
**Giải pháp:**
1. Kiểm tra logs: `gcloud run services logs read electroshop --region=asia-southeast1`
2. Kiểm tra service status: `gcloud run services describe electroshop --region=asia-southeast1`

### Database connection error
**Giải pháp:**
1. Kiểm tra Cloud SQL instance đang chạy: `gcloud sql instances describe electroshop-db`
2. Kiểm tra connection string đúng chưa
3. Kiểm tra user và password đúng chưa

---

## 📚 Tài liệu tham khảo

- Google Cloud Run Docs: https://cloud.google.com/run/docs
- Cloud SQL Docs: https://cloud.google.com/sql/docs
- Docker Docs: https://docs.docker.com/

---

**Chúc bạn deploy thành công! 🚀**

Nếu gặp vấn đề, xem phần Troubleshooting hoặc hỏi trong các diễn đàn cộng đồng.

