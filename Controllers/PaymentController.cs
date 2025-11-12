using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using System.Security.Claims;
using System.Text.Json;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PaymentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Payment/Checkout
        public async Task<IActionResult> Checkout(string? couponCode = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _context.ShoppingCartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            // Calculate totals
            var subtotal = cartItems.Sum(c => c.Price * c.Quantity);
            var discountAmount = 0m;
            Coupon? appliedCoupon = null;

            // Apply coupon if provided
            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                appliedCoupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Code.ToUpper() == couponCode.ToUpper() && 
                                             c.IsActive &&
                                             (c.StartDate == null || c.StartDate <= DateTime.UtcNow) &&
                                             (c.EndDate == null || c.EndDate >= DateTime.UtcNow));

                if (appliedCoupon != null)
                {
                    // Check usage limits
                    if (appliedCoupon.UsageLimit.HasValue && appliedCoupon.UsageCount >= appliedCoupon.UsageLimit.Value)
                    {
                        TempData["ErrorMessage"] = "Mã giảm giá đã được sử dụng hết";
                    }
                    else if (appliedCoupon.MinOrderAmount.HasValue && subtotal < appliedCoupon.MinOrderAmount.Value)
                    {
                        TempData["ErrorMessage"] = $"Đơn hàng tối thiểu {appliedCoupon.MinOrderAmount.Value:C} để sử dụng mã này";
                    }
                    else
                    {
                        // Calculate discount
                        if (appliedCoupon.Type == "percentage")
                        {
                            discountAmount = subtotal * (appliedCoupon.Value / 100);
                        }
                        else
                        {
                            discountAmount = appliedCoupon.Value;
                        }

                        // Ensure discount doesn't exceed subtotal
                        if (discountAmount > subtotal)
                        {
                            discountAmount = subtotal;
                        }

                        TempData["SuccessMessage"] = $"Áp dụng mã giảm giá {appliedCoupon.Code} thành công!";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn";
                }
            }

            // Calculate fees and total with discount
            var shippingFee = CalculateShippingFee(subtotal);
            var tax = CalculateTax(subtotal);
            var total = subtotal + shippingFee + tax - discountAmount;

            ViewBag.CartItems = cartItems;
            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Tax = tax;
            ViewBag.Total = total;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.CouponCode = appliedCoupon?.Code;
            ViewBag.CouponDescription = appliedCoupon?.Description;

            // Get user addresses
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();

            ViewBag.Addresses = addresses;

            return View();
        }

        // POST: Payment/ProcessPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(string paymentMethod, Guid? addressId, string? notes, string? couponCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                var cartItems = await _context.ShoppingCartItems
                    .Include(c => c.Product)
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng trống" });
                }

                // Validate stock availability
                foreach (var item in cartItems)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        return Json(new { 
                            success = false, 
                            message = $"Sản phẩm {item.Product.Name} chỉ còn {item.Product.StockQuantity} trong kho" 
                        });
                    }
                }

                // Get shipping address
                var shippingAddress = "";
                if (addressId.HasValue)
                {
                    var address = await _context.Addresses
                        .FirstOrDefaultAsync(a => a.Id == addressId.Value && a.UserId == userId);
                    if (address != null)
                    {
                        shippingAddress = $"{address.FirstName} {address.LastName}, {address.Address1}, {address.City}, {address.State}";
                    }
                }

                // Calculate amounts with coupon
                var subtotal = cartItems.Sum(c => c.Price * c.Quantity);
                var shippingFee = CalculateShippingFee(subtotal);
                var tax = CalculateTax(subtotal);
                
                // Apply coupon discount
                var discountAmount = 0m;
                Coupon? appliedCoupon = null;
                
                if (!string.IsNullOrEmpty(couponCode))
                {
                    appliedCoupon = await _context.Coupons
                        .FirstOrDefaultAsync(c => c.Code == couponCode && 
                                           c.IsActive && 
                                           (!c.EndDate.HasValue || c.EndDate > DateTime.UtcNow) &&
                                           (!c.StartDate.HasValue || c.StartDate <= DateTime.UtcNow));
                    
                    if (appliedCoupon != null)
                    {
                        if (appliedCoupon.Type == "percentage")
                        {
                            discountAmount = subtotal * (appliedCoupon.Value / 100);
                        }
                        else
                        {
                            discountAmount = appliedCoupon.Value;
                        }
                        
                        // Check minimum order amount
                        if (appliedCoupon.MinOrderAmount.HasValue && subtotal < appliedCoupon.MinOrderAmount.Value)
                        {
                            return Json(new { 
                                success = false, 
                                message = $"Đơn hàng tối thiểu {appliedCoupon.MinOrderAmount:C} để sử dụng mã giảm giá này" 
                            });
                        }
                    }
                }
                
                var total = subtotal + shippingFee + tax - discountAmount;

                // Create order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    OrderNumber = GenerateOrderNumber(),
                    UserId = userId,
                    Status = "pending",
                    TotalAmount = total,
                    ShippingFee = shippingFee,
                    Tax = tax,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "pending",
                    Notes = notes,
                    ShippingAddress = shippingAddress,
                    BillingAddress = shippingAddress,
                    CouponCode = appliedCoupon?.Code,
                    DiscountAmount = discountAmount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);

                // Create order items
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price,
                        TotalPrice = cartItem.Price * cartItem.Quantity,
                        ProductName = cartItem.Product.Name,
                        ProductSKU = cartItem.Product.SKU,
                        ProductImage = cartItem.Product.FeaturedImageUrl
                    };

                    _context.OrderItems.Add(orderItem);

                    // Update product stock
                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    if (cartItem.Product.StockQuantity <= 0)
                    {
                        cartItem.Product.InStock = false;
                        cartItem.Product.Status = "out_of_stock";
                    }
                }

                // Process payment based on method
                var paymentResult = await ProcessPaymentMethod(order, paymentMethod);
                
                if (paymentResult.Success)
                {
                    // Update coupon usage count
                    if (appliedCoupon != null)
                    {
                        appliedCoupon.UsageCount++;
                        _context.Coupons.Update(appliedCoupon);
                    }
                    
                    // Clear cart
                    _context.ShoppingCartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();

                    return Json(new { 
                        success = true, 
                        message = "Đặt hàng thành công!", 
                        orderId = order.Id,
                        redirectUrl = Url.Action("OrderConfirmation", new { id = order.Id })
                    });
                }
                else
                {
                    // Remove the order if payment failed
                    _context.Orders.Remove(order);
                    return Json(new { success = false, message = paymentResult.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for user {UserId}", userId);
                return Json(new { success = false, message = "Có lỗi xảy ra khi xử lý thanh toán" });
            }
        }

        // GET: Payment/OrderConfirmation/5
        public async Task<IActionResult> OrderConfirmation(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Payment/VNPay/Return
        public async Task<IActionResult> VNPayReturn()
        {
            // Handle VNPay return
            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
            var vnp_TxnRef = Request.Query["vnp_TxnRef"];
            var vnp_Amount = Request.Query["vnp_Amount"];
            var vnp_OrderInfo = Request.Query["vnp_OrderInfo"];

            if (vnp_ResponseCode == "00")
            {
                // Payment successful
                var orderId = Guid.Parse(vnp_TxnRef.ToString());
                var order = await _context.Orders.FindAsync(orderId);
                
                if (order != null)
                {
                    order.PaymentStatus = "paid";
                    order.Status = "processing";
                    order.UpdatedAt = DateTime.UtcNow;

                    // Create payment record
                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderId,
                        PaymentMethod = "vnpay",
                        Status = "completed",
                        Amount = order.TotalAmount,
                        TransactionId = Request.Query["vnp_TransactionNo"],
                        GatewayResponse = JsonSerializer.Serialize(Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString())),
                        ProcessedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("OrderConfirmation", new { id = orderId });
                }
            }

            TempData["Error"] = "Thanh toán không thành công. Vui lòng thử lại.";
            return RedirectToAction("Checkout");
        }

        // GET: Payment/MoMo/Return
        public async Task<IActionResult> MoMoReturn()
        {
            // Handle MoMo return
            var resultCode = Request.Query["resultCode"];
            var orderId = Request.Query["orderId"];
            var transId = Request.Query["transId"];

            if (resultCode == "0")
            {
                // Payment successful
                var orderGuid = Guid.Parse(orderId.ToString());
                var order = await _context.Orders.FindAsync(orderGuid);
                
                if (order != null)
                {
                    order.PaymentStatus = "paid";
                    order.Status = "processing";
                    order.UpdatedAt = DateTime.UtcNow;

                    // Create payment record
                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        OrderId = orderGuid,
                        PaymentMethod = "momo",
                        Status = "completed",
                        Amount = order.TotalAmount,
                        TransactionId = transId,
                        GatewayResponse = JsonSerializer.Serialize(Request.Query.ToDictionary(kv => kv.Key, kv => kv.Value.ToString())),
                        ProcessedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("OrderConfirmation", new { id = orderGuid });
                }
            }

            TempData["Error"] = "Thanh toán không thành công. Vui lòng thử lại.";
            return RedirectToAction("Checkout");
        }

        private decimal CalculateShippingFee(decimal subtotal)
        {
            // Tự động miễn phí vận chuyển cho đơn hàng >= 500,000đ
            if (subtotal >= 500000)
                return 0;
            
            // Standard shipping fee
            return 30000;
        }

        private decimal CalculateTax(decimal subtotal)
        {
            // 10% VAT
            return subtotal * 0.1m;
        }

        private string GenerateOrderNumber()
        {
            return $"JH{DateTime.Now:yyyyMMdd}{new Random().Next(1000, 9999)}";
        }

        private async Task<PaymentResult> ProcessPaymentMethod(Order order, string paymentMethod)
        {
            switch (paymentMethod.ToLower())
            {
                case "cod":
                    return await ProcessCODPayment(order);
                case "bank_transfer":
                    return await ProcessBankTransferPayment(order);
                case "vnpay":
                    return await ProcessVNPayPayment(order);
                case "momo":
                    return await ProcessMoMoPayment(order);
                default:
                    return new PaymentResult { Success = false, Message = "Phương thức thanh toán không hợp lệ" };
            }
        }

        private Task<PaymentResult> ProcessCODPayment(Order order)
        {
            // COD payment is always successful initially
            order.PaymentStatus = "pending";
            order.Status = "processing";
            
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PaymentMethod = "cod",
                Status = "pending",
                Amount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            return Task.FromResult(new PaymentResult { Success = true, Message = "Đặt hàng thành công!" });
        }

        private Task<PaymentResult> ProcessBankTransferPayment(Order order)
        {
            // Bank transfer requires manual verification
            order.PaymentStatus = "pending";
            order.Status = "pending";
            
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                PaymentMethod = "bank_transfer",
                Status = "pending",
                Amount = order.TotalAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            return Task.FromResult(new PaymentResult { Success = true, Message = "Đặt hàng thành công! Vui lòng chuyển khoản theo thông tin được cung cấp." });
        }

        private Task<PaymentResult> ProcessVNPayPayment(Order order)
        {
            // For demo purposes, simulate VNPay integration
            // In real implementation, you would redirect to VNPay gateway
            order.PaymentStatus = "pending";
            order.Status = "pending";
            
            return Task.FromResult(new PaymentResult { 
                Success = true, 
                Message = "Chuyển hướng đến VNPay...",
                RedirectUrl = $"/Payment/VNPay/Return?vnp_ResponseCode=00&vnp_TxnRef={order.Id}"
            });
        }

        private Task<PaymentResult> ProcessMoMoPayment(Order order)
        {
            // For demo purposes, simulate MoMo integration
            // In real implementation, you would integrate with MoMo API
            order.PaymentStatus = "pending";
            order.Status = "pending";
            
            return Task.FromResult(new PaymentResult { 
                Success = true, 
                Message = "Chuyển hướng đến MoMo...",
                RedirectUrl = $"/Payment/MoMo/Return?resultCode=0&orderId={order.Id}&transId=123456"
            });
        }

        // POST: Payment/SaveAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddress(
            string firstName, 
            string lastName, 
            string phone, 
            string address1, 
            string? address2,
            string city, 
            string state, 
            string postalCode,
            bool isDefault = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    return Json(new { success = false, message = "Vui lòng nhập họ tên" });
                }

                if (string.IsNullOrWhiteSpace(phone))
                {
                    return Json(new { success = false, message = "Vui lòng nhập số điện thoại" });
                }

                if (string.IsNullOrWhiteSpace(address1) || string.IsNullOrWhiteSpace(city))
                {
                    return Json(new { success = false, message = "Vui lòng nhập địa chỉ đầy đủ" });
                }

                // If setting as default, unset other default addresses
                if (isDefault)
                {
                    var existingDefaults = await _context.Addresses
                        .Where(a => a.UserId == userId && a.IsDefault)
                        .ToListAsync();
                    
                    foreach (var addr in existingDefaults)
                    {
                        addr.IsDefault = false;
                    }
                }

                // Create new address
                var newAddress = new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = "shipping",
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    Phone = phone.Trim(),
                    Address1 = address1.Trim(),
                    Address2 = address2?.Trim(),
                    City = city.Trim(),
                    State = state?.Trim() ?? "",
                    PostalCode = postalCode?.Trim() ?? "",
                    Country = "Vietnam",
                    IsDefault = isDefault,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Addresses.Add(newAddress);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"New address saved for user {userId}: {newAddress.Id}");

                return Json(new { 
                    success = true, 
                    message = "Đã lưu địa chỉ thành công",
                    addressId = newAddress.Id,
                    addressHtml = RenderAddressHtml(newAddress)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving address");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu địa chỉ" });
            }
        }

        private string RenderAddressHtml(Address address)
        {
            var isDefaultBadge = address.IsDefault ? "<span class=\"badge bg-primary ms-2\">Mặc định</span>" : "";
            var address2Line = !string.IsNullOrEmpty(address.Address2) ? $"<br><span class=\"text-muted\">{address.Address2}</span>" : "";
            var phoneLine = !string.IsNullOrEmpty(address.Phone) ? $"<br><span class=\"text-muted\">SĐT: {address.Phone}</span>" : "";

            return $@"
<div class=""form-check border rounded p-3 mb-3"">
    <input class=""form-check-input"" type=""radio"" name=""addressId"" value=""{address.Id}"" 
           id=""address_{address.Id}"" checked>
    <label class=""form-check-label w-100"" for=""address_{address.Id}"">
        <div class=""d-flex justify-content-between"">
            <div>
                <strong>{address.FirstName} {address.LastName}</strong>
                {isDefaultBadge}
                <br>
                <span class=""text-muted"">{address.Address1}</span>
                {address2Line}
                <br>
                <span class=""text-muted"">{address.City}, {address.State} {address.PostalCode}</span>
                {phoneLine}
            </div>
        </div>
    </label>
</div>";
        }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; }
    }
}
