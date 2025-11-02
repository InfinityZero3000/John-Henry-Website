# Banner Import System - Summary

## ✅ Công Việc Đã Hoàn Thành

### 1. Tạo Script Import Banners
- **SQL Script**: `Scripts/SeedBanners.sql` - Script PostgreSQL để import banners
- **C# Script**: `Scripts/SeedBannersScript.cs` - Alternative C# implementation
- **Documentation**: `Scripts/BANNER_IMPORT_GUIDE.md` - Hướng dẫn chi tiết

### 2. Cập Nhật Controllers

#### HomeController.cs
- **Index Action**: Đã có sẵn code load banners
  - `home_main` → ViewBag.HeroCarouselBanners
  - `home_side` → ViewBag.SmallBanners

- **JohnHenry Action**: ✅ Đã thêm code load banners
  ```csharp
  var collectionBanners = await _context.MarketingBanners
      .Where(b => b.IsActive 
          && b.Position == "collection_hero"
          && b.TargetPage == "JohnHenry"
          && b.StartDate <= now
          && (b.EndDate == null || b.EndDate >= now))
      .OrderBy(b => b.SortOrder)
      .ToListAsync();
  ViewBag.CollectionBanners = collectionBanners;
  ```

- **Freelancer Action**: ✅ Đã thêm code load banners
  ```csharp
  var collectionBanners = await _context.MarketingBanners
      .Where(b => b.IsActive 
          && b.Position == "collection_hero"
          && b.TargetPage == "Freelancer"
          && b.StartDate <= now
          && (b.EndDate == null || b.EndDate >= now))
      .OrderBy(b => b.SortOrder)
      .ToListAsync();
  ViewBag.CollectionBanners = collectionBanners;
  ```

### 3. Cập Nhật Views

#### Views/Home/JohnHenry.cshtml
✅ Đã cập nhật để load banners động từ database:
- Kiểm tra `ViewBag.CollectionBanners`
- Nếu có 1 banner → Hiển thị banner đơn
- Nếu có nhiều banners → Hiển thị carousel với controls
- Fallback → Banner mặc định `banner-man-main.jpg`

#### Views/Home/Freelancer.cshtml
✅ Đã cập nhật để load banners động từ database:
- Kiểm tra `ViewBag.CollectionBanners`
- Nếu có 1 banner → Hiển thị banner đơn
- Nếu có nhiều banners → Hiển thị carousel với controls
- Fallback → Banner mặc định `banner-women-0.jpg`

#### Views/Home/Index.cshtml
✅ Đã có sẵn code load banners động:
- Hero carousel: `ViewBag.HeroCarouselBanners`
- Small banners: `ViewBag.SmallBanners`
- Tự động fallback về banners mặc định nếu không có data

## 📊 Banners Sẽ Được Import

### Trang Chủ (/)
| Position | Count | Files |
|----------|-------|-------|
| home_main | 3 | banner-home-1.jpg, banner-home-2.jpg, banner-home-3.jpg |
| home_side | 2 | web-01.jpg, web-02.jpg |

### Trang John Henry (/Home/JohnHenry)
| Position | TargetPage | Count | Files |
|----------|------------|-------|-------|
| collection_hero | JohnHenry | 4 | banner-man-main.jpg, banner-man-0.jpg, banner-man-1.jpg, banner-man-2.jpg |

### Trang Freelancer (/Home/Freelancer)
| Position | TargetPage | Count | Files |
|----------|------------|-------|-------|
| collection_hero | Freelancer | 4 | banner-women-main.jpg, banner-women-0.jpg, banner-women-1.jpg, banner-women-2.jpg |

### Best Seller
| Position | TargetPage | Count | Files |
|----------|------------|-------|-------|
| collection_hero | BestSeller | 2 | banner-man-bestseller.jpg, banner-women-bestseller.jpg |

### Category Banners (Dự trữ)
| Position | TargetPage | Count | Files |
|----------|------------|-------|-------|
| category_banner | Various | 8 | banner-shirt-man.jpg, banner-trousers-man.jpg, banner-accessory-man.jpg, banner-shirt-woman.jpg, banner-dress-woman.jpg, banner-skirt-woman.jpg, banner-pant-short-woman.jpg, banner-accessory-woman.jpg |

### Blog Banner
| Position | TargetPage | Count | Files |
|----------|------------|-------|-------|
| page_hero | Blog | 1 | banner-blog.jpg |

**Tổng cộng: 24 banners**

## 🚀 Cách Chạy Import Script

### Option 1: Sử dụng SQL Script (Khuyến nghị)

