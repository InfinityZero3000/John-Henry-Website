# 🚀 Quick Start: Import Banners

## 1️⃣ Import Banners (Chọn 1 trong 3 cách)

### Cách 1: SQL Script (Khuyến nghị - Nhanh nhất)
```bash
psql -h localhost -U postgres -d johnhenry_db -f Scripts/SeedBanners.sql
```

### Cách 2: pgAdmin
1. Mở pgAdmin
2. Connect tới `johnhenry_db`
3. Mở Query Tool
4. Copy paste nội dung `Scripts/SeedBanners.sql`
5. Execute (F5)

### Cách 3: C# Script
- Uncomment code trong `Program.cs`
- Hoặc tạo endpoint API temporary
- Chi tiết xem `Scripts/SeedBannersScript.cs`

## 2️⃣ Kiểm Tra Import Thành Công

```sql
-- Kiểm tra tổng số banners
SELECT COUNT(*) FROM "MarketingBanners";
-- Expected: 24 banners

-- Xem chi tiết
SELECT "Position", "TargetPage", COUNT(*) 
FROM "MarketingBanners"
GROUP BY "Position", "TargetPage"
ORDER BY "Position";
```

## 3️⃣ Test Trên Website

### Khởi động lại app:
```bash
# Stop current app (Ctrl+C)
dotnet run
```

### Truy cập các trang:
- 🏠 **Trang chủ**: http://localhost:5101/
  - 3 banners carousel (home_main)
  - 2 banners nhỏ (home_side)

- 👔 **John Henry**: http://localhost:5101/Home/JohnHenry
  - 4 banners carousel

- 👗 **Freelancer**: http://localhost:5101/Home/Freelancer
  - 4 banners carousel

- ⚙️ **Admin Panel**: http://localhost:5101/admin/banners
  - Quản lý tất cả banners

## 4️⃣ Quản Lý Banners Qua Admin

### Truy cập Admin Panel:
```
http://localhost:5101/admin/banners
```

### Chức năng:
- ➕ Thêm banner mới
- ✏️ Sửa banner
- 🗑️ Xóa banner
- 🔄 Bật/Tắt banner
- 🎯 Filter theo vị trí
- 📊 Xem thống kê

## 📍 Vị Trí Banners

| Vị Trí | Position | TargetPage | Số Lượng |
|--------|----------|------------|----------|
| Trang chủ - Carousel | home_main | null | 3 |
| Trang chủ - Small | home_side | null | 2 |
| John Henry | collection_hero | JohnHenry | 4 |
| Freelancer | collection_hero | Freelancer | 4 |
| Best Seller | collection_hero | BestSeller | 2 |
| Categories | category_banner | Various | 8 |
| Blog | page_hero | Blog | 1 |

**Tổng: 24 banners**

## 🛠️ Troubleshooting

### Banners không hiển thị?
1. ✅ Check `IsActive = true`
2. ✅ Check `StartDate <= NOW()`
3. ✅ Check `EndDate IS NULL OR EndDate >= NOW()`
4. ✅ Check `Position` và `TargetPage` đúng
5. ✅ Clear cache browser (Ctrl+Shift+R)

### Ảnh không load?
1. ✅ Check file tồn tại: `/wwwroot/images/Banner/`
2. ✅ Check đường dẫn: `/images/Banner/banner-xxx.jpg`
3. ✅ Check permissions thư mục

### Error khi import?
```sql
-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
```

## 📁 Files Quan Trọng

### Scripts:
- `Scripts/SeedBanners.sql` - SQL import script
- `Scripts/SeedBannersScript.cs` - C# alternative
- `Scripts/BANNER_IMPORT_GUIDE.md` - Documentation

### Controllers:
- `Controllers/HomeController.cs` - Load banners cho pages

### Views:
- `Views/Home/Index.cshtml` - Homepage banners
- `Views/Home/JohnHenry.cshtml` - John Henry banners
- `Views/Home/Freelancer.cshtml` - Freelancer banners
- `Views/Admin/Banners.cshtml` - Admin management

### Summary:
- `BANNER_IMPORT_SUMMARY.md` - Chi tiết đầy đủ

## ✅ Checklist

- [ ] Import banners vào database
- [ ] Verify 24 banners imported
- [ ] Restart application
- [ ] Test trang chủ
- [ ] Test John Henry page
- [ ] Test Freelancer page
- [ ] Test admin panel
- [ ] Thử thêm/sửa/xóa banner qua admin

## 🎯 Kết Quả Mong Đợi

✅ **Trang chủ**: 3 banners carousel + 2 small banners
✅ **John Henry**: 4 banners carousel
✅ **Freelancer**: 4 banners carousel
✅ **Admin panel**: Full CRUD operations
✅ **Responsive**: Mobile + Desktop
✅ **Dynamic**: Load từ database

---

**Done!** 🎉 Hệ thống banner động đã sẵn sàng!
