# 🏠 Cấu Trúc Banner Trang Chủ

## 📊 Tổng Quan

Trang chủ hiện có **2 loại banner chính**:

### 1. 🎯 Banner Chính (Hero Carousel)
- **Position**: `home_main`
- **Số lượng**: 2 banners
- **Hiển thị**: Carousel tự động chuyển ảnh
- **Vị trí**: Đầu trang chủ (section đầu tiên)

### 2. 🎨 Banner Phụ (Small Banners)
- **Position**: `home_side`
- **Số lượng**: 3 banners
- **Hiển thị**: 3 cột ngang, mỗi banner có tiêu đề + nút "Xem ngay"
- **Vị trí**: Ngay dưới Hero Carousel

---

## 📋 Chi Tiết Banner Database

### Banner Chính (home_main) - 2 Banners

| # | Title | ImageUrl | LinkUrl | SortOrder |
|---|-------|----------|---------|-----------|
| 1 | Banner Chính Trang Chủ 1 | `/images/Banner/banner-women-main.jpg` | `/` | 1 |
| 2 | Banner Chính Trang Chủ 2 | `/images/Banner/banner-man-main.jpg` | `/` | 2 |

**Đặc điểm**:
- TargetPage: `null` (áp dụng cho trang chủ)
- Carousel tự động: Active với 2 slides
- Desktop/Mobile: Hỗ trợ responsive images
- Controls: Prev/Next buttons + indicators

### Banner Phụ (home_side) - 3 Banners

| # | Title | ImageUrl | LinkUrl | SortOrder |
|---|-------|----------|---------|-----------|
| 1 | JOHN HENRY - NEW ARRIVAL | `/images/Banner/banner-home-1.jpg` | `/Home/JohnHenry` | 1 |
| 2 | FREELANCER - NEW ARRIVAL | `/images/Banner/banner-home-2.jpg` | `/Home/Freelancer` | 2 |
| 3 | BEST SELLER | `/images/Banner/banner-home-3.jpg` | `/Products` | 3 |

**Đặc điểm**:
- TargetPage: `null` (áp dụng cho trang chủ)
- Layout: 3 cột (`col-lg-4 col-md-4`)
- Hiển thị:
  - Hình ảnh banner
  - Tiêu đề (Title từ database)
  - Nút "Xem ngay" (hard-coded trong view)
- Có link đến trang tương ứng

---

## 🎨 Giao Diện Banner Phụ

```html
┌─────────────────┬─────────────────┬─────────────────┐
│   Banner 1      │   Banner 2      │   Banner 3      │
├─────────────────┼─────────────────┼─────────────────┤
│  [Hình ảnh]     │  [Hình ảnh]     │  [Hình ảnh]     │
│                 │                 │                 │
│ JOHN HENRY -    │ FREELANCER -    │  BEST SELLER    │
│ NEW ARRIVAL     │ NEW ARRIVAL     │                 │
│                 │                 │                 │
│  Xem ngay →     │  Xem ngay →     │  Xem ngay →     │
└─────────────────┴─────────────────┴─────────────────┘
```

**Lưu ý**:
- Text "Xem ngay" được hard-coded trong view (`Views/Home/Index.cshtml`)
- **Description** trong database dùng để mô tả banner (cho admin), không hiển thị trên giao diện
- Khi hover vào banner → hiệu ứng transition, text "Xem ngay" đổi màu

---

## 🔧 Quản Lý Banner trong Admin

### Tạo Banner Chính (Hero Carousel)

1. Vào `/Admin/Banners`
2. Click "Tạo banner mới"
3. Điền thông tin:
   - **Title**: VD: "Banner Chính Trang Chủ 3"
   - **Description**: Mô tả banner (cho admin)
   - **Position**: Chọn `home_main`
   - **TargetPage**: Để trống
   - **ImageUrl**: Upload ảnh desktop (recommended: 1920x600px)
   - **MobileImageUrl**: Upload ảnh mobile (recommended: 768x500px)
   - **LinkUrl**: VD: `/` hoặc `/Products`
   - **SortOrder**: Thứ tự hiển thị (số càng nhỏ càng hiển thị trước)
   - **IsActive**: ✅ Check
   - **StartDate/EndDate**: Tùy chọn thời gian hiển thị
4. Lưu

### Tạo Banner Phụ (Small Banners)

1. Vào `/Admin/Banners`
2. Click "Tạo banner mới"
3. Điền thông tin:
   - **Title**: VD: "JOHN HENRY - NEW ARRIVAL" (hiển thị trên giao diện)
   - **Description**: Mô tả banner (VD: "Banner phụ bộ sưu tập John Henry trang chủ")
   - **Position**: Chọn `home_side`
   - **TargetPage**: Để trống
   - **ImageUrl**: Upload ảnh (recommended: 600x400px)
   - **MobileImageUrl**: Upload ảnh mobile (recommended: 768x500px)
   - **LinkUrl**: VD: `/Home/JohnHenry`
   - **SortOrder**: 1, 2, hoặc 3 (chỉ hiển thị 3 banner đầu tiên)
   - **IsActive**: ✅ Check
4. Lưu

**Lưu ý quan trọng**:
- Chỉ 3 banner phụ đầu tiên (SortOrder nhỏ nhất) được hiển thị
- Text "Xem ngay" tự động hiển thị, không cần nhập Description

