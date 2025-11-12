# 🎨 Payment Page Redesign Plan

## 📋 Tổng Quan

Tài liệu này mô tả chi tiết kế hoạch cải thiện trang Payment để tập trung vào thanh toán QR code.

---

## 🎯 Mục Tiêu

1. **Đơn giản hóa UI** - Loại bỏ thông tin thừa, tập trung vào QR code
2. **Tự động hóa** - Hiển thị QR code ngay khi load trang
3. **Real-time** - Polling payment status, tự động redirect khi thành công
4. **Mobile-first** - Tối ưu cho điện thoại (scan QR dễ dàng)

---

## 🔧 Nhiệm Vụ Chi Tiết

### **Task 1: Backend - Create Payment QR API** ⭐⭐⭐

**File:** `Controllers/CheckoutController.cs`

**Endpoint mới:**
```csharp
[HttpPost]
[Route("Checkout/GeneratePaymentQR")]
public async Task<IActionResult> GeneratePaymentQR(string sessionId, string paymentMethod)
{
    // 1. Validate session
    // 2. Call VNPay/MoMo API to get QR code
    // 3. Return QR image URL or base64
    // 4. Save payment reference to database
}
```

**Endpoint check status:**
```csharp
[HttpGet]
[Route("Checkout/CheckPaymentStatus")]
public async Task<IActionResult> CheckPaymentStatus(string sessionId)
{
    // 1. Query database for payment status
    // 2. Return { status: "pending" | "paid" | "failed", redirectUrl: "..." }
}
```

**Ước tính:** 2-3 giờ

---

### **Task 2: Frontend - Redesign Payment Page Layout** ⭐⭐

**File:** `Views/Checkout/Payment.cshtml`

**Layout mới:**

```
┌──────────────────────────────────────────┐
│   [◄ Quay lại]     🔒 Thanh toán an toàn│
├──────────────────────────────────────────┤
│                                          │
│         [QR CODE LỚNNN]                  │
│         (centered, 300x300px)            │
│                                          │
│      💰 1,500,000 ₫                      │
│      Quét mã để thanh toán               │
│                                          │
│  ⏱ Mã QR có hiệu lực trong 15:00       │
│                                          │
│  📱 Hướng dẫn:                           │
│  1. Mở app VNPay/MoMo                    │
│  2. Quét mã QR                           │
│  3. Xác nhận thanh toán                  │
│                                          │
│  ℹ️ Nội dung: Thanh toan DH123456       │
│                                          │
│  [Hoặc thanh toán bằng cách khác]       │
│                                          │
└──────────────────────────────────────────┘
```

**Những thay đổi:**
- ❌ Xóa: Order summary (đã có ở checkout)
- ❌ Xóa: Payment method selection (đã chọn ở checkout)
- ❌ Xóa: Terms checkbox (đã đồng ý ở checkout)
- ✅ Giữ: Payment method info (VNPay/MoMo)
- ✅ Thêm: QR code lớn ở trung tâm
- ✅ Thêm: Countdown timer cho QR code
- ✅ Thêm: Real-time status updates

**Ước tính:** 3-4 giờ

---

### **Task 3: VNPay QR Code Integration** ⭐⭐⭐

**File:** `Services/PaymentService.cs`

**VNPay QR API:**

```csharp
public async Task<VNPayQRResponse> GenerateVNPayQR(VNPayQRRequest request)
{
    // VNPay API endpoint: https://sandbox.vnpayment.vn/qrpayauth/api/merchant/get_qrcode
    // Method: POST
    // Headers: 
    //   - Content-Type: application/json
    //   - vnp_Version: 2.1.0
    //   - vnp_Command: get_qrcode
    
    var requestData = new 
    {
        vnp_TmnCode = _config["VNPAY_TMN_CODE"],
        vnp_Amount = request.Amount * 100, // Convert to VND smallest unit
        vnp_OrderInfo = request.OrderInfo,
        vnp_OrderId = request.OrderId,
        vnp_ReturnUrl = request.ReturnUrl,
        vnp_IpAddr = request.IpAddress,
        vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"),
        vnp_QRType = "DYNAMIC" // Static QR or Dynamic QR
    };
    
    // Sign request
    var signature = GenerateVNPaySignature(requestData);
    requestData.vnp_SecureHash = signature;
    
    // Call VNPay API
    var response = await _httpClient.PostAsJsonAsync(vnpayQRUrl, requestData);
    
    // Parse response
    var result = await response.Content.ReadFromJsonAsync<VNPayQRResponse>();
    
    // Return QR code URL or base64
    return result;
}
```

