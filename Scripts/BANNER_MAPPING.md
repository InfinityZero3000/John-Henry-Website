# Banner Position & TargetPage Mapping Guide

Hướng dẫn mapping banners để admin có thể quản lý banners trên các trang một cách tùy chỉnh.

## 📍 Cấu Trúc Banner

Mỗi banner có 2 thuộc tính quan trọng:
- **Position**: Vị trí hiển thị (home_main, home_side, collection_hero, category_banner, page_hero)
- **TargetPage**: Trang đích (tùy chọn, dùng để phân biệt banners cho các trang khác nhau)

---

## 🏠 Trang Chủ (Index)

### Hero Carousel (Main Banners)
- **Position**: `home_main`
- **TargetPage**: *(để trống hoặc null)*
- **Mô tả**: Banner carousel chính ở đầu trang chủ
- **Số lượng**: Tối đa 3 banners (hiển thị dạng carousel nếu >1)
- **Kích thước khuyến nghị**: 1920x700px (desktop), 750x500px (mobile)

### Small Banners (3-Column)
- **Position**: `home_side`
- **TargetPage**: *(để trống hoặc null)*
- **Mô tả**: 3 banners nhỏ dưới hero carousel
- **Số lượng**: Tối đa 3 banners
- **Kích thước khuyến nghị**: 600x400px

### Collection Hero Banners (trong trang Index)
#### John Henry Collection
- **Position**: `collection_hero`
- **TargetPage**: `JohnHenry`
- **Mô tả**: Banner cho section John Henry trong trang chủ
- **Số lượng**: 1 banner
- **Kích thước**: 1200x400px

#### Freelancer Collection
- **Position**: `collection_hero`
- **TargetPage**: `Freelancer`
- **Mô tả**: Banner cho section Freelancer trong trang chủ
- **Số lượng**: 1 banner
- **Kích thước**: 1200x400px

#### Best Seller Collection
- **Position**: `collection_hero`
- **TargetPage**: `BestSeller`
- **Mô tả**: Banner cho section Best Seller trong trang chủ
- **Số lượng**: 1 banner
- **Kích thước**: 1200x400px

### Category Banners (Product Categories Section)
#### Áo Nam
- **Position**: `category_banner`
- **TargetPage**: `AoNam`
- **Mô tả**: Banner cho category Áo Nam
- **Số lượng**: 1 banner
- **Kích thước**: 800x500px

#### Áo Nữ
- **Position**: `category_banner`
- **TargetPage**: `AoNu`
- **Mô tả**: Banner cho category Áo Nữ
- **Số lượng**: 1 banner
- **Kích thước**: 800x500px

---

## 👔 Trang Collection John Henry & Freelancer

### John Henry Main Page
- **Position**: `collection_hero`
- **TargetPage**: `JohnHenry`
- **Mô tả**: Banner hero cho trang /Home/JohnHenry
- **Số lượng**: Nhiều banners (hiển thị carousel nếu >1)
- **Kích thước**: 1200x500px

### Freelancer Main Page
- **Position**: `collection_hero`
- **TargetPage**: `Freelancer`
- **Mô tả**: Banner hero cho trang /Home/Freelancer
- **Số lượng**: Nhiều banners (hiển thị carousel nếu >1)
- **Kích thước**: 1200x500px

---

## 👕 Trang Category Landing Pages

### John Henry - Áo Sơ Mi Nam
- **Position**: `category_banner`
- **TargetPage**: `AoSoMiNam`
- **Mô tả**: Banner cho trang /Home/JohnHenryShirt
- **URL**: /Home/JohnHenryShirt
- **Kích thước**: 1200x400px

### John Henry - Quần Tây Nam
- **Position**: `category_banner`
- **TargetPage**: `QuanTayNam`
- **Mô tả**: Banner cho trang /Home/JohnHenryTrousers
- **URL**: /Home/JohnHenryTrousers
- **Kích thước**: 1200x400px

### John Henry - Phụ Kiện Nam
- **Position**: `category_banner`
- **TargetPage**: `PhuKienNam`
- **Mô tả**: Banner cho trang /Home/JohnHenryAccessories
- **URL**: /Home/JohnHenryAccessories
- **Kích thước**: 1200x400px

### Freelancer - Áo Sơ Mi Nữ
- **Position**: `category_banner`
- **TargetPage**: `AoSoMiNu`
- **Mô tả**: Banner cho trang /Home/FreelancerShirt
- **URL**: /Home/FreelancerShirt
- **Kích thước**: 1200x400px

### Freelancer - Quần Short Nữ
- **Position**: `category_banner`
- **TargetPage**: `QuanShortNu`
- **Mô tả**: Banner cho trang /Home/FreelancerTrousers
- **URL**: /Home/FreelancerTrousers
- **Kích thước**: 1200x400px

