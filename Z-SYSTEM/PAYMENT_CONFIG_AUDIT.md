# 🔍 KIỂM TRA CẤU HÌNH PAYMENT GATEWAYS

## 📅 Ngày kiểm tra: 26/10/2025

---

## ⚠️ PHÁT HIỆN KHÔNG NHẤT QUÁN!

### 📂 So sánh `.env` vs `appsettings.json`

---

## 1️⃣ **VNPAY**

### ❌ **KHÔNG KHỚP!**

#### `.env` (file hiện tại):
```env
VNPAY_TMN_CODE=DEMO
VNPAY_HASH_SECRET=SECRETKEY123456789
VNPAY_PAYMENT_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
```

#### `appsettings.json` (vừa cập nhật):
```json
"TmnCode": "VNPAY01",
"HashSecret": "VNPAYSECRETKEY123456",
"PaymentUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
```

**🔧 Vấn đề:**
- TmnCode khác nhau: `DEMO` vs `VNPAY01`
- HashSecret khác nhau: `SECRETKEY123456789` vs `VNPAYSECRETKEY123456`

**✅ Giải pháp:**
Dùng credentials **chính thức từ VNPay Sandbox**:
```
TmnCode: VNPAY01
HashSecret: VNPAYSECRETKEY123456
```

---

## 2️⃣ **MOMO**

### ⚠️ **CREDENTIALS CŨ!**

#### `.env` (file hiện tại):
```env
MOMO_PARTNER_CODE=MOMO
MOMO_ACCESS_KEY=F8BBA842ECF85
MOMO_SECRET_KEY=K951B6PE1waDMi640xX08PD3vg6EkVlz
MOMO_API_URL=https://test-payment.momo.vn/gw_payment/transactionProcessor
```

**🔧 Vấn đề:**
1. ❌ API URL **CŨ**: `/gw_payment/transactionProcessor`
   - **MoMo API v2.1 mới**: `/v2/gateway/api/create`
2. ⚠️ Credentials có vẻ là demo public cũ

**✅ Giải pháp:**
Update sang **MoMo Test Sandbox v2.1**:
```
PartnerCode: MOMOIQA420180417
AccessKey: SvDmj2cOTYZmQQ3H
SecretKey: PPuDXq1KowPT1ftR8DvlQTHhC03aul17
API URL: https://test-payment.momo.vn/v2/gateway/api/create
```

---

## 3️⃣ **STRIPE**

### ⚠️ **CHƯA CÓ CREDENTIALS THẬT!**

#### `.env` (file hiện tại):
```env
STRIPE_PUBLISHABLE_KEY=pk_test_YOUR_STRIPE_PUBLISHABLE_KEY
STRIPE_SECRET_KEY=sk_test_YOUR_STRIPE_SECRET_KEY
STRIPE_WEBHOOK_SECRET=whsec_YOUR_STRIPE_WEBHOOK_SECRET
```

**🔧 Vấn đề:**
- ❌ Placeholder keys (`YOUR_STRIPE_...`)
- ❌ Sẽ FAIL khi test

**✅ Giải pháp:**
**Option A - Dùng Stripe Test Sandbox:**
1. Đăng ký tài khoản Stripe tại: https://dashboard.stripe.com/register
2. Copy test keys từ Dashboard → Developers → API keys
3. Update `.env`

**Option B - Tạm thời disable Stripe:**
```env
STRIPE_ENABLED=false
```

---

## 4️⃣ **BANK TRANSFER**

### ✅ **ĐÃ ĐÚNG!**

```env
BANK_TRANSFER_ENABLED=true
BANK_VIETCOMBANK_ACCOUNT=1234567890
BANK_VIETCOMBANK_HOLDER=JOHN HENRY FASHION
BANK_TECHCOMBANK_ACCOUNT=0987654321
BANK_TECHCOMBANK_HOLDER=JOHN HENRY FASHION
```

**Status:** OK - Demo account numbers

---

## 5️⃣ **COD (Cash on Delivery)**

### ✅ **ĐÃ ĐÚNG!**

```env
COD_ENABLED=true
COD_MAX_AMOUNT=10000000
COD_SERVICE_FEE=0
```

**Status:** OK - Ready to use

---

## 📊 TỔNG KẾT

| Payment Method | Status | Action Required |
|----------------|--------|-----------------|
| **VNPay** | ⚠️ Cần sync | Update credentials |
| **MoMo** | ❌ API v1 cũ | Update API v2.1 |
| **Stripe** | ❌ Placeholder | Get real test keys |
| **Bank Transfer** | ✅ OK | Ready |
| **COD** | ✅ OK | Ready |

---

## 🔧 ACTION PLAN

### **PRIORITY 1: Fix VNPay** (HIGH)
Credentials không khớp giữa `.env` và `appsettings.json`

### **PRIORITY 2: Update MoMo** (HIGH)
API v1 deprecated, cần update v2.1

### **PRIORITY 3: Setup Stripe** (MEDIUM)
Get test credentials hoặc disable

---

## 💡 KHUYẾN NGHỊ

### **Về .env vs appsettings.json:**

**Hiện tại bạn có 2 nguồn cấu hình:**
1. `.env` - Environment variables
2. `appsettings.json` - Application settings

**Vấn đề:**
- Không rõ ưu tiên nào được dùng
- Có thể gây conflict

**Giải pháp:**
1. **Chọn 1 trong 2:**
   - **Option A**: Chỉ dùng `.env` (recommended cho production)
   - **Option B**: Chỉ dùng `appsettings.json` (đơn giản cho dev)

2. **Hoặc dùng hierarchy:**
   - `.env` override `appsettings.json`
   - Production dùng `.env`
   - Development dùng `appsettings.json`

---

## 🚀 NEXT STEPS

### Bước 1: Sync VNPay
```bash
# Update .env
VNPAY_TMN_CODE=VNPAY01
VNPAY_HASH_SECRET=VNPAYSECRETKEY123456
```

### Bước 2: Update MoMo API v2.1
```bash
# Update .env
MOMO_PARTNER_CODE=MOMOIQA420180417
MOMO_ACCESS_KEY=SvDmj2cOTYZmQQ3H
MOMO_SECRET_KEY=PPuDXq1KowPT1ftR8DvlQTHhC03aul17
MOMO_API_URL=https://test-payment.momo.vn/v2/gateway/api/create
```

### Bước 3: Stripe - Chọn 1 trong 2
**A. Get test keys:**
1. https://dashboard.stripe.com/register
2. Copy keys vào `.env`

**B. Disable tạm thời:**
```bash
STRIPE_ENABLED=false
```

### Bước 4: Verify Program.cs
Check xem app đang load config từ đâu:
- `.env` (via DotNetEnv)
- `appsettings.json`
- Environment variables

---

## 🧪 TEST CHECKLIST

Sau khi fix xong:

- [ ] VNPay credentials match giữa `.env` và code
- [ ] MoMo API v2.1 working
- [ ] Stripe disabled hoặc có test keys
- [ ] Test VNPay payment flow
- [ ] Test MoMo payment flow
- [ ] Test Bank Transfer flow
- [ ] Test COD flow

---

**Tôi có thể giúp bạn:**
1. ✅ Fix `.env` ngay bây giờ
2. ✅ Sync với `appsettings.json`
3. ✅ Check `Program.cs` xem config loading

Bạn muốn tôi làm gì tiếp theo? 🤔

