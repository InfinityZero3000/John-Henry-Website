# HƯỚNG DẪN KIỂM TRA VÀ CẢI TIẾN HỆ THỐNG SELLER

## Ngày tạo: 10/11/2025

## 📋 TỔNG QUAN

Tài liệu này mô tả các chức năng của Seller trong hệ thống John Henry Fashion, các vấn đề hiện tại cần khắc phục, và danh sách các cải tiến cần thực hiện.

---

## 🎯 CÁC CHỨC NĂNG CHÍNH CỦA SELLER

### 1. **Dashboard (Trang chủ Seller)**
- ✅ Hiển thị thống kê tổng quan:
  - Số lượng sản phẩm của seller
  - Số lượng đơn hàng
  - Doanh thu tổng
  - Doanh thu tháng hiện tại
- ✅ Hiển thị 5 đơn hàng gần nhất
- ✅ Hiển thị top 5 sản phẩm bán chạy
- ⚠️ **VẤN ĐỀ**: Hiện tại đang hiển thị TẤT CẢ sản phẩm/đơn hàng trong hệ thống, chưa lọc theo seller cụ thể

### 2. **Quản lý Sản phẩm**
- ✅ Xem danh sách sản phẩm (có phân trang, tìm kiếm, lọc)
- ✅ Thêm sản phẩm mới
- ✅ Chỉnh sửa sản phẩm
- ✅ Xóa sản phẩm
- ✅ Upload hình ảnh sản phẩm
- ✅ Quản lý trạng thái sản phẩm (active/inactive)
- ⚠️ **VẤN ĐỀ**: Chưa có cột `SellerId` trong bảng `Products` để liên kết sản phẩm với seller

### 3. **Quản lý Đơn hàng**
- ✅ Xem danh sách đơn hàng (có phân trang, tìm kiếm, lọc)
- ✅ Xem chi tiết đơn hàng
- ✅ Cập nhật trạng thái đơn hàng
- ⚠️ **VẤN ĐỀ**: Hiện tại hiển thị TẤT CẢ đơn hàng, chưa lọc theo sản phẩm của seller

### 4. **Quản lý Tồn kho**
- ✅ Xem danh sách tồn kho
- ✅ Cập nhật số lượng tồn kho
- ✅ Cảnh báo sản phẩm sắp hết hàng (quantity <= 10)
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo seller

### 5. **Quản lý Mã giảm giá (Coupons)**
- ✅ Xem danh sách mã giảm giá
- ✅ Tạo mã giảm giá mới
- ✅ Chỉnh sửa mã giảm giá
- ✅ Xóa mã giảm giá
- ✅ Quản lý trạng thái active/inactive
- ⚠️ **VẤN ĐỀ**: Chưa có cột `SellerId` trong bảng `Coupons` để liên kết với seller

### 6. **Quản lý Đánh giá (Reviews)**
- ✅ Xem danh sách đánh giá sản phẩm
- ✅ Phê duyệt đánh giá
- ✅ Từ chối đánh giá
- ✅ Thống kê đánh giá (tổng số, trung bình rating, phân bố rating)
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo sản phẩm của seller

### 7. **Báo cáo Doanh thu (Sales)**
- ✅ Xem doanh thu theo khoảng thời gian
- ✅ Xem số lượng đơn hàng
- ✅ Biểu đồ doanh thu theo ngày
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo seller

### 8. **Analytics (Phân tích)**
- ✅ Top sản phẩm bán chạy
- ⚠️ **VẤN ĐỀ**: Chức năng chưa hoàn chỉnh, cần bổ sung thêm metrics

### 9. **Quản lý Thông báo (Notifications)**
- ✅ Xem danh sách thông báo
- ✅ Đánh dấu đã đọc
- ✅ Đánh dấu tất cả đã đọc
- ✅ Lọc theo loại thông báo
- ✅ Hiển thị số lượng thông báo chưa đọc

### 10. **Quản lý Hoa hồng (Commissions)**
- ✅ Xem tổng doanh thu và hoa hồng
- ✅ Xem báo cáo hoa hồng theo tháng
- ✅ Xem tỷ lệ hoa hồng (15%)
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo seller

### 11. **Quản lý Khách hàng (Customers)**
- ✅ Xem danh sách khách hàng
- ✅ Xem top khách hàng theo doanh thu
- ✅ Xem khách hàng mới
- ✅ Thống kê khách hàng
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo khách hàng đã mua sản phẩm của seller

