# 🚀 Hướng Dẫn Cấu Hình Payment Gateways

## 📋 Tổng Quan

Document này hướng dẫn chi tiết cách cấu hình các cổng thanh toán từ **Test/Sandbox** sang **Production**.

---

## 🔵 1. VNPay - Cấu Hình Test (HIỆN TẠI)

### ✅ Credentials Test Hiện Tại
```bash
# File .env
VNPAY_TMN_CODE=VNPAY01
VNPAY_HASH_SECRET=VNPAYSECRETKEY123456
VNPAY_PAYMENT_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
VNPAY_SANDBOX=true
```

### ⚠️ **LƯU Ý QUAN TRỌNG**
- Credentials trên là **DEMO** từ docs VNPay, **KHÔNG** thực sự hoạt động
- Chỉ dùng để test UI/UX flow
- **KHÔNG** thể tạo giao dịch thật

### 🧪 Test VNPay Sandbox (Nếu có tài khoản test)

#### Bước 1: Đăng ký tài khoản test
1. Truy cập: https://sandbox.vnpayment.vn/
2. Đăng ký tài khoản demo (miễn phí)
3. Nhận credentials test:
   - TMN Code: `DEMO_xxx` 
   - Hash Secret: `xxxxx`

#### Bước 2: Cập nhật .env
```bash
# Thay bằng credentials test từ sandbox
VNPAY_TMN_CODE=YOUR_TEST_TMN_CODE
VNPAY_HASH_SECRET=YOUR_TEST_HASH_SECRET
VNPAY_PAYMENT_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
VNPAY_SANDBOX=true
```

#### Bước 3: Test với thẻ test
```
Thẻ test VNPay:
- Số thẻ: 9704 0000 0000 0018
- Tên: NGUYEN VAN A
- Ngày hết hạn: 03/07
- OTP: 123456
```

### 🏭 Chuyển sang Production VNPay

#### Bước 1: Đăng ký Merchant VNPay
```bash
1. Truy cập: https://vnpay.vn/dang-ky-merchant
2. Chuẩn bị hồ sơ:
   ✓ Giấy phép kinh doanh (GPKD)
   ✓ Giấy tờ pháp nhân
   ✓ Thông tin website/app
   ✓ Thông tin sản phẩm/dịch vụ
   
3. Điền form đăng ký online

4. Chờ VNPay liên hệ (2-3 ngày làm việc)

5. Ký hợp đồng điện tử

6. Chờ duyệt (7-14 ngày)
```

#### Bước 2: Nhận Production Credentials
```
VNPay sẽ gửi email chứa:
- TMN Code: VNPXXXXXX
- Hash Secret: 32-64 ký tự ngẫu nhiên
- Tài liệu API Production
```

#### Bước 3: Cập nhật Production Config

**File .env:**
```bash
# Production VNPay
VNPAY_TMN_CODE=VNPXXXXXX              # TMN Code thật từ VNPay
VNPAY_HASH_SECRET=your_real_secret    # Hash Secret thật
VNPAY_PAYMENT_URL=https://vnpayment.vn/paymentv2/vpcpay.html  # ⚠️ HTTPS thật
VNPAY_API_URL=https://vnpayment.vn/merchant_webapi/api/transaction
VNPAY_SANDBOX=false                    # ⚠️ Tắt sandbox mode
VNPAY_ENABLED=true
```

#### Bước 4: Cập nhật Return URL
```bash
# File appsettings.json hoặc .env
VNPAY_RETURN_URL=https://your-domain.com/Payment/VNPayReturn
VNPAY_IPN_URL=https://your-domain.com/Payment/VNPayIPN
```

#### Bước 5: Test Production
```bash
1. Dùng thẻ thật để test
2. Kiểm tra webhook/IPN có hoạt động
3. Verify signature đúng
4. Test refund/query transaction
```

---

## 🟣 2. MoMo - Cấu Hình Production

### ✅ Credentials Test Hiện Tại
```bash
# File .env - CÓ THỂ TEST THẬT
MOMO_PARTNER_CODE=MOMOIQA420180417
MOMO_ACCESS_KEY=SvDmj2cOTYZmQQ3H
MOMO_SECRET_KEY=PPuDXq1KowPT1ftR8DvlQTHhC03aul17
MOMO_API_URL=https://test-payment.momo.vn/v2/gateway/api/create
MOMO_SANDBOX=true
```

### 🧪 Test với MoMo Sandbox
```bash
# Credentials trên là THẬT từ MoMo
# Bạn có thể test ngay:

1. Tải app MoMo (test version hoặc production đều được)
2. Đăng ký tài khoản MoMo
3. Nạp tiền test (không mất tiền thật trong sandbox)
4. Quét QR code từ website
5. Xác nhận thanh toán
```

### 🏭 Chuyển sang Production MoMo

