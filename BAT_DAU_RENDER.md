# 🚀 BẮT ĐẦU NGAY - Deploy lên Render.com (FREE)

## ⚡ Làm theo 6 bước này là xong!

---

## 📝 BƯỚC 1: Tạo tài khoản Render.com (2 phút)

1. Vào: **https://render.com/**
2. Click **"Get Started for Free"**
3. Đăng ký bằng **GitHub** (khuyến nghị)
4. Cho phép Render truy cập repositories

**✅ Xong!**

---

## 📝 BƯỚC 2: Tạo PostgreSQL Database (3 phút)

1. Vào Dashboard: **https://dashboard.render.com/**
2. Click **"New +"** > **"PostgreSQL"**
3. Điền thông tin:
   - **Name:** `electroshop-db`
   - **Database:** `DoAnWebNCDB`
   - **Plan:** **"Free"**
   - **Region:** Singapore
4. Click **"Create Database"**
5. **Lưu lại Internal Database URL** (sẽ dùng ở bước 5)

**✅ Xong!**

---

## 📝 BƯỚC 3: Cập nhật code (Đã được làm sẵn!)

Code đã được cập nhật để hỗ trợ PostgreSQL. Bạn chỉ cần:

1. **Pull code mới nhất** (nếu có)
2. **Restore packages:**
```bash
dotnet restore
```

**✅ Xong!**

---

## 📝 BƯỚC 4: Push code lên Git (2 phút)

```bash
git add .
git commit -m "Add PostgreSQL support for Render"
git push
```

**✅ Xong!**

---

## 📝 BƯỚC 5: Deploy Website (5 phút)

### 5.1. Tạo Web Service

1. Vào Dashboard: **https://dashboard.render.com/**
2. Click **"New +"** > **"Web Service"**
3. Chọn repository của bạn
4. Điền thông tin:
   - **Name:** `electroshop`
   - **Region:** Singapore
   - **Branch:** `main`
   - **Runtime:** `Docker`
   - **Instance Type:** **"Free"**
   - **Auto-Deploy:** `Yes`
5. Click **"Create Web Service"**

### 5.2. Cấu hình Environment Variables

Trong trang Web Service, vào tab **"Environment"**:

Thêm các biến sau:

```
ConnectionStrings__DefaultConnection = [Internal Database URL từ Bước 2]
ASPNETCORE_ENVIRONMENT = Production
VnPay__TmnCode = SJBLAJF0
VnPay__HashSecret = 3BY72RWVVTO43M9JEYSHVG9KHA1MA5TU
VnPay__BaseUrl = https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
VnPay__ReturnUrl = https://your-app-name.onrender.com/Order/PaymentCallBack
EmailSettings__SmtpServer = smtp.gmail.com
EmailSettings__SmtpPort = 587
EmailSettings__SenderEmail = nguyenminh01060210@gmail.com
EmailSettings__SenderPassword = dseh xfyl eplj uuxg
```

**Lưu ý:**
- Thay `[Internal Database URL]` bằng URL từ Bước 2
- Thay `your-app-name.onrender.com` bằng URL thực tế (sẽ có sau khi deploy)

Click **"Save Changes"**

### 5.3. Đợi Deploy

Render sẽ tự động build và deploy (mất 5-10 phút)

**✅ Xong!**

---

## 📝 BƯỚC 6: Kiểm tra (2 phút)

### 6.1. Truy cập Website

Sau khi deploy xong, bạn sẽ thấy URL:
```
https://your-app-name.onrender.com
```

Truy cập URL này trong trình duyệt

### 6.2. Kiểm tra Logs

1. Vào trang Web Service
2. Click tab **"Logs"**
3. Xem logs để kiểm tra có lỗi không

### 6.3. Test Website

- [ ] Trang chủ load được
- [ ] Đăng ký/Đăng nhập hoạt động
- [ ] Xem sản phẩm được
- [ ] Database hoạt động

**✅ Xong!**

---

## 🎉 HOÀN THÀNH!

Website của bạn đã được deploy lên Render.com!

### URL Website:
```
https://your-app-name.onrender.com
```

### Lưu ý về Free Tier:
- ⚠️ Website sẽ **sleep sau 15 phút** không có traffic
- ⚠️ Request đầu tiên sau khi sleep sẽ mất vài giây để wake up
- ✅ Đây là bình thường với free tier

---

## 🔧 Troubleshooting

### Lỗi: "Database connection failed"
- Kiểm tra Internal Database URL đúng chưa
- Kiểm tra environment variable `ConnectionStrings__DefaultConnection`

### Lỗi: "Build failed"
- Kiểm tra Dockerfile có đúng không
- Kiểm tra logs để xem lỗi cụ thể

### Website sleep
- Đây là bình thường với free tier
- Request đầu tiên sẽ mất vài giây để wake up

---

## 📝 Các bước tiếp theo:

1. ✅ Deploy website (Đã xong)
2. ⏭️ Đăng ký Google Search Console (Xem `CHECKLIST_NHANH.md`)
3. ⏭️ Submit sitemap (Xem `CHECKLIST_NHANH.md`)
4. ⏭️ Request indexing (Xem `CHECKLIST_NHANH.md`)

---

## 📚 Xem thêm

- **Hướng dẫn chi tiết:** `HUONG_DAN_RENDER.md`
- **Deploy nhanh:** `DEPLOY_RENDER_NHANH.md`
- **Checklist SEO:** `CHECKLIST_NHANH.md`

---

**Chúc bạn deploy thành công! 🚀**

Nếu gặp vấn đề, xem phần Troubleshooting hoặc file hướng dẫn chi tiết.