### 12. **Báo cáo Tổng hợp (Reports)**
- ✅ Báo cáo doanh thu tổng hợp
- ✅ Biểu đồ doanh thu 7 ngày gần nhất
- ✅ Biểu đồ đơn hàng 7 ngày gần nhất
- ✅ Thống kê sản phẩm, khách hàng
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo seller

### 13. **Hiệu suất Sản phẩm (Product Performance)**
- ✅ Xem top sản phẩm bán chạy
- ✅ Xem sản phẩm bán kém
- ✅ Hiển thị doanh thu, số lượng bán, rating
- ⚠️ **VẤN ĐỀ**: Chưa lọc theo seller

### 14. **Quản lý Hồ sơ (Profile)**
- ✅ Xem thông tin cá nhân
- ✅ Cập nhật thông tin:
  - Tên công ty
  - Giấy phép kinh doanh
  - Mã số thuế
  - Địa chỉ
  - Số điện thoại
  - Email
  - Họ tên

### 15. **Cài đặt (Settings)**
- ✅ Cài đặt cửa hàng (trạng thái hoạt động)
- ✅ Cài đặt giờ làm việc
- ✅ Cài đặt ngưỡng cảnh báo tồn kho
- ✅ Cài đặt email thông báo
- ⚠️ **VẤN ĐỀ**: Các cài đặt chưa được lưu vào database

### 16. **Quản lý Cửa hàng (Store Management)**
- ✅ Xem thông tin cửa hàng
- ✅ Xem tồn kho cửa hàng (StoreInventory)
- ✅ Xem cài đặt cửa hàng (StoreSettings)
- ✅ Xem nhân viên cửa hàng
- ✅ Thống kê cửa hàng
- ✅ Cập nhật tồn kho cửa hàng
- ✅ Cập nhật cài đặt cửa hàng

---

## 🚨 CÁC VẤN ĐỀ CẦN KHẮC PHỤC

### **Vấn đề 1: Thiếu cột SellerId trong bảng Products**
**Mô tả**: Hiện tại bảng `Products` không có cột `SellerId` để liên kết sản phẩm với seller cụ thể. Điều này dẫn đến việc tất cả seller có thể thấy và quản lý TẤT CẢ sản phẩm trong hệ thống.

**Giải pháp**:
1. Thêm migration để thêm cột `SellerId` vào bảng `Products`
2. Cập nhật model `Product` để bao gồm property `SellerId`
3. Cập nhật tất cả queries trong `SellerController` và `SellerProductsController` để filter theo `SellerId`

**File cần sửa**:
- `/Models/Product.cs` - Thêm property `SellerId`
- Tạo migration mới
- `/Controllers/SellerController.cs` - Cập nhật queries
- `/Controllers/SellerProductsController.cs` - Cập nhật queries

### **Vấn đề 2: Thiếu cột SellerId trong bảng Coupons**
**Mô tả**: Tương tự Products, bảng `Coupons` không có cột `SellerId` nên tất cả seller có thể thấy mã giảm giá của nhau.

**Giải pháp**:
1. Thêm cột `SellerId` vào bảng `Coupons` (có thể NULL cho coupons của hệ thống)
2. Filter coupons theo seller khi hiển thị

### **Vấn đề 3: Dashboard hiển thị dữ liệu của toàn hệ thống**
**Mô tả**: Method `GetSellerDashboardStats()` hiện tại đang query toàn bộ products và orders thay vì chỉ của seller cụ thể.

**Giải pháp**: Cập nhật logic trong method này để filter theo `SellerId` sau khi đã thêm cột `SellerId` vào Products.

### **Vấn đề 4: Orders không filter theo seller**
**Mô tả**: Seller hiện có thể xem TẤT CẢ đơn hàng trong hệ thống, không chỉ đơn hàng có sản phẩm của mình.

**Giải pháp**: 
- Sau khi có `SellerId` trong Products, cần join OrderItems với Products để chỉ hiển thị orders có chứa sản phẩm của seller hiện tại

### **Vấn đề 5: Settings không được persist vào database**
**Mô tả**: Trong method `Settings()`, các cài đặt chỉ được hiển thị hardcode, không lưu vào database.

**Giải pháp**:
- Tạo bảng `SellerSettings` hoặc sử dụng bảng `StoreSettings` hiện có
- Lưu và load settings từ database

### **Vấn đề 6: Analytics chưa hoàn chỉnh**
**Mô tả**: Method `Analytics()` chỉ có top products, thiếu nhiều metrics quan trọng.

**Giải pháp**: Bổ sung thêm:
- Tỷ lệ conversion
- Traffic sources
- Customer retention rate
- Revenue growth rate
- Product categories performance

