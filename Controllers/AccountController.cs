using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JohnHenryFashionWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly ISecurityService _securityService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger,
            ISecurityService securityService,
            IEmailService emailService,
            IAuthService authService,
            ICacheService cacheService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            _securityService = securityService;
            _emailService = emailService;
            _authService = authService;
            _cacheService = cacheService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                // Check if account is locked
                if (await _authService.IsAccountLockedAsync(model.Email))
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa do quá nhiều lần đăng nhập sai. Vui lòng thử lại sau.");
                    return View(model);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Bạn phải xác nhận email trước khi đăng nhập.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                
                if (result.Succeeded)
                {
                    // Track successful login
                    await _authService.TrackLoginAttemptAsync(model.Email, true, HttpContext.Connection.RemoteIpAddress?.ToString());
                    await _authService.ResetFailedLoginAttemptsAsync(model.Email);
                    
                    _logger.LogInformation("User {Email} logged in successfully.", model.Email);
                    
                    // Update last login date
                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                    }
                    
                    return await RedirectToLocal(returnUrl, user);
                }
                
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account {Email} locked out.", model.Email);
                    await _authService.TrackLoginAttemptAsync(model.Email, false, HttpContext.Connection.RemoteIpAddress?.ToString());
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    // Track failed login attempt
                    await _authService.TrackLoginAttemptAsync(model.Email, false, HttpContext.Connection.RemoteIpAddress?.ToString());
                    
                    var failedAttempts = await _authService.GetFailedLoginAttemptsAsync(model.Email);
                    
                    if (failedAttempts >= 2)
                    {
                        ModelState.AddModelError(string.Empty, $"Email hoặc mật khẩu không đúng. Bạn còn {3 - failedAttempts} lần thử.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                    }
                    
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    // Check if email verification is required
                    if (model.RequireEmailVerification)
                    {
                        // Generate 6-digit verification code
                        var verificationCode = new Random().Next(100000, 999999).ToString();
                        
                        // Store verification code in cache or database (for 10 minutes)
                        var cacheKey = $"email_verification_{user.Email}";
                        await _cacheService.SetAsync(cacheKey, verificationCode, TimeSpan.FromMinutes(10));
                        
                        // Send verification code email
                        await _emailService.SendEmailAsync(user.Email, "Mã xác thực tài khoản John Henry",
                            $@"
                            <h2>Xác thực tài khoản</h2>
                            <p>Chào {user.FirstName} {user.LastName},</p>
                            <p>Mã xác thực của bạn là: <strong style='font-size: 24px; color: #007bff;'>{verificationCode}</strong></p>
                            <p>Mã này sẽ hết hiệu lực sau 10 phút.</p>
                            <p>Trân trọng,<br>Đội ngũ John Henry</p>
                            ", isHtml: true);

                        // Add user to default role but keep email unconfirmed
                        await _userManager.AddToRoleAsync(user, "Customer");

                        // Redirect to email verification page
                        return RedirectToAction("EmailVerification", new { email = user.Email, returnUrl });
                    }
                    else
                    {
                        // Original flow with email confirmation link
                        var code = await _authService.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, code }, Request.Scheme);

                        await _emailService.SendEmailAsync(user.Email, "Xác nhận tài khoản",
                            $"Vui lòng xác nhận tài khoản của bạn bằng cách click vào link: <a href='{callbackUrl}'>Xác nhận email</a>", isHtml: true);

                        await _userManager.AddToRoleAsync(user, "Customer");

                        ViewBag.Message = "Tài khoản đã được tạo thành công. Vui lòng kiểm tra email để xác nhận tài khoản.";
                        return View("RegisterConfirmation");
                    }
                }
                
                AddErrors(result);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Lỗi từ nhà cung cấp dịch vụ: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Lỗi tải thông tin đăng nhập từ bên ngoài.";
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return await RedirectToLocal(returnUrl);
            }
            
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then create one automatically.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname);
                var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);

                // Parse name if firstName/lastName are null
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                {
                    if (!string.IsNullOrEmpty(fullName))
                    {
                        var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        firstName = nameParts.FirstOrDefault() ?? "";
                        lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                    }
                }

                if (string.IsNullOrEmpty(email))
                {
                    ErrorMessage = "Không thể lấy thông tin email từ tài khoản Google.";
                    _logger.LogError("Failed to get email from Google OAuth for provider {Provider}", info.LoginProvider);
                    return RedirectToAction(nameof(Login));
                }

                _logger.LogInformation("Google OAuth: Email={Email}, FirstName={FirstName}, LastName={LastName}", email, firstName, lastName);

                // Check if user with this email already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    // Link this external login to existing account
                    var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        _logger.LogInformation("External login linked to existing user {Email}", email);
                        return await RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        _logger.LogError("Failed to link external login to existing user {Email}: {Errors}", 
                            email, string.Join(", ", addLoginResult.Errors.Select(e => e.Description)));
                        ErrorMessage = "Đã có tài khoản tồn tại với email này. Vui lòng đăng nhập bằng email/mật khẩu.";
                        return RedirectToAction(nameof(Login));
                    }
                }

                // Check configuration for Google email confirmation
                var requireEmailConfirmationForGoogle = bool.Parse(_configuration["REQUIRE_EMAIL_CONFIRMATION_FOR_GOOGLE"] ?? "true");
                var googleAutoConfirmEmail = bool.Parse(_configuration["GOOGLE_AUTO_CONFIRM_EMAIL"] ?? "false");
                
                // Create new user automatically
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName ?? "User",
                    LastName = lastName ?? "",
                    IsActive = true,
                    EmailConfirmed = googleAutoConfirmEmail || !requireEmailConfirmationForGoogle, // Auto confirm if configured or not required
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createUserResult = await _userManager.CreateAsync(user);
                
                if (createUserResult.Succeeded)
                {
                    // Add user to Customer role
                    await _userManager.AddToRoleAsync(user, "Customer");
                    
                    // Add external login
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        // Check if email confirmation is required for Google users
                        if (requireEmailConfirmationForGoogle && !user.EmailConfirmed)
                        {
                            // Send email confirmation
                            await SendEmailConfirmationForGoogleUser(user);
                            TempData["Message"] = "Tài khoản đã được tạo. Vui lòng kiểm tra email để xác nhận tài khoản.";
                            return RedirectToAction(nameof(Login));
                        }
                        else
                        {
                            // Sign in immediately
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User created and logged in with {Name} provider.", info.LoginProvider);
                            
                            // Send welcome email asynchronously
                            _ = SendWelcomeEmailAsync(user);
                            
                            return await RedirectToLocal(returnUrl);
                        }
                    }
                }
                
                // If we got this far, something failed
                ErrorMessage = "Có lỗi xảy ra khi tạo tài khoản từ Google.";
                foreach (var error in createUserResult.Errors)
                {
                    _logger.LogError("Google account creation error: {Error}", error.Description);
                }
                return RedirectToAction(nameof(Login));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new TwoFactorAuthenticationViewModel 
            { 
                RememberMe = rememberMe,
                ReturnUrl = returnUrl,
                Provider = "Email" // Default provider
            };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactorLogin(TwoFactorAuthenticationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("LoginWith2fa", model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi xác thực.");
                return View("LoginWith2fa", model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(model.Provider ?? "Email", model.Code, 
                model.RememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                await _securityService.RecordLoginAttemptAsync(user.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", true);
                
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa do quá nhiều lần đăng nhập sai.");
            }
            else
            {
                ModelState.AddModelError("", "Mã xác thực không chính xác.");
                await _securityService.RecordLoginAttemptAsync(user.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", false);
            }

            return View("LoginWith2fa", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ProfileViewModel
            {
                Email = user.Email!,
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                Gender = user.Gender ?? "",
                DateOfBirth = user.DateOfBirth
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                AddErrors(updateResult);
                return View(model);
            }

            TempData["StatusMessage"] = "Thông tin của bạn đã được cập nhật.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            TempData["StatusMessage"] = "Mật khẩu của bạn đã được thay đổi thành công.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Mật khẩu của bạn đã được thiết lập thành công.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Orders()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<IActionResult> RedirectToLocal(string? returnUrl, ApplicationUser? user = null)
        {
            // Check user roles first, regardless of returnUrl for admin/seller users
            var currentUser = user ?? await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var roles = await _userManager.GetRolesAsync(currentUser);
                _logger.LogInformation($"User {currentUser.Email} has roles: {string.Join(", ", roles)}");
                
                // Admin and Seller should always go to their dashboards
                if (roles.Contains(UserRoles.Admin))
                {
                    _logger.LogInformation($"Redirecting admin user {currentUser.Email} to admin dashboard");
                    return RedirectToAction("Dashboard", "Admin", new { area = "" });
                }
                else if (roles.Contains(UserRoles.Seller))
                {
                    _logger.LogInformation($"Redirecting seller user {currentUser.Email} to seller dashboard");
                    return RedirectToAction("Dashboard", "Seller");
                }
                
                // For customers, check returnUrl first before redirecting to home
                if (roles.Contains(UserRoles.Customer) || roles.Count == 0)
                {
                    // If there's a valid returnUrl, use it for customers
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        _logger.LogInformation($"Redirecting customer user {currentUser.Email} to returnUrl: {returnUrl}");
                        return Redirect(returnUrl);
                    }
                    
                    // Otherwise redirect to home page
                    _logger.LogInformation($"Redirecting customer user {currentUser.Email} to home page");
                    return RedirectToAction("Index", "Home");
                }
            }
            
            // For other cases, check returnUrl
            if (Url.IsLocalUrl(returnUrl))
            {
                _logger.LogInformation($"Redirecting to returnUrl: {returnUrl}");
                return Redirect(returnUrl);
            }
            
            // Default redirect to home page for authenticated users, home for others
            if (User.Identity?.IsAuthenticated == true)
            {
                _logger.LogInformation("Redirecting authenticated user to home page");
                return RedirectToAction("Index", "Home");
            }
            
            _logger.LogInformation("Redirecting to home page");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [TempData]
        public string? ErrorMessage { get; set; }

        #region Address Management
        [Authorize]
        public async Task<IActionResult> Addresses()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(addresses);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddAddress()
        {
            return View(new Address());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Address model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login");
                }

                model.UserId = userId;
                model.Id = Guid.NewGuid();
                model.CreatedAt = DateTime.UtcNow;
                model.UpdatedAt = DateTime.UtcNow;

                // If this is the first address, make it default
                var existingAddresses = await _context.Addresses.Where(a => a.UserId == userId).CountAsync();
                if (existingAddresses == 0)
                {
                    model.IsDefault = true;
                }

                _context.Addresses.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Địa chỉ đã được thêm thành công!";
                return RedirectToAction("Addresses");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditAddress(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(Guid id, Address model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return RedirectToAction("Login");
                }

                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (address == null)
                {
                    return NotFound();
                }

                address.FirstName = model.FirstName;
                address.LastName = model.LastName;
                address.Company = model.Company;
                address.Address1 = model.Address1;
                address.Address2 = model.Address2;
                address.City = model.City;
                address.State = model.State;
                address.PostalCode = model.PostalCode;
                address.Country = model.Country;
                address.Phone = model.Phone;
                address.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Địa chỉ đã được cập nhật thành công!";
                return RedirectToAction("Addresses");
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return Json(new { success = false, message = "Không tìm thấy địa chỉ" });
            }

            if (address.IsDefault)
            {
                return Json(new { success = false, message = "Không thể xóa địa chỉ mặc định" });
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa địa chỉ thành công!" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(Guid id)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (address == null)
            {
                return Json(new { success = false, message = "Không tìm thấy địa chỉ" });
            }

            // Reset all addresses to non-default
            var userAddresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            foreach (var addr in userAddresses)
            {
                addr.IsDefault = false;
            }

            // Set selected address as default
            address.IsDefault = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã đặt làm địa chỉ mặc định!" });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Security()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var securityCheck = await _securityService.CheckAccountSecurityAsync(userId);
            var activeSessions = await _securityService.GetActiveSessionsAsync(userId);
            var securityLogs = await _securityService.GetSecurityLogsAsync(userId, 20);

            var model = new SecurityDashboardViewModel
            {
                SecurityScore = securityCheck.SecurityScore,
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                ActiveSessions = activeSessions.Select(s => new ActiveSessionViewModel
                {
                    SessionId = s.SessionId,
                    DeviceType = s.DeviceType ?? "Unknown Device",
                    Location = s.Location ?? "Unknown Location",
                    LastActivity = s.LastActivity,
                    IsCurrentSession = s.SessionId == HttpContext.Session.Id
                }).ToList(),
                SecurityLogs = securityLogs.Select(log => new SecurityLogViewModel
                {
                    Action = log.EventType,
                    Timestamp = log.CreatedAt,
                    IpAddress = log.IpAddress ?? "Unknown",
                    IsSuccessful = log.Description?.Contains("success") == true,
                    Details = log.Description
                }).ToList()
            };

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> TwoFactorVerification(string userId, string provider, bool rememberMe = false, string? returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new TwoFactorVerificationViewModel
            {
                UserId = userId,
                Provider = provider,
                RememberMe = rememberMe,
                ReturnUrl = returnUrl
            };

            ViewBag.MaskedEmail = MaskEmail(user.Email ?? "");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyTwoFactor(TwoFactorVerificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaskedEmail = "***@***.com";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi xác thực.");
                return View(model);
            }

            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, 
                model.RememberMe, rememberClient: false);

            if (result.Succeeded)
            {
                await _securityService.RecordLoginAttemptAsync(user.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", true);
                
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa do quá nhiều lần đăng nhập sai.");
            }
            else
            {
                ModelState.AddModelError("", "Mã xác thực không chính xác.");
                await _securityService.RecordLoginAttemptAsync(user.Id, Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "", false);
            }

            ViewBag.MaskedEmail = MaskEmail(user.Email ?? "");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendTwoFactorCode(string userId, string provider)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            var code = await _userManager.GenerateTwoFactorTokenAsync(user, provider);
            
            // Send code via email
            await _emailService.SendTwoFactorCodeEmailAsync(user.Email ?? "", code);

            return Json(new { success = true, message = "Mã xác thực đã được gửi lại." });
        }

        // Email Verification with code
        [HttpGet]
        [AllowAnonymous]
        public IActionResult EmailVerification(string email, string? returnUrl = null)
        {
            var model = new EmailVerificationViewModel
            {
                Email = email,
                ReturnUrl = returnUrl,
                CodeSentTime = DateTime.UtcNow
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailVerification(EmailVerificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get stored verification code from cache
            var cacheKey = $"email_verification_{model.Email}";
            var storedCode = await _cacheService.GetAsync<string>(cacheKey);

            if (string.IsNullOrEmpty(storedCode))
            {
                ModelState.AddModelError("", "Mã xác thực đã hết hạn. Vui lòng yêu cầu gửi lại mã mới.");
                return View(model);
            }

            if (storedCode != model.Code)
            {
                ModelState.AddModelError("Code", "Mã xác thực không chính xác.");
                return View(model);
            }

            // Find user and confirm email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View(model);
            }

            // Confirm email
            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Remove code from cache
                await _cacheService.RemoveAsync(cacheKey);

                // Sign in user
                await _signInManager.SignInAsync(user, isPersistent: false);

                ViewBag.Message = "Email đã được xác thực thành công! Tài khoản của bạn đã được kích hoạt.";
                
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi xác thực email.");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmailVerificationCode(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            // Generate new verification code
            var verificationCode = new Random().Next(100000, 999999).ToString();
            
            // Store in cache
            var cacheKey = $"email_verification_{email}";
            await _cacheService.SetAsync(cacheKey, verificationCode, TimeSpan.FromMinutes(10));
            
            // Send new code
            await _emailService.SendEmailAsync(email, "Mã xác thực tài khoản John Henry",
                $@"
                <h2>Xác thực tài khoản</h2>
                <p>Chào {user.FirstName} {user.LastName},</p>
                <p>Mã xác thực mới của bạn là: <strong style='font-size: 24px; color: #007bff;'>{verificationCode}</strong></p>
                <p>Mã này sẽ hết hiệu lực sau 10 phút.</p>
                <p>Trân trọng,<br>Đội ngũ John Henry</p>
                ", isHtml: true);

            return Json(new { success = true, message = "Mã xác thực đã được gửi lại thành công." });
        }

        // Confirm Email
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _authService.ValidateEmailConfirmationTokenAsync(user, code);
            
            var viewModel = new ConfirmEmailViewModel
            {
                UserId = userId,
                Code = code,
                IsConfirmed = result,
                Message = result ? "Email của bạn đã được xác nhận thành công!" : "Có lỗi xảy ra khi xác nhận email."
            };

            return View(viewModel);
        }

        // Forgot Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _authService.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { code }, Request.Scheme);

                await _emailService.SendEmailAsync(model.Email, "Đặt lại mật khẩu",
                    $"Vui lòng đặt lại mật khẩu của bạn bằng cách click vào link: <a href='{callbackUrl}'>Đặt lại mật khẩu</a>", isHtml: true);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Reset Password
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            if (code == null)
            {
                return BadRequest("Mã xác thực là bắt buộc để đặt lại mật khẩu.");
            }

            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // Two Factor Authentication Setup
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var key = await _authService.GetAuthenticatorKeyAsync(user);
            var authenticatorUri = await _authService.GenerateAuthenticatorUriAsync(user, key);

            var model = new EnableAuthenticatorViewModel
            {
                SharedKey = FormatKey(key),
                AuthenticatorUri = authenticatorUri
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var key = await _authService.GetAuthenticatorKeyAsync(user);
                model.SharedKey = FormatKey(key);
                model.AuthenticatorUri = await _authService.GenerateAuthenticatorUriAsync(user, key);
                return View(model);
            }

            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2faTokenValid = await _authService.ValidateTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Mã xác thực không chính xác.");
                var key = await _authService.GetAuthenticatorKeyAsync(user);
                model.SharedKey = FormatKey(key);
                model.AuthenticatorUri = await _authService.GenerateAuthenticatorUriAsync(user, key);
                return View(model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            var statusMessage = "Ứng dụng xác thực của bạn đã được xác minh.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                var recoveryCodes = await _authService.GenerateRecoveryCodesAsync(user, 10);
                TempData["RecoveryCodes"] = recoveryCodes.ToArray();
                return RedirectToAction(nameof(ShowRecoveryCodes));
            }

            TempData["StatusMessage"] = statusMessage;
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[]?)TempData["RecoveryCodes"];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new UserSecurityViewModel
            {
                RecoveryCodes = recoveryCodes,
                ShowRecoveryCodes = true
            };

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserSecurityViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                Logins = await _userManager.GetLoginsAsync(user),
                PhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user),
                EmailConfirmed = user.EmailConfirmed,
                IsLockedOut = await _userManager.IsLockedOutAsync(user),
                FailedLoginAttempts = await _userManager.GetAccessFailedCountAsync(user)
            };

            return View(model);
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return "***@***.com";

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            var maskedUsername = username.Length > 2 
                ? username.Substring(0, 2) + new string('*', username.Length - 2)
                : new string('*', username.Length);

            var maskedDomain = domain.Length > 2
                ? new string('*', domain.Length - 2) + domain.Substring(domain.Length - 2)
                : new string('*', domain.Length);

            return $"{maskedUsername}@{maskedDomain}";
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new System.Text.StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        #endregion

        #region Google OAuth Email Helpers

        private async Task SendEmailConfirmationForGoogleUser(ApplicationUser user)
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                await _emailService.SendEmailAsync(user.Email ?? string.Empty, "Xác nhận tài khoản John Henry Fashion",
                    $@"
                    <h2>Xác nhận tài khoản của bạn</h2>
                    <p>Xin chào {user.FirstName} {user.LastName},</p>
                    <p>Cảm ơn bạn đã đăng ký tài khoản John Henry Fashion thông qua Google.</p>
                    <p>Để hoàn tất việc tạo tài khoản, vui lòng click vào link bên dưới:</p>
                    <p><a href='{confirmationLink}' style='background-color: #951329; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Xác nhận tài khoản</a></p>
                    <p>Nếu bạn không thể click vào link, vui lòng copy và paste URL sau vào trình duyệt:</p>
                    <p>{confirmationLink}</p>
                    <p>Trân trọng,<br>Đội ngũ John Henry Fashion</p>
                    ", isHtml: true);

                _logger.LogInformation("Email confirmation sent to Google user {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email confirmation to Google user {Email}", user.Email);
            }
        }

        private Task SendWelcomeEmailAsync(ApplicationUser user)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailAsync(user.Email ?? string.Empty, "Chào mừng bạn đến với John Henry Fashion",
                        $@"
                        <h2>Chào mừng {user.FirstName} {user.LastName}!</h2>
                        <p>Tài khoản của bạn đã được tạo thành công thông qua Google.</p>
                        <p>Bạn có thể bắt đầu mua sắm ngay bây giờ!</p>
                        <p>Cảm ơn bạn đã gia nhập cộng đồng John Henry Fashion!</p>
                        <p>Trân trọng,<br>Đội ngũ John Henry</p>
                        ", isHtml: true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to send welcome email to {Email}: {Error}", user.Email, ex.Message);
                }
            });
        }

        #endregion

        #endregion
    }
}
