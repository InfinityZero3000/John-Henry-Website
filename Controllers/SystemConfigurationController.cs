using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Roles = UserRoles.Admin, AuthenticationSchemes = "Identity.Application")]
    [Route("admin/system-config")]
    public class SystemConfigurationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SystemConfigurationController> _logger;

        public SystemConfigurationController(
            ApplicationDbContext context,
            ILogger<SystemConfigurationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== General Settings ====================
        
        [HttpGet("")]
        public async Task<IActionResult> Index(string? category = null)
        {
            var query = _context.SystemConfigurations
                .Include(s => s.Updater)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(s => s.Category == category);
            }

            var settings = await query
                .OrderBy(s => s.Category)
                .ThenBy(s => s.Key)
                .ToListAsync();

            ViewBag.CurrentCategory = category;
            ViewBag.Categories = await _context.SystemConfigurations
                .Select(s => s.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            _logger.LogInformation("Admin viewed system settings. Category: {Category}", category ?? "All");
            return View(settings);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            ViewBag.Categories = new[] { "general", "payment", "shipping", "tax", "email", "notification", "security", "api" };
            ViewBag.Types = new[] { "string", "number", "boolean", "json" };
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SystemConfiguration config)
        {
            if (ModelState.IsValid)
            {
                // Check if key already exists
                var exists = await _context.SystemConfigurations
                    .AnyAsync(s => s.Key == config.Key);

                if (exists)
                {
                    TempData["ErrorMessage"] = $"Setting with key '{config.Key}' already exists.";
                    ViewBag.Categories = new[] { "general", "payment", "shipping", "tax", "email", "notification", "security", "api" };
                    ViewBag.Types = new[] { "string", "number", "boolean", "json" };
                    return View(config);
                }

                config.Id = Guid.NewGuid();
                config.CreatedAt = DateTime.UtcNow;
                config.UpdatedAt = DateTime.UtcNow;
                config.UpdatedBy = User.Identity?.Name;

                _context.SystemConfigurations.Add(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin created system setting: {Key}", config.Key);
                TempData["SuccessMessage"] = "Setting created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new[] { "general", "payment", "shipping", "tax", "email", "notification", "security", "api" };
            ViewBag.Types = new[] { "string", "number", "boolean", "json" };
            return View(config);
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var config = await _context.SystemConfigurations.FindAsync(id);
            if (config == null)
            {
                TempData["ErrorMessage"] = "Setting not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new[] { "general", "payment", "shipping", "tax", "email", "notification", "security", "api" };
            ViewBag.Types = new[] { "string", "number", "boolean", "json" };
            return View(config);
        }

        [HttpPost("{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SystemConfiguration config)
        {
            if (id != config.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    config.UpdatedAt = DateTime.UtcNow;
                    config.UpdatedBy = User.Identity?.Name;

                    _context.Update(config);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Admin updated system setting: {Key}", config.Key);
                    TempData["SuccessMessage"] = "Setting updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.SystemConfigurations.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Categories = new[] { "general", "payment", "shipping", "tax", "email", "notification", "security", "api" };
            ViewBag.Types = new[] { "string", "number", "boolean", "json" };
            return View(config);
        }

        [HttpPost("{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var config = await _context.SystemConfigurations.FindAsync(id);
            if (config != null)
            {
                _context.SystemConfigurations.Remove(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin deleted system setting: {Key}", config.Key);
                TempData["SuccessMessage"] = "Setting deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==================== Shipping Configuration ====================

        [HttpGet("shipping")]
        public async Task<IActionResult> Shipping()
        {
            var providers = await _context.ShippingConfigurations
                .OrderBy(s => s.SortOrder)
                .ToListAsync();

            _logger.LogInformation("Admin viewed shipping configurations");
            return View(providers);
        }

        [HttpGet("shipping/create")]
        public IActionResult CreateShipping()
        {
            return View();
        }

        [HttpPost("shipping/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateShipping(ShippingConfiguration config)
        {
            if (ModelState.IsValid)
            {
                config.Id = Guid.NewGuid();
                config.CreatedAt = DateTime.UtcNow;
                config.UpdatedAt = DateTime.UtcNow;

                _context.ShippingConfigurations.Add(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin created shipping provider: {Provider}", config.ProviderName);
                TempData["SuccessMessage"] = "Shipping provider created successfully!";
                return RedirectToAction(nameof(Shipping));
            }

            return View(config);
        }

        [HttpGet("shipping/{id}/edit")]
        public async Task<IActionResult> EditShipping(Guid id)
        {
            var config = await _context.ShippingConfigurations.FindAsync(id);
            if (config == null)
            {
                TempData["ErrorMessage"] = "Shipping provider not found.";
                return RedirectToAction(nameof(Shipping));
            }

            return View(config);
        }

        [HttpPost("shipping/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditShipping(Guid id, ShippingConfiguration config)
        {
            if (id != config.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    config.UpdatedAt = DateTime.UtcNow;
                    _context.Update(config);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Admin updated shipping provider: {Provider}", config.ProviderName);
                    TempData["SuccessMessage"] = "Shipping provider updated successfully!";
                    return RedirectToAction(nameof(Shipping));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.ShippingConfigurations.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(config);
        }

        [HttpPost("shipping/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShipping(Guid id)
        {
            var config = await _context.ShippingConfigurations.FindAsync(id);
            if (config != null)
            {
                _context.ShippingConfigurations.Remove(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin deleted shipping provider: {Provider}", config.ProviderName);
                TempData["SuccessMessage"] = "Shipping provider deleted successfully!";
            }

            return RedirectToAction(nameof(Shipping));
        }

        // ==================== Email Configuration ====================

        [HttpGet("email")]
        public async Task<IActionResult> Email()
        {
            var configs = await _context.EmailConfigurations
                .OrderByDescending(e => e.IsDefault)
                .ThenBy(e => e.Provider)
                .ToListAsync();

            _logger.LogInformation("Admin viewed email configurations");
            return View(configs);
        }

        [HttpGet("email/create")]
        public IActionResult CreateEmail()
        {
            ViewBag.Providers = new[] { "SMTP", "SendGrid", "AWS SES", "Mailgun", "Gmail" };
            return View();
        }

        [HttpPost("email/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmail(EmailConfiguration config)
        {
            if (ModelState.IsValid)
            {
                config.Id = Guid.NewGuid();
                config.CreatedAt = DateTime.UtcNow;
                config.UpdatedAt = DateTime.UtcNow;

                // If this is set as default, unset others
                if (config.IsDefault)
                {
                    var existing = await _context.EmailConfigurations
                        .Where(e => e.IsDefault)
                        .ToListAsync();
                    
                    foreach (var e in existing)
                    {
                        e.IsDefault = false;
                    }
                }

                _context.EmailConfigurations.Add(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin created email configuration: {Provider}", config.Provider);
                TempData["SuccessMessage"] = "Email configuration created successfully!";
                return RedirectToAction(nameof(Email));
            }

            ViewBag.Providers = new[] { "SMTP", "SendGrid", "AWS SES", "Mailgun", "Gmail" };
            return View(config);
        }

        [HttpGet("email/{id}/edit")]
        public async Task<IActionResult> EditEmail(Guid id)
        {
            var config = await _context.EmailConfigurations.FindAsync(id);
            if (config == null)
            {
                TempData["ErrorMessage"] = "Email configuration not found.";
                return RedirectToAction(nameof(Email));
            }

            ViewBag.Providers = new[] { "SMTP", "SendGrid", "AWS SES", "Mailgun", "Gmail" };
            return View(config);
        }

        [HttpPost("email/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmail(Guid id, EmailConfiguration config)
        {
            if (id != config.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If this is set as default, unset others
                    if (config.IsDefault)
                    {
                        var existing = await _context.EmailConfigurations
                            .Where(e => e.IsDefault && e.Id != id)
                            .ToListAsync();
                        
                        foreach (var e in existing)
                        {
                            e.IsDefault = false;
                        }
                    }

                    config.UpdatedAt = DateTime.UtcNow;
                    _context.Update(config);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Admin updated email configuration: {Provider}", config.Provider);
                    TempData["SuccessMessage"] = "Email configuration updated successfully!";
                    return RedirectToAction(nameof(Email));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.EmailConfigurations.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Providers = new[] { "SMTP", "SendGrid", "AWS SES", "Mailgun", "Gmail" };
            return View(config);
        }

        [HttpPost("email/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmail(Guid id)
        {
            var config = await _context.EmailConfigurations.FindAsync(id);
            if (config != null)
            {
                _context.EmailConfigurations.Remove(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin deleted email configuration: {Provider}", config.Provider);
                TempData["SuccessMessage"] = "Email configuration deleted successfully!";
            }

            return RedirectToAction(nameof(Email));
        }

        // ==================== Payment Gateway Configuration ====================

        [HttpGet("payment-gateways")]
        public async Task<IActionResult> PaymentGateways()
        {
            var gateways = await _context.PaymentGatewayConfigurations
                .OrderBy(p => p.SortOrder)
                .ToListAsync();

            _logger.LogInformation("Admin viewed payment gateway configurations");
            return View(gateways);
        }

        [HttpGet("payment-gateways/create")]
        public IActionResult CreatePaymentGateway()
        {
            ViewBag.Gateways = new[] { "VNPay", "Momo", "ZaloPay", "Stripe", "PayPal", "Bank Transfer", "Cash on Delivery" };
            return View();
        }

        [HttpPost("payment-gateways/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePaymentGateway(PaymentGatewayConfiguration config)
        {
            if (ModelState.IsValid)
            {
                config.Id = Guid.NewGuid();
                config.CreatedAt = DateTime.UtcNow;
                config.UpdatedAt = DateTime.UtcNow;

                _context.PaymentGatewayConfigurations.Add(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin created payment gateway: {Gateway}", config.GatewayName);
                TempData["SuccessMessage"] = "Payment gateway created successfully!";
                return RedirectToAction(nameof(PaymentGateways));
            }

            ViewBag.Gateways = new[] { "VNPay", "Momo", "ZaloPay", "Stripe", "PayPal", "Bank Transfer", "Cash on Delivery" };
            return View(config);
        }

        [HttpGet("payment-gateways/{id}/edit")]
        public async Task<IActionResult> EditPaymentGateway(Guid id)
        {
            var config = await _context.PaymentGatewayConfigurations.FindAsync(id);
            if (config == null)
            {
                TempData["ErrorMessage"] = "Payment gateway not found.";
                return RedirectToAction(nameof(PaymentGateways));
            }

            ViewBag.Gateways = new[] { "VNPay", "Momo", "ZaloPay", "Stripe", "PayPal", "Bank Transfer", "Cash on Delivery" };
            return View(config);
        }

        [HttpPost("payment-gateways/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPaymentGateway(Guid id, PaymentGatewayConfiguration config)
        {
            if (id != config.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    config.UpdatedAt = DateTime.UtcNow;
                    _context.Update(config);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Admin updated payment gateway: {Gateway}", config.GatewayName);
                    TempData["SuccessMessage"] = "Payment gateway updated successfully!";
                    return RedirectToAction(nameof(PaymentGateways));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.PaymentGatewayConfigurations.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Gateways = new[] { "VNPay", "Momo", "ZaloPay", "Stripe", "PayPal", "Bank Transfer", "Cash on Delivery" };
            return View(config);
        }

        [HttpPost("payment-gateways/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePaymentGateway(Guid id)
        {
            var config = await _context.PaymentGatewayConfigurations.FindAsync(id);
            if (config != null)
            {
                _context.PaymentGatewayConfigurations.Remove(config);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Admin deleted payment gateway: {Gateway}", config.GatewayName);
                TempData["SuccessMessage"] = "Payment gateway deleted successfully!";
            }

            return RedirectToAction(nameof(PaymentGateways));
        }
    }
}