**VNPay QR Response:**
```json
{
  "RspCode": "00",
  "Message": "Success",
  "QRCodeURL": "https://qr.vnpay.vn/...",
  "QRDataURL": "data:image/png;base64,iVBORw0KGgo...",
  "OrderId": "ORD123456",
  "ExpireTime": 900 // 15 minutes
}
```

**Ước tính:** 4-5 giờ (bao gồm testing)

---

### **Task 4: MoMo QR Code Integration** ⭐⭐⭐

**File:** `Services/PaymentService.cs`

**MoMo QR API:**

```csharp
public async Task<MoMoQRResponse> GenerateMoMoQR(MoMoQRRequest request)
{
    // MoMo API endpoint: https://test-payment.momo.vn/v2/gateway/api/create
    // Method: POST
    // Documentation: https://developers.momo.vn/
    
    var requestData = new 
    {
        partnerCode = _config["MOMO_PARTNER_CODE"],
        accessKey = _config["MOMO_ACCESS_KEY"],
        requestId = Guid.NewGuid().ToString(),
        amount = request.Amount.ToString(),
        orderId = request.OrderId,
        orderInfo = request.OrderInfo,
        redirectUrl = request.ReturnUrl,
        ipnUrl = request.IpnUrl,
        requestType = "captureWallet", // or "qrCode"
        extraData = "",
        lang = "vi"
    };
    
    // Generate signature
    var rawSignature = $"accessKey={requestData.accessKey}&amount={requestData.amount}&extraData={requestData.extraData}&ipnUrl={requestData.ipnUrl}&orderId={requestData.orderId}&orderInfo={requestData.orderInfo}&partnerCode={requestData.partnerCode}&redirectUrl={requestData.redirectUrl}&requestId={requestData.requestId}&requestType={requestData.requestType}";
    var signature = ComputeHmacSha256(rawSignature, _config["MOMO_SECRET_KEY"]);
    requestData.signature = signature;
    
    // Call MoMo API
    var response = await _httpClient.PostAsJsonAsync(momoApiUrl, requestData);
    var result = await response.Content.ReadFromJsonAsync<MoMoQRResponse>();
    
    return result;
}
```

**MoMo QR Response:**
```json
{
  "partnerCode": "MOMO",
  "orderId": "ORD123456",
  "requestId": "uuid-here",
  "amount": 1500000,
  "responseTime": 1699376400000,
  "message": "Successful.",
  "resultCode": 0,
  "payUrl": "https://payment.momo.vn/...",
  "qrCodeUrl": "data:image/png;base64,...",
  "deeplink": "momo://..."
}
```

**Ước tính:** 4-5 giờ

---

### **Task 5: Payment Status Polling** ⭐⭐

**File:** `wwwroot/js/payment.js`

**JavaScript polling:**

