using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    public class SellerCouponsViewModel
    {
        public List<Coupon> Coupons { get; set; } = new List<Coupon>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; } = "";
        public string Status { get; set; } = "";
        public int TotalCount { get; set; }
    }

    public class CouponCreateEditViewModel
    {
        public Guid? Id { get; set; }
        public string Code { get; set; } = "";
        public string Description { get; set; } = "";
        public string DiscountType { get; set; } = "percentage"; // percentage or fixed
        public decimal DiscountValue { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public int? UsageLimit { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class SellerReviewsViewModel
    {
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; } = "";
        public int? Rating { get; set; }
        public string Status { get; set; } = "";
        public int TotalCount { get; set; }
    }

    public class SellerNotificationsViewModel
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Type { get; set; } = "";
        public bool? IsRead { get; set; }
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
    }

    public class SellerCommissionsViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalCommission { get; set; }
        public int TotalOrders { get; set; }
        public decimal CommissionRate { get; set; }
        public List<MonthlyCommissionData> MonthlyData { get; set; } = new List<MonthlyCommissionData>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }

    public class MonthlyCommissionData
    {
        public string Month { get; set; } = "";
        public decimal Sales { get; set; }
        public decimal Commission { get; set; }
        public int OrderCount { get; set; }
    }

    public class SellerProductPerformanceViewModel
    {
        public List<ProductPerformanceItem> TopProducts { get; set; } = new List<ProductPerformanceItem>();
        public List<ProductPerformanceItem> LowPerformingProducts { get; set; } = new List<ProductPerformanceItem>();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class ProductPerformanceItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal Price { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
        public int ViewCount { get; set; }
        public double ConversionRate { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }

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
        public double AverageOrderValue { get; set; }
        public double ConversionRate { get; set; }
        
        // Charts data
        public List<string> SalesChartLabels { get; set; } = new List<string>();
        public List<decimal> SalesChartData { get; set; } = new List<decimal>();
        public List<string> OrdersChartLabels { get; set; } = new List<string>();
        public List<int> OrdersChartData { get; set; } = new List<int>();
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class SellerCustomersViewModel
    {
        public List<CustomerInfo> Customers { get; set; } = new List<CustomerInfo>();
        public List<CustomerInfo> TopCustomers { get; set; } = new List<CustomerInfo>();
        public List<CustomerInfo> NewCustomers { get; set; } = new List<CustomerInfo>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public string Search { get; set; } = "";
        public int TotalCount { get; set; }
    }

    public class CustomerInfo
    {
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime FirstOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public double AverageOrderValue { get; set; }
        public string Status { get; set; } = "";
    }
}