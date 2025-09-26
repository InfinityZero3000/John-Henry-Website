using System.ComponentModel.DataAnnotations;

namespace JohnHenryFashionWeb.Models
{
    public class Store
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Phone { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Province { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string StoreType { get; set; } = string.Empty; // "Cửa hàng nhượng quyền", "Đối tác phân phối"
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(100)]
        public string? WorkingHours { get; set; }
        
        [MaxLength(255)]
        public string? Website { get; set; }
        
        [MaxLength(255)]
        public string? Email { get; set; }
        
        [MaxLength(100)]
        public string? SocialMedia { get; set; } // JSON string for social links
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<SellerStore> SellerStores { get; set; } = new List<SellerStore>();
    }

    public class SellerStore
    {
        public Guid Id { get; set; }
        public string SellerId { get; set; } = string.Empty;
        public Guid StoreId { get; set; }
        public string Role { get; set; } = "manager"; // owner, manager, staff
        public bool IsActive { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ApplicationUser Seller { get; set; } = null!;
        public Store Store { get; set; } = null!;
    }

    public class StoreSettings
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation properties
        public Store Store { get; set; } = null!;
        public ApplicationUser UpdatedByUser { get; set; } = null!;
    }

    public class StoreInventory
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int MinimumStock { get; set; } = 0;
        public int MaximumStock { get; set; } = 0;
        public string? Location { get; set; } // Shelf location, etc.
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation properties
        public Store Store { get; set; } = null!;
        public Product Product { get; set; } = null!;
        public ApplicationUser UpdatedByUser { get; set; } = null!;
    }
}