# 🎨 Thêm Quản Lý Collection Banners trong Admin Panel

## 📊 Tổng Quan

Đã thêm giao diện quản lý trực quan cho **3 Collections** trong admin panel:
- ✅ **John Henry Collection** - 4 banner slots
- ✅ **Freelancer Collection** - 4 banner slots  
- ✅ **Best Seller Collection** - 4 banner slots

## 🎯 Những Gì Đã Thêm

### 1. Variables Declaration (Lines 16-18)

```csharp
// Collection banners
var johnHenryBanners = Model.Where(b => b.Position == "collection_hero" && b.TargetPage == "JohnHenry").OrderBy(b => b.SortOrder).ToList();
var freelancerBanners = Model.Where(b => b.Position == "collection_hero" && b.TargetPage == "Freelancer").OrderBy(b => b.SortOrder).ToList();
var bestSellerBanners = Model.Where(b => b.Position == "collection_hero" && b.TargetPage == "BestSeller").OrderBy(b => b.SortOrder).ToList();
```

### 2. Filter Tabs (Lines 100-117)

Thêm 3 tabs mới:

```html
<a href="#johnhenry-banners" class="admin-filter-tab" onclick="filterBanners('collection_johnhenry')">
    <i data-lucide="users"></i>
    John Henry (@johnHenryBanners.Count)
</a>

<a href="#freelancer-banners" class="admin-filter-tab" onclick="filterBanners('collection_freelancer')">
    <i data-lucide="briefcase"></i>
    Freelancer (@freelancerBanners.Count)
</a>

<a href="#bestseller-banners" class="admin-filter-tab" onclick="filterBanners('collection_bestseller')">
    <i data-lucide="star"></i>
    Best Seller (@bestSellerBanners.Count)
</a>
```

### 3. Collection Banner Sections (After "Banner phụ")

Mỗi collection có:
- **Header**: Tiêu đề + Badge hiển thị số lượng banners
- **Grid Layout**: 4 cột (`col-lg-3`) để hiển thị tối đa 4 banners
- **Banner Cards**: Preview ảnh + Edit/Delete buttons
- **Placeholder**: Nút "+" để thêm banner mới nếu chưa đủ 4

#### John Henry Collection Section
```razor
<div class="admin-card mt-4">
    <div class="admin-card-header">
        <h5 class="admin-card-title">Collection: John Henry</h5>
        <span class="admin-badge admin-badge-primary">@johnHenryBanners.Count banner(s)</span>
    </div>
    <div class="admin-card-body">
        <!-- 4 banner slots with placeholders -->
    </div>
</div>
```

#### Freelancer Collection Section
```razor
<div class="admin-card mt-4">
    <div class="admin-card-header">
        <h5 class="admin-card-title">Collection: Freelancer</h5>
        <span class="admin-badge admin-badge-info">@freelancerBanners.Count banner(s)</span>
    </div>
    <!-- Grid with banners -->
</div>
```

#### Best Seller Collection Section
```razor
<div class="admin-card mt-4">
    <div class="admin-card-header">
        <h5 class="admin-card-title">Collection: Best Seller</h5>
        <span class="admin-badge admin-badge-warning">@bestSellerBanners.Count banner(s)</span>
    </div>
    <!-- Grid with banners -->
</div>
```

### 4. JavaScript Function: `addCollectionBanner()`

```javascript
function addCollectionBanner(targetPage) {
    resetForm();
    document.getElementById('bannerPosition').value = 'collection_hero';
    document.getElementById('bannerTargetPage').value = targetPage;
    document.getElementById('bannerModalTitle').textContent = 'Thêm banner cho ' + targetPage;
    document.getElementById('btnSaveText').textContent = 'Tạo banner';
    currentBannerId = null;
    
    const modal = new bootstrap.Modal(document.getElementById('createBannerModal'));
    modal.show();
}
```

**Chức năng**:
- Tự động điền `Position = "collection_hero"`
- Tự động điền `TargetPage` (JohnHenry/Freelancer/BestSeller)
- Mở modal với title tùy chỉnh
- Admin chỉ cần upload ảnh và điền thông tin còn lại

