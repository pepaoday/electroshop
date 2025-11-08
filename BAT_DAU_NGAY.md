# 🚀 BẮT ĐẦU NGAY - Deploy Website lên Google Cloud Run

## ⚡ Làm theo 5 bước này là xong!

---

## 📝 BƯỚC 1: Tạo Project Google Cloud (5 phút)

1. Vào: **https://console.cloud.google.com/**
2. Đăng nhập bằng Google
3. Click **"New Project"**
4. Đặt tên: `ElectroShop`
5. Click **"Create"**
6. **Lưu Project ID** (ví dụ: `electroshop-123456`)

✅ **Xong!**

---

## 📝 BƯỚC 2: Cài đặt Tools (10 phút)

### 2.1. Cài Google Cloud SDK
- Windows: Tải từ **https://cloud.google.com/sdk/docs/install**
- Mac: `brew install --cask google-cloud-sdk`

### 2.2. Cài Docker Desktop
- Tải từ: **https://www.docker.com/products/docker-desktop**

### 2.3. Đăng nhập và Setup

Mở **PowerShell** (Windows) và chạy:

```powershell
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

✅ **Xong!**

---

## 📝 BƯỚC 3: Tạo Database (10 phút)

Chạy các lệnh này (THAY `YOUR_STRONG_PASSWORD`):

```powershell
# Tạo Cloud SQL instance
gcloud sql instances create electroshop-db `
  --database-version=SQLSERVER_2019_STANDARD `
  --tier=db-f1-micro `
  --region=asia-southeast1 `
  --root-password=YOUR_STRONG_PASSWORD

# Đợi 5-10 phút để instance được tạo, sau đó:

# Tạo database
gcloud sql databases create DoAnWebNCDB --instance=electroshop-db

# Tạo user
gcloud sql users create dbuser `
  --instance=electroshop-db `
  --password=YOUR_DB_PASSWORD

# Lấy connection name
gcloud sql instances describe electroshop-db --format="value(connectionName)"
```

**Lưu lại connection name** (ví dụ: `electroshop-123456:asia-southeast1:electroshop-db`)

✅ **Xong!**

---

## 📝 BƯỚC 4: Cấu hình appsettings.Production.json (2 phút)

1. Tạo file `appsettings.Production.json` trong thư mục project
2. Copy nội dung này (THAY `PROJECT_ID` và `YOUR_DB_PASSWORD`):

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

✅ **Xong!**

---

## 📝 BƯỚC 5: Deploy (5 phút)

### Cách 1: Dùng Script (Dễ nhất)

```powershell
.\deploy.ps1 -ProjectId "YOUR_PROJECT_ID"
```

Khi hỏi "Bạn đã tạo Cloud SQL instance chưa?", gõ `y`

### Cách 2: Deploy thủ công

```powershell
$PROJECT_ID = "YOUR_PROJECT_ID"
$REGION = "asia-southeast1"
$IMAGE_NAME = "electroshop"

# Tạo repository
gcloud artifacts repositories create electroshop-repo `
  --repository-format=docker `
  --location=$REGION

# Build và push
$IMAGE_URI = "$REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME`:latest"
docker build -t $IMAGE_URI .
docker push $IMAGE_URI

# Deploy
gcloud run deploy electroshop `
  --image $IMAGE_URI `
  --platform managed `
  --region $REGION `
  --allow-unauthenticated `
  --memory 512Mi `
  --cpu 1 `
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production `
  --port 8080 `
  --add-cloudsql-instances "$PROJECT_ID`:asia-southeast1:electroshop-db"
```

**Đợi 5-10 phút** để deploy xong

Sau khi deploy xong, bạn sẽ thấy URL website (ví dụ: `https://electroshop-xxxxx-xx.a.run.app`)

**Lưu lại URL này!**

✅ **Xong!**

---

## 🎉 HOÀN THÀNH!

### Website của bạn đã được deploy!

**URL:** `https://your-domain.run.app`

### Kiểm tra:

1. Truy cập URL website
2. Kiểm tra sitemap: `https://your-domain.run.app/sitemap.xml`
3. Kiểm tra robots.txt: `https://your-domain.run.app/robots.txt`

---

## 📝 Các bước tiếp theo:

1. ✅ Deploy website (Đã xong)
2. ⏭️ Đăng ký Google Search Console → Xem `CHECKLIST_NHANH.md`
3. ⏭️ Submit sitemap → Xem `CHECKLIST_NHANH.md`
4. ⏭️ Request indexing → Xem `CHECKLIST_NHANH.md`

---

## 🔧 Gặp vấn đề?

### Lỗi "Permission denied"
```powershell
gcloud auth login
```

### Lỗi "Docker not running"
- Mở Docker Desktop

### Website không load
```powershell
gcloud run services logs read electroshop --region=asia-southeast1
```

### Xem hướng dẫn chi tiết
- `DEPLOY_NHANH.md` - Hướng dẫn nhanh
- `HUONG_DAN_DEPLOY_CHI_TIET.md` - Hướng dẫn chi tiết từng bước

---

## 📚 Files hướng dẫn:

- 🚀 **BAT_DAU_NGAY.md** (File này) - Bắt đầu ngay
- ⚡ **DEPLOY_NHANH.md** - Deploy nhanh
- 📖 **HUONG_DAN_DEPLOY_CHI_TIET.md** - Hướng dẫn chi tiết
- ✅ **CHECKLIST_NHANH.md** - Checklist SEO
- 🔍 **TANG_TOC_INDEX_GOOGLE.md** - Tăng tốc index Google

---

**Chúc bạn deploy thành công! 🚀**

Nếu cần giúp đỡ, xem các file hướng dẫn chi tiết!