### Freelancer - Chân Váy Nữ
- **Position**: `category_banner`
- **TargetPage**: `ChanVayNu`
- **Mô tả**: Banner cho trang /Home/FreelancerSkirt
- **URL**: /Home/FreelancerSkirt
- **Kích thước**: 1200x400px

### Freelancer - Phụ Kiện Nữ
- **Position**: `category_banner`
- **TargetPage**: `PhuKienNu`
- **Mô tả**: Banner cho trang /Home/FreelancerAccessories
- **URL**: /Home/FreelancerAccessories
- **Kích thước**: 1200x400px

---

## 📝 Trang Blog

### Blog Hero Banner
- **Position**: `page_hero`
- **TargetPage**: `Blog`
- **Mô tả**: Banner hero cho trang Blog
- **URL**: /Blog
- **Số lượng**: 1 banner
- **Kích thước**: 1920x400px

---

## 📋 Bảng Tổng Hợp

| Position | TargetPage | Trang | Số Lượng | Hiển Thị |
|----------|------------|-------|----------|----------|
| home_main | *(null)* | Trang chủ - Hero | 3 | Carousel |
| home_side | *(null)* | Trang chủ - 3 cột | 3 | Grid |
| collection_hero | JohnHenry | Index - JH section | 1 | Single |
| collection_hero | Freelancer | Index - FL section | 1 | Single |
| collection_hero | BestSeller | Index - BS section | 1 | Single |
| collection_hero | JohnHenry | /Home/JohnHenry | Nhiều | Carousel |
| collection_hero | Freelancer | /Home/Freelancer | Nhiều | Carousel |
| category_banner | AoNam | Index - Categories | 1 | Single |
| category_banner | AoNu | Index - Categories | 1 | Single |
| category_banner | AoSoMiNam | /Home/JohnHenryShirt | 1 | Single |
| category_banner | QuanTayNam | /Home/JohnHenryTrousers | 1 | Single |
| category_banner | PhuKienNam | /Home/JohnHenryAccessories | 1 | Single |
| category_banner | AoSoMiNu | /Home/FreelancerShirt | 1 | Single |
| category_banner | QuanShortNu | /Home/FreelancerTrousers | 1 | Single |
| category_banner | ChanVayNu | /Home/FreelancerSkirt | 1 | Single |
| category_banner | PhuKienNu | /Home/FreelancerAccessories | 1 | Single |
| page_hero | Blog | /Blog | 1 | Single |

---

## 🎨 Hướng Dẫn Tạo Banner Trong Admin

### Bước 1: Vào trang quản lý banner
- URL: `/Admin/Banners`

### Bước 2: Click "Tạo banner mới"

### Bước 3: Điền thông tin
1. **Tiêu đề**: Tên mô tả banner (ví dụ: "Banner Áo Sơ Mi Nam")
2. **Mô tả**: Mô tả chi tiết (tùy chọn)
3. **Vị trí hiển thị (Position)**:
   - Chọn từ dropdown: `home_main`, `home_side`, `collection_hero`, `category_banner`, `page_hero`
4. **Trang đích (TargetPage)**:
   - Nhập chính xác giá trị từ bảng mapping ở trên (ví dụ: `AoSoMiNam`, `JohnHenry`, `Blog`)
   - Có thể để trống cho `home_main` và `home_side`
5. **Hình ảnh banner**: Upload file ảnh
6. **URL liên kết**: URL khi click vào banner (tùy chọn)
7. **Thứ tự sắp xếp**: Số thứ tự (0 = hiển thị đầu tiên)
8. **Trạng thái**: Bật "Đang hoạt động" để hiển thị

### Bước 4: Lưu banner

---

## ⚠️ Lưu Ý Quan Trọng

1. **Position và TargetPage phải khớp chính xác** với giá trị trong bảng mapping
2. **Chữ hoa/thường quan trọng**: `JohnHenry` ≠ `johnhenry`
3. **Fallback banners**: Nếu không có banner trong DB, hệ thống sẽ hiển thị banner mặc định hard-coded
4. **Số lượng banners**:
   - Carousel: Tự động hiển thị nếu có >1 banner active
   - Single: Chỉ lấy banner đầu tiên (theo SortOrder)
5. **Ngày hiển thị**: Sử dụng StartDate/EndDate để lên lịch banner

---

## 🔄 Cập Nhật Seed Data

Nếu cần re-import banners, chạy script:

```bash
PGPASSWORD='JohnHenry@2025!' psql -h localhost -p 5432 -U johnhenry_user -d johnhenry_db -f "Scripts/SeedBanners.sql"
```

Hoặc sử dụng programmatic seeder trong `Scripts/SeedBannersScript.cs`.

---

## 📞 Hỗ Trợ

Nếu cần thêm vị trí banner mới hoặc thay đổi mapping, liên hệ developer để cập nhật:
1. Controller code (load banners từ DB)
2. View code (render banners)
3. Seed scripts (tạo banners mẫu)
4. Tài liệu này
