# Hướng dẫn Cài đặt Database John Henry E-Commerce

## PostgreSQL Database với Docker

Dự án này sử dụng PostgreSQL làm cơ sở dữ liệu chính, được đóng gói trong Docker để dễ dàng cài đặt và triển khai.

## ⚠️ LƯU Ý QUAN TRỌNG - Sửa lỗi Store

Nếu bạn gặp lỗi `PostgresException: 42703: column s0.SocialMedia does not exist` khi truy cập trang seller/store, vui lòng xem:

📄 **[FIX_STORE_ERROR.md](./FIX_STORE_ERROR.md)** - Hướng dẫn chi tiết  
📄 **[ANALYSIS_REPORT.md](./ANALYSIS_REPORT.md)** - Báo cáo phân tích đầy đủ  
💾 **[complete_fix.sql](./complete_fix.sql)** - Script SQL để sửa lỗi  

**Sửa nhanh:**
```bash
# Chạy script SQL trong PostgreSQL
psql -h localhost -U johnhenry_user -d johnhenry_db -f database/complete_fix.sql

# Hoặc sử dụng Python script
python3 database/fix_database.py
```

## Bắt đầu nhanh

### Yêu cầu trước khi cài đặt
- Docker & Docker Compose
- .NET 9.0 SDK

### 1. Khởi động Database
```bash
# Khởi động PostgreSQL và pgAdmin
docker-compose up -d

# Kiểm tra xem containers có đang chạy không
docker-compose ps
```

### 2. Truy cập Database
- **PostgreSQL**: `localhost:5432`
- **pgAdmin**: http://localhost:8080
  - Email: `admin@johnhenry.com`
  - Password: `admin123`

### 3. Thông tin kết nối Database

#### 🔴 QUAN TRỌNG: Kết nối từ pgAdmin
Khi kết nối từ **pgAdmin (trong Docker)**, dùng tên container:
```
Host: postgres
Port: 5432
Database: johnhenry_db
Username: johnhenry_user
Password: JohnHenry@2025!
```

#### Kết nối từ máy host (Terminal, DBeaver, VS Code, v.v.)
Khi kết nối từ **bên ngoài Docker**, dùng localhost:
```
Host: localhost
Port: 5432
Database: johnhenry_db
Username: johnhenry_user
Password: JohnHenry@2025!
```

**Tại sao khác nhau?**
- pgAdmin chạy **TRONG Docker container** → cần dùng tên container `postgres`
- Ứng dụng .NET, terminal chạy **NGOÀI Docker** → dùng `localhost`

### 4. Chạy ứng dụng
```bash
# Khôi phục packages
dotnet restore

# Chạy ứng dụng
dotnet run
```

## Cấu trúc Database

### Bảng chính
- **users** - Tài khoản người dùng và xác thực
- **addresses** - Địa chỉ giao hàng/thanh toán của người dùng
- **categories** - Danh mục sản phẩm (theo cấp bậc)
- **brands** - Thương hiệu sản phẩm
- **products** - Danh mục sản phẩm chính
- **product_variants** - Biến thể sản phẩm (kích thước, màu sắc)
- **product_images** - Thư viện ảnh sản phẩm
- **product_attributes** - Thuộc tính có thể cấu hình
- **shopping_carts** - Sản phẩm trong giỏ hàng
- **wishlists** - Sản phẩm yêu thích của người dùng

### Quản lý Đơn hàng
- **orders** - Thông tin đơn hàng
- **order_items** - Chi tiết sản phẩm trong đơn hàng
- **order_status_history** - Theo dõi trạng thái đơn hàng
- **coupons** - Mã giảm giá
- **coupon_usages** - Theo dõi việc sử dụng mã giảm giá

### Quản lý Nội dung
- **blog_posts** - Bài viết blog
- **blog_categories** - Danh mục blog
- **product_reviews** - Đánh giá và xếp hạng sản phẩm

### Bảng hệ thống
- **settings** - Cấu hình hệ thống
- **email_templates** - Mẫu email
- **audit_logs** - Nhật ký kiểm toán hệ thống

## Biến môi trường

Sao chép `.env.example` thành `.env` và cấu hình:

