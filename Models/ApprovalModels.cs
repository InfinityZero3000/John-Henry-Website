using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JohnHenryFashionWeb.Models
{
    /// <summary>
    /// Product approval workflow for seller-submitted products
    /// Admin must approve products before they go live
    /// </summary>
    public class ProductApproval
    {
        public Guid Id { get; set; }
        
        public Guid ProductId { get; set; }
        
        [Required]
        public string SellerId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";  // pending, approved, rejected, requires_changes
        
        public string? ReviewNotes { get; set; }  // Admin feedback
        
        public string? RejectionReason { get; set; }
        
        // Checklist for review (JSON)
        public string? ReviewChecklist { get; set; }  // e.g., {"images_ok": true, "description_ok": false}
        
        [StringLength(50)]
        public string? Priority { get; set; } = "normal";  // low, normal, high, urgent
        
        public string? ReviewedBy { get; set; }  // Admin user ID
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public int RevisionCount { get; set; } = 0;  // Number of times seller revised
        
        // Navigation properties
        [ForeignKey("ProductId")]
        public Product Product { get; set; } = null!;
        
        [ForeignKey("SellerId")]
        public ApplicationUser Seller { get; set; } = null!;
        
        [ForeignKey("ReviewedBy")]
        public ApplicationUser? Reviewer { get; set; }
        
        public ICollection<ProductApprovalHistory> History { get; set; } = new List<ProductApprovalHistory>();
    }

    /// <summary>
    /// History log for product approval process
    /// </summary>
    public class ProductApprovalHistory
    {
        public Guid Id { get; set; }
        
        public Guid ProductApprovalId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;  // submitted, approved, rejected, revision_requested
        
        public string? Notes { get; set; }
        
        [Required]
        public string PerformedBy { get; set; } = string.Empty;  // User ID
        
        [StringLength(50)]
        public string PerformedByType { get; set; } = "admin";  // admin, seller
        
        [StringLength(50)]
        public string? PreviousStatus { get; set; }
        
        [StringLength(50)]
        public string? NewStatus { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("ProductApprovalId")]
        public ProductApproval ProductApproval { get; set; } = null!;
        
        [ForeignKey("PerformedBy")]
        public ApplicationUser Performer { get; set; } = null!;
    }

    /// <summary>
    /// Note: Product model already has these fields in DomainModels.cs:
    /// - ApprovalStatus (already exists in DB)
    /// - SellerId (already exists in DB) 
    /// - IsApproved, ApprovedAt, ApprovedBy (will be added by migration if not exists)
    /// No need to duplicate here - Product partial class already defined in DomainModels.cs
    /// </summary>

    /// <summary>
    /// Category-specific approval rules
    /// Some categories may require special review
    /// </summary>
    public class CategoryApprovalRule
    {
        public Guid Id { get; set; }
        
        public Guid CategoryId { get; set; }
        
        public bool RequiresManualApproval { get; set; } = true;
        
        public bool RequiresDetailedDescription { get; set; } = true;
        
        public int MinimumImages { get; set; } = 3;
        
        public bool RequiresBrandVerification { get; set; } = false;
        
        public bool RequiresCertification { get; set; } = false;  // For regulated products
        
        public string? RequiredFields { get; set; }  // JSON array of required field names
        
        [StringLength(50)]
        public string? ApprovalTier { get; set; } = "standard";  // standard, strict, auto
        
        public int ExpectedReviewTimeDays { get; set; } = 2;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;
    }

    /// <summary>
    /// Content moderation for reviews, comments, etc.
    /// </summary>
    public class ContentModeration
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ContentType { get; set; } = string.Empty;  // review, comment, product_description, blog_post
        
        public Guid ContentId { get; set; }  // ID of the content being moderated
        
        [Required]
        public string Content { get; set; } = string.Empty;  // The actual content
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "pending";  // pending, approved, rejected, flagged
        
        [StringLength(100)]
        public string? FlaggedReason { get; set; }  // spam, inappropriate, offensive, etc.
        
        public string? ModeratorNotes { get; set; }
        
        public string? SubmittedBy { get; set; }  // User who submitted content
        
        public string? ModeratedBy { get; set; }  // Admin who reviewed
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModeratedAt { get; set; }
        
        // AI moderation score (if using AI)
        [Column(TypeName = "decimal(3,2)")]
        public decimal? AutoModerationScore { get; set; }  // 0.00 to 1.00
        
        public bool AutoFlagged { get; set; } = false;
        
        // Navigation properties
        [ForeignKey("SubmittedBy")]
        public ApplicationUser? Submitter { get; set; }
        
        [ForeignKey("ModeratedBy")]
        public ApplicationUser? Moderator { get; set; }
    }
}
