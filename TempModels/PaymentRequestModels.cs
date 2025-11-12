using System.ComponentModel.DataAnnotations;

namespace JohnHenryFashionWeb.Models
{
    // VNPay QR Code Request
    public class VNPayQRRequest
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string OrderId { get; set; } = string.Empty;
        
        public string OrderInfo { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }

    // MoMo QR Code Request
    public class MoMoQRRequest
    {
        [Required]
        public string SessionId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public string OrderId { get; set; } = string.Empty;
        
        public string OrderInfo { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string NotifyUrl { get; set; } = string.Empty;
    }

    // QR Code Result
    public class QRCodeResult
    {
        public bool IsSuccess { get; set; }
        public string? QRCodeUrl { get; set; }
        public string? QRDataUrl { get; set; }  // Base64 encoded QR image
        public string? DeepLink { get; set; }  // For mobile app deep linking
        public string? PaymentUrl { get; set; }  // Alternative web payment URL
        public string OrderId { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int ExpiresInSeconds { get; set; } = 900; // 15 minutes
        public string? ErrorMessage { get; set; }
        public Dictionary<string, string>? AdditionalData { get; set; }
    }
}
