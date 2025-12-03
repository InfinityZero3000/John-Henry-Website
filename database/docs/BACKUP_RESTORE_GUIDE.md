# Hướng Dẫn Backup và Restore Database

## Backup Database Là Gì?

**Backup database** = Sao lưu toàn bộ dữ liệu hiện có trong PostgreSQL thành file SQL.

### File backup chứa gì?
```
backup_johnhenry_db_20251009_143022.sql
│
├── Schema (Cấu trúc)
│   ├── CREATE TABLE statements
│   ├── Constraints, Indexes
│   └── Foreign Keys
│
├── Data (Dữ liệu)
│   ├── INSERT INTO "Products" ...
│   ├── INSERT INTO "Orders" ...
│   ├── INSERT INTO "AspNetUsers" ...
│   └── Tất cả dữ liệu trong mọi bảng
│
└── Functions & Sequences
    ├── Stored procedures
    └── Auto-increment counters
```

### Ví dụ nội dung file backup:
```sql
--
-- PostgreSQL database dump
--

CREATE TABLE "Products" (
    "Id" uuid NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Price" numeric(10,2) NOT NULL,
    ...
);

INSERT INTO "Products" VALUES 
    ('uuid-1', 'Áo Sơ Mi Nam', 599000, ...),
    ('uuid-2', 'Quần Jean Nam', 799000, ...),
    ...
```

---

## Tại Sao Cần Backup?

### Trước khi import 903 sản phẩm mới:
- Nếu import lỗi → Restore lại database cũ
- Nếu duplicate data → Xóa và thử lại
- Nếu muốn so sánh trước/sau

### Trong quá trình phát triển:
- Test tính năng mới → Rollback nếu lỗi
- Migrate schema → Backup trước khi đổi
- Update code → Đảm bảo có dữ liệu safe

### Production (Vận hành):
- Backup định kỳ hàng ngày
- Trước mỗi deployment
- Trước khi update critical data

---

## Cách Sử Dụng Scripts

### **1. Backup Database (Tạo bản sao lưu)**

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database"

# Cấp quyền chạy (chỉ làm 1 lần)
chmod +x backup_database.sh

# Chạy backup
./backup_database.sh
```

**Output:**
```
==========================================
PostgreSQL Database Backup Tool
==========================================

Reading database connection...
✓ Database: johnhenry_db
✓ Host: localhost:5432

Creating backup...
   File: backup_johnhenry_db_20251009_143022.sql

Backup completed successfully!

Backup Information:
   File: backup_johnhenry_db_20251009_143022.sql
   Size: 2.5M
   Path: /Users/.../database/backups/backup_johnhenry_db_20251009_143022.sql

Database Statistics:
   Products: 150
   Categories: 10
   Brands: 2
   Orders: 45
   Users: 23

To restore this backup:
   psql -h localhost -p 5432 -U postgres -d johnhenry_db < backup_johnhenry_db_20251009_143022.sql
```

**Kết quả:**
- File backup được lưu trong thư mục `database/backups/`
- Tên file có timestamp: `backup_johnhenry_db_YYYYMMDD_HHMMSS.sql`
- Có thể có nhiều backup files

---

### **2. Restore Database (Khôi phục từ backup)**

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database"

# Cấp quyền chạy (chỉ làm 1 lần)
chmod +x restore_database.sh

# Chạy restore
./restore_database.sh
```

**Output:**
```
==========================================
PostgreSQL Database Restore Tool
==========================================

Available backups:

   [1] backup_johnhenry_db_20251009_143022.sql (2.5M)
   [2] backup_johnhenry_db_20251009_120000.sql (2.3M)
   [3] backup_johnhenry_db_20251008_180000.sql (2.1M)

Select backup number to restore (or 0 to cancel): 1

Selected backup: backup_johnhenry_db_20251009_143022.sql

Target Database:
   Database: johnhenry_db
   Host: localhost:5432

 WARNING: This will DELETE all current data and restore from backup!

Are you sure? Type 'YES' to confirm: YES

Restoring database...

1. Dropping existing tables...
2. Restoring from backup...

Restore completed successfully!

Restored Database Statistics:
   Products: 150
   Categories: 10
   Brands: 2
   Orders: 45
```

**CẢNH BÁO:** 
- Restore sẽ **XÓA TẤT CẢ** dữ liệu hiện tại
- Phải gõ chính xác `YES` để xác nhận
- Không thể undo sau khi restore!

---

## Quy Trình Khuyến Nghị

### **Trước khi import 903 sản phẩm:**