#### Bước 1: Đăng ký MoMo Business
```bash
1. Truy cập: https://business.momo.vn/

2. Chọn gói dịch vụ:
   - MoMo Business Basic (miễn phí, phí 1-2%)
   - MoMo Business Plus (có phí setup, phí thấp hơn)

3. Chuẩn bị hồ sơ:
   ✓ GPKD (bản scan màu)
   ✓ CMND/CCCD người đại diện
   ✓ Giấy ủy quyền (nếu cần)
   ✓ Logo công ty (300x300px PNG)
   ✓ Thông tin website/app
   
4. Upload hồ sơ online

5. Chờ MoMo liên hệ xác minh (1-2 ngày)

6. Ký hợp đồng điện tử

7. Nhận credentials (3-5 ngày)
```

#### Bước 2: Nhận Production Credentials
```
MoMo gửi email chứa:
- Partner Code: MOMOXXX
- Access Key: xxx
- Secret Key: xxx  
- Public Key: xxx (RSA 2048)
- Tài liệu API v2.1
```

#### Bước 3: Cập nhật Production Config

**File .env:**
```bash
# Production MoMo
MOMO_PARTNER_CODE=MOMOXXX               # Partner Code thật
MOMO_ACCESS_KEY=your_access_key         # Access Key thật
MOMO_SECRET_KEY=your_secret_key         # Secret Key thật
MOMO_API_URL=https://payment.momo.vn/v2/gateway/api/create  # ⚠️ URL Production
MOMO_PUBLIC_KEY=your_rsa_public_key     # RSA Public Key
MOMO_SANDBOX=false                       # ⚠️ Tắt sandbox
MOMO_ENABLED=true
```

#### Bước 4: Cập nhật Return URL
```bash
MOMO_RETURN_URL=https://your-domain.com/Payment/MoMoReturn
MOMO_IPN_URL=https://your-domain.com/Payment/MoMoIPN
```

#### Bước 5: Test Production
```bash
1. Test với QR code
2. Test với deep link (mobile app)
3. Test webhook/IPN
4. Test refund
5. Test query status
```

---

## 🟢 3. Stripe - Chuyển sang Production

### ✅ Credentials Test Hiện Tại
```bash
# File .env - TEST MODE (hoạt động tốt)
STRIPE_PUBLISHABLE_KEY=pk_test_51SMLP3K...
STRIPE_SECRET_KEY=sk_test_51SMLP3K...
STRIPE_SANDBOX=true
```

### 🧪 Test với Stripe Test Cards
```bash
# Thẻ test Stripe (miễn phí)
Thành công:
- 4242 4242 4242 4242 (Visa)
- 5555 5555 5555 4444 (Mastercard)
- Exp: Bất kỳ tương lai
- CVC: Bất kỳ 3 số

Thất bại:
- 4000 0000 0000 0002 (Card declined)
- 4000 0000 0000 9995 (Insufficient funds)
```

### 🏭 Chuyển sang Production Stripe

#### Bước 1: Complete Business Profile
```bash
1. Đăng nhập: https://dashboard.stripe.com
2. Click "Activate your account"
3. Điền thông tin:
   ✓ Business type: Company
   ✓ Country: Vietnam
   ✓ Business name: JOHN HENRY FASHION
   ✓ Tax ID (MST): Mã số thuế công ty
   ✓ Website: https://your-domain.com
   ✓ Business address
   ✓ Phone number
```

#### Bước 2: Verify Identity
```bash
4. Upload documents:
   ✓ GPKD (Business Registration)
   ✓ CMND/CCCD người đại diện
   ✓ Proof of address (hóa đơn điện/nước)
   
5. Chờ Stripe verify (1-3 ngày)

6. Nhận email confirmation
```

#### Bước 3: Enable Payment Methods
```bash
7. Dashboard → Settings → Payment methods
8. Enable:
   ✓ Cards (Visa, Mastercard, JCB)
   ✓ Apple Pay (optional)
   ✓ Google Pay (optional)
```

#### Bước 4: Get Live API Keys
```bash
9. Dashboard → Developers → API keys
10. Toggle từ "Test mode" → "Live mode"
11. Copy keys:
    - Publishable key: pk_live_xxx
    - Secret key: sk_live_xxx
```

#### Bước 5: Setup Webhooks
```bash
12. Dashboard → Developers → Webhooks
13. Add endpoint: https://your-domain.com/api/stripe/webhook
14. Select events:
    - payment_intent.succeeded
    - payment_intent.payment_failed
    - charge.refunded
15. Copy webhook secret: whsec_xxx
```

#### Bước 6: Cập nhật Production Config

**File .env:**
```bash
# Production Stripe
STRIPE_PUBLISHABLE_KEY=pk_live_xxx      # Live publishable key
STRIPE_SECRET_KEY=sk_live_xxx           # Live secret key
STRIPE_WEBHOOK_SECRET=whsec_xxx         # Webhook secret
STRIPE_API_URL=https://api.stripe.com   # Không đổi
STRIPE_CURRENCY=vnd                     # Không đổi
STRIPE_SANDBOX=false                    # ⚠️ Tắt test mode
STRIPE_ENABLED=true
```

