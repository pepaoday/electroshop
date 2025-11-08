# Script để sửa lỗi Docker I/O Error
# Chạy PowerShell as Administrator

Write-Host "🔧 Đang sửa lỗi Docker I/O Error..." -ForegroundColor Yellow
Write-Host ""

# Bước 1: Kiểm tra Docker có chạy không
Write-Host "Bước 1: Kiểm tra Docker..." -ForegroundColor Cyan
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker đã cài đặt: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker chưa được cài đặt hoặc chưa được thêm vào PATH" -ForegroundColor Red
    Write-Host "Vui lòng cài đặt Docker Desktop từ: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    exit 1
}

# Bước 2: Xóa Docker cache
Write-Host ""
Write-Host "Bước 2: Xóa Docker cache..." -ForegroundColor Cyan
try {
    docker builder prune -a -f
    Write-Host "✅ Đã xóa Docker cache" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Không thể xóa cache (có thể Docker đang chạy)" -ForegroundColor Yellow
}

# Bước 3: Xóa images không dùng
Write-Host ""
Write-Host "Bước 3: Xóa images không dùng..." -ForegroundColor Cyan
try {
    docker image prune -a -f
    Write-Host "✅ Đã xóa images không dùng" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Không thể xóa images" -ForegroundColor Yellow
}

# Bước 4: Restart WSL2 (nếu có)
Write-Host ""
Write-Host "Bước 4: Restart WSL2..." -ForegroundColor Cyan
try {
    wsl --shutdown
    Write-Host "✅ Đã shutdown WSL2" -ForegroundColor Green
    Write-Host "⏳ Đợi 10 giây..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
} catch {
    Write-Host "⚠️  WSL2 không được cài đặt hoặc đã được shutdown" -ForegroundColor Yellow
}

# Bước 5: Hướng dẫn
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Hoàn thành!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Các bước tiếp theo:" -ForegroundColor Yellow
Write-Host "1. Mở Docker Desktop" -ForegroundColor White
Write-Host "2. Đợi Docker khởi động hoàn toàn (biểu tượng không còn spinning)" -ForegroundColor White
Write-Host "3. Chạy lại: docker build -t electroshop ." -ForegroundColor White
Write-Host ""
Write-Host "Nếu vẫn lỗi, thử:" -ForegroundColor Yellow
Write-Host "docker build --no-cache -t electroshop ." -ForegroundColor White
Write-Host ""