### 5. Enhanced Sidebar List with Filter Support

```razor
var filterKey = banner.Position;
if (banner.Position == "collection_hero")
{
    filterKey = banner.TargetPage == "JohnHenry" ? "collection_johnhenry" :
               banner.TargetPage == "Freelancer" ? "collection_freelancer" :
               banner.TargetPage == "BestSeller" ? "collection_bestseller" :
               "collection_hero";
}

<div class="list-group-item" data-position="@filterKey" data-active="@banner.IsActive.ToString().ToLower()">
    <!-- Banner info with collection-specific labels -->
</div>
```

**Improved Display**:
- "John Henry Collection" thay vì "collection_hero"
- "Freelancer Collection" 
- "Best Seller Collection"

## 🎨 Layout Admin Panel

```
┌─────────────────────────────────────────────────────────────┐
│  Tất cả | Trang chủ | Banner phụ | JH | FL | BS | Hoạt động │
└─────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│ Banner trang chủ chính                                        │
│ [Banner 1]  [Banner 2]                                        │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│ Banner phụ                                                    │
│ [Banner 1]  [Banner 2]  [Banner 3]                            │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│ Collection: John Henry                             [4 banners]│
│ [Banner 1]  [Banner 2]  [Banner 3]  [Banner 4]                │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│ Collection: Freelancer                             [4 banners]│
│ [Banner 1]  [Banner 2]  [Banner 3]  [Banner 4]                │
└───────────────────────────────────────────────────────────────┘

┌───────────────────────────────────────────────────────────────┐
│ Collection: Best Seller                            [2 banners]│
│ [Banner 1]  [Banner 2]  [+ Thêm]    [+ Thêm]                  │
└───────────────────────────────────────────────────────────────┘
```

## 📋 Cách Sử Dụng

### Thêm Banner Collection Mới

#### Cách 1: Click vào Placeholder
1. Vào `/Admin/Banners`
2. Scroll đến section collection mong muốn
3. Click nút **"+ Thêm banner"** trong slot trống
4. Modal tự động mở với:
   - Position: `collection_hero` ✅ (pre-filled)
   - TargetPage: `JohnHenry/Freelancer/BestSeller` ✅ (pre-filled)
5. Upload ảnh và điền thông tin:
   - Title: VD: "John Henry Banner 3"
   - Description: Mô tả banner
   - ImageUrl: Upload ảnh desktop
   - MobileImageUrl: Upload ảnh mobile
   - LinkUrl: `/Home/JohnHenry`
   - SortOrder: 1, 2, 3, 4
   - IsActive: ✅
6. Click "Tạo banner"

#### Cách 2: Manual Create
1. Click "Tạo banner mới" (top-right)
2. Điền form:
   - Position: Chọn `collection_hero`
   - TargetPage: Gõ `JohnHenry`, `Freelancer`, hoặc `BestSeller`
   - Upload ảnh và điền thông tin khác
3. Lưu

### Chỉnh Sửa Banner Collection

1. Hover vào banner card
2. Click nút **Edit** (icon bút)
3. Modal mở với data đã điền sẵn
4. Sửa thông tin cần thay đổi
5. Click "Cập nhật banner"

### Xóa Banner Collection

1. Hover vào banner card
2. Click nút **Delete** (icon thùng rác)
3. Confirm xóa
4. Banner bị xóa, placeholder "+" xuất hiện

### Filter Banners

Click vào tab filter để lọc:
- **Tất cả**: Hiển thị tất cả banners
- **John Henry**: Chỉ hiển thị banners collection John Henry
- **Freelancer**: Chỉ hiển thị banners collection Freelancer
- **Best Seller**: Chỉ hiển thị banners collection Best Seller
- **Hoạt động**: Chỉ hiển thị banners đang active

## 📊 Database Mapping

### John Henry Collection
```sql
SELECT "Title", "SortOrder", "IsActive"
FROM "MarketingBanners"
WHERE "Position" = 'collection_hero' 
  AND "TargetPage" = 'JohnHenry'
ORDER BY "SortOrder";
```

