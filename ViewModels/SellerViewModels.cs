using System.ComponentModel.DataAnnotations;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    // ===== ORDERS MANAGEMENT =====
    
    /// <summary>
    /// ViewModel for Seller Orders List page with pagination and filters
    /// </summary>
    public class SellerOrdersViewModel
    {
        public List<OrderListItemViewModel> Orders { get; set; } = new();
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalOrders { get; set; } = 0;
        
        // Filters
        public string SearchQuery { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
        
        // Statistics for status counts
        public Dictionary<string, int> StatusCounts { get; set; } = new()
        {
            { "all", 0 },
            { "pending", 0 },
            { "processing", 0 },
            { "completed", 0 },
            { "cancelled", 0 }
        };
        
        // Quick stats for dashboard cards
        public decimal TodayRevenue { get; set; } = 0;
        public int TodayOrders { get; set; } = 0;
        public int PendingOrders { get; set; } = 0;
    }
    
    /// <summary>
    /// ViewModel for Order Detail page with full order information
    /// </summary>
    public class SellerOrderDetailViewModel
    {
        // Order basic info
        public Guid Id { get; set; }
        public Guid OrderId { get; set; } // Alias for Id
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? Notes { get; set; }
        
        // Customer information
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        
        // Address information
        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        
        // Order items
        public List<OrderItemDetailViewModel> Items { get; set; } = new();
        public List<OrderItemDetailViewModel> OrderItems { get; set; } = new(); // Alias for Items
        
        // Financial breakdown
        public decimal Subtotal { get; set; } = 0;
        public decimal ShippingFee { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public string? CouponCode { get; set; }
        
        // Shipping information
        public string? ShippingMethod { get; set; }
        public string? TrackingNumber { get; set; }
        
        // Order history timeline
        public List<OrderStatusHistoryViewModel> StatusHistory { get; set; } = new();
        
        // Seller permissions
        public bool CanUpdateStatus { get; set; } = true;
        public bool CanCancel { get; set; } = false;
        public bool CanViewFullOrder { get; set; } = false;
        public List<string> AvailableStatusTransitions { get; set; } = new();
    }
    
    /// <summary>
    /// ViewModel for individual order item in detail view
    /// </summary>
    public class OrderItemDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public string? ProductSku { get; set; }
        public string? ProductSKU { get; set; } // Alias for ProductSku
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SelectedSize { get; set; }
        public string? SelectedColor { get; set; }
        
        // Seller can see if this is their product
        public bool IsSellerProduct { get; set; } = false;
    }
    
    /// <summary>
    /// ViewModel for order status history timeline
    /// </summary>
    public class OrderStatusHistoryViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? ChangedBy { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }
    
    // ===== PROFILE & SETTINGS =====
    // Note: SellerProfileViewModel and SellerSettingsViewModel are defined in AdminViewModels.cs
    
    // ===== DASHBOARD =====
    // Note: SellerDashboardViewModel is defined in AdminViewModels.cs and used by SellerController
    
    /// <summary>
    /// ViewModel for recent orders in dashboard
    /// </summary>
    public class RecentOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }
    
    /// <summary>
    /// ViewModel for top selling products in dashboard
    /// </summary>
    public class TopSellingProductViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
        public int StockRemaining { get; set; }
    }
    
    /// <summary>
    /// ViewModel for daily revenue chart data
    /// </summary>
    public class DailyRevenueViewModel
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
    
    // ===== COUPON MANAGEMENT =====
    // Note: CouponCreateEditViewModel is defined in AdminViewModels.cs
}

