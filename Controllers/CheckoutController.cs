using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Services;
using JohnHenryFashionWeb.ViewModels;
using System.Text.Json;

namespace JohnHenryFashionWeb.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IPaymentService paymentService,
            IEmailService emailService,
            INotificationService notificationService,
            ICacheService cacheService,
            ILogger<CheckoutController> logger)
        {
            _context = context;
            _userManager = userManager;
            _paymentService = paymentService;
            _emailService = emailService;
            _notificationService = notificationService;
            _cacheService = cacheService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                // For anonymous users, get cart from session
                var sessionCart = GetCartFromSession();
                if (sessionCart == null || !sessionCart.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index", "Cart");
                }

                var model = await CreateCheckoutViewModelAsync(null, sessionCart);
                return View(model);
            }

            // For authenticated users, get cart from database
            var cartItems = await _context.ShoppingCartItems
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            var checkoutModel = await CreateCheckoutViewModelAsync(userId, cartItems.Select(c => new CartItemViewModel
            {
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                Price = c.Price,
                Quantity = c.Quantity,
                Size = c.Size,
                Color = c.Color,
                ImageUrl = c.Product.FeaturedImageUrl
            }).ToList());

            return View(checkoutModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSession(CheckoutCreateViewModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                
                // Validate model
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Get cart items
                List<CartItemViewModel> cartItems;
                if (string.IsNullOrEmpty(userId))
                {
                    cartItems = GetCartFromSession() ?? new List<CartItemViewModel>();
                }
                else
                {
                    var dbCartItems = await _context.ShoppingCartItems
                        .Include(c => c.Product)
                        .Where(c => c.UserId == userId)
                        .ToListAsync();

                    cartItems = dbCartItems.Select(c => new CartItemViewModel
                    {
                        ProductId = c.ProductId,
                        ProductName = c.Product.Name,
                        Price = c.Price,
                        Quantity = c.Quantity,
                        Size = c.Size,
                        Color = c.Color,
                        ImageUrl = c.Product.FeaturedImageUrl
                    }).ToList();
                }

                if (!cartItems.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Calculate totals
                var subtotal = cartItems.Sum(item => item.Price * item.Quantity);
                var shippingFee = await CalculateShippingFeeAsync(model.ShippingMethod, subtotal);
                var discountAmount = await CalculateDiscountAsync(model.CouponCode, subtotal);
                var tax = CalculateTax(subtotal);
                var total = subtotal + shippingFee + tax - discountAmount;

                // Create checkout session
                var session = new CheckoutSession
                {
                    Id = Guid.NewGuid(),
                    UserId = userId ?? string.Empty,
                    Email = model.Email ?? string.Empty,
                    Status = "active",
                    TotalAmount = total,
                    ShippingFee = shippingFee,
                    Tax = tax,
                    DiscountAmount = discountAmount,
                    CouponCode = model.CouponCode,
                    ShippingMethod = model.ShippingMethod,
                    ShippingAddress = JsonSerializer.Serialize(model.ShippingAddress),
                    BillingAddress = JsonSerializer.Serialize(model.BillingAddress),
                    Notes = model.Notes,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                _context.CheckoutSessions.Add(session);

                // Add session items
                foreach (var item in cartItems)
                {
                    var sessionItem = new CheckoutSessionItem
                    {
                        CheckoutSessionId = session.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        TotalPrice = item.Price * item.Quantity,
                        Size = item.Size,
                        Color = item.Color,
                        ProductName = item.ProductName,
                        ProductImage = item.ImageUrl
                    };

                    _context.CheckoutSessionItems.Add(sessionItem);
                }

                await _context.SaveChangesAsync();

                return Json(new 
                { 
                    success = true, 
                    sessionId = session.Id.ToString(),
                    message = "Phiên thanh toán đã được tạo thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi tạo phiên thanh toán" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Payment(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || !Guid.TryParse(sessionId, out var sessionGuid))
            {
                TempData["ErrorMessage"] = "Phiên thanh toán không hợp lệ.";
                return RedirectToAction("Index");
            }

            var session = await _context.CheckoutSessions
                .Include(s => s.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(s => s.Id == sessionGuid && s.Status == "active");

            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                TempData["ErrorMessage"] = "Phiên thanh toán đã hết hạn hoặc không tồn tại.";
                return RedirectToAction("Index");
            }

            var paymentMethods = await _paymentService.GetAvailablePaymentMethodsAsync();
            var shippingMethods = await GetAvailableShippingMethodsAsync();

            var model = new PaymentViewModel
            {
                SessionId = sessionId,
                Session = session,
                PaymentMethods = paymentMethods,
                ShippingMethods = shippingMethods,
                TotalAmount = session.TotalAmount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(ProcessPaymentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu thanh toán không hợp lệ" });
                }

                // Get checkout session
                var session = await _context.CheckoutSessions
                    .Include(s => s.Items)
                    .FirstOrDefaultAsync(s => s.Id == Guid.Parse(model.SessionId) && s.Status == "active");

                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    return Json(new { success = false, message = "Phiên thanh toán đã hết hạn" });
                }

                // Create order first
                var order = await CreateOrderFromSessionAsync(session);

                // Update session
                session.PaymentMethod = model.PaymentMethod;
                session.Status = "processing";
                await _context.SaveChangesAsync();

                // Process payment
                var paymentRequest = new PaymentRequest
                {
                    OrderId = order.Id.ToString(),
                    UserId = session.UserId ?? "",
                    Amount = session.TotalAmount,
                    Currency = "VND",
                    PaymentMethod = model.PaymentMethod,
                    OrderInfo = $"Thanh toán đơn hàng #{order.OrderNumber}",
                    ReturnUrl = Url.Action("PaymentReturn", "Checkout", null, Request.Scheme) ?? string.Empty,
                    NotifyUrl = Url.Action("PaymentNotify", "Checkout", null, Request.Scheme) ?? string.Empty,
                    IpAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                    UserAgent = Request.Headers["User-Agent"].ToString(),
                    PaymentMethodId = model.PaymentMethodId
                };

                var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);

                if (paymentResult.IsSuccess)
                {
                    // Update order payment status
                    order.PaymentStatus = "pending";
                    order.Status = "pending";
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(paymentResult.PaymentUrl))
                    {
                        // Redirect to payment gateway
                        return Json(new 
                        { 
                            success = true, 
                            redirectUrl = paymentResult.PaymentUrl,
                            message = paymentResult.Message
                        });
                    }
                    else
                    {
                        // Direct payment (like COD)
                        await CompleteOrderAsync(order, paymentResult.TransactionId);
                        return Json(new 
                        { 
                            success = true, 
                            redirectUrl = Url.Action("Success", new { orderId = order.Id }),
                            message = paymentResult.Message
                        });
                    }
                }
                else
                {
                    // Payment failed
                    order.PaymentStatus = "failed";
                    order.Status = "cancelled";
                    await _context.SaveChangesAsync();

                    return Json(new { success = false, message = paymentResult.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for session {SessionId}", model.SessionId);
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý thanh toán" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentReturn(string vnp_ResponseCode, string vnp_TxnRef, string vnp_TransactionNo)
        {
            try
            {
                // Handle VNPay return
                if (!string.IsNullOrEmpty(vnp_TxnRef))
                {
                    var order = await _context.Orders
                        .FirstOrDefaultAsync(o => o.Id.ToString() == vnp_TxnRef);

                    if (order != null)
                    {
                        if (vnp_ResponseCode == "00") // Success
                        {
                            await CompleteOrderAsync(order, vnp_TransactionNo);
                            return RedirectToAction("Success", new { orderId = order.Id });
                        }
                        else
                        {
                            // Payment failed
                            order.PaymentStatus = "failed";
                            order.Status = "cancelled";
                            await _context.SaveChangesAsync();

                            TempData["ErrorMessage"] = "Thanh toán không thành công. Vui lòng thử lại.";
                            return RedirectToAction("Failed", new { orderId = order.Id });
                        }
                    }
                }

                TempData["ErrorMessage"] = "Không tìm thấy thông tin đơn hàng.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment return");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xử lý kết quả thanh toán.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PaymentNotify()
        {
            try
            {
                // Handle payment gateway notifications (webhooks)
                // This is called by payment gateways to notify payment status
                
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                
                // Process webhook based on payment method
                // Implementation depends on specific payment gateway
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment notification");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index", "Home");
            }

            // Clear cart for authenticated users
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId) && userId == order.UserId)
            {
                var cartItems = await _context.ShoppingCartItems
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                _context.ShoppingCartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Clear session cart
                HttpContext.Session.Remove("Cart");
            }

            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> Failed(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return View(order);
        }

        // Helper methods
        private async Task<CheckoutViewModel> CreateCheckoutViewModelAsync(string? userId, List<CartItemViewModel> cartItems)
        {
            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                Subtotal = cartItems.Sum(item => item.Price * item.Quantity)
            };

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    model.Email = user.Email ?? "";
                    model.FirstName = user.FirstName ?? string.Empty;
                    model.LastName = user.LastName ?? string.Empty;
                    model.PhoneNumber = user.PhoneNumber ?? "";

                    // Get user addresses
                    model.SavedAddresses = await _context.Addresses
                        .Where(a => a.UserId == userId)
                        .ToListAsync();
                }
            }

            // Get available shipping methods
            model.ShippingMethods = await GetAvailableShippingMethodsAsync();

            return model;
        }

        private List<CartItemViewModel>? GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
                return null;

            return JsonSerializer.Deserialize<List<CartItemViewModel>>(cartJson);
        }

        private async Task<decimal> CalculateShippingFeeAsync(string? shippingMethod, decimal subtotal)
        {
            if (string.IsNullOrEmpty(shippingMethod))
                return 0;

            var method = await _context.ShippingMethods
                .FirstOrDefaultAsync(sm => sm.Code == shippingMethod && sm.IsActive);

            if (method == null)
                return 0;

            // Check minimum order amount for free shipping
            if (method.MinOrderAmount.HasValue && subtotal >= method.MinOrderAmount.Value)
                return 0;

            return method.Cost;
        }

        private async Task<decimal> CalculateDiscountAsync(string? couponCode, decimal subtotal)
        {
            if (string.IsNullOrEmpty(couponCode))
                return 0;

            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == couponCode && 
                                        p.IsActive && 
                                        (p.StartDate == null || p.StartDate <= DateTime.UtcNow) &&
                                        (p.EndDate == null || p.EndDate >= DateTime.UtcNow) &&
                                        (p.UsageLimit == null || p.UsageCount < p.UsageLimit) &&
                                        (p.MinOrderAmount == null || subtotal >= p.MinOrderAmount));

            if (promotion == null)
                return 0;

            var discount = promotion.Type switch
            {
                "percentage" => subtotal * (promotion.Value / 100),
                "fixed_amount" => promotion.Value,
                _ => 0
            };

            // Apply maximum discount limit
            if (promotion.MaxDiscountAmount.HasValue && discount > promotion.MaxDiscountAmount.Value)
                discount = promotion.MaxDiscountAmount.Value;

            return discount;
        }

        private decimal CalculateTax(decimal subtotal)
        {
            // VAT 10% for Vietnam
            return subtotal * 0.1m;
        }

        private async Task<List<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            return await _context.ShippingMethods
                .Where(sm => sm.IsActive)
                .OrderBy(sm => sm.SortOrder)
                .ToListAsync();
        }

        private async Task<Order> CreateOrderFromSessionAsync(CheckoutSession session)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = GenerateOrderNumber(),
                UserId = session.UserId ?? "",
                Status = "pending",
                TotalAmount = session.TotalAmount,
                ShippingFee = session.ShippingFee,
                Tax = session.Tax,
                DiscountAmount = session.DiscountAmount,
                CouponCode = session.CouponCode,
                PaymentMethod = session.PaymentMethod ?? "",
                PaymentStatus = "pending",
                ShippingAddress = session.ShippingAddress ?? "",
                BillingAddress = session.BillingAddress ?? "",
                Notes = session.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);

            // Add order items
            foreach (var item in session.Items)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage
                };

                _context.OrderItems.Add(orderItem);
            }

            await _context.SaveChangesAsync();
            return order;
        }

        private async Task CompleteOrderAsync(Order order, string? transactionId)
        {
            order.PaymentStatus = "paid";
            order.Status = "processing";
            
            // Add status history
            var statusHistory = new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = "processing",
                Notes = "Đơn hàng đã được thanh toán thành công",
                CreatedAt = DateTime.UtcNow
            };

            _context.OrderStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();

            // Send confirmation email
            if (!string.IsNullOrEmpty(order.UserId))
            {
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user?.Email != null)
                {
                    await _emailService.SendOrderConfirmationEmailAsync(user.Email, order);
                }

                // Send notification
                await _notificationService.SendNotificationAsync(order.UserId,
                    "Đơn hàng đã được xác nhận",
                    $"Đơn hàng #{order.OrderNumber} đã được thanh toán và xác nhận thành công",
                    "order_confirmed");
            }

            // Update inventory
            await UpdateInventoryAsync(order);
        }

        private async Task UpdateInventoryAsync(Order order)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();

            foreach (var item in orderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity <= 0)
                    {
                        product.InStock = false;
                        product.Status = "out_of_stock";
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private string GenerateOrderNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"JH{timestamp}{random}";
        }
    }
}