```env
# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=johnhenry_db
DB_USER=johnhenry_user
DB_PASSWORD=JohnHenry@2025!

# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000

# JWT
JWT_SECRET=your-secret-key
JWT_ISSUER=JohnHenryFashion
JWT_AUDIENCE=JohnHenryUsers
```

## Dữ liệu mẫu

Database bao gồm dữ liệu mẫu:
- **Tài khoản Admin**: `admin@johnhenry.com` / `Admin123!`
- **Tài khoản Seller**: `seller@johnhenry.com` / `Seller123!`
- **Sản phẩm mẫu**: Quần áo nam và nữ
- **Danh mục**: Cấu trúc danh mục hoàn chỉnh
- **Bài viết Blog**: Các bài viết blog mẫu
- **Mã giảm giá**: Mã giảm giá để test

## Lệnh phát triển

```bash
# Dừng database
docker-compose down

# Reset database (CẢNH BÁO: Xóa tất cả dữ liệu)
docker-compose down -v
docker-compose up -d

# Xem log database
docker-compose logs postgres

# Truy cập PostgreSQL CLI
docker exec -it johnhenry_postgres psql -U johnhenry_user -d johnhenry_db

# Sao lưu database
docker exec johnhenry_postgres pg_dump -U johnhenry_user johnhenry_db > backup.sql

# Khôi phục database
docker exec -i johnhenry_postgres psql -U johnhenry_user -d johnhenry_db < backup.sql

# Kết nối trực tiếp với PostgreSQL (không Docker)
psql -h localhost -p 5432 -U johnhenry_user -d johnhenry_db

# Xem danh sách bảng
\dt

# Xem cấu trúc bảng
\d table_name

# Thoát PostgreSQL
\q
```

## Theo dõi Database

### Views hiệu suất
- `product_sales_summary` - Phân tích bán hàng sản phẩm
- `order_summary` - Tóm tắt đơn hàng hàng ngày

### Các truy vấn hữu ích

#### Sản phẩm bán chạy nhất
```sql
SELECT p."Name", COUNT(oi."Id") as total_sold, SUM(oi."Price" * oi."Quantity") as total_revenue
FROM "Products" p
JOIN "OrderItems" oi ON p."Id" = oi."ProductId"
JOIN "Orders" o ON oi."OrderId" = o."Id"
WHERE o."OrderStatus" = 'Delivered'
GROUP BY p."Id", p."Name"
ORDER BY total_sold DESC 
LIMIT 10;
```

#### Đơn hàng gần đây
```sql
SELECT "OrderNumber", "TotalAmount", "OrderStatus", "CreatedAt" 
FROM "Orders" 
ORDER BY "CreatedAt" DESC 
LIMIT 20;
```

#### Hiệu suất danh mục
```sql
SELECT c."Name", COUNT(p."Id") as product_count, 
       AVG(p."Price") as avg_price,
       SUM(CASE WHEN p."IsActive" = true THEN 1 ELSE 0 END) as active_products
FROM "Categories" c
LEFT JOIN "Products" p ON c."Id" = p."CategoryId"
GROUP BY c."Id", c."Name"
ORDER BY product_count DESC;
```

#### Top khách hàng
```sql
SELECT u."FirstName", u."LastName", u."Email", 
       COUNT(o."Id") as total_orders,
       SUM(o."TotalAmount") as total_spent
FROM "AspNetUsers" u
JOIN "Orders" o ON u."Id" = o."UserId"
GROUP BY u."Id", u."FirstName", u."LastName", u."Email"
ORDER BY total_spent DESC
LIMIT 10;
```

#### Thống kê blog
```sql
SELECT bp."Title", bp."ViewCount", bp."CreatedAt",
       COUNT(pr."Id") as review_count
FROM "BlogPosts" bp
LEFT JOIN "ProductReviews" pr ON bp."Id"::text = pr."Content" -- Simplified join
GROUP BY bp."Id", bp."Title", bp."ViewCount", bp."CreatedAt"
ORDER BY bp."ViewCount" DESC
LIMIT 10;
```

#### Báo cáo doanh thu theo tháng
```sql
SELECT 
    DATE_TRUNC('month', "CreatedAt") as month,
    COUNT(*) as total_orders,
    SUM("TotalAmount") as total_revenue,
    AVG("TotalAmount") as avg_order_value
FROM "Orders"
WHERE "OrderStatus" = 'Delivered'
GROUP BY DATE_TRUNC('month', "CreatedAt")
ORDER BY month DESC;
```