#### Bước 7: Update Frontend
```javascript
// Views/Checkout/Payment.cshtml
// Thay test key bằng live key
<script src="https://js.stripe.com/v3/"></script>
<script>
    var stripe = Stripe('pk_live_xxx'); // ⚠️ Live key
</script>
```

#### Bước 8: Test Production
```bash
1. Test với thẻ thật (số tiền nhỏ: 1,000đ)
2. Kiểm tra webhook hoạt động
3. Test refund
4. Monitor trong Dashboard
```

---

## 🏦 4. Bank Transfer - Production Ready

### ✅ Current Config (Đã sẵn sàng)
```bash
BANK_TRANSFER_ENABLED=true
BANK_TECHCOMBANK_ACCOUNT=207705092005
BANK_TECHCOMBANK_HOLDER=NGUYEN HUU THANG
```

### ⚠️ Khuyến nghị nâng cấp

#### Option 1: Dùng tài khoản cá nhân (hiện tại)
```
✅ Ưu điểm:
- Không cần giấy tờ
- Setup nhanh
- Phí thấp

❌ Nhược điểm:
- Không chuyên nghiệp
- Giới hạn giao dịch
- Khó quản lý thuế
```

#### Option 2: Mở tài khoản doanh nghiệp (khuyến nghị)
```bash
1. Đến ngân hàng với:
   ✓ GPKD
   ✓ Giấy ủy quyền
   ✓ Con dấu công ty
   
2. Mở tài khoản doanh nghiệp

3. Cập nhật .env:
BANK_TECHCOMBANK_ACCOUNT=xxx
BANK_TECHCOMBANK_HOLDER=CONG TY JOHN HENRY FASHION
```

---

## 💵 5. Cash on Delivery (COD)

### ✅ Đã Production Ready
```bash
# File .env
COD_ENABLED=true
COD_MAX_AMOUNT=10000000      # 10 triệu
COD_SERVICE_FEE=0            # Miễn phí

# Không cần config thêm!
```

---

## 📝 Checklist Chuyển Production

### ✅ Pre-Launch Checklist

#### 1. SSL Certificate
```bash
☐ Website có HTTPS
☐ SSL certificate hợp lệ
☐ Force HTTPS redirect
```

#### 2. Domain & URLs
```bash
☐ Domain production đã setup
☐ Return URLs đã update
☐ IPN/Webhook URLs đã update
☐ Test all URLs accessible
```

#### 3. Environment Variables
```bash
☐ Copy .env sang .env.production
☐ Update tất cả *_SANDBOX=false
☐ Update tất cả API URLs sang production
☐ Double check tất cả secrets
```

#### 4. Security
```bash
☐ .env không commit lên Git
☐ Add .env vào .gitignore
☐ Secrets được encrypt
☐ Access logs được enable
```

#### 5. Testing
```bash
☐ Test mỗi payment method với số tiền nhỏ
☐ Test webhooks/IPNs hoạt động
☐ Test refund flows
☐ Test error handling
☐ Monitor logs 24h đầu
```

---

## 🔐 Bảo Mật Quan Trọng

### ⚠️ KHÔNG BAO GIỜ:
```bash
❌ Commit .env lên Git
❌ Share secrets qua email/chat
❌ Hardcode credentials trong code
❌ Log sensitive data
❌ Expose API keys trong frontend
```

### ✅ NÊN LÀM:
```bash
✅ Dùng Environment Variables
✅ Encrypt secrets at rest
✅ Rotate keys định kỳ (3-6 tháng)
✅ Setup monitoring & alerts
✅ Backup credentials securely
✅ Document emergency procedures
```

---

## 📞 Support Contacts

### VNPay
- Hotline: 1900 55 55 77
- Email: support@vnpay.vn
- Docs: https://sandbox.vnpayment.vn/apis/

### MoMo
- Hotline: 1900 54 54 41
- Email: business@momo.vn  
- Docs: https://developers.momo.vn/

### Stripe
- Email: support@stripe.com
- Docs: https://stripe.com/docs
- Dashboard: https://dashboard.stripe.com

---

## 🎯 Tóm Tắt Chi Phí

| Gateway | Setup Fee | Transaction Fee | Monthly Fee |
|---------|-----------|-----------------|-------------|
| VNPay | 0đ | 1.0% - 2.5% | 0đ |
| MoMo | 0đ - 5tr | 1.5% - 3.0% | 0đ |
| Stripe | 0đ | 3.4% + 8,000đ | 0đ |
| Bank Transfer | 0đ | 0đ | 0đ |
| COD | 0đ | 0đ | 0đ |

**Lưu ý:** Chi phí có thể thay đổi, liên hệ trực tiếp để biết chính xác.

---

## 📅 Timeline Dự Kiến

```
Week 1-2: Đăng ký VNPay
  ↓
Week 2-3: Đăng ký MoMo  
  ↓
Week 3: Complete Stripe verification
  ↓
Week 4: Testing & Integration
  ↓
Week 5: Production Launch
```

---

**⚡ Ready to go production? Follow this guide step by step!**
