# 🔧 Sửa Lỗi Deploy trên Render.com

## ❌ Lỗi hiện tại:
- **Status:** Failed (Exit code 139)
- **Nguyên nhân:** App đang cố kết nối SQL Server thay vì PostgreSQL
- **Logs:** `SqlServerConnection.OpenDbConnection` errors

---

## ✅ GIẢI PHÁP: 3 bước đơn giản

### 🔍 BƯỚC 1: Kiểm tra Environment Variable trên Render (QUAN TRỌNG NHẤT)

1. **Vào Render Dashboard:**
   - Truy cập: **https://dashboard.render.com/**
   - Chọn service **"electroshop"**

2. **Vào tab Environment:**
   - Click tab **"Environment"** ở sidebar bên trái
   - Hoặc vào **Settings** > **Environment**

3. **Kiểm tra biến `ConnectionStrings__DefaultConnection`:**
   - Tìm biến có key: `ConnectionStrings__DefaultConnection`
   - **Nếu CHƯA CÓ hoặc giá trị RỖNG:**
     - Click **"Add Environment Variable"**
     - Key: `ConnectionStrings__DefaultConnection`
     - Value: Copy **Internal Database URL** từ database (xem Bước 2)

4. **Lấy Internal Database URL:**
   - Vào database **"electroshop-db"** trên Render
   - Tìm phần **"Connections"** hoặc **"Internal Database URL"**
   - Copy toàn bộ connection string
   - Format thường là: `postgresql://user:password@dpg-xxxxx-a.singapore-postgres.render.com/database`

---

### 🗄️ BƯỚC 2: Kiểm tra Database đã tạo chưa

1. **Vào Render Dashboard**
2. **Kiểm tra có database `electroshop-db` chưa:**
   - Nếu CHƯA CÓ:
     - Click **"New +"** > **"PostgreSQL"**
     - Name: `electroshop-db`
     - Database: `DoAnWebNCDB`
     - Plan: **Free**
     - Region: Singapore
     - Click **"Create Database"**
     - Đợi 2-3 phút để database khởi động

3. **Lấy Internal Database URL:**
   - Vào database vừa tạo
   - Tìm **"Internal Database URL"**
   - Copy toàn bộ (sẽ dùng ở Bước 1)

---

### 📤 BƯỚC 3: Commit và Push code mới

Code đã được cập nhật để:
- ✅ Detect PostgreSQL tốt hơn (nhiều pattern hơn)
- ✅ Log connection string để debug
- ✅ Xử lý lỗi tốt hơn

**Chạy các lệnh sau:**

```bash
cd eee
git add .
git commit -m "Improve PostgreSQL detection and add debug logging"
git push
```

**Sau khi push:**
- Render sẽ tự động deploy lại
- Đợi 5-10 phút
- Xem logs để kiểm tra

---

## 🔍 KIỂM TRA SAU KHI DEPLOY

### 1. Xem Logs trên Render:

1. Vào service **"electroshop"**
2. Click tab **"Logs"**
3. Tìm các dòng:
   - `[DEBUG] Connection string detected: ...` - Xem connection string có đúng không
   - `[DEBUG] Using PostgreSQL database provider` - Phải thấy dòng này!
   - Nếu thấy `Using SQL Server` → Environment variable chưa đúng

### 2. Kiểm tra kết quả:

**✅ THÀNH CÔNG nếu thấy:**
- `[DEBUG] Using PostgreSQL database provider`
- `Migration error: ...` (có thể có, nhưng app vẫn chạy)
- App start thành công
- Không còn lỗi `SqlServerConnection`

**❌ VẪN LỖI nếu thấy:**
- `Using SQL Server database provider` → Environment variable chưa set đúng
- `Connection string is not configured` → Chưa có environment variable
- `SqlServerConnection` errors → Vẫn đang dùng SQL Server

---

## 🐛 TROUBLESHOOTING

### Lỗi: "Connection string is not configured"
**Nguyên nhân:** Environment variable chưa được set
**Giải pháp:** 
1. Vào Settings > Environment
2. Thêm `ConnectionStrings__DefaultConnection`
3. Value = Internal Database URL từ database

### Lỗi: Vẫn thấy "Using SQL Server"
**Nguyên nhân:** Connection string không match pattern PostgreSQL
**Giải pháp:**
1. Kiểm tra Internal Database URL có đúng format không
2. Phải có: `postgresql://` hoặc `dpg-` hoặc `.postgres.render.com`
3. Nếu không có, copy lại từ database

### Lỗi: "Database connection failed"
**Nguyên nhân:** Database chưa ready hoặc connection string sai
**Giải pháp:**
1. Đợi database khởi động xong (2-3 phút sau khi tạo)
2. Kiểm tra database status = "Available"
3. Copy lại Internal Database URL

### Lỗi: "Migration failed"
**Nguyên nhân:** Database schema chưa được tạo
**Giải pháp:**
- Code đã có fallback, app vẫn sẽ chạy
- Nhưng cần chạy migrations thủ công nếu cần

---

## 📝 CHECKLIST NHANH

- [ ] Database `electroshop-db` đã được tạo
- [ ] Environment variable `ConnectionStrings__DefaultConnection` đã được set
- [ ] Value = Internal Database URL từ database
- [ ] Code mới đã được push lên Git
- [ ] Render đã deploy lại
- [ ] Logs hiển thị "Using PostgreSQL database provider"
- [ ] App start thành công

---

## 💡 LƯU Ý QUAN TRỌNG

1. **Internal Database URL** khác với External URL
   - Phải dùng **Internal** cho app trên Render
   - External chỉ dùng để kết nối từ local

2. **Environment variable format:**
   - Key: `ConnectionStrings__DefaultConnection` (2 dấu gạch dưới)
   - Value: Toàn bộ connection string từ Render

3. **Sau khi sửa environment variable:**
   - Render sẽ tự động redeploy
   - Đợi 5-10 phút
   - Kiểm tra logs

4. **Nếu dùng render.yaml:**
   - File đã cấu hình sẵn `fromDatabase`
   - Nhưng vẫn cần kiểm tra database name đúng: `electroshop-db`

---

## 🆘 VẪN KHÔNG ĐƯỢC?

1. **Xem logs chi tiết:**
   - Copy toàn bộ logs
   - Tìm dòng `[DEBUG]` để xem connection string

2. **Kiểm tra lại:**
   - Database status
   - Environment variables
   - Code đã push chưa

3. **Thử manual deploy:**
   - Vào service
   - Click "Manual Deploy" > "Deploy latest commit"




