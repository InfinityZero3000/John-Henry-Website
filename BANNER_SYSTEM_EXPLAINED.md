# 🎨 Banner System - Giải thích Chi tiết & Vị trí Hiển thị

## 📍 VỊ TRÍ HIỂN THỊ BANNER TRÊN TRANG CHỦ

### Hiện tại (HARDCODED - Không dùng Database)

```
┌─────────────────────────────────────────────────────────────┐
│                     NAVIGATION BAR                          │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│  🔴 VỊ TRÍ 1: HERO CAROUSEL (To, full-width)               │
│  ═══════════════════════════════════════════════════════    │
│                                                              │
│  [← Slide 1: Women Banner ←]                                │
│  [→ Slide 2: Men Banner   →]                                │
│                                                              │
│  • Chiều cao: ~500-600px                                     │
│  • Toàn bộ chiều rộng màn hình                              │
│  • Auto-play carousel (Bootstrap)                            │
│  • Hiện đang HARDCODED 2 slides                             │
│                                                              │
└─────────────────────────────────────────────────────────────┘
┌───────────────┬───────────────┬───────────────────────────┐
│ 🔵 VỊ TRÍ 2:  │  SMALL        │  BANNERS (3 cột)          │
│ ───────────── │ ────────────  │ ───────────────────────── │
│               │               │                            │
│ John Henry -  │  Best Seller  │  Freelancer -              │
│ New Arrival   │               │  New Arrival               │
│               │               │                            │
│ [Xem ngay]    │ [Xem ngay]    │ [Xem ngay]                 │
│               │               │                            │
└───────────────┴───────────────┴───────────────────────────┘
│                                                              │
│              COLLECTIONS (John Henry, Freelancer...)        │
│                                                              │
│              FEATURED PRODUCTS                               │
│                                                              │
│              BLOG SECTION (Đã upgrade - dynamic)            │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

### Sau khi Nâng cấp (SỬ DỤNG DATABASE - DYNAMIC)

```
┌─────────────────────────────────────────────────────────────┐
│                     NAVIGATION BAR                          │
└─────────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────────┐
│  ✅ VỊ TRÍ 1: HERO CAROUSEL - BANNER CHÍNH                 │
│  ═══════════════════════════════════════════════════════    │
│                                                              │
│  Position: "home_main"                                       │
│  Source: MarketingBanners table (database)                   │
│  Filter:                                                     │
│    - IsActive = true                                         │
│    - StartDate <= Today <= EndDate                           │
│    - Position = "home_main"                                  │
│  Order: SortOrder ASC                                        │
│                                                              │
│  [← Banner 1 (từ admin) ←]                                  │
│  [→ Banner 2 (từ admin) →]                                  │
│  [→ Banner 3 (từ admin) →]                                  │
│                                                              │
│  • Số lượng: Tùy admin tạo bao nhiêu                         │
│  • Link: Có thể click vào banner → LinkUrl                  │
│  • Responsive: Desktop image / Mobile image khác nhau        │
│  • Analytics: Track ViewCount, ClickCount                    │
│                                                              │
└─────────────────────────────────────────────────────────────┘
┌───────────────┬───────────────┬───────────────────────────┐
│ ✅ VỊ TRÍ 2:  │  SMALL        │  BANNERS - 3 CỘT          │
│ ───────────── │ ────────────  │ ───────────────────────── │
│               │               │                            │
│ Position:     │  Position:    │  Position:                 │
│ "home_side"   │  "home_side"  │  "home_side"               │
│               │               │                            │
│ Small banner  │  Small banner │  Small banner              │
│ từ database   │  từ database  │  từ database               │
│               │               │                            │
│ [Link 1]      │  [Link 2]     │  [Link 3]                  │
│               │               │                            │
└───────────────┴───────────────┴───────────────────────────┘
│                                                              │
│              COLLECTIONS                                     │
│                                                              │
│              FEATURED PRODUCTS                               │
│                                                              │
│              BLOG SECTION ✅ (Đã upgrade)                   │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 💡 Ý TƯỞNG THIẾT KẾ BANNER SYSTEM

### 1. **Position-Based Banner System** 🎯

Banner được thiết kế theo **VỊ TRÍ** (Position), không phải theo loại content. Điều này giúp hệ thống linh hoạt và tái sử dụng được.

