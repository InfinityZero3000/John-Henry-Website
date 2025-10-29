# 🚀 HƯỚNG DẪN DEPLOY ASP.NET CORE LÊN RAILWAY.APP (MIỄN PHÍ)

**Platform:** Railway.app  
**Cost:** $5 credit miễn phí/tháng (đủ cho testing)  
**Setup Time:** 15-20 phút  
**URL:** johnhenry.up.railway.app

---

## ✅ TẠI SAO CHỌN RAILWAY.APP?

- ✅ **Miễn phí** $5 credit/tháng (đủ chạy test)
- ✅ **Hỗ trợ .NET** native (không cần Docker)
- ✅ **PostgreSQL** database included free
- ✅ **Auto deploy** từ GitHub
- ✅ **Custom domain** free (sau khi có domain)
- ✅ **SSL** certificate tự động
- ✅ **Environment variables** easy setup

---

## 📋 BƯỚC 1: CHUẨN BỊ CODE

### 1.1. Tạo Dockerfile (Railway cần file này)

```dockerfile
# File: Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["JohnHenryFashionWeb.csproj", "./"]
RUN dotnet restore "JohnHenryFashionWeb.csproj"
COPY . .
RUN dotnet build "JohnHenryFashionWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JohnHenryFashionWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JohnHenryFashionWeb.dll"]
```

### 1.2. Cập nhật Program.cs để lắng nghe PORT từ Railway

```csharp
// Thêm vào Program.cs trước builder.Build()
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
```

### 1.3. Tạo file railway.json (optional, config Railway)

```json
{
  "$schema": "https://railway.app/railway.schema.json",
  "build": {
    "builder": "DOCKERFILE"
  },
  "deploy": {
    "startCommand": "dotnet JohnHenryFashionWeb.dll",
    "restartPolicyType": "ON_FAILURE",
    "restartPolicyMaxRetries": 10
  }
}
```

---

## 📋 BƯỚC 2: PUSH CODE LÊN GITHUB

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"

# Add Dockerfile
git add Dockerfile railway.json

# Commit
git commit -m "Add Railway deployment config"

