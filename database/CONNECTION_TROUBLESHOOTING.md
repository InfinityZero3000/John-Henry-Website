# 🔧 Hướng dẫn Khắc phục Lỗi Kết nối Database

## ❌ Lỗi thường gặp

### 1. "Connection refused" khi kết nối từ pgAdmin

**Triệu chứng:**
```
connection to server at "127.0.0.1", port 5432 failed: Connection refused
Is the server running on that host and accepting TCP/IP connections?
```

**Nguyên nhân:**
- pgAdmin chạy trong Docker container
- Không thể kết nối đến `localhost` hoặc `127.0.0.1` của máy host

**✅ Giải pháp:**
Trong pgAdmin, **dùng tên container thay vì localhost**:

| Sai ❌ | Đúng ✅ |
|--------|---------|
| `localhost` | `postgres` |
| `127.0.0.1` | `postgres` |

**Cách sửa:**
1. Trong pgAdmin, chuột phải vào server → **Properties**
2. Tab **Connection** → Đổi **Host name/address**: `postgres`
3. Nhấn **Save**

---

### 2. "password authentication failed"

**Triệu chứng:**
```
FATAL: password authentication failed for user "johnhenry_user"
```

**✅ Giải pháp:**
Kiểm tra mật khẩu đúng: `JohnHenry@2025!`

Nếu vẫn lỗi, reset lại containers:
```bash
docker-compose down -v
docker-compose up -d
```

---

### 3. Container không chạy

**Kiểm tra:**
```bash
docker ps --filter "name=johnhenry"
```

**Nếu không thấy containers:**
```bash
# Khởi động lại
docker-compose up -d

# Xem logs nếu có lỗi
docker-compose logs postgres
docker-compose logs pgadmin
```

---

### 4. Port 5432 đã được sử dụng

**Triệu chứng:**
```
Error response from daemon: driver failed programming external connectivity on endpoint johnhenry_postgres: 
Bind for 0.0.0.0:5432 failed: port is already allocated
```

**Nguyên nhân:**
- PostgreSQL khác đang chạy trên port 5432
- Container cũ chưa dừng

**✅ Giải pháp 1: Dừng PostgreSQL cục bộ**
```bash
# macOS với Homebrew
brew services stop postgresql@15
# hoặc
brew services stop postgresql@14

# Linux
sudo systemctl stop postgresql

# Windows
# Vào Services và stop PostgreSQL service
```

**✅ Giải pháp 2: Đổi port trong docker-compose.yml**
```yaml
services:
  postgres:
    ports:
      - "5433:5432"  # Đổi port host thành 5433
```

Sau đó cập nhật connection string trong `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;..."
}
```

---

### 5. Không thể truy cập pgAdmin qua localhost:8080

**Kiểm tra:**
```bash
# Xem pgAdmin có chạy không
docker ps | grep pgadmin

# Xem logs
docker logs johnhenry_pgadmin
```

**✅ Giải pháp:**
```bash
# Khởi động lại pgAdmin
docker-compose restart pgadmin

# Hoặc rebuild
docker-compose up -d --force-recreate pgadmin
```

---

### 6. Database không tồn tại

**Triệu chứng:**
```
FATAL: database "johnhenry_db" does not exist
```

**✅ Giải pháp:**
```bash
# Kết nối vào PostgreSQL
docker exec -it johnhenry_postgres psql -U johnhenry_user -d postgres

# Tạo database
CREATE DATABASE johnhenry_db;

# Thoát
\q
```

Hoặc chạy migrations:
```bash
dotnet ef database update
```

---

## 🔍 Checklist Tổng hợp

Khi gặp lỗi kết nối, kiểm tra theo thứ tự:

### ✅ Bước 1: Containers có chạy không?
```bash
docker ps --filter "name=johnhenry"
```
Phải thấy 2 containers: `johnhenry_postgres` và `johnhenry_pgadmin`

### ✅ Bước 2: Port có bị chiếm không?
```bash
# macOS/Linux
lsof -i :5432
lsof -i :8080

# Windows (PowerShell)
netstat -ano | findstr :5432
netstat -ano | findstr :8080
```

### ✅ Bước 3: Host name đúng chưa?
- **Từ pgAdmin (trong Docker)**: Dùng `postgres`
- **Từ máy host (Terminal, .NET)**: Dùng `localhost`

### ✅ Bước 4: Credentials đúng chưa?
```
Username: johnhenry_user
Password: JohnHenry@2025!
Database: johnhenry_db
```

### ✅ Bước 5: Network có đúng không?
```bash
# Kiểm tra network
docker network ls | grep johnhenry

# Kiểm tra containers trong network
docker network inspect johnhenrywebsite_johnhenry_network
```

---

## 📊 Bảng So sánh Kết nối

| Từ đâu? | Host | Port | Username | Password | Database |
|---------|------|------|----------|----------|----------|
| **pgAdmin (Docker)** | `postgres` | 5432 | johnhenry_user | JohnHenry@2025! | johnhenry_db |
| **Terminal/CLI** | `localhost` | 5432 | johnhenry_user | JohnHenry@2025! | johnhenry_db |
| **.NET Application** | `localhost` | 5432 | johnhenry_user | JohnHenry@2025! | johnhenry_db |
| **DBeaver/DataGrip** | `localhost` | 5432 | johnhenry_user | JohnHenry@2025! | johnhenry_db |

---

## 🧪 Test Kết nối

### Test từ Terminal
```bash
# Test connection
docker exec -it johnhenry_postgres psql -U johnhenry_user -d johnhenry_db -c "SELECT version();"

# Nếu thành công sẽ hiện PostgreSQL version
```

### Test từ .NET Application
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet ef database update
```

### Test từ pgAdmin
1. Mở http://localhost:8080
2. Đăng nhập: `admin@johnhenry.com` / `admin123`
3. Tạo server mới với host: `postgres`

---

## 🚨 Reset Toàn bộ (Last Resort)

Nếu mọi cách đều thất bại:

```bash
# Dừng và xóa tất cả
docker-compose down -v

# Xóa images cũ (optional)
docker rmi postgres:15
docker rmi dpage/pgadmin4:latest

# Pull và khởi động lại
docker-compose pull
docker-compose up -d

# Chờ 15 giây cho containers khởi động
sleep 15

# Chạy migrations
dotnet ef database update
```

---

## 📞 Hỗ trợ Thêm

Nếu vẫn gặp vấn đề, gửi thông tin sau:

1. **Logs của PostgreSQL:**
```bash
docker logs johnhenry_postgres --tail 50
```

2. **Logs của pgAdmin:**
```bash
docker logs johnhenry_pgadmin --tail 50
```

3. **Trạng thái containers:**
```bash
docker ps -a
```

4. **Network info:**
```bash
docker network inspect johnhenrywebsite_johnhenry_network
```

5. **Application logs:**
```bash
tail -n 100 logs/john-henry-$(date +%Y%m%d).txt
```
