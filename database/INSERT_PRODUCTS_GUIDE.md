# Hướng Dẫn Thêm Sản Phẩm Vào Database PostgreSQL

## Tổng Quan

Script này sẽ thêm **903 sản phẩm** từ file CSV vào database PostgreSQL, bao gồm:

### Thống Kê Sản Phẩm:
- **Áo nam**: 323 sản phẩm
- **Áo nữ**: 188 sản phẩm  
- **Quần nam**: 154 sản phẩm
- **Đầm nữ**: 82 sản phẩm
- **Quần nữ**: 65 sản phẩm
- **Phụ kiện nam**: 57 sản phẩm
- **Chân váy nữ**: 27 sản phẩm
- **Phụ kiện nữ**: 7 sản phẩm

### Thống Kê Thương Hiệu:
- **John Henry**: 535 sản phẩm
- **Freelancer**: 368 sản phẩm

### Trạng Thái Hình Ảnh:
- **Có hình ảnh**: 903 sản phẩm (100%)
- **Chưa có ảnh**: 0 sản phẩm (0%)

---

## Các File Đã Tạo

1. **`generate_product_inserts.py`** - Script Python để phân loại và tạo SQL
2. **`insert_products_from_csv.sql`** - File SQL chứa 903 INSERT statements
3. **`PRODUCT_CLASSIFICATION_REPORT.md`** - Báo cáo chi tiết phân loại
4. **`run_insert_products.sh`** - Shell script để chạy SQL vào PostgreSQL

---

## Phương Pháp 1: Sử dụng Shell Script (Khuyến Nghị)

### Bước 1: Cấp quyền thực thi cho script
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database"
chmod +x run_insert_products.sh
```

### Bước 2: Chạy script
```bash
./run_insert_products.sh
```

Script sẽ tự động:
- Đọc connection string từ `appsettings.json`
- Kết nối đến PostgreSQL
- Thực thi file SQL
- Báo cáo kết quả

---

## Phương Pháp 2: Sử dụng psql Trực Tiếp

### Bước 1: Lấy thông tin connection từ appsettings.json
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
cat appsettings.json | grep "DefaultConnection"
```

### Bước 2: Chạy SQL với psql
```bash
psql "Host=localhost;Port=5432;Database=johnhenry_db;Username=postgres;Password=your_password" \
  -f database/insert_products_from_csv.sql
```

Thay thế các giá trị:
- `localhost` - Database host của bạn
- `5432` - Port PostgreSQL
- `johnhenry_db` - Tên database
- `postgres` - Username
- `your_password` - Password

---

## Phương Pháp 3: Sử dụng pgAdmin

### Bước 1: Mở pgAdmin
1. Kết nối đến PostgreSQL server
2. Chọn database `johnhenry_db`
3. Click Tools → Query Tool

### Bước 2: Load và chạy SQL
1. Click File → Open
2. Chọn file `insert_products_from_csv.sql`
3. Click Execute (F5)

---

## Phương Pháp 4: Sử dụng Docker (Nếu chạy trong Docker)

```bash
# Copy file SQL vào container
docker cp database/insert_products_from_csv.sql johnhenry_db:/tmp/

# Chạy SQL trong container
docker exec -i johnhenry_db psql -U postgres -d johnhenry_db -f /tmp/insert_products_from_csv.sql
```

---

## Phương Pháp 5: Sử dụng .NET Migration

### Tạo Migration tùy chỉnh:
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet ef migrations add SeedProductsFromCSV
```

### Chỉnh sửa migration file để chạy SQL:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    var sqlFile = File.ReadAllText("database/insert_products_from_csv.sql");
    migrationBuilder.Sql(sqlFile);
}
```

### Apply migration:
```bash
dotnet ef database update
```

---

## Kiểm Tra Kết Quả

### 1. Kiểm tra số lượng categories:
```sql
SELECT "Name", COUNT(*) 
FROM "Products" p
JOIN "Categories" c ON p."CategoryId" = c."Id"
GROUP BY "Name"
ORDER BY COUNT(*) DESC;
```

