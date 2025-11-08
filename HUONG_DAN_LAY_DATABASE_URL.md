# 📍 Cách Lấy Internal Database URL trên Render.com

## 🎯 Mục tiêu: Lấy connection string để set vào Environment Variable

---

## 📝 BƯỚC 1: Vào Database trên Render

1. **Đăng nhập Render Dashboard:**
   - Truy cập: **https://dashboard.render.com/**
   - Đăng nhập vào tài khoản của bạn

2. **Tìm Database:**
   - Trong sidebar bên trái, tìm section **"Databases"** hoặc **"PostgreSQL"**
   - Hoặc vào **"My Workspace"** > tìm database có tên **"electroshop-db"**
   - Click vào database đó

---

## 📝 BƯỚC 2: Tìm Internal Database URL

Sau khi vào trang database, bạn sẽ thấy các tab/thông tin sau:

### Cách 1: Tab "Info" hoặc "Overview"

1. Ở trang database, tìm tab **"Info"** hoặc **"Overview"**
2. Scroll xuống phần **"Connections"** hoặc **"Connection Information"**
3. Tìm dòng **"Internal Database URL"** hoặc **"Internal Connection String"**
4. Click vào icon **copy** (📋) bên cạnh để copy

### Cách 2: Tab "Connections"

1. Click tab **"Connections"** (nếu có)
2. Tìm phần **"Internal Database URL"**
3. Copy connection string

### Cách 3: Trong phần "Connection Information"

1. Trong trang database, tìm section **"Connection Information"**
2. Sẽ có 2 loại URL:
   - **Internal Database URL** ← **Dùng cái này!**
   - **External Database URL** ← Không dùng (chỉ để kết nối từ local)

---

## 📋 VÍ DỤ Internal Database URL

Format thường là:

```
postgresql://electroshop_user:password123@dpg-xxxxx-a.singapore-postgres.render.com/doanwebncdb
```

Hoặc:

```
postgresql://electroshop_user:password123@dpg-xxxxx-a.singapore-postgres.render.com:5432/doanwebncdb
```

**Các phần trong URL:**
- `postgresql://` - Protocol
- `electroshop_user` - Username
- `password123` - Password (Render tự tạo)
- `dpg-xxxxx-a.singapore-postgres.render.com` - Host (Internal)
- `5432` - Port (có thể có hoặc không)
- `doanwebncdb` - Database name

---

## ⚠️ LƯU Ý QUAN TRỌNG

### ✅ Dùng Internal Database URL
- **Internal Database URL** = Dùng cho app trên Render
- Format: `postgresql://user:pass@dpg-xxx.postgres.render.com/db`
- Chỉ hoạt động từ bên trong Render network

### ❌ KHÔNG dùng External Database URL
- **External Database URL** = Chỉ để kết nối từ máy local
- Format: `postgresql://user:pass@external-host:5432/db`
- Không hoạt động từ app trên Render

---

## 📝 BƯỚC 3: Copy và Set vào Environment Variable

Sau khi copy Internal Database URL:

1. **Vào Web Service "electroshop":**
   - Quay lại Dashboard
   - Click vào service **"electroshop"**

2. **Vào tab Environment:**
   - Click tab **"Environment"** ở sidebar
   - Hoặc vào **Settings** > **Environment**

3. **Thêm/Edit Environment Variable:**
   - Tìm biến `ConnectionStrings__DefaultConnection`
   - Nếu chưa có: Click **"Add Environment Variable"**
   - Nếu đã có: Click **"Edit"** (icon bút chì)
   - **Key:** `ConnectionStrings__DefaultConnection
   - **Value:** Paste Internal Database URL vừa copy
   - Click **"Save Changes"**

4. **Render sẽ tự động redeploy:**
   - Đợi 5-10 phút
   - Kiểm tra logs để xem kết quả

---

## 🖼️ HÌNH ẢNH MÔ TẢ (Text-based)

```
Render Dashboard
│
├── Databases
│   └── electroshop-db  ← Click vào đây
│       │
│       ├── Info Tab
│       │   └── Connections Section
│       │       ├── Internal Database URL  ← Copy cái này!
│       │       │   └── postgresql://user:pass@dpg-xxx.postgres.render.com/db
│       │       │
│       │       └── External Database URL  ← KHÔNG dùng
│       │
│       └── Connections Tab (nếu có)
│           └── Internal Database URL
```

---

## 🔍 NẾU KHÔNG TÌM THẤY Internal Database URL

### Trường hợp 1: Database mới tạo
- Đợi 2-3 phút để database khởi động xong
- Refresh lại trang
- Internal URL sẽ xuất hiện

### Trường hợp 2: Database chưa được tạo
- Tạo database trước:
  1. Click **"New +"** > **"PostgreSQL"**
  2. Name: `electroshop-db`
  3. Database: `DoAnWebNCDB`
  4. Plan: **Free**
  5. Region: Singapore
  6. Click **"Create Database"**

### Trường hợp 3: Giao diện khác
- Tìm phần **"Connection Information"** hoặc **"Connection Details"**
- Hoặc tìm icon **"🔗"** hoặc **"📋"** (copy)
- Internal URL thường có chữ **"Internal"** trong tên

---

## ✅ CHECKLIST

- [ ] Đã vào database `electroshop-db` trên Render
- [ ] Đã tìm thấy phần "Connections" hoặc "Connection Information"
- [ ] Đã copy **Internal Database URL** (không phải External)
- [ ] Đã vào Web Service "electroshop"
- [ ] Đã vào tab "Environment"
- [ ] Đã thêm/sửa biến `ConnectionStrings__DefaultConnection`
- [ ] Đã paste Internal Database URL vào Value
- [ ] Đã save changes
- [ ] Đang đợi Render redeploy

---

## 🆘 VẪN KHÔNG TÌM THẤY?

1. **Chụp màn hình** trang database và gửi cho tôi
2. Hoặc mô tả bạn thấy gì trên trang database
3. Tôi sẽ hướng dẫn cụ thể hơn

---

## 💡 MẸO NHỎ

- Internal URL thường có chữ **"dpg-"** trong hostname
- Internal URL thường có domain **".postgres.render.com"**
- Nếu thấy **"external"** hoặc **"public"** trong URL → Đó là External URL (không dùng)


