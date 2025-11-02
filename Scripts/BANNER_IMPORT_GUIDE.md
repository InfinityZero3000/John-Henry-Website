# Hướng Dẫn Import Banners vào Database

## Tổng Quan

Hệ thống banner động đã được thiết lập để quản lý tất cả các banner trên website từ database thay vì hardcode trong HTML. Điều này cho phép admin dễ dàng thêm, sửa, xóa banner thông qua trang quản trị `/admin/banners`.

## Các Vị Trí Banner

### 1. **Trang Chủ (Index.cshtml)**
- **Position:** `home_main` - Banner carousel chính (Hero banners)
- **Position:** `home_side` - Banner phụ bên dưới carousel
- **Số lượng:** 3 banners cho carousel, 2-3 banners cho phần phụ

### 2. **Trang John Henry (JohnHenry.cshtml)**
- **Position:** `collection_hero`
- **TargetPage:** `JohnHenry`
- **Số lượng:** 1-4 banners (nếu >1 sẽ hiện carousel)

### 3. **Trang Freelancer (Freelancer.cshtml)**
- **Position:** `collection_hero`
- **TargetPage:** `Freelancer`
- **Số lượng:** 1-4 banners (nếu >1 sẽ hiện carousel)

### 4. **Trang Danh Mục (Dự trữ)**
- **Position:** `category_banner`
- **TargetPage:** Tên danh mục (VD: `AoSoMiNam`, `DamNu`)

### 5. **Trang Blog (Dự trữ)**
- **Position:** `page_hero`
- **TargetPage:** `Blog`

## Cách Import Banners

### Bước 1: Kết Nối Database

#### Sử dụng PostgreSQL Command Line:
```bash
# Kết nối tới database
psql -h localhost -U postgres -d johnhenry_db

# Hoặc nếu đang dùng Railway/Cloud
psql postgresql://username:password@host:port/database
```

#### Sử dụng pgAdmin:
1. Mở pgAdmin
2. Kết nối tới server PostgreSQL
3. Chọn database `johnhenry_db`
4. Mở Query Tool

### Bước 2: Chạy Script Import

```bash
# Từ thư mục Scripts
psql -h localhost -U postgres -d johnhenry_db -f SeedBanners.sql
```

Hoặc copy nội dung file `SeedBanners.sql` vào Query Tool và Execute.

### Bước 3: Xác Nhận Dữ Liệu

Chạy query kiểm tra:

```sql
-- Kiểm tra tổng số banner đã import
SELECT COUNT(*) as "TotalBanners" FROM "MarketingBanners";

-- Xem banner theo vị trí
SELECT "Position", "TargetPage", COUNT(*) as "BannerCount"
FROM "MarketingBanners"
GROUP BY "Position", "TargetPage"
ORDER BY "Position", "TargetPage";

-- Xem chi tiết tất cả banners
SELECT "Title", "Position", "TargetPage", "ImageUrl", "IsActive", "SortOrder"
FROM "MarketingBanners"
ORDER BY "Position", "TargetPage", "SortOrder";
```

## Cấu Trúc Banner Đã Import

### Trang Chủ (Index)
```
home_main (3 banners):
- banner-home-1.jpg
- banner-home-2.jpg
- banner-home-3.jpg

home_side (2 banners):
- web-01.jpg
- web-02.jpg
```

### Trang John Henry
```
collection_hero / JohnHenry (4 banners):
- banner-man-main.jpg
- banner-man-0.jpg
- banner-man-1.jpg
- banner-man-2.jpg
```

### Trang Freelancer
```
collection_hero / Freelancer (4 banners):
- banner-women-main.jpg
- banner-women-0.jpg
- banner-women-1.jpg
- banner-women-2.jpg
```

## Quản Lý Banners Qua Admin Panel

Sau khi import, bạn có thể quản lý banners tại:
```
http://localhost:5101/admin/banners
```

### Các Tính Năng Admin Panel:
1. ✅ **Xem Danh Sách** - Tất cả banners với preview
2. ✅ **Thêm Banner Mới** - Upload ảnh desktop + mobile
3. ✅ **Chỉnh Sửa** - Cập nhật thông tin, thay ảnh
4. ✅ **Xóa Banner** - Xóa banner và ảnh liên quan
5. ✅ **Bật/Tắt** - Toggle trạng thái active/inactive
6. ✅ **Sắp Xếp** - Drag & drop để thay đổi thứ tự
7. ✅ **Lọc** - Theo vị trí (Position) và trạng thái

