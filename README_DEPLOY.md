# 🚀 Deploy ElectroShop lên Google Cloud Run - Hướng dẫn nhanh

## Yêu cầu
- Google Cloud account (có free trial $300)
- Google Cloud SDK đã cài đặt
- Docker đã cài đặt

## Deploy nhanh (3 bước)

### Bước 1: Setup Database
```bash
# Tạo Cloud SQL instance
gcloud sql instances create electroshop-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-f1-micro \
  --region=asia-southeast1 \
  --root-password=YOUR_STRONG_PASSWORD

# Tạo database
gcloud sql databases create DoAnWebNCDB --instance=electroshop-db

# Tạo user
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=YOUR_DB_PASSWORD
```

### Bước 2: Cấu hình appsettings.Production.json
Copy `appsettings.Production.example.json` thành `appsettings.Production.json` và cập nhật:
- Connection string (sau khi có Cloud SQL)
- VnPay ReturnUrl (sau khi deploy xong)
- Email settings

### Bước 3: Deploy
**Windows (PowerShell):**
```powershell
.\deploy.ps1 -ProjectId "YOUR_PROJECT_ID"
```

**Linux/Mac:**
```bash
chmod +x deploy.sh
./deploy.sh YOUR_PROJECT_ID
```

## Chi tiết đầy đủ
Xem file [HUONG_DAN_DEPLOY.md](./HUONG_DAN_DEPLOY.md) để biết hướng dẫn chi tiết từng bước.

## Sau khi deploy
1. Lấy URL từ output
2. Cập nhật VnPay ReturnUrl trong `appsettings.Production.json`
3. Redeploy để áp dụng thay đổi
4. Chạy migrations để tạo database schema
5. Truy cập URL và test ứng dụng

## Lưu ý
- ⚠️ Cloud SQL SQL Server có phí (~$50/tháng)
- 💰 Cloud Run có free tier (2 triệu requests/tháng)
- 🔒 Không commit file `appsettings.Production.json` (đã có trong .gitignore)

## Hỗ trợ
Nếu gặp vấn đề, xem phần Troubleshooting trong [HUONG_DAN_DEPLOY.md](./HUONG_DAN_DEPLOY.md)