---

## 🗑️ Xóa Banner

### Xóa Mềm (Soft Delete)

Admin có 2 cách "xóa" banner:

#### 1. Toggle IsActive (Khuyến nghị)
- Vào danh sách banners
- Click nút "Active/Inactive" để toggle
- Banner vẫn còn trong database nhưng không hiển thị trên website
- Có thể bật lại bất cứ lúc nào

#### 2. Đặt EndDate
- Edit banner
- Đặt **EndDate** = ngày hôm nay hoặc ngày trong quá khứ
- Banner tự động ẩn sau ngày EndDate
- File ảnh vẫn còn nguyên

### Xóa Cứng (Hard Delete)

⚠️ **CẢNH BÁO**: Chỉ admin mới có quyền xóa hẳn banner

- Vào `/Admin/Banners`
- Click nút "Xóa" trên banner cần xóa
- **Hệ quả**:
  - ❌ Record bị xóa khỏi database
  - ✅ File ảnh vẫn còn trong `/wwwroot/images/Banner/`
  - ⚠️ Không thể khôi phục record (phải tạo mới)

**Best Practice**:
```
✅ Khuyến nghị: Toggle IsActive thay vì xóa hẳn
✅ File ảnh luôn giữ nguyên (cho dù xóa banner)
✅ Có thể tái sử dụng ảnh cho banner mới
```

---

## 📁 File Structure

### View Files
```
Views/
  Home/
    Index.cshtml          ← Trang chủ với hero carousel + small banners
```

### Controller
```
Controllers/
  HomeController.cs       ← Load ViewBag.HeroCarouselBanners + ViewBag.SmallBanners
```

### Database Seeds
```
Scripts/
  SeedBanners.sql         ← Seed 2 home_main + 3 home_side banners
```

### Image Files
```
wwwroot/
  images/
    Banner/
      banner-women-main.jpg    ← Hero carousel 1
      banner-man-main.jpg      ← Hero carousel 2
      banner-home-1.jpg        ← Small banner 1 (John Henry)
      banner-home-2.jpg        ← Small banner 2 (Freelancer)
      banner-home-3.jpg        ← Small banner 3 (Best Seller)
```

---

## 🔄 Fallback Behavior

Nếu **không có banner trong database** (`IsActive=false` hoặc chưa seed):

### Hero Carousel Fallback
```razor
<img src="~/images/Banner/banner-women-main.jpg" alt="Women Fashion">
<img src="~/images/Banner/banner-man-main.jpg" alt="Men Fashion">
```

### Small Banners Fallback
```razor
Banner 1: banner-home-1.jpg → "JOHN HENRY - NEW ARRIVAL"
Banner 2: banner-home-2.jpg → "FREELANCER - NEW ARRIVAL"
Banner 3: banner-home-3.jpg → "BEST SELLER"
```

**Ưu điểm**:
- Website luôn có banner mặc định
- Không bị lỗi khi database trống
- Admin có thể thay thế dần dần

---

## 📊 Query Database Banners

### Kiểm tra số lượng banners
```sql
SELECT "Position", COUNT(*) as "Count"
FROM "MarketingBanners"
WHERE "Position" IN ('home_main', 'home_side')
  AND "IsActive" = true
GROUP BY "Position";
```

### Xem chi tiết banners trang chủ
```sql
SELECT "Title", "Position", "SortOrder", "IsActive"
FROM "MarketingBanners"
WHERE "Position" IN ('home_main', 'home_side')
ORDER BY "Position", "SortOrder";
```

### Đếm total banners active
```sql
SELECT COUNT(*) 
FROM "MarketingBanners" 
WHERE "IsActive" = true 
  AND (
    ("StartDate" IS NULL OR "StartDate" <= NOW())
    AND ("EndDate" IS NULL OR "EndDate" >= NOW())
  );
```

---

## 🎯 Recommended Image Sizes

| Banner Type | Desktop Size | Mobile Size | Format |
|------------|--------------|-------------|--------|
| Hero Carousel (home_main) | 1920 x 600px | 768 x 500px | JPG/WebP |
| Small Banners (home_side) | 600 x 400px | 768 x 500px | JPG/WebP |

**Tối ưu hóa**:
- Dùng WebP cho file size nhỏ hơn
- Compress ảnh trước khi upload (TinyPNG, Squoosh)
- Đặt tên file có ý nghĩa (VD: `banner-home-johnhenry.jpg`)

---

## 🚀 Next Steps

1. ✅ Database đã có đủ 5 banners (2 main + 3 side)
2. ✅ View đã render đúng với nút "Xem ngay"
3. ✅ Fallback đã được giữ nguyên
4. ⏳ Test trên browser để xác nhận giao diện
5. ⏳ Thêm/sửa banner qua admin panel
6. ⏳ Thử toggle IsActive để test soft delete

---

## 📞 Support

Nếu cần thay đổi:
- **Số lượng banner phụ**: Sửa `.Take(3)` trong `Index.cshtml`
- **Text nút**: Sửa `"Xem ngay"` trong view
- **Layout**: Sửa CSS class `col-lg-4` thành `col-lg-6` (2 cột) hoặc `col-lg-3` (4 cột)
- **Carousel speed**: Thêm `data-bs-interval="5000"` trong carousel div

🎉 **Banner System Trang Chủ Hoàn Thành!**