```bash
# Kết nối PostgreSQL và chạy script
psql -h localhost -U postgres -d johnhenry_db -f Scripts/SeedBanners.sql

# Hoặc copy paste vào pgAdmin Query Tool
```

### Option 2: Sử dụng C# Script

1. Tạo temporary endpoint trong AdminController.cs:
```csharp
[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> SeedBanners()
{
    await SeedBannersScript.SeedBanners(_context);
    return Ok("Banners seeded successfully!");
}
```

2. Truy cập: `POST /admin/seed-banners`

### Option 3: Chạy từ Program.cs (Một lần)

Thêm vào Program.cs trước `app.Run()`:
```csharp
// Seed banners (chạy một lần rồi comment out)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedBannersScript.SeedBanners(context);
}
```

## ✅ Kiểm Tra Kết Quả

### 1. Kiểm tra Database
```sql
-- Tổng số banners
SELECT COUNT(*) FROM "MarketingBanners";

-- Banners theo vị trí
SELECT "Position", "TargetPage", COUNT(*) 
FROM "MarketingBanners"
GROUP BY "Position", "TargetPage"
ORDER BY "Position", "TargetPage";
```

### 2. Kiểm tra Trang Web
- ✅ Trang chủ: http://localhost:5101/
- ✅ John Henry: http://localhost:5101/Home/JohnHenry
- ✅ Freelancer: http://localhost:5101/Home/Freelancer

### 3. Quản lý qua Admin Panel
- ✅ Admin Banners: http://localhost:5101/admin/banners

## 🎯 Features Hiện Có

### Trang Chủ (Index.cshtml)
- ✅ Hero carousel với 3 banners
- ✅ Small banners section với 2 banners
- ✅ Auto-play carousel
- ✅ Responsive design
- ✅ Fallback banners nếu không có data

### Trang John Henry (JohnHenry.cshtml)
- ✅ Load banners từ database
- ✅ Hỗ trợ 1 banner hoặc nhiều banners (carousel)
- ✅ Carousel controls (prev/next)
- ✅ Carousel indicators
- ✅ Clickable banners với LinkUrl
- ✅ Fallback banner

### Trang Freelancer (Freelancer.cshtml)
- ✅ Load banners từ database
- ✅ Hỗ trợ 1 banner hoặc nhiều banners (carousel)
- ✅ Carousel controls (prev/next)
- ✅ Carousel indicators
- ✅ Clickable banners với LinkUrl
- ✅ Fallback banner

### Admin Panel (/admin/banners)
- ✅ View all banners
- ✅ Add new banner
- ✅ Edit banner
- ✅ Delete banner
- ✅ Toggle active/inactive
- ✅ Filter by position
- ✅ Statistics dashboard
- ✅ Image upload (desktop + mobile)
- ✅ Preview banners

## 📝 Lưu Ý

### Banner Files
Tất cả banner images đã có sẵn trong:
```
/wwwroot/images/Banner/
```

### Database Schema
```
MarketingBanners table:
- Position: "home_main", "home_side", "collection_hero", "category_banner", "page_hero"
- TargetPage: "JohnHenry", "Freelancer", "BestSeller", null (for home page)
- IsActive: true/false
- StartDate, EndDate: Scheduling
- SortOrder: Display order
```

### Carousel Behavior
- **1 banner**: Hiển thị banner đơn (không có carousel)
- **2+ banners**: Hiển thị carousel với controls và indicators
- **Auto-play**: 5 seconds interval
- **Responsive**: Tự động điều chỉnh theo màn hình

## 🔄 Next Steps

1. **Import Banners**
   ```bash
   psql -h localhost -U postgres -d johnhenry_db -f Scripts/SeedBanners.sql
   ```

2. **Restart Application**
   ```bash
   # Ctrl+C để stop
   dotnet run
   ```

3. **Kiểm tra Trang Web**
   - Trang chủ: http://localhost:5101/
   - John Henry: http://localhost:5101/Home/JohnHenry
   - Freelancer: http://localhost:5101/Home/Freelancer

4. **Quản lý Banners**
   - Admin panel: http://localhost:5101/admin/banners
   - Thêm/sửa/xóa banners tại đây

## 📞 Support

Nếu gặp vấn đề:
1. Kiểm tra database connection
2. Kiểm tra file banners tồn tại trong `/wwwroot/images/Banner/`
3. Xem logs trong terminal
4. Check browser console cho errors

## 🎉 Hoàn Thành

- ✅ SQL Script tạo xong
- ✅ C# Script tạo xong
- ✅ Documentation viết xong
- ✅ Controllers updated
- ✅ Views updated
- ✅ No compilation errors
- ✅ Ready to import và test!

**Tất cả files banner đã được map vào database schema và sẵn sàng để import!**
