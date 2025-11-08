# 🗄️ Hướng Dẫn Tạo PostgreSQL Database trên Render.com

## 🎯 Mục tiêu: Tạo database miễn phí để deploy app

---

## 📝 BƯỚC 1: Đăng nhập Render Dashboard

1. **Truy cập:** https://dashboard.render.com/
2. **Đăng nhập** bằng tài khoản của bạn
   - Nếu chưa có tài khoản: Click **"Get Started for Free"** và đăng ký

---

## 📝 BƯỚC 2: Tạo PostgreSQL Database

1. **Trong Dashboard, tìm nút "New +":**
   - Ở góc trên bên trái màn hình
   - Hoặc ở sidebar bên trái
   - Click vào **"New +"**

2. **Chọn "PostgreSQL":**
   - Menu dropdown sẽ hiện ra
   - Click **"PostgreSQL"** (hoặc tìm trong danh sách)

3. **Điền thông tin database:**
   
   **Name:**
   - Nhập: `electroshop-db`
   - (Hoặc tên bạn muốn, nhưng phải khớp với tên trong `render.yaml`)

   **Database:**
   - Nhập: `DoAnWebNCDB`
   - (Tên database bên trong PostgreSQL)

   **User:**
   - Có thể để mặc định: `electroshop_user`
   - Hoặc Render sẽ tự tạo user

   **Region:**
   - Chọn: **Singapore** (hoặc region gần nhất với bạn)
   - ⚠️ **QUAN TRỌNG:** Phải chọn cùng region với Web Service sau này!

   **PostgreSQL Version:**
   - Chọn version mới nhất (thường là 15 hoặc 16)
   - Hoặc để mặc định

   **Plan:**
   - Chọn **"Free"** (Free tier - miễn phí)
   - ⚠️ Free tier có giới hạn 1GB storage, nhưng đủ cho project nhỏ

4. **Click "Create Database":**
   - Render sẽ bắt đầu tạo database
   - Đợi 2-5 phút để database khởi động

---

## 📝 BƯỚC 3: Đợi Database Khởi Động

Sau khi click "Create Database":

1. **Bạn sẽ thấy trang database:**
   - Status: **"Creating"** hoặc **"Provisioning"**
   - Đợi status chuyển thành **"Available"** hoặc **"Ready"**

2. **Thời gian chờ:**
   - Thường mất **2-5 phút**
   - Có thể lâu hơn nếu Render đang bận

3. **Khi status = "Available":**
   - Database đã sẵn sàng!
   - Có thể lấy connection string

---

## 📝 BƯỚC 4: Lấy Internal Database URL

Sau khi database status = "Available":

1. **Trong trang database, tìm phần "Connections":**
   - Scroll xuống
   - Tìm section **"Connection Information"** hoặc **"Connections"**

2. **Tìm "Internal Database URL":**
   - Sẽ có 2 loại URL:
     - ✅ **Internal Database URL** ← **Dùng cái này!**
     - ❌ **External Database URL** ← Không dùng

3. **Copy Internal Database URL:**
   - Click icon **copy** (📋) bên cạnh
   - Hoặc click vào URL và copy
   - Format: `postgresql://user:password@dpg-xxxxx.postgres.render.com/database`

---

## 📝 BƯỚC 5: Set Environment Variable cho Web Service

Sau khi có Internal Database URL:

1. **Quay lại Dashboard:**
   - Click **"My Workspace"** hoặc logo Render ở góc trên

2. **Vào Web Service "electroshop":**
   - Tìm service **"electroshop"** trong danh sách
   - Click vào service đó
   - (Nếu chưa có service, xem Bước 6)

3. **Vào tab "Environment":**
   - Click tab **"Environment"** ở sidebar bên trái
   - Hoặc vào **Settings** > **Environment**

4. **Thêm Environment Variable:**
   - Click **"Add Environment Variable"**
   - **Key:** `ConnectionStrings__DefaultConnection`
   - **Value:** Paste Internal Database URL vừa copy
   - Click **"Save Changes"**

5. **Render sẽ tự động redeploy:**
   - Đợi 5-10 phút
   - Kiểm tra logs để xem kết quả

---

## 📝 BƯỚC 6: Tạo Web Service (Nếu chưa có)

