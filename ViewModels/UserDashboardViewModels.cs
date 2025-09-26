using System.ComponentModel.DataAnnotations;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int WishlistCount { get; set; }
        public int CartItemsCount { get; set; }
        public int UnreadNotifications { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }

    public class UserProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ là bắt buộc")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        [Display(Name = "Tên")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Avatar")]
        public string? Avatar { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public class UserOrdersViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalOrders { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class UserNotificationsViewModel
    {
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalNotifications { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class DashboardSecurityViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();
        public List<ActiveSession> ActiveSessions { get; set; } = new List<ActiveSession>();
        public bool TwoFactorEnabled { get; set; }
    }

    public class UserAddressViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người nhận không được vượt quá 100 ký tự")]
        [Display(Name = "Tên người nhận")]
        public string RecipientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ bổ sung không được vượt quá 200 ký tự")]
        [Display(Name = "Địa chỉ bổ sung")]
        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        [StringLength(50, ErrorMessage = "Thành phố không được vượt quá 50 ký tự")]
        [Display(Name = "Thành phố")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tỉnh/Bang là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tỉnh/Bang không được vượt quá 50 ký tự")]
        [Display(Name = "Tỉnh/Bang")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã bưu điện là bắt buộc")]
        [StringLength(10, ErrorMessage = "Mã bưu điện không được vượt quá 10 ký tự")]
        [Display(Name = "Mã bưu điện")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        [StringLength(50, ErrorMessage = "Quốc gia không được vượt quá 50 ký tự")]
        [Display(Name = "Quốc gia")]
        public string Country { get; set; } = "Vietnam";

        [Display(Name = "Đặt làm địa chỉ mặc định")]
        public bool IsDefault { get; set; }

        public string AddressType { get; set; } = "shipping";
    }
}