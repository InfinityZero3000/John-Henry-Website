using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnHenryFashionWeb.Models
{
    /// <summary>
    /// Payment transaction record for all payments in the system
    /// Tracks payments between customers, sellers, and the platform
    /// </summary>
    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        
        public Guid OrderId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;  // Customer who made payment
        
        public string? SellerId { get; set; }  // Seller receiving payment
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PlatformFee { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellerAmount { get; set; }  // Amount after platform fee
        
        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;  // COD, Card, Wallet, Banking
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";  // pending, completed, failed, refunded
        
        [StringLength(255)]
        public string? TransactionReference { get; set; }  // External payment gateway reference
        
        [StringLength(100)]
        public string? PaymentGateway { get; set; }  // VNPay, Momo, ZaloPay, etc.
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;
        
        [ForeignKey("UserId")]
        public ApplicationUser Customer { get; set; } = null!;
        
        [ForeignKey("SellerId")]
        public ApplicationUser? Seller { get; set; }
    }

    /// <summary>
    /// Settlement record for sellers - tracks revenue reconciliation
    /// Admin uses this to calculate and transfer money to sellers
    /// </summary>
    public class SellerSettlement
    {
        public Guid Id { get; set; }
        
        [Required]
        public string SellerId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string SettlementNumber { get; set; } = string.Empty;  // e.g., STL-2025-001
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; }  // Total sales in period
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PlatformFee { get; set; }  // Platform commission
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetAmount { get; set; }  // Amount seller receives
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal PreviousBalance { get; set; } = 0;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalBalance { get; set; }
        
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";  // pending, processing, completed, cancelled
        
        public DateTime? SettledAt { get; set; }
        public string? SettledBy { get; set; }  // Admin user ID
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; } = null!;
        
        [ForeignKey("SettledBy")]
        public ApplicationUser? SettlementAdmin { get; set; }
    }

    /// <summary>
    /// Withdrawal request from sellers to get their earnings
    /// </summary>
    public class WithdrawalRequest
    {
        public Guid Id { get; set; }
        
        [Required]
        public string SellerId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string WithdrawalNumber { get; set; } = string.Empty;  // e.g., WD-2025-001
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(100)]
        public string BankName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string AccountName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Branch { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";  // pending, approved, rejected, processing, completed
        
        public string? RejectionReason { get; set; }
        
        [StringLength(255)]
        public string? TransactionReference { get; set; }
        
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        public string? ProcessedBy { get; set; }  // Admin user ID
        
        public string? AdminNotes { get; set; }
        
        // Navigation properties
        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; } = null!;
        
        [ForeignKey("ProcessedBy")]
        public ApplicationUser? Processor { get; set; }
    }

    /// <summary>
    /// Payment method configuration (COD, Online Banking, E-wallet, etc.)
    /// </summary>
    public class PaymentMethodConfig
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;  // e.g., "VNPay", "COD", "Momo"
        
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;  // vnpay, cod, momo
        
        public string? Description { get; set; }
        
        [StringLength(255)]
        public string? LogoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal TransactionFeePercent { get; set; } = 0;  // % fee charged
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TransactionFeeFixed { get; set; } = 0;  // Fixed fee per transaction
        
        public int SortOrder { get; set; } = 0;
        
        // API Configuration (stored as JSON)
        public string? ApiConfiguration { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
