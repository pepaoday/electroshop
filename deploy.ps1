# Script deploy tự động lên Google Cloud Run (PowerShell)
# Sử dụng: .\deploy.ps1 -ProjectId "YOUR_PROJECT_ID"

param(
    [Parameter(Mandatory=$true)]
    [string]$ProjectId
)

$ErrorActionPreference = "Stop"

$Region = "asia-southeast1"
$ImageName = "electroshop"
$ServiceName = "electroshop"
$RepoName = "electroshop-repo"

Write-Host "🚀 Bắt đầu deploy ElectroShop lên Google Cloud Run..." -ForegroundColor Green
Write-Host "Project ID: $ProjectId" -ForegroundColor Cyan
Write-Host "Region: $Region" -ForegroundColor Cyan

# Set project
Write-Host "`n📋 Setting project..." -ForegroundColor Yellow
gcloud config set project $ProjectId

# Enable APIs
Write-Host "🔧 Enabling required APIs..." -ForegroundColor Yellow
gcloud services enable run.googleapis.com --quiet
gcloud services enable artifactregistry.googleapis.com --quiet
gcloud services enable sqladmin.googleapis.com --quiet

# Configure Docker
Write-Host "🐳 Configuring Docker..." -ForegroundColor Yellow
gcloud auth configure-docker "$Region-docker.pkg.dev" --quiet

# Create Artifact Registry repository if not exists
Write-Host "📦 Checking Artifact Registry repository..." -ForegroundColor Yellow
$repoExists = gcloud artifacts repositories describe $RepoName --location=$Region --quiet 2>$null
if (-not $repoExists) {
    Write-Host "Creating repository $RepoName..." -ForegroundColor Yellow
    gcloud artifacts repositories create $RepoName `
        --repository-format=docker `
        --location=$Region `
        --description="ElectroShop Docker repository" `
        --quiet
}

# Build Docker image
Write-Host "🏗️  Building Docker image..." -ForegroundColor Yellow
$ImageUri = "$Region-docker.pkg.dev/$ProjectId/$RepoName/${ImageName}:latest"
docker build -t $ImageUri .

# Push image
Write-Host "📤 Pushing image to Artifact Registry..." -ForegroundColor Yellow
docker push $ImageUri

# Deploy to Cloud Run
Write-Host "🚀 Deploying to Cloud Run..." -ForegroundColor Yellow
Write-Host "⚠️  Lưu ý: Nếu chưa tạo Cloud SQL instance, bỏ phần --add-cloudsql-instances" -ForegroundColor Yellow
Write-Host "    Hoặc tạo Cloud SQL instance trước khi deploy" -ForegroundColor Yellow
Write-Host ""

$CloudSqlInstance = "$ProjectId`:asia-southeast1:electroshop-db"
$AddCloudSql = Read-Host "Bạn đã tạo Cloud SQL instance chưa? (y/n)"

if ($AddCloudSql -eq "y" -or $AddCloudSql -eq "Y") {
    Write-Host "Deploying với Cloud SQL connection..." -ForegroundColor Cyan
    gcloud run deploy $ServiceName `
        --image $ImageUri `
        --platform managed `
        --region $Region `
        --allow-unauthenticated `
        --memory 512Mi `
        --cpu 1 `
        --min-instances 0 `
        --max-instances 10 `
        --set-env-vars ASPNETCORE_ENVIRONMENT=Production `
        --port 8080 `
        --add-cloudsql-instances $CloudSqlInstance
} else {
    Write-Host "Deploying không có Cloud SQL connection..." -ForegroundColor Cyan
    gcloud run deploy $ServiceName `
        --image $ImageUri `
        --platform managed `
        --region $Region `
        --allow-unauthenticated `
        --memory 512Mi `
        --cpu 1 `
        --min-instances 0 `
        --max-instances 10 `
        --set-env-vars ASPNETCORE_ENVIRONMENT=Production `
        --port 8080
}

# Get service URL
Write-Host "`n📡 Getting service URL..." -ForegroundColor Yellow
$ServiceUrl = gcloud run services describe $ServiceName --region=$Region --format="value(status.url)"

Write-Host "`n✅ Deploy thành công!" -ForegroundColor Green
Write-Host "🌐 URL: $ServiceUrl" -ForegroundColor Cyan
Write-Host "`n📝 Lưu ý:" -ForegroundColor Yellow
Write-Host "1. Cập nhật VnPay ReturnUrl trong appsettings.Production.json với URL trên"
Write-Host "2. Đảm bảo Cloud SQL instance đã được setup và connection string đúng"
Write-Host "3. Chạy migrations để tạo database schema"
Write-Host "`nĐể xem logs: gcloud run services logs read $ServiceName --region=$Region" -ForegroundColor Cyan