#### **Các Position có sẵn**:

```csharp
Position = "home_main"         // Banner chính trang chủ (Hero carousel)
Position = "home_side"         // Banner phụ trang chủ (3 cột nhỏ)
Position = "category"          // Banner trên trang danh mục
Position = "product_detail"    // Banner trên trang chi tiết sản phẩm
Position = "popup"             // Banner popup (có thể làm modal)
```

**Tại sao thiết kế như vậy?**
- ✅ Admin có thể tạo banner cho BẤT KỲ vị trí nào
- ✅ Một position có thể có NHIỀU banner (carousel)
- ✅ Dễ mở rộng: Thêm position mới không cần sửa code nhiều
- ✅ Dễ quản lý: Admin biết rõ banner sẽ xuất hiện ở đâu

---

### 2. **Date Range Scheduling** 📅

Banner có StartDate và EndDate để tự động bật/tắt theo thời gian.

```csharp
StartDate = DateTime.UtcNow;   // Bắt đầu hiển thị
EndDate = DateTime.UtcNow + 7 days;  // Kết thúc sau 7 ngày
```

**Use Cases**:
- 🎄 **Banner Giáng Sinh**: StartDate = 20/12, EndDate = 26/12
- 🎆 **Banner Tết**: StartDate = 01/02, EndDate = 15/02
- 🔥 **Flash Sale**: StartDate = Today 8AM, EndDate = Today 8PM
- 📢 **Khuyến mãi tháng 11**: StartDate = 01/11, EndDate = 30/11

**Lợi ích**:
- ✅ Tự động hiển thị/ẩn banner theo lịch
- ✅ Admin không cần nhớ tắt banner thủ công
- ✅ Chuẩn bị trước nhiều campaign

---

### 3. **SortOrder Priority** 🔢

Khi có nhiều banner cùng Position, SortOrder quyết định thứ tự hiển thị.

```csharp
SortOrder = 0;  // Banner này hiển thị đầu tiên (slide 1)
SortOrder = 1;  // Banner này hiển thị thứ 2 (slide 2)
SortOrder = 2;  // Banner này hiển thị thứ 3 (slide 3)
```

**Ví dụ thực tế**:
```
Position = "home_main"
├── Banner A (SortOrder = 0) → Slide 1 ⭐ Quan trọng nhất
├── Banner B (SortOrder = 1) → Slide 2
└── Banner C (SortOrder = 2) → Slide 3
```

**Lợi ích**:
- ✅ Admin kiểm soát được banner nào hiển thị trước
- ✅ Banner quan trọng (khuyến mãi lớn) có thể đặt slide 1
- ✅ Dễ sắp xếp lại thứ tự

---

### 4. **Responsive Design** 📱💻

Banner có 2 image URLs riêng biệt:

```csharp
ImageUrl = "~/images/banner-desktop.jpg";     // Ảnh desktop (1920x600px)
MobileImageUrl = "~/images/banner-mobile.jpg"; // Ảnh mobile (768x400px)
```

**HTML Implementation**:
```html
<picture>
    <!-- Nếu màn hình <= 768px, dùng ảnh mobile -->
    <source media="(max-width: 768px)" srcset="@banner.MobileImageUrl">
    
    <!-- Nếu màn hình > 768px, dùng ảnh desktop -->
    <img src="@banner.ImageUrl" alt="@banner.Title">
</picture>
```

**Tại sao cần 2 ảnh riêng?**
- ❌ **Vấn đề nếu chỉ dùng 1 ảnh**:
  - Ảnh desktop trên mobile: Quá rộng, text nhỏ, không đọc được
  - Ảnh mobile trên desktop: Bị kéo giãn, mờ, không chuyên nghiệp
  
- ✅ **Giải pháp với 2 ảnh**:
  - Desktop: Text lớn ở bên trái/phải, ảnh model ở giữa
  - Mobile: Text chồng lên ảnh, layout dọc, font size tối ưu

**Ví dụ**:
```
Desktop Banner (1920x600):
┌────────────────────────────────────────────┐
│  GIẢM GIÁ 50%      [Model]    [Shop Now]  │
│  NEW COLLECTION                            │
└────────────────────────────────────────────┘

Mobile Banner (768x400):
┌─────────────────┐
│    [Model]      │
│                 │
│  GIẢM GIÁ 50%   │
│  NEW COLLECTION │
│  [Shop Now]     │
└─────────────────┘
```

