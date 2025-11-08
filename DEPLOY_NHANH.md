# ⚡ Deploy Nhanh - Copy Paste Làm Ngay

## 🎯 Mục tiêu: Deploy website lên Google Cloud Run trong 30 phút

---

## 📋 CHECKLIST NHANH

- [ ] Bước 1: Tạo Project Google Cloud
- [ ] Bước 2: Cài đặt Google Cloud SDK
- [ ] Bước 3: Setup Database
- [ ] Bước 4: Deploy Website
- [ ] Bước 5: Kiểm tra

---

## BƯỚC 1: Tạo Project Google Cloud (5 phút)

### 1.1. Truy cập Google Cloud Console
👉 **https://console.cloud.google.com/**

### 1.2. Tạo Project
1. Click dropdown project ở trên cùng
2. Click **"New Project"**
3. Đặt tên: `ElectroShop`
4. Click **"Create"**
5. **Lưu lại Project ID** (ví dụ: `electroshop-123456`)

### 1.3. Kích hoạt Free Trial (Nếu chưa)
- Click **"Dùng thử miễn phí"**
- Điền thông tin thanh toán (có $300 credit miễn phí)

**✅ Xong Bước 1!**

---

## BƯỚC 2: Cài đặt Google Cloud SDK (10 phút)

### 2.1. Tải và cài đặt

**Windows:**
1. Tải: **https://cloud.google.com/sdk/docs/install**
2. Chạy file installer
3. Cài đặt Docker Desktop: **https://www.docker.com/products/docker-desktop**

**Mac:**
```bash
brew install --cask google-cloud-sdk
```

**Linux:**
```bash
# Xem hướng dẫn chi tiết trong HUONG_DAN_DEPLOY_CHI_TIET.md
```

### 2.2. Đăng nhập và Setup

Mở **PowerShell** (Windows) hoặc **Terminal** (Mac/Linux) và chạy:

```bash
# Đăng nhập
gcloud auth login

# Set project (THAY YOUR_PROJECT_ID)
gcloud config set project YOUR_PROJECT_ID

# Bật APIs
gcloud services enable run.googleapis.com
gcloud services enable artifactregistry.googleapis.com
gcloud services enable sqladmin.googleapis.com

# Cấu hình Docker
gcloud auth configure-docker asia-southeast1-docker.pkg.dev
```

**Ví dụ:**
```bash
gcloud config set project electroshop-123456
```

**✅ Xong Bước 2!**

---

## BƯỚC 3: Setup Database (10 phút)

### 3.1. Tạo Cloud SQL Instance

Chạy lệnh (THAY `YOUR_STRONG_PASSWORD` bằng mật khẩu mạnh):

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

**⏱️ Đợi 5-10 phút** để instance được tạo

### 3.2. Tạo Database

```bash
gcloud sql databases create DoAnWebNCDB --instance=electroshop-db
```

### 3.3. Tạo User

```bash
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=YOUR_DB_PASSWORD
```

**Ví dụ:**
```bash
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=DbUserPass123!
```

### 3.4. Lấy Connection Name

```bash
gcloud sql instances describe electroshop-db --format="value(connectionName)"
```

**Lưu lại kết quả** (ví dụ: `electroshop-123456:asia-southeast1:electroshop-db`)

**✅ Xong Bước 3!**

---

## BƯỚC 4: Cấu hình và Deploy (10 phút)

### 4.1. Tạo file appsettings.Production.json

Tạo file `appsettings.Production.json` với nội dung:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=/cloudsql/PROJECT_ID:asia-southeast1:electroshop-db;Database=DoAnWebNCDB;User Id=dbuser;Password=YOUR_DB_PASSWORD;TrustServerCertificate=True;"
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

**Thay:**
- `PROJECT_ID` → Project ID của bạn
- `YOUR_DB_PASSWORD` → Mật khẩu bạn đã tạo ở Bước 3.3
- `YOUR_DOMAIN` → Sẽ cập nhật sau khi deploy

### 4.2. Deploy bằng Script (Dễ nhất)

**Windows (PowerShell):**
```powershell
.\deploy.ps1 -ProjectId "YOUR_PROJECT_ID"
```

**Ví dụ:**
```powershell
.\deploy.ps1 -ProjectId "electroshop-123456"
```

### 4.3. Hoặc Deploy thủ công

**Windows (PowerShell):**
```powershell
$PROJECT_ID = "YOUR_PROJECT_ID"
$REGION = "asia-southeast1"
$IMAGE_NAME = "electroshop"
$SERVICE_NAME = "electroshop"

# Tạo repository
gcloud artifacts repositories create electroshop-repo `
  --repository-format=docker `
  --location=$REGION `
  --description="ElectroShop Docker repository"

# Build image
$IMAGE_URI = "$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME`:latest"
docker build -t $IMAGE_URI .

# Push image
docker push $IMAGE_URI

# Deploy
gcloud run deploy $SERVICE_NAME `
  --image $IMAGE_URI `
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

# Tạo repository
gcloud artifacts repositories create electroshop-repo \
  --repository-format=docker \
  --location=$REGION \
  --description="ElectroShop Docker repository"

# Build image
IMAGE_URI="$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest"
docker build -t $IMAGE_URI .

# Push image
docker push $IMAGE_URI

# Deploy
gcloud run deploy $SERVICE_NAME \
  --image $IMAGE_URI \
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

**⏱️ Đợi 5-10 phút** để deploy xong

### 4.4. Lấy URL Website

Sau khi deploy xong, bạn sẽ thấy:
```
Service URL: https://electroshop-xxxxx-xx.a.run.app
```

**Lưu lại URL này!**

### 4.5. Cập nhật VnPay ReturnUrl

1. Mở file `appsettings.Production.json`
2. Cập nhật `ReturnUrl` với URL thực tế
3. Rebuild và redeploy (lặp lại Bước 4.2 hoặc 4.3)

**✅ Xong Bước 4!**

---

## BƯỚC 5: Kiểm tra (5 phút)

### 5.1. Truy cập Website

Mở trình duyệt và truy cập URL website của bạn

### 5.2. Kiểm tra Sitemap

Truy cập: `https://your-domain.run.app/sitemap.xml`

### 5.3. Kiểm tra Robots.txt

Truy cập: `https://your-domain.run.app/robots.txt`

### 5.4. Test Website

- [ ] Trang chủ load được
- [ ] Đăng ký/Đăng nhập hoạt động
- [ ] Xem sản phẩm được
- [ ] Thêm vào giỏ hàng được

**✅ Xong Bước 5!**

---

## 🎉 HOÀN THÀNH!

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
```bash
gcloud auth login
```

### Lỗi: "Project not found"
```bash
gcloud config get-value project
gcloud config set project YOUR_PROJECT_ID
```

### Lỗi: "Docker not running"
- Mở Docker Desktop và đợi nó chạy xong

### Website không load
```bash
# Xem logs
gcloud run services logs read electroshop --region=asia-southeast1
```

### Database connection error
- Kiểm tra connection string trong `appsettings.Production.json`
- Kiểm tra Cloud SQL instance đang chạy:
```bash
gcloud sql instances describe electroshop-db
```

---

## 📚 Xem thêm

- **Hướng dẫn chi tiết:** `HUONG_DAN_DEPLOY_CHI_TIET.md`
- **Checklist SEO:** `CHECKLIST_NHANH.md`
- **Tăng tốc index:** `TANG_TOC_INDEX_GOOGLE.md`

---

**Chúc bạn deploy thành công! 🚀**

Nếu gặp vấn đề, xem phần Troubleshooting hoặc file hướng dẫn chi tiết.

