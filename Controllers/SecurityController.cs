using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Services;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            ISecurityService securityService,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<SecurityController> logger)
        {
            _securityService = securityService;
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Kiểm tra mức độ bảo mật tài khoản
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckAccountSecurity()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _securityService.CheckAccountSecurityAsync(userId);
                
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking account security");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi kiểm tra bảo mật" });
            }
        }

        /// <summary>
        /// Lấy danh sách nhật ký bảo mật
        /// </summary>
        [HttpGet("logs")]
        public async Task<IActionResult> GetSecurityLogs([FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var logs = await _securityService.GetUserSecurityLogsAsync(userId, pageSize);
                
                return Ok(new
                {
                    success = true,
                    data = logs.Select(l => new
                    {
                        id = l.Id,
                        eventType = l.EventType,
                        description = l.Description,
                        ipAddress = l.IpAddress,
                        userAgent = l.UserAgent,
                        createdAt = l.CreatedAt,
                        timeAgo = GetTimeAgo(l.CreatedAt)
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security logs");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Thay đổi mật khẩu với kiểm tra bảo mật
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Validate new password
                var isPasswordValid = await _securityService.IsPasswordValidAsync(request.NewPassword);
                if (!isPasswordValid)
                {
                    return BadRequest(new { success = false, message = "Mật khẩu mới không đáp ứng yêu cầu bảo mật" });
                }

                // Check password reuse
                var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, request.NewPassword);
                var isReused = await _securityService.CheckPasswordReuseAsync(userId, newPasswordHash);
                if (isReused)
                {
                    return BadRequest(new { success = false, message = "Không thể sử dụng lại mật khẩu cũ" });
                }

                // Change password
                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Không thể thay đổi mật khẩu", 
                        errors = result.Errors.Select(e => e.Description) 
                    });
                }

                // Save password history
                await _securityService.SavePasswordHistoryAsync(userId, newPasswordHash);

                // Log security event
                await _securityService.LogSecurityEventAsync(
                    userId, 
                    "PasswordChanged", 
                    "Password changed successfully",
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                // Invalidate all sessions except current
                await _securityService.InvalidateAllUserSessionsAsync(userId);

                return Ok(new { success = true, message = "Đã thay đổi mật khẩu thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi thay đổi mật khẩu" });
            }
        }

        /// <summary>
        /// Bật/tắt xác thực hai yếu tố
        /// </summary>
        [HttpPost("two-factor")]
        public async Task<IActionResult> ToggleTwoFactor([FromBody] ToggleTwoFactorRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
                }

                if (request.Enable)
                {
                    // Enable 2FA
                    await _userManager.SetTwoFactorEnabledAsync(user, true);
                    
                    await _securityService.LogSecurityEventAsync(
                        userId, 
                        "TwoFactorEnabled", 
                        "Two-factor authentication enabled",
                        Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                    );

                    return Ok(new { success = true, message = "Đã bật xác thực hai yếu tố" });
                }
                else
                {
                    // Disable 2FA - require password confirmation
                    var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password ?? "");
                    if (!isPasswordValid)
                    {
                        return BadRequest(new { success = false, message = "Mật khẩu không chính xác" });
                    }

                    await _userManager.SetTwoFactorEnabledAsync(user, false);
                    
                    await _securityService.LogSecurityEventAsync(
                        userId, 
                        "TwoFactorDisabled", 
                        "Two-factor authentication disabled",
                        Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                    );

                    return Ok(new { success = true, message = "Đã tắt xác thực hai yếu tố" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling two-factor authentication");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Gửi mã xác thực hai yếu tố
        /// </summary>
        [HttpPost("send-2fa-token")]
        public async Task<IActionResult> SendTwoFactorToken()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
                }

                var token = await _securityService.GenerateTwoFactorTokenAsync(userId);
                
                // Send email with token
                if (!string.IsNullOrEmpty(user.Email))
                {
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Mã xác thực hai yếu tố",
                        $"Mã xác thực của bạn là: <strong>{token}</strong><br/>Mã này có hiệu lực trong 5 phút.",
                        null,
                        null,
                        true
                    );
                }

                return Ok(new { success = true, message = "Đã gửi mã xác thực đến email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending two-factor token");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Xác thực mã hai yếu tố
        /// </summary>
        [HttpPost("verify-2fa-token")]
        public async Task<IActionResult> VerifyTwoFactorToken([FromBody] VerifyTwoFactorRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var isValid = await _securityService.ValidateTwoFactorTokenAsync(userId, request.Token);
                
                if (isValid)
                {
                    return Ok(new { success = true, message = "Xác thực thành công" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Mã xác thực không chính xác hoặc đã hết hạn" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying two-factor token");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Lấy danh sách phiên đăng nhập đang hoạt động
        /// </summary>
        [HttpGet("active-sessions")]
        public async Task<IActionResult> GetActiveSessions()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var sessions = await _securityService.GetActiveSessionsAsync(userId);
                
                return Ok(new
                {
                    success = true,
                    data = sessions.Select(s => new
                    {
                        id = s.Id,
                        sessionId = s.SessionId,
                        ipAddress = s.IpAddress,
                        userAgent = s.UserAgent,
                        createdAt = s.CreatedAt,
                        lastActivity = s.LastActivity,
                        expiresAt = s.ExpiresAt,
                        isActive = s.IsActive,
                        isCurrent = s.SessionId == Request.HttpContext.Session.Id
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active sessions");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Đăng xuất khỏi phiên cụ thể
        /// </summary>
        [HttpDelete("sessions/{sessionId}")]
        public async Task<IActionResult> InvalidateSession(string sessionId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _securityService.InvalidateSessionAsync(sessionId);
                
                await _securityService.LogSecurityEventAsync(
                    userId, 
                    "SessionInvalidated", 
                    $"Session {sessionId} invalidated",
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Ok(new { success = true, message = "Đã đăng xuất khỏi phiên" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating session");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        /// <summary>
        /// Đăng xuất khỏi tất cả phiên
        /// </summary>
        [HttpDelete("sessions")]
        public async Task<IActionResult> InvalidateAllSessions()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _securityService.InvalidateAllUserSessionsAsync(userId);
                
                await _securityService.LogSecurityEventAsync(
                    userId, 
                    "AllSessionsInvalidated", 
                    "All sessions invalidated",
                    Request.HttpContext.Connection.RemoteIpAddress?.ToString()
                );

                return Ok(new { success = true, message = "Đã đăng xuất khỏi tất cả phiên" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating all sessions");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var now = DateTime.UtcNow;
            var timeSpan = now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            else if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            else if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
            else if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            else
                return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ToggleTwoFactorRequest
    {
        public bool Enable { get; set; }
        public string? Password { get; set; }
    }

    public class VerifyTwoFactorRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