---

### 5. **Smart Linking** 🔗

Banner có LinkUrl và OpenInNewTab để điều hướng người dùng.

```csharp
LinkUrl = "/products?category=women";  // Link nội bộ
OpenInNewTab = false;                  // Mở cùng tab

LinkUrl = "https://affiliate.com/deal"; // Link ngoài
OpenInNewTab = true;                    // Mở tab mới
```

**Use Cases**:
- 🛍️ **Banner Sản phẩm mới**: LinkUrl = `/products?tag=new-arrival`
- 🎯 **Banner Danh mục**: LinkUrl = `/products?category=men-shirts`
- 💰 **Banner Flash Sale**: LinkUrl = `/products?flashsale=active`
- 🔗 **Banner Affiliate**: LinkUrl = `https://partner.com`, OpenInNewTab = true
- ℹ️ **Banner Thông báo**: LinkUrl = `/blog/announcement-2024`

**Lợi ích**:
- ✅ Click vào banner = Đưa khách hàng đến đúng nơi
- ✅ Tăng conversion rate (không chỉ đẹp mà còn có chức năng)
- ✅ Tracking: ClickCount để đo hiệu quả banner

---

### 6. **Analytics Tracking** 📊

Banner tự động track số liệu để đo lường hiệu quả.

```csharp
ViewCount = 0;   // Số lần banner được hiển thị
ClickCount = 0;  // Số lần người dùng click vào banner
```

**Cách hoạt động**:
1. **ViewCount**: Tăng khi banner load trên trang
   ```javascript
   // JavaScript tracking
   fetch('/api/banner/trackView/' + bannerId, { method: 'POST' });
   ```

2. **ClickCount**: Tăng khi user click vào banner
   ```html
   <a href="@banner.LinkUrl" onclick="trackClick('@banner.Id')">
   ```

**Báo cáo Admin sẽ thấy**:
```
Banner A: 10,000 views → 500 clicks = CTR 5%  ⭐ Hiệu quả tốt
Banner B: 10,000 views → 50 clicks  = CTR 0.5% ❌ Cần thay đổi
Banner C: 5,000 views  → 300 clicks = CTR 6%  ⭐⭐ Rất tốt
```

**Lợi ích**:
- ✅ Biết banner nào hiệu quả, banner nào không
- ✅ Data-driven decisions: Đầu tư vào banner có CTR cao
- ✅ A/B testing: So sánh 2 design khác nhau

---

### 7. **Active/Inactive Toggle** ⚡

Banner có IsActive để bật/tắt nhanh mà không cần xóa.

```csharp
IsActive = true;   // Banner đang hiển thị
IsActive = false;  // Banner bị ẩn, không hiển thị
```

**Use Cases**:
- ⏸️ **Tạm dừng banner**: Flash sale hết hàng sớm → Tắt banner ngay
- 🔄 **Tái sử dụng**: Banner Tết năm ngoái → Inactive → Năm sau Active lại
- 🧪 **Testing**: Tắt banner để test layout không có banner
- 🚨 **Emergency**: Phát hiện typo trên banner → Tắt ngay → Sửa → Bật lại

**Khác với Delete**:
- ❌ **Delete**: Mất hẳn banner, không lấy lại được
- ✅ **Inactive**: Banner vẫn còn trong database, chỉ ẩn đi
- ✅ Giữ lại lịch sử ViewCount, ClickCount
- ✅ Có thể Active lại bất cứ lúc nào

---

### 8. **TargetPage Filtering** 🎯

Banner có thể hiển thị trên trang cụ thể hoặc tất cả trang.

```csharp
TargetPage = null;            // Hiển thị trên TẤT CẢ trang
TargetPage = "products";      // Chỉ hiển thị trên /products
TargetPage = "category-men";  // Chỉ hiển thị trên /products?category=men
```

**Ví dụ**:
```
Position = "category", TargetPage = "category-women"
→ Banner này CHỈ hiển thị trên trang Women's Products

Position = "category", TargetPage = null
→ Banner này hiển thị trên TẤT CẢ category pages
```

