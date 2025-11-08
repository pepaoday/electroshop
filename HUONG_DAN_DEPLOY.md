# Hướng Dẫn Deploy ElectroShop lên Google Cloud Run

Hướng dẫn này sẽ giúp bạn deploy ứng dụng ASP.NET Core lên Google Cloud Run để có thể truy cập qua đường link công khai.

## Bước 1: Chuẩn bị

### 1.1. Cài đặt Google Cloud SDK

1. Tải Google Cloud SDK: https://cloud.google.com/sdk/docs/install
2. Cài đặt và mở Terminal/PowerShell
3. Chạy lệnh để đăng nhập:
```bash
gcloud auth login
```

### 1.2. Tạo Project trên Google Cloud

1. Truy cập: https://console.cloud.google.com/
2. Tạo project mới (hoặc chọn project có sẵn)
3. Ghi nhớ **Project ID** (ví dụ: `my-electroshop`)

### 1.3. Bật các API cần thiết

Chạy các lệnh sau trong Terminal:

```bash
# Đăng nhập và chọn project
gcloud config set project YOUR_PROJECT_ID

# Bật Cloud Run API
gcloud services enable run.googleapis.com

# Bật Cloud SQL Admin API (nếu dùng Cloud SQL)
gcloud services enable sqladmin.googleapis.com

# Bật Container Registry API
gcloud services enable containerregistry.googleapis.com

# Bật Artifact Registry API (khuyến nghị dùng thay vì Container Registry)
gcloud services enable artifactregistry.googleapis.com
```

## Bước 2: Setup Database (Cloud SQL)

### 2.1. Tạo Cloud SQL instance

**Lưu ý:** Cloud SQL cho SQL Server có phí. Nếu muốn tiết kiệm, có thể dùng PostgreSQL (cần migrate database).

#### Tạo SQL Server instance:

```bash
gcloud sql instances create electroshop-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-f1-micro \
  --region=asia-southeast1 \
  --root-password=YOUR_STRONG_PASSWORD
```

**Lưu ý:** 
- `db-f1-micro` là tier miễn phí (chỉ để test)
- Đổi `YOUR_STRONG_PASSWORD` thành mật khẩu mạnh
- Region `asia-southeast1` là Singapore (gần Việt Nam)

#### Tạo database:

```bash
gcloud sql databases create DoAnWebNCDB --instance=electroshop-db
```

#### Tạo user cho database:

```bash
gcloud sql users create dbuser \
  --instance=electroshop-db \
  --password=YOUR_DB_USER_PASSWORD
```

### 2.2. Lấy Connection String

Sau khi tạo xong, lấy connection name:

```bash
gcloud sql instances describe electroshop-db --format="value(connectionName)"
```

Kết quả sẽ có dạng: `PROJECT_ID:REGION:electroshop-db`

### 2.3. Cập nhật appsettings.Production.json

Mở file `appsettings.Production.json` và cập nhật connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=/cloudsql/PROJECT_ID:REGION:electroshop-db;Database=DoAnWebNCDB;User Id=dbuser;Password=YOUR_DB_USER_PASSWORD;TrustServerCertificate=True;"
  }
}
```

**Hoặc nếu dùng Public IP:**

1. Cho phép Cloud SQL chấp nhận kết nối từ bên ngoài:
```bash
gcloud sql instances patch electroshop-db --authorized-networks=0.0.0.0/0
```

2. Lấy Public IP:
```bash
gcloud sql instances describe electroshop-db --format="value(ipAddresses[0].ipAddress)"
```

3. Cập nhật connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PUBLIC_IP;Database=DoAnWebNCDB;User Id=dbuser;Password=YOUR_DB_USER_PASSWORD;TrustServerCertificate=True;"
  }
}
```

## Bước 3: Build và Push Docker Image

### 3.1. Cấu hình Docker để push lên Google Cloud

```bash
# Cấu hình Docker để dùng gcloud làm credential helper
gcloud auth configure-docker asia-southeast1-docker.pkg.dev
```

### 3.2. Tạo Artifact Registry repository

```bash
gcloud artifacts repositories create electroshop-repo \
  --repository-format=docker \
  --location=asia-southeast1 \
  --description="ElectroShop Docker repository"
```

### 3.3. Build và push Docker image

```bash
# Thay YOUR_PROJECT_ID bằng Project ID của bạn
export PROJECT_ID=YOUR_PROJECT_ID
export REGION=asia-southeast1
export IMAGE_NAME=electroshop

# Build image
docker build -t $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest .

# Push image
docker push $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest
```

## Bước 4: Deploy lên Cloud Run

### 4.1. Deploy service

```bash
gcloud run deploy electroshop \
  --image $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest \
  --platform managed \
  --region $REGION \
  --allow-unauthenticated \
  --memory 512Mi \
  --cpu 1 \
  --min-instances 0 \
  --max-instances 10 \
  --set-env-vars ASPNETCORE_ENVIRONMENT=Production \
  --add-cloudsql-instances $PROJECT_ID:$REGION:electroshop-db
```

