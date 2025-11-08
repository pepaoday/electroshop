# ⚡ Sửa Lỗi Docker I/O Error - NHANH NHẤT

## ❌ Lỗi bạn gặp:
```
ERROR: input/output error
docker: request returned 502 Bad Gateway
```

## ✅ SỬA NGAY (3 bước):

### Bước 1: Restart Docker Desktop

1. **Đóng Docker Desktop:**
   - Click chuột phải vào biểu tượng Docker ở system tray
   - Chọn **"Quit Docker Desktop"**

2. **Mở lại Docker Desktop**
3. **Đợi nó khởi động xong** (biểu tượng không còn spinning)

---

### Bước 2: Xóa Cache và Build lại

Mở PowerShell và chạy:

```powershell
# Xóa cache
docker builder prune -a -f

# Build lại không dùng cache
docker build --no-cache -t electroshop .
```

---

### Bước 3: Nếu vẫn lỗi - Restart WSL2

Mở PowerShell as Administrator:

```powershell
wsl --shutdown
```

Đợi 10 giây, sau đó:
1. Mở lại Docker Desktop
2. Đợi khởi động xong
3. Chạy lại: `docker build --no-cache -t electroshop .`

---

## 🚀 HOẶC DÙNG SCRIPT TỰ ĐỘNG:

Chạy script này (PowerShell as Administrator):

```powershell
.\fix-docker.ps1
```

Sau đó làm theo hướng dẫn.

---

## ✅ Kiểm tra:

```bash
# Kiểm tra Docker hoạt động
docker ps

# Build lại
docker build --no-cache -t electroshop .
```

---

**Nếu vẫn lỗi, xem file `FIX_DOCKER_IO_ERROR.md` để biết thêm cách sửa!**