## Thêm Banner Mới Qua Admin

1. Truy cập `/admin/banners`
2. Click nút "Add New Banner"
3. Điền thông tin:
   - **Title**: Tên banner (bắt buộc)
   - **Description**: Mô tả (tùy chọn)
   - **Position**: Chọn vị trí hiển thị
     - `home_main` - Carousel trang chủ
     - `home_side` - Banner phụ trang chủ
     - `collection_hero` - Banner bộ sưu tập
     - `category_banner` - Banner danh mục
     - `page_hero` - Banner trang
   - **Target Page**: Trang cụ thể (nếu cần)
     - `JohnHenry`, `Freelancer`, `BestSeller`, etc.
   - **Desktop Image**: Upload ảnh cho màn hình lớn
   - **Mobile Image**: Upload ảnh cho mobile (tùy chọn)
   - **Link URL**: Link khi click vào banner (tùy chọn)
   - **Sort Order**: Thứ tự hiển thị (số càng nhỏ càng lên đầu)
   - **Active**: Bật/Tắt hiển thị
   - **Start/End Date**: Thời gian hiển thị

4. Click "Save Banner"

## Kiến Trúc Hệ Thống

### Database Schema
```sql
MarketingBanners
- Id: UUID (Primary Key)
- Title: string (Required)
- Description: string (Optional)
- ImageUrl: string (Required) - Desktop image path
- MobileImageUrl: string (Optional) - Mobile image path
- LinkUrl: string (Optional) - Click destination
- OpenInNewTab: boolean - Open link in new tab
- Position: string (Required) - Banner position
- TargetPage: string (Optional) - Specific page
- SortOrder: int - Display order
- IsActive: boolean - Active status
- StartDate: DateTime - Start showing
- EndDate: DateTime (Optional) - Stop showing
- ViewCount: int - Number of views
- ClickCount: int - Number of clicks
- CreatedAt: DateTime
- UpdatedAt: DateTime
- CreatedBy: string - Admin user ID
```

### Controllers
- **HomeController**: Load banners cho Index, JohnHenry, Freelancer
- **AdminController**: CRUD operations cho banner management

### Views
- **Index.cshtml**: Hiển thị `home_main` + `home_side` banners
- **JohnHenry.cshtml**: Hiển thị `collection_hero/JohnHenry` banners
- **Freelancer.cshtml**: Hiển thị `collection_hero/Freelancer` banners
- **Admin/Banners.cshtml**: Trang quản lý banners

## Troubleshooting

### Lỗi: "Gen_random_uuid() function does not exist"
PostgreSQL cần extension UUID:
```sql
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
-- Hoặc
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
```

### Lỗi: "Banners not showing on website"
1. Kiểm tra `IsActive = true`
2. Kiểm tra `StartDate <= NOW()`
3. Kiểm tra `EndDate IS NULL OR EndDate >= NOW()`
4. Xác nhận `Position` và `TargetPage` đúng
5. Clear cache và reload trang

### Lỗi: "Image not found (404)"
1. Kiểm tra đường dẫn trong `ImageUrl`
2. Xác nhận file tồn tại trong `/wwwroot/images/Banner/`
3. Kiểm tra permissions của thư mục

## Best Practices

1. **Tên File Ảnh**: Dùng lowercase, dấu gạch ngang
   - ✅ `banner-home-1.jpg`
   - ❌ `Banner Home 1.JPG`

2. **Kích Thước Ảnh**:
   - Desktop: 1920x800px (hoặc tỉ lệ tương tự)
   - Mobile: 768x600px (hoặc tỉ lệ tương tự)
   - Format: JPG/JPEG (tối ưu size)

3. **Sort Order**:
   - Bắt đầu từ 1
   - Tăng dần 1, 2, 3, 4...
   - Banner có SortOrder thấp nhất hiện đầu tiên

4. **Active Banners**:
   - Chỉ active banner đang dùng
   - Inactive banner cũ để backup
   - Set EndDate cho campaigns có thời hạn

5. **Performance**:
   - Giữ số lượng banner vừa phải (3-5 per carousel)
   - Optimize ảnh trước khi upload
   - Sử dụng CDN nếu có nhiều traffic

## Liên Hệ Support

Nếu gặp vấn đề, liên hệ:
- Email: support@johnhenry.vn
- Team: Development Team