### **Vấn đề 7: Thiếu validation và error handling**
**Mô tả**: Nhiều methods thiếu validation đầu vào và error handling đầy đủ.

**Giải pháp**:
- Thêm try-catch blocks
- Validate input parameters
- Return proper error messages

---

## 🔧 DANH SÁCH CÔNG VIỆC CẦN THỰC HIỆN

### **PHASE 1: Database Schema Updates (Ưu tiên cao)**

#### 1.1. Thêm SellerId vào Products
```csharp
// File: /Models/Product.cs
public class Product
{
    // ... existing properties ...
    
    [MaxLength(450)]
    public string? SellerId { get; set; }
    
    [ForeignKey("SellerId")]
    public ApplicationUser? Seller { get; set; }
}
```

#### 1.2. Tạo Migration
```bash
dotnet ef migrations add AddSellerIdToProducts
dotnet ef database update
```

#### 1.3. Thêm SellerId vào Coupons
```csharp
// File: /Models/Coupon.cs
[MaxLength(450)]
public string? SellerId { get; set; }  // NULL = system-wide coupon

[ForeignKey("SellerId")]
public ApplicationUser? Seller { get; set; }
```

#### 1.4. Tạo bảng SellerSettings
```csharp
public class SellerSettings
{
    public Guid Id { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("SellerId")]
    public ApplicationUser Seller { get; set; } = null!;
}
```

### **PHASE 2: Controller Updates (Ưu tiên cao)**

#### 2.1. Cập nhật SellerController.cs
Sửa tất cả các TODO comments:

**Dashboard:**
```csharp
private async Task<SellerDashboardViewModel> GetSellerDashboardStats(string sellerId)
{
    var myProductsCount = await _context.Products
        .Where(p => p.SellerId == sellerId)
        .CountAsync();
    
    var myProductIds = await _context.Products
        .Where(p => p.SellerId == sellerId)
        .Select(p => p.Id)
        .ToListAsync();
    
    var myOrders = await _context.OrderItems
        .Include(oi => oi.Order)
        .Where(oi => myProductIds.Contains(oi.ProductId) && 
                     oi.Order.Status == "completed")
        .Select(oi => oi.Order)
        .Distinct()
        .ToListAsync();
    
    // ... rest of logic
}
```

**Inventory:**
```csharp
[HttpGet("inventory")]
public async Task<IActionResult> Inventory(string search = "", bool lowStock = false)
{
    var currentUser = await _userManager.GetUserAsync(User);
    
    var query = _context.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Where(p => p.SellerId == currentUser.Id);  // FIXED: Filter by seller
    
    // ... rest of logic
}
```

**Orders:**
```csharp
[HttpGet("orders")]
public async Task<IActionResult> Orders(...)
{
    var currentUser = await _userManager.GetUserAsync(User);
    
    var myProductIds = await _context.Products
        .Where(p => p.SellerId == currentUser.Id)
        .Select(p => p.Id)
        .ToListAsync();
    
    var query = _context.OrderItems
        .Include(oi => oi.Order)
        .ThenInclude(o => o.User)
        .Where(oi => myProductIds.Contains(oi.ProductId))
        .Select(oi => oi.Order)
        .Distinct();
    
    // ... rest of logic
}
```

#### 2.2. Cập nhật SellerProductsController.cs
```csharp
[HttpGet("")]
public async Task<IActionResult> Index(...)
{
    var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    var query = _context.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Where(p => p.SellerId == currentUserId);  // FIXED: Filter by seller
    
    // ... rest of logic
}

[HttpPost("create")]
public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
{
    if (ModelState.IsValid)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        product.SellerId = currentUserId;  // FIXED: Set seller ID
        
        // ... rest of logic
    }
}
```

### **PHASE 3: ViewModels Updates**

#### 3.1. Thêm ViewModels còn thiếu vào AdminViewModels.cs