## Quản lý dữ liệu thông dụng

### Thêm sản phẩm mới
```sql
INSERT INTO "Products" ("Id", "Name", "Slug", "Description", "Price", "CategoryId", "BrandId", "IsActive", "CreatedAt", "UpdatedAt")
VALUES (
    gen_random_uuid(),
    'Áo sơ mi nam cao cấp',
    'ao-so-mi-nam-cao-cap',
    'Áo sơ mi nam chất liệu cotton cao cấp',
    299000,
    (SELECT "Id" FROM "Categories" WHERE "Name" = 'Áo sơ mi' LIMIT 1),
    (SELECT "Id" FROM "Brands" WHERE "Name" = 'John Henry' LIMIT 1),
    true,
    NOW(),
    NOW()
);
```

### Thêm danh mục mới
```sql
INSERT INTO "Categories" ("Id", "Name", "Slug", "Description", "IsActive", "CreatedAt", "UpdatedAt")
VALUES (
    gen_random_uuid(),
    'Áo khoác mùa đông',
    'ao-khoac-mua-dong',
    'Bộ sưu tập áo khoác mùa đông',
    true,
    NOW(),
    NOW()
);
```

### Cập nhật giá sản phẩm
```sql
UPDATE "Products" 
SET "Price" = 350000, "UpdatedAt" = NOW()
WHERE "Name" LIKE '%Áo sơ mi%';
```

### Xóa sản phẩm (soft delete)
```sql
UPDATE "Products" 
SET "IsActive" = false, "UpdatedAt" = NOW()
WHERE "Id" = 'product-id-here';
```

## Cân nhắc bảo mật

1. **Bảo mật mật khẩu**: Sử dụng BCrypt để mã hóa mật khẩu
2. **JWT Tokens**: Xác thực bảo mật với thời gian hết hạn có thể cấu hình
3. **SQL Injection**: Sử dụng parameterized queries thông qua Entity Framework
4. **Validation đầu vào**: FluentValidation để xác thực dữ liệu
5. **Audit Trail**: Ghi log kiểm toán hoàn chỉnh cho các hoạt động nhạy cảm

## Triển khai Production

Đối với môi trường production:
1. Sử dụng mật khẩu mạnh
2. Cấu hình SSL/TLS
3. Thiết lập sao lưu database định kỳ
4. Cấu hình monitoring
5. Sử dụng connection strings riêng cho từng môi trường

## Bảo trì Database định kỳ

### Sao lưu tự động
```bash
# Tạo script backup hàng ngày
#!/bin/bash
DATE=$(date +%Y%m%d_%H%M%S)
pg_dump -h localhost -U johnhenry_user johnhenry_db > /backup/johnhenry_db_$DATE.sql
```

### Dọn dọn dữ liệu cũ
```sql
-- Xóa logs cũ hơn 30 ngày
DELETE FROM "SecurityLogs" WHERE "CreatedAt" < NOW() - INTERVAL '30 days';

-- Xóa sessions hết hạn
DELETE FROM "ActiveSessions" WHERE "ExpiresAt" < NOW();
```

### Tối ưu hóa performance
```sql
-- Rebuild indexes
REINDEX DATABASE johnhenry_db;

-- Analyze tables
ANALYZE;

-- Vacuum tables
VACUUM;
```

## Hỗ trợ

Khi gặp sự cố database:
1. Kiểm tra logs của Docker container
2. Xác minh connection strings
3. Đảm bảo PostgreSQL đang chạy
4. Kiểm tra cài đặt firewall
5. Kiểm tra file `.env` có đúng cấu hình không

### Các lệnh debug phổ biến
```bash
# Kiểm tra trạng thái PostgreSQL
brew services list | grep postgresql

# Khởi động PostgreSQL nếu chưa chạy
brew services start postgresql@14

# Kiểm tra kết nối
psql -h localhost -p 5432 -U johnhenry_user -d johnhenry_db -c "SELECT 1;"

# Xem logs ứng dụng
tail -f logs/john-henry-$(date +%Y%m%d).txt
```