```bash
# Bước 1: Backup database hiện tại
cd database
./backup_database.sh

# Bước 2: Chạy import
./run_insert_products.sh

# Bước 3: Kiểm tra kết quả
# Nếu OK → Xong!
# Nếu có vấn đề → Restore backup ở bước 1
```

### **Nếu import bị lỗi hoặc muốn làm lại:**

```bash
# Restore về trạng thái ban đầu
./restore_database.sh
# → Chọn backup file gần nhất
# → Gõ YES để xác nhận

# Import lại
./run_insert_products.sh
```

---

## Sử Dụng Nâng Cao

### **Backup bằng command line:**
```bash
# Backup toàn bộ database
pg_dump -U postgres -h localhost -d johnhenry_db \
  > backup_manual_$(date +%Y%m%d).sql

# Backup chỉ schema (không có data)
pg_dump -U postgres -h localhost -d johnhenry_db --schema-only \
  > schema_only.sql

# Backup chỉ data (không có schema)
pg_dump -U postgres -h localhost -d johnhenry_db --data-only \
  > data_only.sql

# Backup chỉ 1 bảng cụ thể
pg_dump -U postgres -h localhost -d johnhenry_db -t "Products" \
  > products_backup.sql
```

### **Restore bằng command line:**
```bash
# Restore toàn bộ
psql -U postgres -h localhost -d johnhenry_db < backup_20251009.sql

# Restore chỉ 1 bảng
psql -U postgres -h localhost -d johnhenry_db < products_backup.sql
```

---

## Quản Lý Backup Files

### **Vị trí lưu backup:**
```
database/
└── backups/
    ├── backup_johnhenry_db_20251009_143022.sql  (2.5M)
    ├── backup_johnhenry_db_20251009_120000.sql  (2.3M)
    ├── backup_johnhenry_db_20251008_180000.sql  (2.1M)
    └── ...
```

### **Xem danh sách backups:**
```bash
cd database/backups
ls -lh *.sql
```

### **Xóa backup cũ (để tiết kiệm dung lượng):**
```bash
# Xóa backups cũ hơn 7 ngày
find database/backups -name "*.sql" -mtime +7 -delete

# Chỉ giữ lại 5 backups gần nhất
cd database/backups
ls -t *.sql | tail -n +6 | xargs rm
```

---

## So Sánh Trước/Sau Import

### **Kiểm tra trước khi import:**
```bash
psql -U postgres -d johnhenry_db -c "SELECT COUNT(*) FROM \"Products\";"
# Output: 150 (ví dụ)
```

### **Import 903 sản phẩm mới:**
```bash
./run_insert_products.sh
```

### **Kiểm tra sau khi import:**
```bash
psql -U postgres -d johnhenry_db -c "SELECT COUNT(*) FROM \"Products\";"
# Output: 1053 (150 cũ + 903 mới)
```

### **Nếu muốn rollback:**
```bash
./restore_database.sh
# Chọn backup trước khi import
```

---

## Lưu Ý Quan Trọng

### **NÊN:**
- Backup trước mỗi lần thay đổi lớn
- Lưu backup ở nhiều nơi (local + cloud)
- Đặt tên backup rõ ràng với timestamp
- Test restore định kỳ để đảm bảo backup hoạt động

### **KHÔNG NÊN:**
- Không backup trên production server
- Không lưu backup trong git repository (file quá lớn)
- Không share backup file có chứa password
- Không xóa tất cả backup (giữ ít nhất 3 bản)

---

## Troubleshooting

### **Lỗi: "permission denied"**
```bash
chmod +x backup_database.sh
chmod +x restore_database.sh
```

### **Lỗi: "pg_dump: command not found"**
```bash
# MacOS
brew install postgresql

# Ubuntu/Debian
sudo apt-get install postgresql-client
```

### **Lỗi: "password authentication failed"**
- Kiểm tra connection string trong `appsettings.json`
- Đảm bảo PostgreSQL đang chạy: `pg_isready`

### **File backup quá lớn (>100MB)**
```bash
# Nén backup file
gzip backup_johnhenry_db_20251009.sql
# → Tạo file .sql.gz (nhỏ hơn 5-10 lần)

# Restore từ file nén
gunzip -c backup_johnhenry_db_20251009.sql.gz | psql -U postgres -d johnhenry_db
```

---

## Tóm Tắt Commands

```bash
# Backup
cd database
./backup_database.sh

# Restore
./restore_database.sh

# Import sản phẩm mới
./run_insert_products.sh

# Xem backups
ls -lh backups/*.sql

# Manual backup
pg_dump -U postgres -d johnhenry_db > backup.sql

# Manual restore
psql -U postgres -d johnhenry_db < backup.sql
```

---

**Ngày cập nhật:** 9 tháng 10, 2025