**Lợi ích**:
- ✅ Personalization: Banner phù hợp với từng trang
- ✅ Trang nam: Banner sản phẩm nam
- ✅ Trang nữ: Banner sản phẩm nữ
- ✅ Không bị spam banner không liên quan

---

## 🔄 SO SÁNH HIỆN TẠI vs SAU NÂNG CẤP

### ❌ Hiện tại (Hardcoded)

```csharp
// Views/Home/Index.cshtml - STATIC
<div class="carousel-item active">
    <img src="~/images/Banner/banner-women-main.jpg" alt="Women">
</div>
<div class="carousel-item">
    <img src="~/images/Banner/banner-man-main.jpg" alt="Men">
</div>
```

**Vấn đề**:
- ❌ Admin không thể thay đổi banner qua giao diện
- ❌ Muốn đổi banner → Phải sửa code → Deploy lại
- ❌ Không có schedule: Không tự động đổi banner theo thời gian
- ❌ Không track được analytics
- ❌ Không responsive: Chỉ có 1 ảnh cho cả desktop và mobile
- ❌ Luôn hiển thị 2 banner cố định

---

### ✅ Sau nâng cấp (Dynamic from Database)

```csharp
// Controllers/HomeController.cs
var activeBanners = await _context.MarketingBanners
    .Where(b => b.IsActive 
        && b.Position == "home_main"
        && b.StartDate <= DateTime.UtcNow
        && (b.EndDate == null || b.EndDate >= DateTime.UtcNow))
    .OrderBy(b => b.SortOrder)
    .ToListAsync();
ViewBag.Banners = activeBanners;
```

```html
<!-- Views/Home/Index.cshtml - DYNAMIC -->
@if (ViewBag.Banners != null && ((List<MarketingBanner>)ViewBag.Banners).Any())
{
    <div id="heroCarousel" class="carousel slide" data-bs-ride="carousel">
        <div class="carousel-inner">
            @foreach (var (banner, index) in ((List<MarketingBanner>)ViewBag.Banners).Select((b, i) => (b, i)))
            {
                <div class="carousel-item @(index == 0 ? "active" : "")">
                    <a href="@banner.LinkUrl" target="@(banner.OpenInNewTab ? "_blank" : "_self")">
                        <picture>
                            <source media="(max-width: 768px)" 
                                    srcset="@(banner.MobileImageUrl ?? banner.ImageUrl)">
                            <img src="@banner.ImageUrl" 
                                 class="d-block w-100" 
                                 alt="@banner.Title">
                        </picture>
                    </a>
                </div>
            }
        </div>
        <button class="carousel-control-prev" ...></button>
        <button class="carousel-control-next" ...></button>
    </div>
}
else
{
    <!-- Fallback: Hiển thị banner mặc định nếu database trống -->
    <div class="hero-placeholder">
        <h3>Chưa có banner nào được cấu hình</h3>
        <p>Vui lòng vào Admin Panel để tạo banner</p>
    </div>
}
```

**Ưu điểm**:
- ✅ Admin tạo/sửa/xóa banner qua giao diện web
- ✅ Tự động lọc banner theo date range
- ✅ Responsive: Desktop/Mobile images riêng
- ✅ Clickable: Mỗi banner dẫn đến link khác nhau
- ✅ Track analytics: ViewCount, ClickCount
- ✅ Số lượng banner linh hoạt: 1, 3, 5, 10... tùy admin
- ✅ Fallback: Có UI placeholder nếu chưa có banner

---

## 🎬 QUY TRÌNH TẠO BANNER (Admin Workflow)

### Bước 1: Admin vào trang Marketing
```
URL: /admin/marketing/banners
```

### Bước 2: Click "Tạo Banner Mới"
```
URL: /admin/marketing/banner/create
```

### Bước 3: Điền form
```
┌─────────────────────────────────────────────┐
│  TẠO BANNER MỚI                             │
├─────────────────────────────────────────────┤
│                                              │
│  Tiêu đề: ___Khuyến mãi Black Friday_____   │
│  Mô tả:   ___Giảm giá lên đến 70%________   │
│                                              │
│  URL Ảnh Desktop: _/images/bf-desktop.jpg_  │
│  URL Ảnh Mobile:  _/images/bf-mobile.jpg__  │
│                                              │
│  Link đến:        _/products?sale=black-f_  │
│  Mở tab mới:      [ ] (checkbox)            │
│                                              │
│  Vị trí:          [home_main ▼]             │
│  Thứ tự:          [0_____]                  │
│                                              │
│  Ngày bắt đầu:    [20/11/2024]              │
│  Ngày kết thúc:   [30/11/2024]              │
│                                              │
│  Trạng thái:      [x] Active                │
│                                              │
│  [Lưu Banner]  [Hủy]                        │
└─────────────────────────────────────────────┘
```

