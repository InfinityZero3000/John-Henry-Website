using Microsoft.AspNetCore.Identity;

namespace JohnHenryFashionWeb.Models
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Seller = "Seller";
        public const string Customer = "Customer";
    }

    public static class AdminPolicies
    {
        public const string RequireAdminRole = "RequireAdminRole";
        public const string RequireSellerRole = "RequireSellerRole";
        public const string RequireAdminOrSellerRole = "RequireAdminOrSellerRole";
    }

    // Mở rộng ApplicationUser để hỗ trợ seller
    public partial class ApplicationUser
    {
        public string? CompanyName { get; set; }
        public string? BusinessLicense { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? Notes { get; set; }
    }

    // Model cho thống kê dashboard
    public class DashboardStats
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalSellers { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        
        // Additional properties for enhanced dashboard
        public int TodayOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal GrowthRate { get; set; }
        
        public List<RecentOrder> RecentOrders { get; set; } = new();
        public List<TopSellingProduct> TopSellingProducts { get; set; } = new();
        public List<MonthlyRevenue> MonthlyRevenueData { get; set; } = new();
    }

    public class RecentOrder
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class TopSellingProduct
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    // Model cho quản lý kho
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int MinStock { get; set; } = 10;
        public int MaxStock { get; set; } = 1000;
        public decimal CostPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        
        public Product Product { get; set; } = null!;
        public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    }

    public class StockMovement
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Type { get; set; } = string.Empty; // IN, OUT, ADJUSTMENT
        public int Quantity { get; set; }
        public string? Reason { get; set; }
        public string? Reference { get; set; } // Order ID, PO Number, etc.
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
