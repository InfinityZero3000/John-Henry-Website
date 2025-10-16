using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnHenryFashionWeb.Models
{
    /// <summary>
    /// System-wide promotions created by admin
    /// </summary>
    public class SystemPromotion
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "percentage";  // percentage, fixed, free_shipping, buy_x_get_y
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }  // Percentage or fixed amount
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinOrderAmount { get; set; }  // Minimum order to qualify
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxDiscountAmount { get; set; }  // Max discount cap
        
        public int? UsageLimit { get; set; }  // Total uses allowed (null = unlimited)
        public int? UsageLimitPerUser { get; set; }  // Uses per user (null = unlimited)
        public int UsageCount { get; set; } = 0;
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Targeting options (stored as JSON)
        public string? ApplicableCategories { get; set; }  // JSON array of category IDs
        public string? ApplicableProducts { get; set; }  // JSON array of product IDs
        public string? ApplicableUserGroups { get; set; }  // JSON array: ["new_users", "vip", etc.]
        
        public string? BannerImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("CreatedBy")]
        public ApplicationUser? Creator { get; set; }
    }

    /// <summary>
    /// Marketing banners for homepage, category pages, etc.
    /// </summary>
    public class MarketingBanner
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? MobileImageUrl { get; set; }  // Separate mobile image
        
        [StringLength(500)]
        public string? LinkUrl { get; set; }
        
        public bool OpenInNewTab { get; set; } = false;
        
        [Required]
        [StringLength(50)]
        public string Position { get; set; } = "home_main";  // home_main, home_side, category, product_detail, popup
        
        [StringLength(100)]
        public string? TargetPage { get; set; }  // Specific page to show on (null = all pages)
        
        public int SortOrder { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        
        public int ViewCount { get; set; } = 0;
        public int ClickCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("CreatedBy")]
        public ApplicationUser? Creator { get; set; }
    }

    /// <summary>
    /// Email marketing campaigns
    /// </summary>
    public class EmailCampaign
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Subject { get; set; } = string.Empty;
        
        [Required]
        public string HtmlContent { get; set; } = string.Empty;
        
        public string? PlainTextContent { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TargetAudience { get; set; } = "all";  // all, customers, sellers, specific_segment
        
        public string? TargetSegmentCriteria { get; set; }  // JSON criteria for targeting
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "draft";  // draft, scheduled, sending, sent, cancelled
        
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        
        public int TotalRecipients { get; set; } = 0;
        public int SentCount { get; set; } = 0;
        public int OpenCount { get; set; } = 0;
        public int ClickCount { get; set; } = 0;
        public int UnsubscribeCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("CreatedBy")]
        public ApplicationUser? Creator { get; set; }
    }

    /// <summary>
    /// Push notifications sent to users
    /// </summary>
    public class PushNotificationCampaign
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [StringLength(500)]
        public string? ActionUrl { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TargetAudience { get; set; } = "all";  // all, customers, sellers, specific_users
        
        public string? TargetUserIds { get; set; }  // JSON array of user IDs
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "draft";  // draft, scheduled, sending, sent, cancelled
        
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        
        public int TotalRecipients { get; set; } = 0;
        public int SentCount { get; set; } = 0;
        public int OpenCount { get; set; } = 0;
        public int ClickCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("CreatedBy")]
        public ApplicationUser? Creator { get; set; }
    }

    /// <summary>
    /// Flash sale events
    /// </summary>
    public class FlashSale
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [StringLength(500)]
        public string? BannerImageUrl { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal? DiscountPercentage { get; set; }
        
        public string? ProductIds { get; set; }  // JSON array of product IDs in the sale
        
        public int? StockLimit { get; set; }  // Total stock available in flash sale
        public int SoldCount { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("CreatedBy")]
        public ApplicationUser? Creator { get; set; }
    }
}
