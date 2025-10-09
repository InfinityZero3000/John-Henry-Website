# 🐛 Troubleshooting: Backup Script Failed

## ❌ Vấn Đề
Khi chạy `./backup_database.sh`, nhận lỗi:
```
❌ Backup failed!
Exit code: 1
```

---

## 🔍 Nguyên Nhân

### 1. **Connection String Rỗng**
File `appsettings.json` có `DefaultConnection` = `""`
```json
"ConnectionStrings": {
  "DefaultConnection": "",  // ← RỖN G!
  ...
}
```

### 2. **User PostgreSQL Không Đúng**
- Script dùng default user: `postgres`
- Nhưng user thực tế là: `nguyenhuuthang` (owner của database)
- PostgreSQL error: `role "postgres" does not exist`

### 3. **Cấu Hình MacOS/Homebrew**
Trên macOS, PostgreSQL từ Homebrew thường:
- Tạo user trùng tên với username MacOS
- Không tạo user `postgres` mặc định
- Trust authentication cho local connections

---

## ✅ Giải Pháp Đã Áp Dụng

### **Sửa 3 Scripts:**

#### 1. `backup_database.sh`
```bash
# Trước (SAI):
DB_USER=${DB_USER:-postgres}  # ← Always use postgres

# Sau (ĐÚNG):
DB_USER=${DB_USER:-$(whoami)} # ← Use current user as fallback

# Và chỉ export password khi có:
if [ -n "$DB_PASS" ]; then
    PGPASSWORD="$DB_PASS"
    export PGPASSWORD
fi
```

#### 2. `run_insert_products.sh`
```bash
# Tương tự
DB_USER=${DB_USER:-$(whoami)}

if [ -n "$DB_PASS" ]; then
    PGPASSWORD="$DB_PASS"
    export PGPASSWORD
fi
```

#### 3. `restore_database.sh`
```bash
# Tương tự  
DB_USER=${DB_USER:-$(whoami)}

if [ -n "$DB_PASS" ]; then
    PGPASSWORD="$DB_PASS"
    export PGPASSWORD
fi
```

---

## ✅ Kết Quả Sau Khi Sửa

```bash
./backup_database.sh
```

**Output:**
```
==========================================
PostgreSQL Database Backup Tool
==========================================

🔍 Reading database connection...
✓ Database: johnhenry_db
✓ Host: localhost:5432

📦 Creating backup...
   File: backup_johnhenry_db_20251009_221029.sql

✅ Backup completed successfully!

📊 Backup Information:
   File: backup_johnhenry_db_20251009_221029.sql
   Size: 324K
   Path: .../database/backups/backup_johnhenry_db_20251009_221029.sql

📈 Database Statistics:
   Products: 188
   Categories: 2
   Brands: 1
   Orders: 0
   Users: 4
```

---

## 🎓 Bài Học

### **1. Luôn Kiểm Tra User PostgreSQL**
```bash
# Xem user hiện tại
whoami
# → nguyenhuuthang

# List databases và owners
psql -l
# → johnhenry_db | nguyenhuuthang

# Test kết nối
psql -U $(whoami) -d johnhenry_db -c "SELECT 1"
```

### **2. Connection String Có Thể Rỗng**
Nhiều project ASP.NET Core sử dụng:
- Environment variables
- User secrets (`dotnet user-secrets`)
- Appsettings.Development.json

Nếu `appsettings.json` rỗng, cần check:
```bash
# Check user secrets
dotnet user-secrets list

# Check environment
echo $ASPNETCORE_CONNECTIONSTRINGS__DEFAULTCONNECTION
```

### **3. MacOS PostgreSQL Khác Linux**
| Hệ điều hành | Default User | Authentication |
|--------------|--------------|----------------|
| Linux (apt) | `postgres` | Password required |
| MacOS (Homebrew) | `$(whoami)` | Trust (no password) |
| Docker | `postgres` | Password in env |

---

## 📋 Checklist Debug PostgreSQL

Khi gặp lỗi kết nối, check theo thứ tự:

### 1. PostgreSQL có chạy không?
```bash
pg_isready
# → /tmp:5432 - accepting connections
```

### 2. Database có tồn tại không?
```bash
psql -l
# Tìm database trong list
```

### 3. User có quyền truy cập không?
```bash
psql -U your_username -d database_name -c "SELECT 1"
```

### 4. Connection string đúng chưa?
```bash
# Format chuẩn
Host=localhost;Port=5432;Database=db_name;Username=user;Password=pass

# Hoặc URI style
postgresql://user:pass@localhost:5432/db_name
```

### 5. Password có cần không?
```bash
# Check authentication method
cat /usr/local/var/postgresql@14/pg_hba.conf | grep local
# → local   all   all   trust  (không cần password)
# → local   all   all   md5    (cần password)
```

---

## 🔧 Commands Hữu Ích

```bash
# Xem current user
whoami

# List all databases
psql -l

# List all PostgreSQL users/roles
psql -c "\du"

# Test connection
psql -U username -h localhost -d database_name -c "SELECT current_user, current_database()"

# Create new user
createuser -s username

# Grant privileges
psql -c "GRANT ALL PRIVILEGES ON DATABASE database_name TO username"

# Check PostgreSQL version
psql --version

# Check if PostgreSQL is running
brew services list | grep postgresql
# hoặc
pg_isready
```

---

## ✨ Bây Giờ Scripts Hoạt Động!

Tất cả 3 scripts giờ đã fix và hoạt động:

```bash
cd database

# 1. Backup (WORKS! ✅)
./backup_database.sh

# 2. Import products (READY! ✅)
./run_insert_products.sh

# 3. Restore nếu cần (READY! ✅)
./restore_database.sh
```

---

**Ngày sửa**: 9 tháng 10, 2025  
**Trạng thái**: ✅ RESOLVED