Nếu bạn chưa tạo Web Service:

1. **Click "New +"** > **"Web Service"**

2. **Chọn Repository:**
   - Kết nối GitHub/GitLab/Bitbucket
   - Chọn repository chứa code của bạn

3. **Điền thông tin:**
   - **Name:** `electroshop`
   - **Region:** **Singapore** (phải giống với database!)
   - **Branch:** `main` (hoặc branch bạn muốn deploy)
   - **Root Directory:** (Để trống nếu code ở root)
   - **Runtime:** `Docker` (Render sẽ tự detect Dockerfile)
   - **Instance Type:** **Free**
   - **Auto-Deploy:** `Yes`

4. **Click "Create Web Service"**

5. **Sau khi tạo xong:**
   - Làm theo Bước 5 để set environment variable

---

## ✅ CHECKLIST

- [ ] Đã đăng nhập Render Dashboard
- [ ] Đã click "New +" > "PostgreSQL"
- [ ] Đã điền Name: `electroshop-db`
- [ ] Đã điền Database: `DoAnWebNCDB`
- [ ] Đã chọn Region: Singapore
- [ ] Đã chọn Plan: Free
- [ ] Đã click "Create Database"
- [ ] Đã đợi database status = "Available"
- [ ] Đã copy Internal Database URL
- [ ] Đã vào Web Service "electroshop"
- [ ] Đã vào tab "Environment"
- [ ] Đã thêm biến `ConnectionStrings__DefaultConnection`
- [ ] Đã paste Internal Database URL vào Value
- [ ] Đã save changes
- [ ] Đang đợi Render redeploy

---

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. Region phải giống nhau
- Database và Web Service phải cùng region
- Nếu database ở Singapore, service cũng phải ở Singapore
- Nếu khác region → Không kết nối được!

### 2. Dùng Internal URL, không dùng External
- ✅ **Internal Database URL** = Cho app trên Render
- ❌ **External Database URL** = Chỉ cho kết nối từ local

### 3. Free tier có giới hạn
- 1GB storage
- Database có thể sleep sau 90 ngày không dùng
- Đủ cho project nhỏ và học tập

### 4. Database name trong render.yaml
- Nếu dùng `render.yaml`, database name phải khớp
- Trong file `render.yaml` có: `name: electroshop-db`
- Nên đặt tên database là `electroshop-db`

---

## 🐛 TROUBLESHOOTING

### Database không tạo được
- **Nguyên nhân:** Có thể đã hết quota free tier
- **Giải pháp:** 
  - Kiểm tra xem đã có database free nào chưa
  - Render free tier chỉ cho 1 PostgreSQL database
  - Xóa database cũ nếu không dùng

### Không thấy "New +" button
- **Nguyên nhân:** Chưa đăng nhập hoặc chưa có quyền
- **Giải pháp:** 
  - Đăng nhập lại
  - Kiểm tra tài khoản có quyền tạo service không

### Database status mãi "Creating"
- **Nguyên nhân:** Render đang bận hoặc có lỗi
- **Giải pháp:** 
  - Đợi thêm 5-10 phút
  - Refresh trang
  - Nếu vẫn không được, thử tạo lại

### Không tìm thấy Internal Database URL
- **Nguyên nhân:** Database chưa khởi động xong
- **Giải pháp:** 
  - Đợi status = "Available"
  - Refresh trang
  - Tìm trong tab "Info" hoặc "Connections"

---

## 📞 CẦN HỖ TRỢ?

Nếu gặp vấn đề:
1. Chụp màn hình trang database
2. Mô tả bạn thấy gì
3. Tôi sẽ hướng dẫn cụ thể hơn

---

## 🎯 TÓM TẮT NHANH

1. **Dashboard** → Click **"New +"** → **"PostgreSQL"**
2. **Điền:** Name = `electroshop-db`, Database = `DoAnWebNCDB`, Region = Singapore, Plan = Free
3. **Click "Create Database"** → Đợi 2-5 phút
4. **Copy Internal Database URL** khi status = "Available"
5. **Vào Web Service** → Tab "Environment" → Thêm `ConnectionStrings__DefaultConnection` = Internal URL
6. **Done!** Render sẽ tự động deploy lại


