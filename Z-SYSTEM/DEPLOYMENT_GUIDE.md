# 🌐 DEPLOYMENT GUIDE - LOCAL SERVER & PRODUCTION

## 📅 Created: October 26, 2025

---

## 🎯 OVERVIEW

Hướng dẫn này giúp bạn deploy application trong 3 môi trường:
1. **Development**: `https://localhost:5001` (testing với HTTPS)
2. **Local Server**: `http://localhost:8080` (deploy local trên máy bạn)
3. **Production**: `https://johnhenry-infinityzero.com` (placeholder cho production)

---

## 📂 FILES CẤU HÌNH

### **3 Environment Files:**

| File | Purpose | Base URL | Payment Mode |
|------|---------|----------|--------------|
| `.env` | Development (default) | `https://localhost:5001` | Sandbox |
| `.env.local` | Local Server | `http://localhost:8080` | Sandbox |
| `.env.production` | Production | `https://johnhenry-infinityzero.com` | Production |

---

## 🔄 SWITCH GIỮA CÁC MÔI TRƯỜNG

### **Sử dụng Script (Recommended)**

```bash
# Switch to Development (localhost:5001)
./switch-env.sh dev

# Switch to Local Server (localhost:8080)
./switch-env.sh local

# Switch to Production
./switch-env.sh prod
```

### **Manual Switch**

```bash
# Backup current
cp .env .env.backup

# Switch to local server
cp .env.local .env

# Or switch to production
cp .env.production .env
```

---

## 🖥️ DEPLOYMENT - LOCAL SERVER (localhost:8080)

### **Scenario:** Deploy trên máy local với port 8080

### **Step 1: Switch Environment**

```bash
./switch-env.sh local
```

**Kết quả:**
- ✅ Base URL: `http://localhost:8080`
- ✅ Payment gateways: Sandbox mode
- ✅ Return URLs tự động dùng `http://localhost:8080`

### **Step 2: Verify Configuration**

```bash
cat .env | grep BASE_URL
# Should show: BASE_URL=http://localhost:8080
```

### **Step 3: Run Application**

```bash
# Option A: Using environment variable
ASPNETCORE_URLS=http://localhost:8080 dotnet run

# Option B: Using --urls parameter
dotnet run --urls=http://localhost:8080

# Option C: Using environment from .env (auto-loaded)
dotnet run
```

### **Step 4: Access Application**

```
Browser: http://localhost:8080
```

### **Step 5: Test Payment Gateways**

**Tất cả Return URLs sẽ tự động dùng `http://localhost:8080`:**

```
VNPay Return:  http://localhost:8080/Checkout/PaymentReturn
MoMo Return:   http://localhost:8080/Payment/MoMo/Return
MoMo Notify:   http://localhost:8080/Payment/MoMo/Notify
Stripe Return: http://localhost:8080/Checkout/Stripe-Return
```

---

## 🚀 DEPLOYMENT - PRODUCTION

### **Scenario:** Deploy với domain thật hoặc placeholder

### **Step 1: Update Production Credentials**

Edit `.env.production`:

```bash
# VNPay Production (need to register)
VNPAY_TMN_CODE=YOUR_REAL_PRODUCTION_CODE
VNPAY_HASH_SECRET=YOUR_REAL_PRODUCTION_SECRET
VNPAY_SANDBOX=false

# MoMo Production (need to register)
MOMO_PARTNER_CODE=YOUR_REAL_PARTNER_CODE
MOMO_ACCESS_KEY=YOUR_REAL_ACCESS_KEY
MOMO_SECRET_KEY=YOUR_REAL_SECRET_KEY
MOMO_SANDBOX=false

# Stripe Production (need live keys)
STRIPE_PUBLISHABLE_KEY=pk_live_xxx
STRIPE_SECRET_KEY=sk_live_xxx
STRIPE_SANDBOX=false
```

### **Step 2: Switch to Production**

```bash
./switch-env.sh prod
```

**⚠️ Warning:**
```
⚠️  Payment Gateways: PRODUCTION mode (need real credentials!)
```