### Bước 4: Lưu → Banner tự động hiển thị
```
✅ Banner đã được tạo thành công!
→ Redirect về: /admin/marketing/banners

Database: MarketingBanners table
└── New row inserted with all fields

Homepage: /
└── HomeController.Index() loads banner
    └── Views/Home/Index.cshtml renders banner
        └── Customer sees banner in carousel
```

---

## 🎨 WORKFLOW DIAGRAM

```
┌──────────────┐
│   ADMIN      │
│   PANEL      │
└──────┬───────┘
       │
       │ 1. Tạo Banner
       ↓
┌──────────────────────────────┐
│   MarketingBanners Table     │
│  (Database)                  │
│                              │
│  - Id                        │
│  - Title                     │
│  - ImageUrl                  │
│  - MobileImageUrl            │
│  - Position: "home_main"     │
│  - IsActive: true            │
│  - StartDate: 2024-11-20     │
│  - EndDate: 2024-11-30       │
└──────────┬───────────────────┘
           │
           │ 2. HomeController queries
           ↓
┌────────────────────────────────┐
│   HomeController.Index()       │
│                                │
│   activeBanners = _context     │
│     .MarketingBanners          │
│     .Where(b => b.IsActive     │
│         && b.Position ==       │
│            "home_main"         │
│         && IsInDateRange)      │
│     .OrderBy(b => b.SortOrder) │
│     .ToListAsync();            │
│                                │
│   ViewBag.Banners = banners;   │
└────────────┬───────────────────┘
             │
             │ 3. Pass to View
             ↓
┌──────────────────────────────────┐
│   Views/Home/Index.cshtml        │
│                                  │
│   @foreach (var banner in        │
│       ViewBag.Banners)           │
│   {                              │
│       <div class="carousel-item">│
│           <picture>              │
│               <source media="..." │
│                   srcset="@..."/> │
│               <img src="@..." />  │
│           </picture>              │
│       </div>                      │
│   }                               │
└────────────┬─────────────────────┘
             │
             │ 4. Render to HTML
             ↓
┌───────────────────────────────────┐
│   CUSTOMER BROWSER                │
│                                   │
│   ┌─────────────────────────────┐ │
│   │  [← Banner Carousel →]      │ │
│   │                             │ │
│   │  ████████████████████████   │ │
│   │  █  BLACK FRIDAY 70% OFF █  │ │
│   │  ████████████████████████   │ │
│   │                             │ │
│   │  ● ○ ○  (indicators)       │ │
│   └─────────────────────────────┘ │
└───────────────────────────────────┘
```

---

## 🚀 TÓM TẮT

### Ý tưởng chính:
1. **Position-based**: Banner theo vị trí, không phải theo content
2. **Time-scheduled**: Tự động hiển thị/ẩn theo ngày tháng
3. **Responsive**: 2 ảnh riêng cho desktop và mobile
4. **Clickable**: Mỗi banner dẫn đến link marketing
5. **Analytics**: Track views và clicks để đo hiệu quả
6. **Flexible**: Admin tạo bao nhiêu banner cũng được

### Vị trí hiển thị:
- 🔴 **Hero Carousel** (to, full-width, đầu trang chủ)
- 🔵 **Small Banners** (3 cột, dưới hero carousel)
- 🟢 **Category Pages** (banner trên trang danh mục)
- 🟡 **Product Pages** (banner trên trang sản phẩm)
- 🟣 **Popup** (modal quảng cáo)

### Hiện tại vs Sau nâng cấp:
- ❌ **Hiện tại**: 2 banner HARDCODED, không thể thay đổi
- ✅ **Sau upgrade**: Unlimited banners, admin quản lý qua web UI

**Next step**: Nâng cấp HomeController và Index.cshtml để load banners từ database!
