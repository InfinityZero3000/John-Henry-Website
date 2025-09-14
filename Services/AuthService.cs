using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Data;
using QRCoder;
using System.Text.Encodings.Web;

namespace JohnHenryFashionWeb.Services;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<bool> ValidateEmailConfirmationTokenAsync(ApplicationUser user, string token);
    Task<bool> ValidatePasswordResetTokenAsync(ApplicationUser user, string token);
    Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string provider);
    Task<bool> ValidateTwoFactorTokenAsync(ApplicationUser user, string provider, string token);
    Task<byte[]> GenerateQrCodeAsync(string text);
    Task<string> GetAuthenticatorKeyAsync(ApplicationUser user);
    Task<string> GenerateAuthenticatorUriAsync(ApplicationUser user, string key);
    Task<IEnumerable<string>> GenerateRecoveryCodesAsync(ApplicationUser user, int numberOfCodes);
    Task<bool> RedeemRecoveryCodeAsync(ApplicationUser user, string code);
    Task TrackLoginAttemptAsync(string email, bool success, string? ipAddress = null);
    Task<bool> IsAccountLockedAsync(string email);
    Task<int> GetFailedLoginAttemptsAsync(string email);
    Task ResetFailedLoginAttemptsAsync(string email);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AuthService> _logger;
    private readonly ApplicationDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<AuthService> logger,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.GivenName, user.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, user.LastName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:DurationInDays"] ?? "7")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        try
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating email confirmation token for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        try
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating password reset token for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> ValidateEmailConfirmationTokenAsync(ApplicationUser user, string token)
    {
        try
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating email confirmation token for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<bool> ValidatePasswordResetTokenAsync(ApplicationUser user, string token)
    {
        try
        {
            // Just verify the token without resetting password
            var result = await _userManager.VerifyUserTokenAsync(user, 
                _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating password reset token for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string provider)
    {
        try
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating 2FA token for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> ValidateTwoFactorTokenAsync(ApplicationUser user, string provider, string token)
    {
        try
        {
            return await _userManager.VerifyTwoFactorTokenAsync(user, provider, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating 2FA token for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task<byte[]> GenerateQrCodeAsync(string text)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new BitmapByteQRCode(qrCodeData);
            return await Task.FromResult(qrCode.GetGraphic(20));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating QR code");
            throw;
        }
    }

    public async Task<string> GetAuthenticatorKeyAsync(ApplicationUser user)
    {
        try
        {
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            return key ?? throw new InvalidOperationException("Failed to generate authenticator key");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authenticator key for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<string> GenerateAuthenticatorUriAsync(ApplicationUser user, string key)
    {
        const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        
        var appName = _configuration["AppSettings:AppName"] ?? "John Henry Fashion";
        var email = await _userManager.GetEmailAsync(user) ?? throw new InvalidOperationException("User email not found");
        
        return string.Format(
            AuthenticatorUriFormat,
            UrlEncoder.Default.Encode(appName),
            UrlEncoder.Default.Encode(email),
            key);
    }

    public async Task<IEnumerable<string>> GenerateRecoveryCodesAsync(ApplicationUser user, int numberOfCodes)
    {
        try
        {
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, numberOfCodes);
            return recoveryCodes ?? Enumerable.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recovery codes for user {UserId}", user.Id);
            throw;
        }
    }

    public async Task<bool> RedeemRecoveryCodeAsync(ApplicationUser user, string code)
    {
        try
        {
            var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, code);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error redeeming recovery code for user {UserId}", user.Id);
            return false;
        }
    }

    public async Task TrackLoginAttemptAsync(string email, bool success, string? ipAddress = null)
    {
        try
        {
            var cacheKey = $"login_attempts_{email}";
            var attempts = _cache.Get<List<LoginAttempt>>(cacheKey) ?? new List<LoginAttempt>();

            attempts.Add(new LoginAttempt
            {
                Email = email,
                Success = success,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress
            });

            // Keep only last 10 attempts
            if (attempts.Count > 10)
            {
                attempts = attempts.TakeLast(10).ToList();
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24),
                SlidingExpiration = TimeSpan.FromHours(1)
            };

            _cache.Set(cacheKey, attempts, cacheOptions);

            // Log security event
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var securityLog = new SecurityLog
                {
                    UserId = user.Id,
                    EventType = success ? "Login Success" : "Login Failed",
                    IpAddress = ipAddress,
                    UserAgent = null, // Could be passed as parameter
                    CreatedAt = DateTime.UtcNow,
                    Description = $"Login attempt for {email}"
                };

                _context.SecurityLogs.Add(securityLog);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking login attempt for {Email}", email);
        }
    }

    public async Task<bool> IsAccountLockedAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            return await _userManager.IsLockedOutAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking account lock status for {Email}", email);
            return false;
        }
    }

    public async Task<int> GetFailedLoginAttemptsAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return 0;

            return await _userManager.GetAccessFailedCountAsync(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting failed login attempts for {Email}", email);
            return 0;
        }
    }

    public async Task ResetFailedLoginAttemptsAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting failed login attempts for {Email}", email);
        }
    }

    private class LoginAttempt
    {
        public string Email { get; set; } = string.Empty;
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
    }
}
