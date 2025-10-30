# 🔄 ENVIRONMENT SWITCHER - QUICK GUIDE

## 🎯 3 ENVIRONMENTS

```bash
# Development (HTTPS testing)
./switch-env.sh dev
→ https://localhost:5001

# Local Server (your local port 8080)
./switch-env.sh local
→ http://localhost:8080

# Production (placeholder domain)
./switch-env.sh prod
→ https://johnhenry-infinityzero.com
```

---

## 📋 CURRENT SETUP

| File | Environment | Base URL | Payment Mode |
|------|-------------|----------|--------------|
| `.env` | Development (default) | `https://localhost:5001` | ✅ Sandbox |
| `.env.local` | Local Server | `http://localhost:8080` | ✅ Sandbox |
| `.env.production` | Production | `https://johnhenry-infinityzero.com` | ⚠️ Production |

---

## 🚀 COMMON WORKFLOWS

### **Test on localhost:8080**
```bash
# 1. Switch
./switch-env.sh local

# 2. Run
dotnet run --urls=http://localhost:8080

# 3. Browse
open http://localhost:8080
```

### **Test on localhost:5001 (default)**
```bash
# 1. Switch (if needed)
./switch-env.sh dev

# 2. Run
dotnet run

# 3. Browse
open https://localhost:5001
```

### **Prepare for Production**
```bash
# 1. Update credentials in .env.production
vim .env.production

# 2. Switch
./switch-env.sh prod

# 3. Verify
cat .env | grep BASE_URL
```

---

## 🔧 WHAT GETS UPDATED

When you switch environment, these change automatically:

```
BASE_URL                    → All return URLs use this
ASPNETCORE_ENVIRONMENT      → Development/Production mode
Payment Gateway Sandbox     → true/false
SWAGGER_ENABLED             → true/false
DETAILED_ERRORS             → true/false
```

**Return URLs auto-generated from BASE_URL:**
```
VNPay:  {BASE_URL}/Checkout/PaymentReturn
MoMo:   {BASE_URL}/Payment/MoMo/Return
Stripe: {BASE_URL}/Checkout/Stripe-Return
```

---

## ⚠️ IMPORTANT

### **Sandbox vs Production**

| Mode | Test Cards | Real Money | Need Registration |
|------|------------|------------|-------------------|
| Sandbox | ✅ Works | ❌ No charges | ❌ No |
| Production | ❌ Won't work | ✅ Real charges | ✅ YES |

### **Before Production:**
- [ ] Get real VNPay credentials (register at vnpay.vn)
- [ ] Get real MoMo credentials (register at business.momo.vn)
- [ ] Get Stripe live keys (pk_live_xxx, sk_live_xxx)
- [ ] Update return URLs in payment gateway dashboards
- [ ] Get real domain with SSL certificate

---

## 🧪 TESTING

### **Which environment for what?**

**Development (localhost:5001):**
- ✅ Daily development
- ✅ HTTPS testing
- ✅ Payment gateway sandbox
- ✅ Gmail OAuth (need HTTPS)

**Local Server (localhost:8080):**
- ✅ Test deployment locally
- ✅ Simulate production server
- ✅ Payment gateway sandbox still works
- ⚠️ Gmail OAuth might not work (needs HTTPS)

**Production:**
- ✅ Real deployment
- ✅ Real payment processing
- ⚠️ Need real credentials
- ⚠️ Need real domain + SSL

---

## 📞 TROUBLESHOOTING

### **Check current environment:**
```bash
cat .env | head -20
```

### **Reset to development:**
```bash
cp .env.backup.* .env  # Use most recent backup
# Or
./switch-env.sh dev
```

### **Verify configuration:**
```bash
./verify-config.sh
```

---

## 📚 FULL GUIDE

See: `DEPLOYMENT_GUIDE.md` for complete documentation

---

**Quick switch, no hassle! 🎯**