**Giải thích các tham số:**
- `--allow-unauthenticated`: Cho phép truy cập công khai (không cần đăng nhập Google)
- `--memory 512Mi`: Bộ nhớ (có thể tăng nếu cần)
- `--min-instances 0`: Tắt instance khi không dùng (tiết kiệm chi phí)
- `--max-instances 10`: Giới hạn số instance tối đa
- `--add-cloudsql-instances`: Kết nối với Cloud SQL instance

### 4.2. Cập nhật VnPay ReturnUrl

Sau khi deploy, bạn sẽ nhận được URL dạng: `https://electroshop-xxxxx-xx.a.run.app`

1. Mở file `appsettings.Production.json`
2. Cập nhật `ReturnUrl`:
```json
"ReturnUrl": "https://electroshop-xxxxx-xx.a.run.app/Order/PaymentCallBack"
```

3. Rebuild và redeploy:
```bash
docker build -t $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest .
docker push $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest
gcloud run deploy electroshop \
  --image $REGION-docker.pkg.dev/$PROJECT_ID/electroshop-repo/$IMAGE_NAME:latest \
  --region $REGION
```

## Bước 5: Chạy Migrations và Seed Data

### 5.1. Kết nối với Cloud SQL và chạy migrations

**Cách 1: Dùng Cloud SQL Proxy (khuyến nghị)**

1. Tải Cloud SQL Proxy: https://cloud.google.com/sql/docs/mysql/connect-admin-proxy

2. Chạy proxy:
```bash
cloud_sql_proxy -instances=PROJECT_ID:REGION:electroshop-db=tcp:5432
```

3. Trong terminal khác, chạy migrations:
```bash
dotnet ef database update
```

**Cách 2: Dùng gcloud sql connect**

```bash
gcloud sql connect electroshop-db --user=dbuser
```

Sau đó chạy các lệnh SQL cần thiết.

**Cách 3: Deploy một container tạm thời để chạy migrations**

Tạo script `run-migrations.sh`:
```bash
#!/bin/bash
dotnet ef database update
```

Deploy như một Cloud Run job (tạm thời).

## Bước 6: (Tùy chọn) Cấu hình Custom Domain

### 6.1. Map domain tùy chỉnh

1. Trong Google Cloud Console, vào Cloud Run
2. Chọn service `electroshop`
3. Click "MANAGE CUSTOM DOMAINS"
4. Thêm domain của bạn
5. Làm theo hướng dẫn để verify domain và cấu hình DNS

## Bước 7: Kiểm tra và Test

1. Truy cập URL: `https://electroshop-xxxxx-xx.a.run.app`
2. Test các chức năng:
   - Đăng ký/Đăng nhập
   - Xem sản phẩm
   - Thêm vào giỏ hàng
   - Thanh toán (test với VnPay sandbox)

## Lưu ý quan trọng

### Về chi phí:
- **Cloud Run**: Tính theo request và thời gian sử dụng (có free tier)
- **Cloud SQL**: SQL Server có phí (~$50/tháng cho db-f1-micro). Nếu muốn miễn phí, dùng PostgreSQL
- **Storage**: Images và files static được lưu trong container (có thể dùng Cloud Storage nếu cần)

### Về bảo mật:
- ✅ Đổi mật khẩu admin sau khi deploy
- ✅ Không commit file `appsettings.Production.json` có thông tin nhạy cảm
- ✅ Dùng Secret Manager cho các thông tin nhạy cảm (password, API keys)

### Về performance:
- Cloud Run tự động scale theo traffic
- Có thể tăng memory/CPU nếu cần
- Cân nhắc dùng CDN cho static files

## Troubleshooting

### Lỗi kết nối database:
- Kiểm tra Cloud SQL instance đã được thêm vào Cloud Run service
- Kiểm tra connection string trong appsettings.Production.json
- Kiểm tra firewall rules của Cloud SQL

### Lỗi 502 Bad Gateway:
- Kiểm tra logs: `gcloud run services logs read electroshop --region=$REGION`
- Kiểm tra ứng dụng có chạy đúng port 8080 không

### Lỗi build Docker:
- Kiểm tra Dockerfile có đúng không
- Kiểm tra .dockerignore không loại bỏ file cần thiết

## Các lệnh hữu ích

```bash
# Xem logs
gcloud run services logs read electroshop --region=asia-southeast1

# Xem thông tin service
gcloud run services describe electroshop --region=asia-southeast1

# Update service
gcloud run services update electroshop --region=asia-southeast1 --memory=1Gi

# Xóa service (nếu không dùng nữa)
gcloud run services delete electroshop --region=asia-southeast1
```

## Tài liệu tham khảo

- Google Cloud Run Docs: https://cloud.google.com/run/docs
- Cloud SQL Docs: https://cloud.google.com/sql/docs
- ASP.NET Core on Cloud Run: https://cloud.google.com/run/docs/quickstarts/build-and-deploy/deploy-aspnet-core-service

---

**Chúc bạn deploy thành công! 🚀**

