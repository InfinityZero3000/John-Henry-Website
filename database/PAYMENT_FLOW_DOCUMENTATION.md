# PAYMENT FLOW DOCUMENTATION
## Hướng dẫn luồng xử lý thanh toán - John Henry Fashion

---

## 📋 Tổng quan

Hệ thống thanh toán của John Henry Fashion hỗ trợ nhiều phương thức thanh toán khác nhau, từ COD, ví điện tử đến chuyển khoản ngân hàng.

---

## 🔄 Luồng xử lý chính

### 1. **Khách hàng chọn sản phẩm và thanh toán**

```
Customer → Add to Cart → Checkout → Select Payment Method → Process Payment
```

### 2. **Controller Flow**

#### **CheckoutController.cs**
- `GET /checkout` - Hiển thị trang thanh toán
- Lấy danh sách Payment Methods từ DB
- Lấy danh sách Shipping Methods từ DB
- Hiển thị form cho khách hàng

#### **PaymentController.cs**
- `POST /payment/process` - Xử lý thanh toán
  - **Input**: 
    - `paymentMethod` (cod, vnpay, momo, bank_transfer, etc.)
    - `addressId` (Guid)
    - `notes` (string)
    - `couponCode` (string)
  
  - **Process**:
    1. Validate giỏ hàng
    2. Kiểm tra coupon (nếu có)
    3. Tính tổng tiền (bao gồm phí vận chuyển, giảm giá)
    4. Tạo Order mới
    5. Gọi `ProcessPaymentMethod(order, paymentMethod)`
    6. Lưu PaymentTransaction vào DB
    7. Clear giỏ hàng
    8. Redirect hoặc trả về kết quả

---

## 💳 Các phương thức thanh toán

### **1. COD (Cash on Delivery)**
```csharp
Code: "cod"
RequiresRedirect: false
Flow:
  → Tạo Order với status "pending"
  → PaymentTransaction status "pending"
  → Shipper giao hàng → Thu tiền → Update status "completed"
```

### **2. VNPay**
```csharp
Code: "vnpay"
RequiresRedirect: true
Flow:
  → Tạo Order với status "pending_payment"
  → Tạo VNPay payment URL
  → Redirect khách hàng đến VNPay
  → Khách thanh toán
  → VNPay callback /payment/vnpay-return
  → Verify signature
  → Update Order status "confirmed"
  → Update PaymentTransaction status "completed"
```

### **3. MoMo**
```csharp
Code: "momo"
RequiresRedirect: true
Flow:
  → Tạo Order với status "pending_payment"
  → Gọi MoMo API để tạo payment request
  → Redirect đến MoMo App/Web
  → MoMo IPN callback /payment/momo-notify
  → Verify signature
  → Update Order + PaymentTransaction
```

### **4. Bank Transfer**
```csharp
Code: "bank_transfer"
RequiresRedirect: false
Flow:
  → Tạo Order với status "pending_payment"
  → Hiển thị thông tin tài khoản ngân hàng
  → Khách chuyển khoản
  → Admin verify manually
  → Update Order status "confirmed"
```

---

## 🗄️ Database Schema

### **PaymentMethods Table**
```sql
CREATE TABLE "PaymentMethods" (
    "Id" INTEGER PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Code" VARCHAR(50) NOT NULL UNIQUE,
    "Description" TEXT,
    "IconUrl" VARCHAR(255),
    "IsActive" BOOLEAN DEFAULT true,
    "RequiresRedirect" BOOLEAN DEFAULT false,
    "MinAmount" DECIMAL(18,2),
    "MaxAmount" DECIMAL(18,2),
    "SupportedCurrencies" VARCHAR(100),
    "SortOrder" INTEGER DEFAULT 0,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);
```

### **ShippingMethods Table**
```sql
CREATE TABLE "ShippingMethods" (
    "Id" INTEGER PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "Code" VARCHAR(50) NOT NULL UNIQUE,
    "Description" TEXT,
    "Cost" DECIMAL(18,2) NOT NULL,
    "EstimatedDays" INTEGER NOT NULL,
    "IsActive" BOOLEAN DEFAULT true,
    "MinOrderAmount" DECIMAL(18,2),
    "MaxWeight" DECIMAL(10,2),
    "AvailableRegions" VARCHAR(255),
    "SortOrder" INTEGER DEFAULT 0,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP DEFAULT NOW()
);
```

### **PaymentTransactions Table**
```sql
CREATE TABLE "PaymentTransactions" (
    "Id" UUID PRIMARY KEY,
    "OrderId" UUID NOT NULL,
    "UserId" VARCHAR(450) NOT NULL,
    "SellerId" VARCHAR(450),
    "Amount" DECIMAL(18,2) NOT NULL,
    "PlatformFee" DECIMAL(18,2) DEFAULT 0,
    "SellerAmount" DECIMAL(18,2) NOT NULL,
    "PaymentMethod" VARCHAR(50) NOT NULL,
    "Status" VARCHAR(50) DEFAULT 'pending',
    "TransactionReference" VARCHAR(255),
    "PaymentGateway" VARCHAR(100),
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP DEFAULT NOW(),
    "CompletedAt" TIMESTAMP,
    "RefundedAt" TIMESTAMP,
    FOREIGN KEY ("OrderId") REFERENCES "Orders"("Id"),
    FOREIGN KEY ("UserId") REFERENCES "AspNetUsers"("Id"),
    FOREIGN KEY ("SellerId") REFERENCES "AspNetUsers"("Id")
);
```

---