```javascript
let pollingInterval;
let pollingTimeout;
let countdown = 900; // 15 minutes

function startPaymentPolling(sessionId) {
    // Update countdown timer
    updateCountdown();
    
    // Poll every 3 seconds
    pollingInterval = setInterval(async () => {
        try {
            const response = await fetch(`/Checkout/CheckPaymentStatus?sessionId=${sessionId}`);
            const data = await response.json();
            
            if (data.status === 'paid') {
                // Payment successful!
                stopPolling();
                showPaymentSuccess(data);
                
                // Redirect after 2 seconds
                setTimeout(() => {
                    window.location.href = data.redirectUrl;
                }, 2000);
            } else if (data.status === 'failed') {
                // Payment failed
                stopPolling();
                showPaymentError(data.message);
            }
            
            // Update UI with status
            updatePaymentStatus(data);
            
        } catch (error) {
            console.error('Polling error:', error);
        }
    }, 3000); // Poll every 3 seconds
    
    // Stop polling after 15 minutes
    pollingTimeout = setTimeout(() => {
        stopPolling();
        showPaymentTimeout();
    }, 900000); // 15 minutes
}

function updateCountdown() {
    setInterval(() => {
        countdown--;
        
        if (countdown <= 0) {
            stopPolling();
            showPaymentTimeout();
            return;
        }
        
        const minutes = Math.floor(countdown / 60);
        const seconds = countdown % 60;
        $('#countdown').text(`${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`);
        
        // Warning when < 2 minutes
        if (countdown < 120) {
            $('#countdown').addClass('text-danger');
        }
    }, 1000);
}

function stopPolling() {
    if (pollingInterval) clearInterval(pollingInterval);
    if (pollingTimeout) clearTimeout(pollingTimeout);
}

function showPaymentSuccess(data) {
    // Show success animation
    $('#qrCodeSection').html(`
        <div class="payment-success-animation">
            <i class="fas fa-check-circle fa-5x text-success mb-3"></i>
            <h3 class="text-success">Thanh toán thành công!</h3>
            <p class="text-muted">Đang chuyển hướng...</p>
        </div>
    `);
}
```

**Ước tính:** 2-3 giờ

---

### **Task 6: Responsive Optimization** ⭐

**File:** `wwwroot/css/checkout.css`

**CSS changes:**

```css
/* Payment QR Section */
.payment-qr-section {
    max-width: 600px;
    margin: 0 auto;
    padding: 2rem;
    text-align: center;
}

.qr-code-container {
    background: white;
    border-radius: 12px;
    padding: 2rem;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    margin-bottom: 2rem;
}

.qr-code-image {
    width: 300px;
    height: 300px;
    margin: 0 auto;
}

/* Mobile: Full width, larger QR */
@media (max-width: 768px) {
    .qr-code-image {
        width: 280px;
        height: 280px;
    }
    
    .payment-qr-section {
        padding: 1rem;
    }
}

/* Countdown Timer */
.countdown-timer {
    font-size: 2rem;
    font-weight: bold;
    color: #28a745;
    margin: 1rem 0;
}

.countdown-timer.warning {
    color: #ffc107;
    animation: pulse 1s infinite;
}

.countdown-timer.danger {
    color: #dc3545;
    animation: pulse 0.5s infinite;
}

@keyframes pulse {
    0%, 100% { opacity: 1; }
    50% { opacity: 0.5; }
}

/* Payment Success Animation */
.payment-success-animation {
    animation: scaleIn 0.5s ease-out;
}

@keyframes scaleIn {
    0% { transform: scale(0); }
    100% { transform: scale(1); }
}
```

**Ước tính:** 2 giờ

---

### **Task 7: Complete Testing** ⭐⭐⭐

**Test cases:**

1. **VNPay QR Flow:**
   - [ ] Generate QR code successfully
   - [ ] QR code hiển thị đúng (scan được)
   - [ ] Countdown timer hoạt động
   - [ ] Polling detect payment success
   - [ ] Auto redirect sau khi thanh toán
   - [ ] Timeout after 15 minutes

2. **MoMo QR Flow:**
   - [ ] Generate QR code successfully
   - [ ] QR code và deep link hoạt động
   - [ ] Polling detect payment
   - [ ] Deep link mở app MoMo

3. **Responsive:**
   - [ ] Desktop: QR code 300x300px
   - [ ] Tablet: QR code 280x280px
   - [ ] Mobile: Full width, easy scan

4. **Error Handling:**
   - [ ] Network error → Retry
   - [ ] QR generation failed → Show error
   - [ ] Payment failed → Clear message
   - [ ] Timeout → Allow retry

**Ước tính:** 3-4 giờ

---

## 📊 Timeline

| Task | Thời gian | Priority |
|------|-----------|----------|
| 1. Backend API | 2-3h | ⭐⭐⭐ Critical |
| 2. Redesign UI | 3-4h | ⭐⭐ High |
| 3. VNPay QR | 4-5h | ⭐⭐⭐ Critical |
| 4. MoMo QR | 4-5h | ⭐⭐⭐ Critical |
| 5. Polling | 2-3h | ⭐⭐ High |
| 6. Responsive | 2h | ⭐ Medium |
| 7. Testing | 3-4h | ⭐⭐⭐ Critical |
| **Total** | **20-26h** | |

---

## 🎨 Wireframe

### Desktop View:
```
┌─────────────────────────────────────────────────────────┐
│  [◄ Quay lại]                    🔒 Thanh toán an toàn │
├─────────────────────────────────────────────────────────┤
│                                                         │
│                    ┌───────────────┐                    │
│                    │               │                    │
│                    │   [QR CODE]   │                    │
│                    │    300x300    │                    │
│                    │               │                    │
│                    └───────────────┘                    │
│                                                         │
│                   💰 1,500,000 ₫                        │
│             Quét mã QR để thanh toán                    │
│                                                         │
│           ⏱ Mã có hiệu lực: 14:35                      │
│                                                         │
│  ──────────────────────────────────────────────────    │
│                                                         │
│  📱 Hướng dẫn thanh toán:                              │
│  1. Mở ứng dụng VNPay trên điện thoại                  │
│  2. Chọn "Quét mã QR"                                   │
│  3. Quét mã QR ở trên                                   │
│  4. Xác nhận thanh toán                                 │
│                                                         │
│  ℹ️ Nội dung: Thanh toan don hang ORD123456           │
│                                                         │
│  ────────── hoặc ──────────                            │
│                                                         │
│  [📋 Thanh toán bằng cách khác]                        │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Mobile View:
```
┌────────────────────────┐
│  [◄]   🔒 An toàn     │
├────────────────────────┤
│                        │
│   ┌──────────────┐     │
│   │              │     │
│   │  [QR CODE]   │     │
│   │   280x280    │     │
│   │              │     │
│   └──────────────┘     │
│                        │
│   💰 1,500,000 ₫       │
│   Quét mã thanh toán   │
│                        │
│   ⏱ 14:35              │
│                        │
│   📱 Hướng dẫn:        │
│   1. Mở app VNPay      │
│   2. Quét QR           │
│   3. Xác nhận          │
│                        │
│   [Cách khác]          │
│                        │
└────────────────────────┘
```

---

## 🔐 Security Considerations

1. **QR Code Expiry:** 15 phút
2. **One-time use:** Mỗi QR chỉ dùng 1 lần
3. **Signature validation:** Verify tất cả callbacks
4. **HTTPS only:** Bắt buộc SSL
5. **Rate limiting:** Max 3 QR generation / phút

---

## 📝 Database Changes

**Table: PaymentTransactions**
```sql
ALTER TABLE "PaymentTransactions" 
ADD COLUMN qr_code_url VARCHAR(500),
ADD COLUMN qr_expires_at TIMESTAMP,
ADD COLUMN payment_method_detail JSONB;
```

**Example JSONB:**
```json
{
  "qrCodeUrl": "https://...",
  "qrDataUrl": "data:image/png;base64,...",
  "deepLink": "momo://...",
  "expiresAt": "2024-11-07T10:45:00Z"
}
```

---

## ✅ Definition of Done

- [x] Backend API hoạt động
- [ ] VNPay QR generation successful
- [ ] MoMo QR generation successful  
- [ ] UI hiển thị QR đẹp, responsive
- [ ] Polling hoạt động, tự động redirect
- [ ] Error handling đầy đủ
- [ ] Testing pass 100%
- [ ] Documentation updated
- [ ] Code review completed

---

**Created:** 2024-11-07  
**Status:** 🚧 Planning Complete  
**Next Step:** Start Task 1 - Backend API
