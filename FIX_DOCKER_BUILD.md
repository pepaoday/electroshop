# 🔧 Sửa Lỗi Docker Build

## ❌ Lỗi: "docker build -t electroshop ." không chạy được

### Nguyên nhân và cách sửa:

---

## 1. Docker chưa được cài đặt

### Kiểm tra:
```bash
docker --version
```

Nếu báo lỗi: `'docker' is not recognized` → Docker chưa được cài đặt

### Cách sửa:

**Windows:**
1. Tải Docker Desktop: **https://www.docker.com/products/docker-desktop**
2. Cài đặt Docker Desktop
3. Khởi động lại máy
4. Mở Docker Desktop và đợi nó chạy (biểu tượng Docker ở system tray)
5. Chạy lại: `docker --version`

**Mac:**
```bash
brew install --cask docker
```

**Linux:**
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install docker.io
sudo systemctl start docker
sudo systemctl enable docker
```

---

## 2. Docker Desktop chưa chạy

### Kiểm tra:
- Xem có biểu tượng Docker ở system tray (Windows) không
- Hoặc chạy: `docker ps`

Nếu báo lỗi: `Cannot connect to the Docker daemon` → Docker Desktop chưa chạy

### Cách sửa:

**Windows:**
1. Mở Docker Desktop
2. Đợi Docker khởi động hoàn toàn (biểu tượng Docker ở system tray không còn spinning)
3. Chạy lại: `docker ps`

**Mac:**
1. Mở Docker Desktop từ Applications
2. Đợi Docker khởi động
3. Chạy lại: `docker ps`

---

## 3. Đang ở sai thư mục

### Kiểm tra:
Đảm bảo bạn đang ở thư mục chứa `Dockerfile` và `DoAnWebNC.csproj`

```bash
# Windows (PowerShell)
Get-Location
dir Dockerfile
dir *.csproj

# Mac/Linux
pwd
ls Dockerfile
ls *.csproj
```

### Cách sửa:

**Chuyển vào thư mục project:**
```bash
# Windows (PowerShell)
cd "C:\Users\baong\OneDrive\Máy tính\ElectroShop-master"

# Mac/Linux
cd /path/to/ElectroShop-master
```

Sau đó chạy lại:
```bash
docker build -t electroshop .
```

---

## 4. Thiếu file .csproj

### Kiểm tra:
```bash
# Windows
dir *.csproj

# Mac/Linux
ls *.csproj
```

Nếu không thấy file `DoAnWebNC.csproj` → File bị thiếu

### Cách sửa:
Đảm bảo file `DoAnWebNC.csproj` có trong thư mục project

---

## 5. Lỗi trong quá trình build

### Kiểm tra logs:
Khi chạy `docker build`, xem phần nào bị lỗi:

```bash
docker build -t electroshop . 2>&1 | tee build.log
```

### Các lỗi thường gặp:

#### Lỗi: "COPY failed: file not found"
**Nguyên nhân:** Thiếu file cần thiết
**Giải pháp:** Kiểm tra file `.csproj` có tồn tại không

#### Lỗi: "dotnet restore failed"
**Nguyên nhân:** Lỗi khi restore packages
**Giải pháp:** 
1. Kiểm tra internet connection
2. Thử build lại: `docker build --no-cache -t electroshop .`

#### Lỗi: "dotnet publish failed"
**Nguyên nhân:** Lỗi khi build project
**Giải pháp:**
1. Kiểm tra code có lỗi không: `dotnet build`
2. Sửa lỗi trước khi build Docker

---

## 6. Quyền truy cập (Linux/Mac)

### Kiểm tra:
```bash
docker ps
```

Nếu báo lỗi: `permission denied` → Cần quyền sudo

### Cách sửa:

**Linux:**
```bash
# Thêm user vào docker group
sudo usermod -aG docker $USER
# Đăng xuất và đăng nhập lại
```

**Mac:**
Thường không cần, nhưng nếu có lỗi, thử:
```bash
sudo docker build -t electroshop .
```

---

## ✅ Các bước kiểm tra nhanh:

### Bước 1: Kiểm tra Docker đã cài đặt
```bash
docker --version
```
**Kết quả mong đợi:** `Docker version 20.10.x, build ...`

### Bước 2: Kiểm tra Docker đang chạy
```bash
docker ps
```
**Kết quả mong đợi:** Danh sách containers (có thể trống)

### Bước 3: Kiểm tra đang ở đúng thư mục
```bash
# Windows
dir Dockerfile
dir DoAnWebNC.csproj

# Mac/Linux
ls Dockerfile
ls DoAnWebNC.csproj
```
**Kết quả mong đợi:** Thấy cả 2 file

### Bước 4: Build Docker image
```bash
docker build -t electroshop .
```
**Kết quả mong đợi:** Build thành công, thấy message "Successfully tagged electroshop:latest"

---

## 🔍 Debug chi tiết:

### Xem logs đầy đủ:
```bash
docker build -t electroshop . --progress=plain --no-cache
```

### Build từng bước:
```bash
# Build stage 1
docker build -t electroshop:build --target build .

# Build stage 2
docker build -t electroshop --target runtime .
```

### Kiểm tra image đã build:
```bash
docker images | grep electroshop
```

---

## 📝 Ví dụ build thành công:

```
[+] Building 45.2s (12/12) FINISHED
 => [internal] load build definition from Dockerfile
 => => transferring dockerfile: 2.45kB
 => [internal] load .dockerignore
 => => transferring context: 1.12kB
 => [internal] load metadata for mcr.microsoft.com/dotnet/aspnet:8.0
 => [internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0
 => [build 1/6] FROM mcr.microsoft.com/dotnet/sdk:8.0
 => [build 2/6] WORKDIR /src
 => [build 3/6] COPY *.csproj ./
 => [build 4/6] RUN dotnet restore
 => [build 5/6] COPY . ./
 => [build 6/6] RUN dotnet publish -c Release -o /app/publish
 => [runtime 1/4] FROM mcr.microsoft.com/dotnet/aspnet:8.0
 => [runtime 2/4] WORKDIR /app
 => [runtime 3/4] COPY --from=build /app/publish .
 => [runtime 4/4] EXPOSE 8080
 => exporting to image
 => => exporting layers
 => => writing image sha256:...
 => => naming to docker.io/library/electroshop
```

---

## 🆘 Nếu vẫn không được:

1. **Kiểm tra Docker Desktop:**
   - Mở Docker Desktop
   - Xem có lỗi gì không
   - Thử restart Docker Desktop

2. **Kiểm tra code:**
   - Chạy `dotnet build` trước
   - Sửa lỗi nếu có

3. **Xem logs chi tiết:**
   - Chạy build với `--progress=plain`
   - Copy toàn bộ output và tìm lỗi

4. **Thử build lại:**
   - Xóa image cũ: `docker rmi electroshop`
   - Build lại: `docker build -t electroshop .`

---

## 📚 Tài liệu tham khảo:

- Docker Desktop: https://www.docker.com/products/docker-desktop
- Docker Build: https://docs.docker.com/engine/reference/commandline/build/

---

**Nếu vẫn gặp vấn đề, vui lòng cung cấp:**
1. Output đầy đủ của lệnh `docker build`
2. Output của `docker --version`
3. Output của `docker ps`
4. Hệ điều hành (Windows/Mac/Linux)