Expected: 4 banners
```
Banner 1: banner-man-main.jpg
Banner 2: banner-man-0.jpg
Banner 3: banner-man-1.jpg
Banner 4: banner-man-2.jpg
```

### Freelancer Collection
```sql
WHERE "Position" = 'collection_hero' 
  AND "TargetPage" = 'Freelancer'
```

Expected: 4 banners
```
Banner 1: banner-women-main.jpg
Banner 2: banner-women-0.jpg
Banner 3: banner-women-1.jpg
Banner 4: banner-women-2.jpg
```

### Best Seller Collection
```sql
WHERE "Position" = 'collection_hero' 
  AND "TargetPage" = 'BestSeller'
```

Expected: 2 banners
```
Banner 1: banner-man-bestseller.jpg
Banner 2: banner-women-bestseller.jpg
```

## 🎯 Banner Slot Logic

### If có đủ 4 banners:
```
[Banner 1] [Banner 2] [Banner 3] [Banner 4]
  Edit/Del   Edit/Del   Edit/Del   Edit/Del
```

### If có 2 banners:
```
[Banner 1] [Banner 2] [+ Thêm] [+ Thêm]
  Edit/Del   Edit/Del
```

### If không có banner:
```
[+ Thêm] [+ Thêm] [+ Thêm] [+ Thêm]
```

## 🔍 Filter Keys Mapping

| Position | TargetPage | Filter Key | Display Name |
|----------|-----------|------------|--------------|
| collection_hero | JohnHenry | collection_johnhenry | John Henry Collection |
| collection_hero | Freelancer | collection_freelancer | Freelancer Collection |
| collection_hero | BestSeller | collection_bestseller | Best Seller Collection |
| home_main | (null) | home_main | Trang chủ chính |
| home_side | (null) | home_side | Banner phụ |

## 🎨 Badge Colors

- **John Henry**: Primary (blue) - `admin-badge-primary`
- **Freelancer**: Info (cyan) - `admin-badge-info`
- **Best Seller**: Warning (yellow) - `admin-badge-warning`

## 📱 Responsive Behavior

### Desktop (≥1200px)
```
Collection: John Henry
[Banner 1] [Banner 2] [Banner 3] [Banner 4]
   25%        25%        25%        25%
```

### Tablet (768px - 1199px)
```
[Banner 1] [Banner 2]
   50%        50%

[Banner 3] [Banner 4]
   50%        50%
```

### Mobile (<768px)
```
[Banner 1]
   100%

[Banner 2]
   100%

[Banner 3]
   100%

[Banner 4]
   100%
```

## 🚀 Testing Checklist

- [ ] Vào `/Admin/Banners`
- [ ] Kiểm tra 3 sections collection hiển thị đúng
- [ ] Click filter tabs John Henry/Freelancer/Best Seller
- [ ] Verify sidebar list filter đúng theo collection
- [ ] Click placeholder "+" → Modal mở với Position/TargetPage pre-filled
- [ ] Tạo banner mới cho từng collection
- [ ] Edit banner collection
- [ ] Delete banner collection
- [ ] Verify badge counts cập nhật đúng
- [ ] Test responsive trên mobile/tablet

## ✅ Summary

✅ **3 Collection sections** added với visual management  
✅ **Filter tabs** để dễ dàng navigate giữa các collections  
✅ **Placeholder buttons** với pre-filled Position + TargetPage  
✅ **Sidebar filtering** hoạt động với collection banners  
✅ **Badge counts** hiển thị số lượng banners mỗi collection  
✅ **Responsive layout** với col-lg-3 (4 columns)  
✅ **No compilation errors**  

Admin giờ có thể quản lý đầy đủ banners cho:
- 🏠 Trang chủ (2 main + 3 side)
- 👔 John Henry Collection (4 banners)
- 💼 Freelancer Collection (4 banners)
- ⭐ Best Seller Collection (2-4 banners)

**Tất cả từ một giao diện admin duy nhất!** 🎉
