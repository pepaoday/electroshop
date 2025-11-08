# 🔧 Sửa Lỗi Docker I/O Error

## ❌ Lỗi bạn gặp phải:

```
ERROR: failed to build: failed to solve: failed to compute cache key: 
failed commit on ref "layer-sha256:...": commit failed: sync failed: 
sync /var/lib/desktop-containerd/daemon/.../data: input/output error
```

Và:
```
docker: request returned 502 Bad Gateway for API route
```

## 🔍 Nguyên nhân:

1. **Docker Desktop không ổn định** - Daemon bị crash
2. **Disk I/O error** - Lỗi đọc/ghi disk
3. **WSL2 có vấn đề** (Windows)
4. **Disk space đầy**
5. **Docker cache bị corrupt**

---

## ✅ CÁCH SỬA (Làm theo thứ tự):

### Cách 1: Restart Docker Desktop (Thử đầu tiên)

1. **Đóng Docker Desktop hoàn toàn:**
   - Click chuột phải vào biểu tượng Docker ở system tray
   - Chọn **"Quit Docker Desktop"**

2. **Mở lại Docker Desktop:**
   - Mở Docker Desktop
   - Đợi nó khởi động hoàn toàn (biểu tượng không còn spinning)

3. **Thử build lại:**
   ```bash
   docker build -t electroshop .
   ```

---

### Cách 2: Xóa Docker Cache và Build lại

1. **Xóa build cache:**
   ```bash
   docker builder prune -a -f
   ```

2. **Xóa các image không dùng:**
   ```bash
   docker image prune -a -f
   ```

3. **Build lại không dùng cache:**
   ```bash
   docker build --no-cache -t electroshop .
   ```

---

### Cách 3: Restart WSL2 (Windows)

1. **Mở PowerShell as Administrator**

2. **Restart WSL2:**
   ```powershell
   wsl --shutdown
   ```

3. **Đợi 10 giây, sau đó mở lại Docker Desktop**

4. **Thử build lại:**
   ```bash
   docker build -t electroshop .
   ```

---

### Cách 4: Kiểm tra Disk Space

1. **Kiểm tra dung lượng disk:**
   ```powershell
   # Windows
   Get-PSDrive C
   ```

2. **Nếu disk đầy (>90%), giải phóng dung lượng:**
   - Xóa các file không cần thiết
   - Xóa Docker images cũ:
     ```bash
     docker system prune -a -f
     ```

---

### Cách 5: Reset Docker Desktop (Nếu vẫn lỗi)

**⚠️ CẢNH BÁO: Cách này sẽ xóa tất cả Docker images và containers!**

1. **Mở Docker Desktop**

2. **Vào Settings:**
   - Click biểu tượng ⚙️ (Settings)
   - Vào **"Troubleshoot"**

3. **Click "Clean / Purge data"** hoặc **"Reset to factory defaults"**

4. **Restart Docker Desktop**

5. **Thử build lại:**
   ```bash
   docker build -t electroshop .
   ```

---

### Cách 6: Cập nhật Docker Desktop

1. **Kiểm tra phiên bản:**
   ```bash
   docker --version
   ```

2. **Tải phiên bản mới nhất:**
   - https://www.docker.com/products/docker-desktop

3. **Cài đặt và restart**

---

### Cách 7: Thay đổi Docker Engine Storage Driver

1. **Mở Docker Desktop**

2. **Vào Settings > Docker Engine**

3. **Thêm cấu hình:**
   ```json
   {
     "storage-driver": "overlay2"
   }
   ```

4. **Apply & Restart**

---

## 🚀 CÁCH NHANH NHẤT (Khuyến nghị):

### Bước 1: Restart Docker Desktop
```powershell
# Đóng Docker Desktop
# Mở lại Docker Desktop
# Đợi nó khởi động xong
```

### Bước 2: Xóa cache và build lại
```bash
# Xóa cache
docker builder prune -a -f

# Build lại không dùng cache
docker build --no-cache -t electroshop .
```

### Bước 3: Nếu vẫn lỗi, restart WSL2
```powershell
# PowerShell as Administrator
wsl --shutdown
# Đợi 10 giây
# Mở lại Docker Desktop
```

---

## 🔍 KIỂM TRA SAU KHI SỬA:

### 1. Kiểm tra Docker hoạt động:
```bash
docker ps
```

**Kết quả mong đợi:** Danh sách containers (có thể trống, nhưng không báo lỗi)

### 2. Kiểm tra Docker info:
```bash
docker info
```

**Kết quả mong đợi:** Thông tin về Docker, không có lỗi

### 3. Thử build lại:
```bash
docker build -t electroshop .
```

**Kết quả mong đợi:** Build thành công

---

## 📝 LƯU Ý:

1. **Build lần đầu sẽ mất thời gian** (5-10 phút) vì cần download base images
2. **Đảm bảo internet ổn định** khi build
3. **Không tắt Docker Desktop** trong khi build
4. **Kiểm tra disk space** trước khi build

---

## 🆘 NẾU VẪN KHÔNG ĐƯỢC:

### 1. Kiểm tra logs Docker Desktop:
- Mở Docker Desktop
- Vào **"Troubleshoot"** > **"View logs"**
- Xem có lỗi gì không

### 2. Thử build với verbose mode:
```bash
docker build --progress=plain --no-cache -t electroshop . 2>&1 | tee build.log
```

### 3. Kiểm tra Windows Event Viewer:
- Mở Event Viewer
- Xem có lỗi liên quan đến Docker/WSL không

### 4. Uninstall và cài lại Docker Desktop:
- Uninstall Docker Desktop
- Xóa thư mục: `C:\Users\%USERNAME%\.docker`
- Cài lại Docker Desktop

---

## ✅ BUILD THÀNH CÔNG SẼ THẤY:

```
[+] Building 45.2s (12/12) FINISHED
 => [internal] load build definition from Dockerfile
 => [internal] load .dockerignore
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
 => => writing image sha256:...
 => => naming to docker.io/library/electroshop
```

---

**Chúc bạn build thành công! 🚀**

Nếu vẫn gặp vấn đề, thử các cách trên theo thứ tự hoặc hỏi trong Docker community.

