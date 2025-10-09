# ⚡ Quick Start - Import Sản Phẩm

## 🎯 Mục Tiêu
Import **903 sản phẩm** từ CSV vào PostgreSQL database

---

## 📋 Các Bước Thực Hiện

### **Bước 1: Backup Database (BẮT BUỘC!)**
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database"
chmod +x backup_database.sh
./backup_database.sh
```
✅ **Tạo file backup trong `backups/` để có thể rollback nếu lỗi**

---

### **Bước 2: Import Sản Phẩm**
```bash
chmod +x run_insert_products.sh
./run_insert_products.sh
```
✅ **Import 903 sản phẩm + 8 categories + 2 brands vào database**

---

### **Bước 3: Kiểm Tra Kết Quả**
```bash
psql -U postgres -d johnhenry_db -c "
SELECT b.\"Name\" as Brand, c.\"Name\" as Category, COUNT(*) as Total
FROM \"Products\" p
JOIN \"Brands\" b ON p.\"BrandId\" = b.\"Id\"
JOIN \"Categories\" c ON p.\"CategoryId\" = c.\"Id\"
GROUP BY b.\"Name\", c.\"Name\"
ORDER BY b.\"Name\", Total DESC;
"
```

**Kết quả mong đợi:**
```
Brand       | Category      | Total
------------|---------------|------
Freelancer  | Áo nữ         | 188
Freelancer  | Đầm nữ        | 82
Freelancer  | Quần nữ       | 65
...
John Henry  | Áo nam        | 323
John Henry  | Quần nam      | 154
John Henry  | Phụ kiện nam  | 57
...
```

---

## 🔄 Nếu Cần Rollback

```bash
./restore_database.sh
# Chọn backup file gần nhất
# Gõ YES để xác nhận
```

---

## 📊 Thống Kê Import

- **Tổng sản phẩm**: 903
- **John Henry**: 535 sản phẩm
- **Freelancer**: 368 sản phẩm
- **Categories**: 8 (Áo nam, Áo nữ, Quần nam, Quần nữ, Chân váy, Đầm, Phụ kiện)
- **Tỷ lệ có ảnh**: 100%

---

## 📚 Tài Liệu Chi Tiết

- `BACKUP_RESTORE_GUIDE.md` - Hướng dẫn backup/restore chi tiết
- `INSERT_PRODUCTS_GUIDE.md` - 5 phương pháp import khác nhau
- `DATABASE_INTEGRATION_SUMMARY.md` - Tổng quan toàn bộ dự án

---

## 🆘 Lỗi Thường Gặp

### Lỗi: "permission denied"
```bash
chmod +x *.sh
```

### Lỗi: "command not found: psql"
```bash
brew install postgresql
```

### Lỗi: "database does not exist"
```bash
# Tạo database
createdb -U postgres johnhenry_db

# Hoặc chạy migrations
cd ..
dotnet ef database update
```

---

**⚡ Chỉ cần 3 lệnh:**
```bash
./backup_database.sh      # 1. Backup
./run_insert_products.sh  # 2. Import
# 3. Check website!
```