## 📊 Payment Status Flow

```
pending → processing → completed
   ↓           ↓
failed      cancelled
   ↓
refunded
```

### **Status Meanings**:
- **pending**: Đơn hàng đã tạo, chờ thanh toán
- **processing**: Đang xử lý thanh toán (redirect to gateway)
- **completed**: Thanh toán thành công
- **failed**: Thanh toán thất bại
- **cancelled**: Khách hàng hủy
- **refunded**: Đã hoàn tiền

---

## 🔧 Code Examples

### **1. Get Active Payment Methods**
```csharp
// CheckoutController.cs
var paymentMethods = await _context.PaymentMethods
    .Where(pm => pm.IsActive)
    .OrderBy(pm => pm.SortOrder)
    .ToListAsync();

ViewBag.PaymentMethods = paymentMethods;
```

### **2. Get Active Shipping Methods**
```csharp
// CheckoutController.cs
var shippingMethods = await _context.ShippingMethods
    .Where(sm => sm.IsActive)
    .OrderBy(sm => sm.SortOrder)
    .ToListAsync();

ViewBag.ShippingMethods = shippingMethods;
```

### **3. Calculate Shipping Fee**
```csharp
// PaymentController.cs
private async Task<decimal> CalculateShippingFee(string shippingMethodCode, decimal orderAmount)
{
    var method = await _context.ShippingMethods
        .FirstOrDefaultAsync(sm => sm.Code == shippingMethodCode && sm.IsActive);
    
    if (method == null) return 0;
    
    // Free shipping nếu đơn hàng đạt ngưỡng
    if (method.MinOrderAmount.HasValue && orderAmount >= method.MinOrderAmount.Value)
    {
        return 0;
    }
    
    return method.Cost;
}
```

### **4. Process Payment Method**
```csharp
// PaymentController.cs
private async Task<PaymentResult> ProcessPaymentMethod(Order order, string paymentMethod)
{
    switch (paymentMethod.ToLower())
    {
        case "cod":
            return await ProcessCOD(order);
        
        case "vnpay":
            return await ProcessVNPay(order);
        
        case "momo":
            return await ProcessMoMo(order);
        
        case "bank_transfer":
            return await ProcessBankTransfer(order);
        
        default:
            throw new NotSupportedException($"Payment method {paymentMethod} is not supported");
    }
}
```

---

## 🛠️ Admin Management

### **Payment Methods Management**
```
/admin/payment-methods
  → List all payment methods
  → Enable/Disable payment method
  → Edit configuration (fees, limits)
  → Add new payment method
```

### **Shipping Methods Management**
```
/admin/shipping-methods
  → List all shipping methods
  → Enable/Disable shipping method
  → Edit cost and estimated days
  → Configure regional availability
```

### **Transaction Monitoring**
```
/admin/payments/transactions
  → View all payment transactions
  → Filter by status, method, date
  → Export reports
  → Refund management
```

---

## 📦 Sample Data Included

### **Payment Methods** (8 methods)
1. COD - Thanh toán khi nhận hàng ✅
2. VNPay - Cổng thanh toán VNPay ✅
3. MoMo - Ví điện tử MoMo ✅
4. ZaloPay - Ví điện tử ZaloPay ✅
5. Bank Transfer - Chuyển khoản ngân hàng ✅
6. Stripe - Thẻ tín dụng/ghi nợ (disabled)
7. ShopeePay - Ví ShopeePay (disabled)
8. ViettelPay - Ví ViettelPay (disabled)

### **Shipping Methods** (7 methods)
1. Standard - Giao hàng tiêu chuẩn (30k, 3-5 ngày) ✅
2. Express - Giao hàng nhanh (50k, 1-2 ngày) ✅
3. Same Day - Giao siêu tốc (80k, trong ngày) ✅
4. Free Shipping - Miễn phí (đơn từ 1 triệu) ✅
5. Economy - Giao tiết kiệm (20k, 5-7 ngày) ✅
6. Remote - Giao tỉnh xa (60k, 7-10 ngày) ✅
7. Store Pickup - Nhận tại cửa hàng (free) ✅

---

## 🚀 How to Import Sample Data

```bash
# PostgreSQL
psql -U johnhenry_user -d johnhenry_db -f database/insert_payment_shipping_methods.sql

# Or using PGPASSWORD
PGPASSWORD='johnhenry_pass' psql -h localhost -p 5432 -U johnhenry_user -d johnhenry_db -f database/insert_payment_shipping_methods.sql
```

---

## ✅ Testing Checklist

- [ ] COD payment flow works
- [ ] VNPay redirect and callback works
- [ ] MoMo integration works
- [ ] Bank transfer info displayed correctly
- [ ] Free shipping calculation correct
- [ ] Payment method selection validation
- [ ] Transaction records saved correctly
- [ ] Order status updates properly
- [ ] Email notifications sent
- [ ] Admin can view all transactions

---

## 🔐 Security Notes

1. **Never expose API keys in frontend**
2. **Always verify payment gateway signatures**
3. **Use HTTPS for all payment callbacks**
4. **Log all payment attempts**
5. **Implement rate limiting for payment APIs**
6. **Validate amount on server-side (never trust client)**
7. **Use transaction IDs to prevent duplicate payments**

---

## 📞 Support

For payment integration issues, contact:
- **Email**: tech@johnhenryfashion.com
- **Hotline**: 1900-xxxx
- **Documentation**: /docs/payment-integration

---

**Last Updated**: November 6, 2025
**Version**: 1.0.0