Kết quả mong đợi:
```
Áo nam         | 323
Áo nữ          | 188
Quần nam       | 154
Đầm nữ         |  82
Quần nữ        |  65
Phụ kiện nam   |  57
Chân váy nữ    |  27
Phụ kiện nữ    |   7
```

### 2. Kiểm tra số lượng brands:
```sql
SELECT b."Name", COUNT(*) 
FROM "Products" p
JOIN "Brands" b ON p."BrandId" = b."Id"
GROUP BY b."Name"
ORDER BY COUNT(*) DESC;
```

Kết quả mong đợi:
```
John Henry     | 535
Freelancer     | 368
```

### 3. Kiểm tra sản phẩm có hình ảnh:
```sql
SELECT 
    COUNT(*) as total_products,
    SUM(CASE WHEN "FeaturedImageUrl" NOT LIKE '%default%' THEN 1 ELSE 0 END) as has_image,
    SUM(CASE WHEN "FeaturedImageUrl" LIKE '%default%' THEN 1 ELSE 0 END) as no_image
FROM "Products";
```

### 4. Xem danh sách một vài sản phẩm:
```sql
SELECT p."SKU", p."Name", p."Price", c."Name" as Category, b."Name" as Brand
FROM "Products" p
JOIN "Categories" c ON p."CategoryId" = c."Id"
JOIN "Brands" b ON p."BrandId" = b."Id"
LIMIT 10;
```

---

## Xử Lý Lỗi Thường Gặp

### Lỗi: "duplicate key value violates unique constraint"
**Nguyên nhân**: Sản phẩm đã tồn tại trong database

**Giải pháp**: Script đã có `WHERE NOT EXISTS` nên sẽ skip các sản phẩm đã tồn tại. Nếu muốn cập nhật:

```sql
-- Xóa tất cả products hiện tại (CẨN THẬN!)
DELETE FROM "Products";

-- Sau đó chạy lại script insert
```

### Lỗi: "relation 'Categories' does not exist"
**Nguyên nhân**: Bảng chưa được tạo

**Giải pháp**: Chạy migrations trước:
```bash
dotnet ef database update
```

### Lỗi: Connection refused
**Nguyên nhân**: PostgreSQL không chạy hoặc connection string sai

**Giải pháp**: 
1. Kiểm tra PostgreSQL đang chạy: `pg_isready`
2. Kiểm tra connection string trong `appsettings.json`
3. Restart PostgreSQL: `brew services restart postgresql@14`

---

## Rollback (Xóa Dữ Liệu)

Nếu cần xóa tất cả sản phẩm đã import:

```sql
-- Xóa tất cả products
DELETE FROM "Products" 
WHERE "SKU" IN (
    SELECT "SKU" FROM "Products" 
    WHERE "CreatedAt" >= CURRENT_DATE
);

-- Xóa categories không còn products
DELETE FROM "Categories" 
WHERE NOT EXISTS (
    SELECT 1 FROM "Products" WHERE "CategoryId" = "Categories"."Id"
);

-- Xóa brands không còn products
DELETE FROM "Brands" 
WHERE NOT EXISTS (
    SELECT 1 FROM "Products" WHERE "BrandId" = "Brands"."Id"
);
```

---

## Backup Trước Khi Import

**QUAN TRỌNG**: Luôn backup database trước khi import dữ liệu lớn!

```bash
# Backup database
pg_dump -U postgres -d johnhenry_db > backup_before_import_$(date +%Y%m%d).sql

# Restore nếu cần
psql -U postgres -d johnhenry_db < backup_before_import_20251009.sql
```

---

## Liên Hệ & Hỗ Trợ

Nếu gặp vấn đề trong quá trình import, vui lòng kiểm tra:
1. Log files trong thư mục `logs/`
2. PostgreSQL error log
3. Connection string trong `appsettings.json`

---

## Tài Liệu Tham Khảo

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Entity Framework Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [ASP.NET Core Configuration](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