# Push to GitHub
git push origin main
```

---

## 📋 BƯỚC 3: TẠO TÀI KHOẢN VÀ DEPLOY RAILWAY

### 3.1. Đăng ký Railway
1. Truy cập: https://railway.app
2. Click "Start a New Project"
3. Đăng nhập bằng GitHub
4. Authorize Railway access to GitHub

### 3.2. Deploy từ GitHub Repo
```bash
1. Click "Deploy from GitHub repo"
2. Chọn repo: "John-Henry-Website"
3. Railway sẽ tự động detect .NET project
4. Click "Deploy Now"
```

### 3.3. Add PostgreSQL Database
```bash
1. Trong project dashboard, click "New"
2. Chọn "Database" → "PostgreSQL"
3. Railway sẽ tạo database và provide connection string
4. Copy DATABASE_URL (dạng: postgres://user:pass@host:port/db)
```

### 3.4. Cấu hình Environment Variables
```bash
1. Click vào service (web app)
2. Vào tab "Variables"
3. Add các biến từ .env.production:

# Database (Railway auto-provide DATABASE_URL)
DATABASE_URL=${PostgreSQL.DATABASE_URL}  # Railway tự map

# Or manual config:
DB_HOST=${PostgreSQL.PGHOST}
DB_PORT=${PostgreSQL.PGPORT}
DB_NAME=${PostgreSQL.PGDATABASE}
DB_USER=${PostgreSQL.PGUSER}
DB_PASSWORD=${PostgreSQL.PGPASSWORD}

# Application
ASPNETCORE_ENVIRONMENT=Production
BASE_URL=https://johnhenry.up.railway.app
SITE_NAME=John Henry

# JWT
JWT_SECRET=JohnHenry2025SecretKeyForJWTTokenGeneration!@#$%
JWT_ISSUER=JohnHenryFashion
JWT_AUDIENCE=JohnHenryUsers
JWT_EXPIRY_HOURS=24

# Google OAuth (copy từ .env.production)
GOOGLE_CLIENT_ID=1050047621783-9e2oiaukh429a9l0qg739afai2ajjp08.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=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

# Email
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USE_SSL=true
EMAIL_USER=nhthang312@gmail.com
EMAIL_PASSWORD=Thezero2077xx
EMAIL_FROM=nhthang312@gmail.com
EMAIL_FROM_NAME=JohnHenry

# Payment - Chỉ enable Bank + COD
BANK_TRANSFER_ENABLED=true
BANK_TECHCOMBANK_ACCOUNT=207705092005
BANK_TECHCOMBANK_HOLDER=NGUYEN HUU THANG
BANK_TECHCOMBANK_BRANCH=TP.HCM

COD_ENABLED=true
COD_MAX_AMOUNT=10000000
COD_SERVICE_FEE=0

# Disable các payment chưa có
VNPAY_ENABLED=false
MOMO_ENABLED=false
STRIPE_ENABLED=false

# Security
PASSWORD_MIN_LENGTH=8
PASSWORD_REQUIRE_DIGIT=true
PASSWORD_REQUIRE_LOWERCASE=true
PASSWORD_REQUIRE_UPPERCASE=true
PASSWORD_REQUIRE_SPECIAL_CHAR=true
MAX_LOGIN_ATTEMPTS=5
LOCKOUT_DURATION_MINUTES=15
REQUIRE_EMAIL_CONFIRMATION=true
GOOGLE_AUTO_CONFIRM_EMAIL=false

# Production settings
SWAGGER_ENABLED=false
DETAILED_ERRORS=false
ENABLE_HTTPS_REDIRECTION=true
```

---

## 📋 BƯỚC 4: RUN DATABASE MIGRATION

### 4.1. Get Railway CLI (optional)
```bash
# Install Railway CLI
npm install -g @railway/cli

# Login
railway login

# Link to project
railway link
```

### 4.2. Run migrations
```bash
# Option 1: Via Railway CLI
railway run dotnet ef database update

# Option 2: Via local connection
# Copy DATABASE_URL from Railway
export DATABASE_URL="postgresql://user:pass@host:port/db"
dotnet ef database update --connection $DATABASE_URL

# Option 3: SQL script (recommended)
# Export schema từ local
pg_dump -h localhost -U johnhenry_user johnhenry_db --schema-only > schema.sql

# Import vào Railway database
psql $DATABASE_URL < schema.sql
```

---

## 📋 BƯỚC 5: CẤU HÌNH GOOGLE OAUTH REDIRECT URI

```bash
1. Vào: https://console.cloud.google.com/apis/credentials
2. Edit OAuth 2.0 Client ID
3. Add Authorized redirect URIs:
   https://johnhenry.up.railway.app/signin-google
   https://johnhenry.up.railway.app/Account/GoogleCallback

4. Save
```

---

## 📋 BƯỚC 6: TEST DEPLOYMENT

### 6.1. Check deployment logs
```bash
# Railway dashboard → Deployments → View logs
# Hoặc dùng CLI:
railway logs
```

### 6.2. Test website
```bash
# Lấy URL từ Railway dashboard (dạng: johnhenry.up.railway.app)

# Test basic:
curl -I https://johnhenry.up.railway.app

# Test trong browser:
1. Truy cập homepage
2. Test đăng ký user mới
3. Test đăng nhập
4. Test Google OAuth
5. Test add to cart
6. Test checkout với Bank Transfer
7. Test checkout với COD
```

---

## 📋 BƯỚC 7: CUSTOM DOMAIN (OPTIONAL - SAU KHI MUA DOMAIN)

```bash
1. Railway dashboard → Settings → Domains
2. Click "Add Domain"
3. Nhập: johnhenry-infinityzero.com
4. Railway cung cấp CNAME record:
   
   Type: CNAME
   Host: @
   Value: johnhenry.up.railway.app
   
5. Vào DNS provider (Namecheap/GoDaddy)
6. Add CNAME record trên
7. Đợi DNS propagate (5-60 phút)
8. Railway tự động generate SSL certificate
```

---

## 💰 CHI PHÍ RAILWAY.APP

### Free Tier
- ✅ $5 credit/tháng miễn phí
- ✅ 500 MB RAM
- ✅ 1 GB disk
- ✅ Unlimited bandwidth
- ✅ PostgreSQL database included

**Đủ cho:**
- Testing và development
- Low traffic sites (< 1000 requests/day)
- Demo cho khách hàng

### Paid Tier (nếu hết free credit)
- $5/tháng cho 8 GB egress traffic
- Sau đó $0.10/GB

**Chi phí ước tính:**
- ~$5-10/tháng cho traffic vừa phải
- Rẻ hơn Azure Basic B1 ($13/tháng)

---

## 🔧 TROUBLESHOOTING

### Issue 1: Build failed
```bash
# Check Dockerfile syntax
# Ensure .csproj file name correct
# Check .NET version match (9.0)
```

### Issue 2: Database connection failed
```bash
# Verify DATABASE_URL env variable
# Check PostgreSQL service running
# Test connection string format
```

### Issue 3: Application crashes
```bash
# Check logs: railway logs
# Verify PORT env variable used
# Check appsettings.json paths
```

### Issue 4: Static files not loading
```bash
# Ensure wwwroot folder copied in Dockerfile
# Check UseStaticFiles() in Program.cs
# Verify paths in _Layout.cshtml
```

---

## 📊 SO SÁNH CÁC PLATFORM

| Platform | Free Tier | .NET Support | Database | Custom Domain | Setup Ease |
|----------|-----------|--------------|----------|---------------|------------|
| **Railway.app** | ✅ $5/mo | ✅ Native | ✅ Free | ✅ Yes | ⭐⭐⭐⭐⭐ |
| **Azure App Service** | ✅ F1 | ✅ Native | ❌ Pay | ✅ Yes | ⭐⭐⭐⭐ |
| **Render.com** | ✅ Yes | ✅ Docker | ✅ Free | ✅ Yes | ⭐⭐⭐⭐ |
| **Fly.io** | ✅ 3 VMs | ✅ Docker | ❌ Pay | ✅ Yes | ⭐⭐⭐ |
| **Heroku** | ❌ No | ✅ Docker | ❌ Pay | ✅ Yes | ⭐⭐⭐⭐ |
| **Vercel** | ❌ No | ❌ No | ❌ No | ✅ Yes | N/A |

**Winner: Railway.app** ⭐
- Best balance: free tier + ease of use + .NET support

---

## ✅ CHECKLIST DEPLOYMENT

### Pre-Deployment
- [ ] Dockerfile created
- [ ] railway.json created (optional)
- [ ] Program.cs updated (PORT env)
- [ ] Code pushed to GitHub
- [ ] Railway account created

### Deployment
- [ ] GitHub repo connected
- [ ] PostgreSQL database added
- [ ] Environment variables configured
- [ ] Database migrated
- [ ] Google OAuth redirect URI updated

### Post-Deployment Testing
- [ ] Homepage loads
- [ ] User registration works
- [ ] Login works
- [ ] Google OAuth works
- [ ] Products display
- [ ] Add to cart works
- [ ] Checkout works (Bank Transfer)
- [ ] Checkout works (COD)
- [ ] Email notifications send
- [ ] Admin panel accessible

---

## 🎯 KẾT LUẬN

Railway.app là lựa chọn **TỐT NHẤT** để deploy tạm ASP.NET Core của bạn:

✅ **Ưu điểm:**
- Free tier đủ dùng ($5 credit/tháng)
- Setup cực kỳ đơn giản
- Hỗ trợ .NET native
- PostgreSQL database miễn phí
- Auto SSL, custom domain
- Deploy tự động từ GitHub

⚠️ **Lưu ý:**
- Free tier có giới hạn RAM (500MB)
- Sau khi hết $5 credit cần nạp thêm
- Không phù hợp cho production scale lớn

🚀 **Khuyến nghị:**
1. Deploy lên Railway để test và demo
2. Nếu traffic tăng → upgrade Railway ($10/tháng)
3. Hoặc migrate sang Azure App Service sau

**Thời gian deploy: 15-20 phút!** ⚡
