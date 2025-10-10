# 🔥 GIẢI PHÁP TRIỆT ĐỂ - Database Setup

## ⚠️ Vấn đề đã giải quyết:

Lỗi **"relation AspNetRoles already exists"** khi chạy migrations xảy ra do:
1. Docker mount `database/init` folder chạy SQL scripts trước
2. App sử dụng `EnsureCreated()` thay vì `Migrate()`  
3. Migration files không tương thích với database schema

## ✅ Đã Fix:

1. **Removed init mount** từ `docker-compose.yml`
   - Không còn auto-run SQL scripts
   - Database sẽ hoàn toàn trống khi khởi tạo

2. **Changed `EnsureCreated()` → `Migrate()`** trong `Program.cs`
   - `EnsureCreated()`: Tạo schema trực tiếp, KHÔNG dùng migrations ❌
   - `Migrate()`: Apply migrations properly, có history tracking ✅

3. **Removed old SQL init files**
   - File cũ dùng INTEGER, models dùng GUID → Conflict
   - Sử dụng EF Core migrations hoàn toàn

## 🚀 Cách Setup Database (TRIỆT ĐỂ):

### Bước 1: Reset hoàn toàn Docker

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"

# Stop và xóa tất cả
docker-compose down -v

# Pull images mới nhất
docker-compose pull

# Start lại
docker-compose up -d

# Đợi 10 giây cho PostgreSQL khởi động
sleep 10
```

### Bước 2: Chạy ứng dụng (App tự động migrate)

```bash
# App sẽ tự động:
# 1. Apply tất cả migrations
# 2. Seed roles (Admin, Seller, Customer)
# 3. Seed admin user (admin@johnhenry.com / Admin123!)

dotnet run
```

**Hoặc nếu muốn manual:**

```bash
# Apply migrations thủ công
dotnet ef database update

# Sau đó chạy app để seed data
dotnet run
```

### Bước 3: Verify trong pgAdmin

1. Mở: http://localhost:8080
2. Login:
   - Email: `admin@johnhenry.com`
   - Password: `admin123`
3. Connect server (lần đầu):
   - Host: `postgres` ⚠️ (KHÔNG phải localhost!)
   - Port: 5432
   - Database: johnhenry_db
   - Username: johnhenry_user
   - Password: JohnHenry@2025!

## 📋 Kiểm tra Database đã có data:

```bash
# Xem tất cả tables
docker exec johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "\dt"

# Xem users
docker exec johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "SELECT \"Email\", \"FirstName\", \"LastName\" FROM \"AspNetUsers\";"

# Xem roles
docker exec johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "SELECT * FROM \"AspNetRoles\";"

# Xem products
docker exec johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "SELECT \"Name\", \"Price\", \"SKU\" FROM \"Products\" LIMIT 10;"

# Xem migration history
docker exec johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "SELECT * FROM \"__EFMigrationsHistory\" ORDER BY \"MigrationId\";"
```

## 🔧 Troubleshooting

### Nếu vẫn lỗi "already exists":

```bash
# 1. Stop app nếu đang chạy (Ctrl+C)

# 2. Drop và recreate database
docker exec johnhenry_postgres psql -U johnhenry_user -d postgres -c "DROP DATABASE IF EXISTS johnhenry_db;"
docker exec johnhenry_postgres psql -U johnhenry_user -d postgres -c "CREATE DATABASE johnhenry_db;"

# 3. Restart PostgreSQL để clear connection pool
docker restart johnhenry_postgres
sleep 10

# 4. Chạy lại app
dotnet run
```

### Nếu app không seed data:

App chỉ seed khi database trống. Nếu đã có admin user, nó sẽ skip.

```bash
# Xem logs để biết
tail -f logs/john-henry-$(date +%Y%m%d).txt
```

## 📝 Login Credentials

### pgAdmin
- URL: http://localhost:8080
- Email: `admin@johnhenry.com`
- Password: `admin123`

### Admin User (trong app)
- Email: `admin@johnhenry.com`
- Password: `Admin123!`

### Seller User (trong app)
- Email: `seller@johnhenry.com`  
- Password: `Seller123!`

## 🎯 Tóm tắt

**Trước đây:**
- ❌ Docker mount init folder → auto-run SQL
- ❌ `EnsureCreated()` → tạo schema không qua migrations
- ❌ Migration files conflict với SQL scripts
- ❌ Database có schema nhưng không có migration history

**Bây giờ:**
- ✅ Không mount init folder
- ✅ `Migrate()` → apply migrations properly
- ✅ Database luôn sync với C# models
- ✅ Migration history được tracking đầy đủ
- ✅ Chỉ cần chạy app, tất cả tự động!

## 🔑 Key Takeaways

1. **KHÔNG BAO GIỜ mix SQL init scripts với EF Migrations**
2. **LUÔN dùng `Migrate()` thay vì `EnsureCreated()`**
3. **Migrations là source of truth cho database schema**
4. **Khi trong Docker: Host = `postgres`, không phải `localhost`**
5. **App tự động setup mọi thứ, chỉ cần `dotnet run`!**

---

**Nếu vẫn gặp vấn đề, xem**: [CONNECTION_TROUBLESHOOTING.md](./CONNECTION_TROUBLESHOOTING.md)