### **Step 3: Update Domain in .env**

Nếu bạn có domain thật, update trong `.env`:

```bash
# Replace placeholder với domain thật
BASE_URL=https://yourdomain.com
```

### **Step 4: Update Payment Gateway Webhooks**

**VNPay Dashboard:**
```
Return URL: https://yourdomain.com/Checkout/PaymentReturn
```

**MoMo Dashboard:**
```
Return URL: https://yourdomain.com/Payment/MoMo/Return
Notify URL: https://yourdomain.com/Payment/MoMo/Notify
```

**Stripe Dashboard:**
```
Webhook URL: https://yourdomain.com/api/stripe/webhook
```

### **Step 5: Build & Run Production**

```bash
# Build release
dotnet build -c Release

# Run production
dotnet run -c Release --environment Production
```

---

## 🔧 RETURN URLs - CƠ CHẾ HOẠT ĐỘNG

### **Automatic Return URLs (Recommended)**

Application tự động build Return URLs từ `BASE_URL`:

```csharp
// From Program.cs - Environment variables loaded from .env
configuration["SiteSettings:BaseUrl"] = Environment.GetEnvironmentVariable("BASE_URL");

// PaymentService sử dụng BASE_URL
var baseUrl = _configuration["SiteSettings:BaseUrl"];
var returnUrl = $"{baseUrl}/Checkout/PaymentReturn";
```

**Không cần hardcode URLs!**

### **Current Setup:**

| Environment | BASE_URL | VNPay Return URL |
|-------------|----------|------------------|
| Development | `https://localhost:5001` | `https://localhost:5001/Checkout/PaymentReturn` |
| Local Server | `http://localhost:8080` | `http://localhost:8080/Checkout/PaymentReturn` |
| Production | `https://johnhenry-infinityzero.com` | `https://johnhenry-infinityzero.com/Checkout/PaymentReturn` |

---

## 📡 API ENDPOINTS - LOCAL SERVER

### **Khi chạy trên `http://localhost:8080`:**

```
Base:          http://localhost:8080
API:           http://localhost:8080/api/*
Swagger:       http://localhost:8080/swagger
Admin:         http://localhost:8080/admin

Payment Callbacks:
VNPay Return:  http://localhost:8080/Checkout/PaymentReturn
MoMo Return:   http://localhost:8080/Payment/MoMo/Return
MoMo Notify:   http://localhost:8080/Payment/MoMo/Notify
Stripe Return: http://localhost:8080/Checkout/Stripe-Return
```

---

## 🔒 SECURITY - PRODUCTION CHECKLIST

### **Before Going Production:**

- [ ] **Update all production credentials**
  - [ ] VNPay production TmnCode & HashSecret
  - [ ] MoMo production PartnerCode, AccessKey, SecretKey
  - [ ] Stripe live keys (pk_live_xxx, sk_live_xxx)
  
- [ ] **Disable sandbox modes**
  - [ ] `VNPAY_SANDBOX=false`
  - [ ] `MOMO_SANDBOX=false`
  - [ ] `STRIPE_SANDBOX=false`
  
- [ ] **Update return URLs in gateway dashboards**
  - [ ] VNPay merchant portal
  - [ ] MoMo business portal
  - [ ] Stripe webhook settings
  
- [ ] **Enable security features**
  - [ ] `SWAGGER_ENABLED=false`
  - [ ] `DETAILED_ERRORS=false`
  - [ ] `REQUIRE_EMAIL_CONFIRMATION=true`
  - [ ] `REQUIRE_2FA_FOR_ADMIN=true`
  
- [ ] **SSL Certificate**
  - [ ] Install SSL certificate for domain
  - [ ] Force HTTPS: `ENABLE_HTTPS_REDIRECTION=true`
  
- [ ] **Environment variable**
  - [ ] `ASPNETCORE_ENVIRONMENT=Production`

---

## 🧪 TESTING - LOCAL SERVER

### **Test Payment Flow trên localhost:8080:**

1. **Start app:**
   ```bash
   ./switch-env.sh local
   dotnet run --urls=http://localhost:8080
   ```

