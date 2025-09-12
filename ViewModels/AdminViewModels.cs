using System.ComponentModel.DataAnnotations;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    // Dashboard ViewModels
    public class AdminDashboardViewModel
    {
        public DashboardStats Stats { get; set; } = new();
        public List<RecentOrder> RecentOrders { get; set; } = new();
        public List<TopSellingProduct> TopProducts { get; set; } = new();
        public List<MonthlyRevenue> RevenueChart { get; set; } = new();
    }

    public class SellerDashboardViewModel
    {
        public int MyProductsCount { get; set; }
        public int MyOrdersCount { get; set; }
        public decimal MyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<RecentOrder> MyRecentOrders { get; set; } = new();
        public List<TopSellingProduct> MyTopProducts { get; set; } = new();
    }

    // Product Management ViewModels
    public class ProductListViewModel
    {
        public IEnumerable<ProductListItemViewModel> Products { get; set; } = new List<ProductListItemViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Category> Categories { get; set; } = new();
    }

    public class ProductListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class ProductCreateEditViewModel
    {
        public Guid? Id { get; set; }
        
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "SKU là bắt buộc")]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn 0")]
        public decimal? SalePrice { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public Guid CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        [StringLength(1000)]
        public string? ShortDescription { get; set; }

        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        public bool ManageStock { get; set; } = true;
        public bool InStock { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public bool IsActive { get; set; } = true;

        [StringLength(100)]
        public string? Size { get; set; }

        [StringLength(100)]
        public string? Color { get; set; }

        [StringLength(100)]
        public string? Material { get; set; }

        public decimal? Weight { get; set; }

        [StringLength(100)]
        public string? Dimensions { get; set; }

        public string Status { get; set; } = "active";

        // For file uploads
        public IFormFile? FeaturedImage { get; set; }
        public IList<IFormFile>? GalleryImages { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public string[]? ExistingGalleryImages { get; set; }

        // Dropdown data
        public List<Category> Categories { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
    }

    // Category Management ViewModels
    public class CategoryListViewModel
    {
        public IEnumerable<CategoryListItemViewModel> Categories { get; set; } = new List<CategoryListItemViewModel>();
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class CategoryListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CategoryCreateEditViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public Guid? ParentId { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0, int.MaxValue)]
        public int SortOrder { get; set; } = 0;

        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

        public List<Category> ParentCategories { get; set; } = new();
    }

    // Order Management ViewModels
    public class OrderListViewModel
    {
        public IEnumerable<OrderListItemViewModel> Orders { get; set; } = new List<OrderListItemViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class OrderListItemViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
        
        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSKU { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Inventory Management ViewModels
    public class InventoryListViewModel
    {
        public IEnumerable<InventoryItemViewModel> Items { get; set; } = new List<InventoryItemViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty; // all, low_stock, out_of_stock
    }

    public class InventoryItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int CurrentStock { get; set; }
        public int MinStockLevel { get; set; }
        public int MinStock { get; set; }
        public int MaxStock { get; set; }
        public int SoldQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? Location { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock => CurrentStock <= MinStock;
        public bool IsOutOfStock => CurrentStock <= 0;
    }

    public class StockAdjustmentViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int CurrentStock { get; set; }

        [Required(ErrorMessage = "Số lượng điều chỉnh là bắt buộc")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Loại điều chỉnh là bắt buộc")]
        public string Type { get; set; } = string.Empty; // IN, OUT, ADJUSTMENT

        [StringLength(500)]
        public string? Reason { get; set; }

        [StringLength(100)]
        public string? Reference { get; set; }
    }

    // User Management ViewModels
    public class UserListViewModel
    {
        public IEnumerable<UserListItemViewModel> Users { get; set; } = new List<UserListItemViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new();
        public string Role => Roles.FirstOrDefault() ?? "Customer";
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public string Status => IsActive ? "Active" : "Inactive";
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? CompanyName { get; set; }
    }

    // Inventory Management ViewModels
    public class InventoryManagementViewModel
    {
        public int TotalProducts { get; set; }
        public int InStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public List<InventoryItemViewModel> InventoryItems { get; set; } = new();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

    // Order Management ViewModels
    public class OrderManagementViewModel
    {
        public int PendingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int TotalOrders { get; set; }
        public List<OrderSummaryViewModel> Orders { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        
        // Enhanced properties for new Orders view
        public int PageSize { get; set; } = 20;
        public string SearchTerm { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SortBy { get; set; } = "newest";
        
        // Statistics
        public int PendingOrdersCount => PendingOrders;
        public int ProcessingOrdersCount { get; set; }
        public int ShippedOrdersCount => ShippingOrders;
        public decimal TotalRevenue { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class OrderDetailsViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CouponCode { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new();
        public List<OrderHistoryViewModel> OrderHistory { get; set; } = new();
    }

    public class OrderHistoryViewModel
    {
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    // Product Management ViewModels
    public class CreateProductViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? SKU { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? AvailableSizes { get; set; }
        public string? AvailableColors { get; set; }
    }

    public class EditProductViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public bool IsActive { get; set; }
        public string? AvailableSizes { get; set; }
        public string? AvailableColors { get; set; }
        public string? ImageUrl { get; set; }
        public int ViewCount { get; set; }
        public int SoldQuantity { get; set; }
        public decimal? AverageRating { get; set; }
        public Category? Category { get; set; }
    }

    // Reports Supporting Classes
    public class TopSellingProductReport
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class LowStockProductReport
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }

    // User Management ViewModels - Enhanced
    public class UserManagementViewModel
    {
        public IEnumerable<UserListItemViewModel> Users { get; set; } = new List<UserListItemViewModel>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        public string SearchTerm { get; set; } = string.Empty;
        public string RoleFilter { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;
        public string SortBy { get; set; } = "newest";
        
        // Statistics
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int SellersCount { get; set; }
    }

    public class ReportsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public decimal RevenueGrowth { get; set; }
        public int NewCustomers { get; set; }
        public int LowStockProducts { get; set; }
        public decimal InventoryValue { get; set; }
        
        // Chart Data
        public List<string> RevenueChartLabels { get; set; } = new();
        public List<decimal> RevenueChartData { get; set; } = new();
        
        public List<string> CategoryLabels { get; set; } = new();
        public List<decimal> CategoryData { get; set; } = new();
        
        public List<string> OrderStatusLabels { get; set; } = new();
        public List<int> OrderStatusData { get; set; } = new();
        
        public List<string> CustomerChartLabels { get; set; } = new();
        public List<int> NewCustomersData { get; set; } = new();
        public List<int> ReturningCustomersData { get; set; } = new();
        
        // Top Products
        public List<TopProductReportItem> TopProducts { get; set; } = new();
    }

    public class TopProductReportItem
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public decimal GrowthRate { get; set; }
    }

    public class InventoryViewModel
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        
        public string SearchTerm { get; set; } = string.Empty;
        public string SelectedCategory { get; set; } = string.Empty;
        public string StockStatus { get; set; } = string.Empty;
        public string SortBy { get; set; } = "Name";
        
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        
        public Dictionary<string, string> Categories { get; set; } = new();
        public List<InventoryItem> InventoryItems { get; set; } = new();
        public List<StockMovement> RecentMovements { get; set; } = new();
    }

    public class InventoryItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class StockMovement
    {
        public string ProductName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // In, Out, Adjustment
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