```csharp
// Coupon Management ViewModels
public class CouponManagementViewModel
{
    public List<CouponItem> Coupons { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalCount { get; set; }
}

public class CouponItem
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class CouponCreateEditViewModel
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string DiscountType { get; set; } = "percentage"; // percentage, fixed
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal DiscountValue { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal MinOrderAmount { get; set; }
    
    public int? UsageLimit { get; set; }
    
    [Required]
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(30);
    
    public bool IsActive { get; set; } = true;
}

// Reviews ViewModels
public class SellerReviewsViewModel
{
    public List<ProductReview> Reviews { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public ReviewStatistics Statistics { get; set; } = new();
}

public class ReviewStatistics
{
    public int TotalReviews { get; set; }
    public int ApprovedReviews { get; set; }
    public int PendingReviews { get; set; }
    public int RejectedReviews { get; set; }
    public double AverageRating { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
}

// Notifications ViewModels
public class SellerNotificationsViewModel
{
    public List<Notification> Notifications { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool? IsRead { get; set; }
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
}

// Commissions ViewModels
public class SellerCommissionsViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalCommission { get; set; }
    public int TotalOrders { get; set; }
    public decimal CommissionRate { get; set; }
    public List<MonthlyCommissionData> MonthlyData { get; set; } = new();
    public List<RecentOrder> RecentOrders { get; set; } = new();
}

public class MonthlyCommissionData
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal Sales { get; set; }
    public decimal Commission { get; set; }
    public int OrderCount { get; set; }
}

// Customers ViewModels
public class SellerCustomersViewModel
{
    public List<CustomerInfo> Customers { get; set; } = new();
    public List<CustomerInfo> TopCustomers { get; set; } = new();
    public List<CustomerInfo> NewCustomers { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public string Search { get; set; } = string.Empty;
    public int TotalCount { get; set; }
}

public class CustomerInfo
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime FirstOrderDate { get; set; }
    public DateTime LastOrderDate { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AverageOrderValue { get; set; }
    public string Status { get; set; } = string.Empty; // Active, Inactive
}

// Reports ViewModels
public class SellerReportsViewModel
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int MonthlyOrders { get; set; }
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal ConversionRate { get; set; }
    public List<string> SalesChartLabels { get; set; } = new();
    public List<decimal> SalesChartData { get; set; } = new();
    public List<string> OrdersChartLabels { get; set; } = new();
    public List<int> OrdersChartData { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

// Product Performance ViewModels
public class SellerProductPerformanceViewModel
{
    public List<ProductPerformanceItem> TopProducts { get; set; } = new();
    public List<ProductPerformanceItem> LowPerformingProducts { get; set; } = new();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class ProductPerformanceItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int TotalSold { get; set; }
    public decimal Revenue { get; set; }
    public int ReviewCount { get; set; }
    public double AverageRating { get; set; }
}

// Store Management ViewModels
public class StoreManagementViewModel
{
    public Store? Store { get; set; }
    public List<StoreInventoryItem> Inventory { get; set; } = new();
    public List<StoreSettingItem> Settings { get; set; } = new();
    public StoreStatistics Statistics { get; set; } = new();
    public List<SellerStore> StoreStaff { get; set; } = new();
}

public class StoreInventoryItem
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImageUrl { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public string? Location { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class StoreSettingItem
{
    public Guid Id { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedByName { get; set; } = string.Empty;
}

public class StoreStatistics
{
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int StaffCount { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int MonthlyOrders { get; set; }
}

public class StoreSettingsViewModel
{
    public Guid StoreId { get; set; }
    
    [Required]
    [StringLength(255)]
    public string StoreName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string StoreAddress { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? Phone { get; set; }
    
    [EmailAddress]
    [StringLength(255)]
    public string? Email { get; set; }
    
    [StringLength(255)]
    public string? Website { get; set; }
    
    [StringLength(100)]
    public string? WorkingHours { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(100)]
    public string? SocialMedia { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public Dictionary<string, string> AdditionalSettings { get; set; } = new();
}

// Seller Settings ViewModels
public class SellerSettingsViewModel
{
    public bool IsStoreActive { get; set; }
    public TimeSpan BusinessHoursStart { get; set; }
    public TimeSpan BusinessHoursEnd { get; set; }
    public string ReportFrequency { get; set; } = "Weekly"; // Daily, Weekly, Monthly
    public int LowStockThreshold { get; set; }
    public EmailNotificationSettings EmailNotifications { get; set; } = new();
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
}

public class EmailNotificationSettings
{
    public bool NewOrders { get; set; }
    public bool LowStock { get; set; }
    public bool ProductReviews { get; set; }
    public bool SystemUpdates { get; set; }
}

// Seller Profile ViewModels
public class SellerProfileViewModel
{
    [Required]
    [StringLength(255)]
    public string CompanyName { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string BusinessLicense { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string TaxCode { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;
    
    [Phone]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public bool IsApproved { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

// Sales ViewModels
public class SellerSalesViewModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public List<DailySales> SalesData { get; set; } = new();
}

public class DailySales
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

// Analytics ViewModels
public class SellerAnalyticsViewModel
{
    public List<TopSellingProduct> TopProducts { get; set; } = new();
    // TODO: Add more analytics metrics
}

// Inventory ViewModels
public class InventoryListViewModel
{
    public List<InventoryItemViewModel> Items { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public string Filter { get; set; } = "all"; // all, low_stock, out_of_stock
}

public class InventoryItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

### **PHASE 4: View Updates**

Kiểm tra và cập nhật các views trong `/Views/Seller/` để đảm bảo:
- Hiển thị đúng dữ liệu từ ViewModels
- Form validation hoạt động đúng
- UI responsive và user-friendly
- Error messages hiển thị đầy đủ

### **PHASE 5: Testing**

#### 5.1. Unit Tests
- Test tất cả methods trong SellerController
- Test authorization (chỉ seller mới được truy cập)
- Test filtering theo SellerId

#### 5.2. Integration Tests
- Test toàn bộ flow từ login -> dashboard -> quản lý sản phẩm/đơn hàng
- Test upload images
- Test payment flows

#### 5.3. Manual Testing Checklist
- [ ] Login với tài khoản seller
- [ ] Dashboard hiển thị đúng dữ liệu của seller
- [ ] Tạo sản phẩm mới (có upload ảnh)
- [ ] Chỉnh sửa sản phẩm
- [ ] Xóa sản phẩm
- [ ] Xem danh sách đơn hàng (chỉ của seller)
- [ ] Cập nhật trạng thái đơn hàng
- [ ] Quản lý tồn kho
- [ ] Tạo/sửa/xóa mã giảm giá
- [ ] Xem và phê duyệt đánh giá
- [ ] Xem báo cáo doanh thu
- [ ] Xem thông báo
- [ ] Cập nhật hồ sơ
- [ ] Cập nhật cài đặt

---

## 📊 METRICS CẦN TRACK

### KPIs cho Seller Dashboard:
1. **Revenue Metrics**
   - Total revenue
   - Monthly revenue
   - Revenue growth rate (MoM, YoY)
   - Average order value

2. **Product Metrics**
   - Total products
   - Active products
   - Out of stock products
   - Low stock alerts
   - Top selling products

3. **Order Metrics**
   - Total orders
   - Pending orders
   - Completed orders
   - Cancelled orders
   - Order fulfillment rate

4. **Customer Metrics**
   - Total customers
   - New customers
   - Repeat customers
   - Customer retention rate
   - Customer lifetime value

5. **Performance Metrics**
   - Conversion rate
   - Product views
   - Add to cart rate
   - Average rating
   - Review count

---

## 🔐 SECURITY CONSIDERATIONS

1. **Authorization**
   - Đảm bảo seller chỉ có thể truy cập dữ liệu của mình
   - Validate seller ownership trước khi cho phép edit/delete
   - Implement proper role-based access control

2. **Input Validation**
   - Validate tất cả user inputs
   - Sanitize HTML content
   - Prevent SQL injection
   - Prevent XSS attacks

3. **File Upload Security**
   - Validate file types
   - Limit file sizes
   - Scan for malware
   - Use secure file naming

4. **API Security**
   - Implement rate limiting
   - Use HTTPS only
   - Validate API tokens
   - Log all API calls

---

## 📝 DOCUMENTATION

### User Documentation
Cần tạo các tài liệu hướng dẫn:
1. **Seller Getting Started Guide**
2. **Product Management Guide**
3. **Order Management Guide**
4. **Reports & Analytics Guide**
5. **FAQ for Sellers**

### Technical Documentation
1. **API Documentation**
2. **Database Schema Documentation**
3. **Deployment Guide**
4. **Troubleshooting Guide**

---

## 🎉 NEXT STEPS

1. ✅ Đọc và hiểu tài liệu này
2. ⬜ Tạo branch mới: `feature/seller-improvements`
3. ⬜ Implement Phase 1: Database updates
4. ⬜ Implement Phase 2: Controller updates
5. ⬜ Implement Phase 3: ViewModel updates
6. ⬜ Test từng chức năng
7. ⬜ Code review
8. ⬜ Merge vào main branch
9. ⬜ Deploy lên production

---

## 📞 HỖ TRỢ

Nếu có vấn đề trong quá trình implement, vui lòng:
1. Kiểm tra lại tài liệu này
2. Xem logs trong `/logs/`
3. Kiểm tra database schema
4. Liên hệ team lead

---

**Ngày cập nhật cuối**: 10/11/2025
**Người tạo**: GitHub Copilot
**Version**: 1.0
