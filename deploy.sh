#!/bin/bash

# Script deploy tự động lên Google Cloud Run
# Sử dụng: ./deploy.sh YOUR_PROJECT_ID

set -e

PROJECT_ID=$1
REGION="asia-southeast1"
IMAGE_NAME="electroshop"
SERVICE_NAME="electroshop"
REPO_NAME="electroshop-repo"

if [ -z "$PROJECT_ID" ]; then
    echo "❌ Vui lòng cung cấp Project ID"
    echo "Sử dụng: ./deploy.sh YOUR_PROJECT_ID"
    exit 1
fi

echo "🚀 Bắt đầu deploy ElectroShop lên Google Cloud Run..."
echo "Project ID: $PROJECT_ID"
echo "Region: $REGION"

# Set project
echo "📋 Setting project..."
gcloud config set project $PROJECT_ID

# Enable APIs
echo "🔧 Enabling required APIs..."
gcloud services enable run.googleapis.com --quiet
gcloud services enable artifactregistry.googleapis.com --quiet
gcloud services enable sqladmin.googleapis.com --quiet

# Configure Docker
echo "🐳 Configuring Docker..."
gcloud auth configure-docker $REGION-docker.pkg.dev --quiet

# Create Artifact Registry repository if not exists
echo "📦 Checking Artifact Registry repository..."
if ! gcloud artifacts repositories describe $REPO_NAME --location=$REGION --quiet 2>/dev/null; then
    echo "Creating repository $REPO_NAME..."
    gcloud artifacts repositories create $REPO_NAME \
        --repository-format=docker \
        --location=$REGION \
        --description="ElectroShop Docker repository" \
        --quiet
fi

# Build Docker image
echo "🏗️  Building Docker image..."
IMAGE_URI="$REGION-docker.pkg.dev/$PROJECT_ID/$REPO_NAME/$IMAGE_NAME:latest"
docker build -t $IMAGE_URI .

# Push image
echo "📤 Pushing image to Artifact Registry..."
docker push $IMAGE_URI

# Deploy to Cloud Run
echo "🚀 Deploying to Cloud Run..."
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
    --port 8080

# Get service URL
SERVICE_URL=$(gcloud run services describe $SERVICE_NAME --region=$REGION --format="value(status.url)")

echo ""
echo "✅ Deploy thành công!"
echo "🌐 URL: $SERVICE_URL"
echo ""
echo "📝 Lưu ý:"
echo "1. Cập nhật VnPay ReturnUrl trong appsettings.Production.json với URL trên"
echo "2. Đảm bảo Cloud SQL instance đã được setup và connection string đúng"
echo "3. Chạy migrations để tạo database schema"
echo ""
echo "Để xem logs: gcloud run services logs read $SERVICE_NAME --region=$REGION"

