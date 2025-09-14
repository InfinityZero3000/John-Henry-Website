using System.ComponentModel.DataAnnotations;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Chủ đề là bắt buộc")]
        [Display(Name = "Chủ đề")]
        [StringLength(255, ErrorMessage = "Chủ đề không được vượt quá 255 ký tự")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        [Display(Name = "Nội dung")]
        [StringLength(5000, ErrorMessage = "Nội dung không được vượt quá 5000 ký tự")]
        [MinLength(10, ErrorMessage = "Nội dung phải có ít nhất 10 ký tự")]
        public string Message { get; set; } = string.Empty;
    }

    public class ContactManagementViewModel
    {
        public List<ContactMessage> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string? StatusFilter { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class ContactReplyViewModel
    {
        public Guid ContactId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string OriginalSubject { get; set; } = string.Empty;
        public string OriginalMessage { get; set; } = string.Empty;
        public DateTime OriginalDate { get; set; }

        [Required(ErrorMessage = "Nội dung phản hồi là bắt buộc")]
        [Display(Name = "Nội dung phản hồi")]
        [StringLength(5000, ErrorMessage = "Nội dung phản hồi không được vượt quá 5000 ký tự")]
        public string ReplyContent { get; set; } = string.Empty;

        [Display(Name = "Ghi chú nội bộ")]
        [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
        public string? AdminNotes { get; set; }
    }
}