2. **Browse:** `http://localhost:8080`

3. **Test VNPay:**
   - Add product to cart
   - Checkout → Select VNPay
   - Use test card: `9704198526191432198`, OTP: `123456`
   - Should redirect to: `http://localhost:8080/Checkout/PaymentReturn`

4. **Test MoMo:**
   - Checkout → Select MoMo
   - Should redirect to MoMo payment page
   - After payment, return to: `http://localhost:8080/Payment/MoMo/Return`

5. **Test Stripe:**
   - Checkout → Select Stripe
   - Card: `4242 4242 4242 4242`
   - Should redirect to: `http://localhost:8080/Checkout/Stripe-Return`

---

## ❓ FAQ

### **Q: Tôi không có domain, chỉ có localhost:8080. Có test được payment không?**

**A:** CÓ! Hoàn toàn được:
- ✅ VNPay sandbox accept localhost URLs
- ✅ MoMo sandbox accept localhost URLs
- ✅ Stripe test mode accept localhost URLs
- ⚠️ Production mode cần domain thật

### **Q: Làm sao payment gateway biết return về localhost:8080?**

**A:** Application tự động build return URLs từ `BASE_URL` trong `.env`:
```
BASE_URL=http://localhost:8080
→ Return URL = http://localhost:8080/Checkout/PaymentReturn
```

### **Q: Có cần config gì trong payment gateway dashboard không?**

**A:** 
- **Sandbox**: KHÔNG cần, localhost tự động work
- **Production**: CẦN update return URLs trong merchant portal

### **Q: Switch environment có mất data không?**

**A:** KHÔNG. Script tự động backup `.env` trước khi switch:
```
.env.backup.20251026_143052
```

### **Q: Production cần domain thật không?**

**A:** CÓ. Payment gateways yêu cầu:
- ✅ Valid HTTPS domain
- ✅ SSL certificate
- ✅ Registered trong merchant portal

**Alternatives cho development:**
- Use `localhost:8080` với sandbox mode
- Use ngrok/localtunnel để expose local
- Use free domain từ freenom, duckdns

### **Q: localhost:8080/browser/ là gì?**

**A:** Đó là browser interface của local server của bạn. Application ASP.NET chạy trên:
```
http://localhost:8080  ← Main app
```

---

## 🔄 WORKFLOW SUMMARY

### **Development Flow:**

```bash
# 1. Development (default)
dotnet run
→ https://localhost:5001

# 2. Switch to local server
./switch-env.sh local
dotnet run --urls=http://localhost:8080
→ http://localhost:8080

# 3. Test payments
→ All return URLs auto use localhost:8080

# 4. Switch back to dev
./switch-env.sh dev
→ https://localhost:5001
```

### **Production Flow:**

```bash
# 1. Update production credentials in .env.production
vim .env.production

# 2. Switch to production
./switch-env.sh prod

# 3. Update payment gateway dashboards with real domain

# 4. Build & run
dotnet build -c Release
dotnet run -c Release --environment Production
```

---

## 📞 SUPPORT

### **Check current environment:**

```bash
# Show current BASE_URL
cat .env | grep BASE_URL

# Show environment
cat .env | grep ASPNETCORE_ENVIRONMENT

# Show sandbox mode
cat .env | grep SANDBOX
```

### **Debug return URLs:**

```bash
# Start app và check logs
dotnet run | grep -i "return"

# Test payment flow và check callback URL
```

### **Verify configuration:**

```bash
./verify-config.sh
```

---

## ✅ SUMMARY

**3 Environments Setup:**
- ✅ `.env` (dev): `https://localhost:5001`
- ✅ `.env.local`: `http://localhost:8080`
- ✅ `.env.production`: `https://johnhenry-infinityzero.com`

**Easy Switch:**
```bash
./switch-env.sh [dev|local|prod]
```

**Return URLs:**
- Auto-generated từ `BASE_URL`
- Không cần hardcode
- Switch environment = switch URLs

**Ready to deploy!** 🚀

