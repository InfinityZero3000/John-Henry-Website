using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnHenryFashionWeb.Models
{
    /// <summary>
    /// System configuration key-value store
    /// </summary>
    public class SystemConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;  // e.g., "platform_fee_rate", "min_withdrawal_amount"
        
        [Required]
        public string Value { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = "string";  // string, number, boolean, json
        
        [Required]
        [StringLength(50)]
        public string Category { get; set; } = "general";  // general, payment, shipping, tax, email, notification
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsEncrypted { get; set; } = false;  // For sensitive data like API keys
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? UpdatedBy { get; set; }  // Admin user ID
        
        // Navigation properties
        [ForeignKey("UpdatedBy")]
        public ApplicationUser? Updater { get; set; }
    }

    /// <summary>
    /// Shipping provider configuration
    /// </summary>
    public class ShippingConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; } = string.Empty;  // GHN, GHTK, Viettel Post, etc.
        
        [Required]
        [StringLength(50)]
        public string ProviderCode { get; set; } = string.Empty;  // ghn, ghtk, vtp
        
        public string? Description { get; set; }
        
        [StringLength(255)]
        public string? LogoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseRate { get; set; } = 0;  // Base shipping cost
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PerKgRate { get; set; } = 0;  // Cost per kg
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FreeShippingThreshold { get; set; }  // Free shipping over this amount
        
        // Zone-based rates (JSON)
        public string? ZoneRates { get; set; }  // e.g., {"zone1": 25000, "zone2": 35000}
        
        // API Configuration
        [StringLength(500)]
        public string? ApiUrl { get; set; }
        
        [StringLength(255)]
        public string? ApiKey { get; set; }
        
        [StringLength(255)]
        public string? ApiSecret { get; set; }
        
        public string? ApiConfiguration { get; set; }  // Additional API config as JSON
        
        public int EstimatedDeliveryDays { get; set; } = 3;
        
        public int SortOrder { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Tax/VAT configuration by region
    /// </summary>
    public class TaxConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;  // e.g., "VAT Vietnam"
        
        [Required]
        [StringLength(50)]
        public string TaxType { get; set; } = "vat";  // vat, sales_tax, gst
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal Rate { get; set; }  // Tax rate percentage (e.g., 10.00 for 10%)
        
        [StringLength(100)]
        public string? Region { get; set; }  // Country or region code
        
        [StringLength(100)]
        public string? Province { get; set; }  // Specific province/state
        
        public bool IsActive { get; set; } = true;
        
        public bool ApplyToShipping { get; set; } = false;
        
        // Product categories exempt from tax (JSON array)
        public string? ExemptCategories { get; set; }
        
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Email server configuration
    /// </summary>
    public class EmailConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Provider { get; set; } = string.Empty;  // SMTP, SendGrid, AWS SES, etc.
        
        [Required]
        [StringLength(255)]
        public string SmtpHost { get; set; } = string.Empty;
        
        public int SmtpPort { get; set; } = 587;
        
        public bool UseSsl { get; set; } = true;
        
        [Required]
        [StringLength(255)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Password { get; set; } = string.Empty;  // Should be encrypted
        
        [Required]
        [StringLength(255)]
        public string FromEmail { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? FromName { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsDefault { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Payment gateway configuration
    /// </summary>
    public class PaymentGatewayConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string GatewayName { get; set; } = string.Empty;  // VNPay, Momo, ZaloPay, Stripe, etc.
        
        [Required]
        [StringLength(50)]
        public string GatewayCode { get; set; } = string.Empty;  // vnpay, momo, zalopay
        
        public string? Description { get; set; }
        
        [StringLength(255)]
        public string? LogoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public bool IsSandbox { get; set; } = false;  // Test mode
        
        [StringLength(500)]
        public string? ApiUrl { get; set; }
        
        [StringLength(255)]
        public string? MerchantId { get; set; }
        
        [StringLength(255)]
        public string? ApiKey { get; set; }
        
        [StringLength(500)]
        public string? ApiSecret { get; set; }
        
        // Additional configuration as JSON
        public string? Configuration { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal TransactionFeePercent { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TransactionFeeFixed { get; set; } = 0;
        
        public int SortOrder { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Role and permission management
    /// </summary>
    public class RolePermission
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string RoleId { get; set; } = string.Empty;  // Identity role ID
        
        [Required]
        [StringLength(100)]
        public string Permission { get; set; } = string.Empty;  // e.g., "users.view", "products.edit"
        
        [StringLength(100)]
        public string? Module { get; set; }  // users, products, orders, etc.
        
        public bool IsGranted { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }  // Admin user ID
    }

    /// <summary>
    /// Platform fee configuration based on seller tier or category
    /// </summary>
    public class PlatformFeeConfiguration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? SellerTier { get; set; }  // basic, premium, vip (null = default)
        
        public Guid? CategoryId { get; set; }  // Specific category (null = all categories)
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal FeePercent { get; set; }  // Commission percentage
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinFee { get; set; }  // Minimum fee per transaction
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxFee { get; set; }  // Maximum fee cap
        
        public bool IsActive { get; set; } = true;
        
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
