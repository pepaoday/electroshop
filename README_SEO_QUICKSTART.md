# 🚀 Hướng Dẫn Nhanh - Làm Website Hiện Trên Google

## Tóm tắt các bước

### 1️⃣ Deploy Website lên Cloud Run
```bash
# Xem hướng dẫn chi tiết trong HUONG_DAN_DEPLOY.md
./deploy.sh YOUR_PROJECT_ID
```

### 2️⃣ Đăng ký Google Search Console
1. Vào: https://search.google.com/search-console
2. Thêm website của bạn
3. Xác minh quyền sở hữu (dùng HTML tag trong _Layout.cshtml)

### 3️⃣ Submit Sitemap
1. Trong Google Search Console, vào **"Sơ đồ trang web"**
2. Submit: `sitemap.xml`
3. Google sẽ tự động crawl website

### 4️⃣ Yêu cầu Index
1. Vào **"Kiểm tra URL"**
2. Nhập URL trang chủ
3. Click **"Yêu cầu lập chỉ mục"**

### 5️⃣ Chờ đợi
- Thường mất 1-2 tuần để xuất hiện trên Google Search
- Kiểm tra bằng: `site:your-domain.run.app`

## ✅ Đã được tích hợp sẵn

- ✅ Meta tags SEO (title, description, keywords)
- ✅ Open Graph tags (Facebook, LinkedIn)
- ✅ Twitter Card tags
- ✅ Structured Data (JSON-LD)
- ✅ Sitemap động (tự động generate từ database)
- ✅ Robots.txt (cho phép Google crawl)
- ✅ Canonical URLs
- ✅ Mobile-friendly

## 📝 Xem hướng dẫn chi tiết

Xem file **HUONG_DAN_SEO.md** để biết chi tiết từng bước.

## 🔍 Kiểm tra nhanh

Sau khi deploy, kiểm tra:
- Sitemap: `https://your-domain.run.app/sitemap.xml`
- Robots.txt: `https://your-domain.run.app/robots.txt`
- Meta tags: View page source và kiểm tra phần `<head>`

---

**Lưu ý:** Website sẽ xuất hiện trên Google Search sau 1-2 tuần. Hãy kiên nhẫn! 😊

