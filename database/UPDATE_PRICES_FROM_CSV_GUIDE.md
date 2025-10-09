# 📝 HƯỚNG DẪN CẬP NHẬT GIÁ SẢN PHẨM TỪ FILE CSV

## 📋 Tổng Quan

Script này cập nhật giá cho **180 sản phẩm** dựa trên file `matched_products.csv`. Các sản phẩm không có trong danh sách sẽ **giữ nguyên giá**.

**File SQL**: `/database/update_prices_from_csv.sql`

---

## 📊 Phân Tích Giá Mới

### Thống kê giá theo loại sản phẩm:

| Loại Sản Phẩm | Số Lượng | Giá Thấp Nhất | Giá Cao Nhất |
|---------------|----------|---------------|--------------|
| **Áo Khoác Nam** | 5 | 1.100.000₫ | 1.500.000₫ |
| **Áo Len Nam** | 2 | 800.000₫ | 800.000₫ |
| **Áo Polo Nam** | 13 | 550.000₫ | 980.000₫ |
| **Áo Polo Nữ** | 4 | 399.000₫ | 450.000₫ |
| **Áo Sơ Mi Nam** | 11 | 600.000₫ | 900.000₫ |
| **Áo Sơ Mi Nữ** | 5 | 399.000₫ | 449.000₫ |
| **Áo Thun Nam** | 5 | 420.000₫ | 800.000₫ |
| **Áo Thun Nữ** | 11 | 149.000₫ | 650.000₫ |
| **Áo Blouse Nữ** | 5 | 399.000₫ | 549.000₫ |
| **Chân Váy** | 24 | 329.000₫ | 599.000₫ |
| **Đầm Nữ** | 30 | 549.000₫ | 699.000₫ |
| **Quần Tây Nam** | 3 | 800.000₫ | 850.000₫ |
| **Quần Tây Nữ** | 11 | 399.000₫ | 599.000₫ |
| **Quần Jeans Nam** | 8 | 900.000₫ | 900.000₫ |
| **Quần Jeans Nữ** | 10 | 599.000₫ | 699.000₫ |
| **Quần Khaki Nam** | 5 | 800.000₫ | 850.000₫ |
| **Quần Short Nam** | 7 | 490.000₫ | 600.000₫ |
| **Quần Short Nữ** | 5 | 349.000₫ | 369.000₫ |
| **Thắt Lưng Nam** | 5 | 600.000₫ | 600.000₫ |
| **Thắt Lưng Nữ** | 2 | 799.000₫ | 799.000₫ |
| **Ví Nam** | 7 | 400.000₫ | 400.000₫ |
| **Giày Dép Nam** | 3 | 250.000₫ | 1.200.000₫ |
| **Mũ Nam** | 4 | 299.000₫ | 299.000₫ |
| **Túi Xách Nữ** | 2 | 1.249.000₫ | 1.249.000₫ |
| **Mắt Kính Nữ** | 6 | 329.000₫ | 418.000₫ |

**Tổng: 180 sản phẩm**

---

## 🚀 CÁCH 1: CHẠY SCRIPT QUA SQL SERVER MANAGEMENT STUDIO (SSMS)

### Bước 1: Mở SSMS
1. Khởi động **SQL Server Management Studio**
2. Connect đến SQL Server instance của bạn

### Bước 2: Chọn Database
```sql
USE JohnHenryFashionWeb;
GO
```

### Bước 3: Backup Database (Khuyến nghị)
```sql
-- Backup trước khi cập nhật
BACKUP DATABASE JohnHenryFashionWeb 
TO DISK = 'C:\Backup\JohnHenryFashionWeb_BeforePriceUpdate.bak'
WITH FORMAT, INIT, NAME = 'Backup Before Price Update';
```

### Bước 4: Mở và Chạy Script
1. File → Open → File
2. Chọn: `database/update_prices_from_csv.sql`
3. Nhấn **Execute** (F5)

### Bước 5: Kiểm Tra Kết Quả
Script sẽ tự động hiển thị:
- Số sản phẩm đã cập nhật
- 20 sản phẩm vừa cập nhật gần nhất
- Thống kê giá theo danh mục
- Sản phẩm giá cao nhất (>= 1 triệu)
- Sản phẩm giá thấp nhất (<= 300k)

---

## 🚀 CÁCH 2: CHẠY SCRIPT QUA COMMAND LINE (sqlcmd)

### Bước 1: Tìm Connection String
Kiểm tra file `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=JohnHenryFashionWeb;..."
  }
}
```

### Bước 2: Chạy sqlcmd
```bash
# Trên Windows
sqlcmd -S YOUR_SERVER_NAME -d JohnHenryFashionWeb -E -i "database/update_prices_from_csv.sql"

# Trên macOS/Linux với SQL Server
sqlcmd -S YOUR_SERVER_NAME -U sa -P YOUR_PASSWORD -d JohnHenryFashionWeb -i "database/update_prices_from_csv.sql"
```

**Giải thích tham số:**
- `-S`: Server name
- `-d`: Database name
- `-E`: Windows Authentication (hoặc dùng `-U` và `-P` cho SQL Authentication)
- `-i`: Input file (script SQL)

---

## 🚀 CÁCH 3: CHẠY SCRIPT QUA VS CODE (SQLTools Extension)

### Bước 1: Cài Extension
1. Mở VS Code
2. Extensions → Search "SQLTools"
3. Cài: **SQLTools** + **SQLTools SQL Server Driver**

### Bước 2: Kết Nối Database
1. Click biểu tượng SQLTools trên thanh bên
2. Add New Connection
3. Điền thông tin từ `appsettings.json`
4. Test Connection → Save

### Bước 3: Chạy Script
1. Mở file `database/update_prices_from_csv.sql`
2. Right-click → Run on Active Connection
3. Hoặc: Ctrl+E Ctrl+E

---

## 🚀 CÁCH 4: CHẠY TỪ CODE C# (Entity Framework)

Tạo migration mới để chạy script:

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet ef migrations add UpdateProductPricesFromCSV
```

Thêm code vào migration:

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Đọc file SQL
    var sqlScript = File.ReadAllText("database/update_prices_from_csv.sql");
    
    // Chạy script
    migrationBuilder.Sql(sqlScript);
}
```

Sau đó chạy migration:

```bash
dotnet ef database update
```

---

## ✅ KIỂM TRA SAU KHI CẬP NHẬT

### 1. Kiểm tra trong Database

```sql
-- Xem 10 sản phẩm có giá cao nhất
SELECT TOP 10
    Name,
    Price,
    SalePrice
FROM Products
WHERE IsActive = 1
ORDER BY Price DESC;

-- Xem 10 sản phẩm có giá thấp nhất
SELECT TOP 10
    Name,
    Price,
    SalePrice
FROM Products
WHERE IsActive = 1
ORDER BY Price ASC;

-- Kiểm tra các sản phẩm vẫn giữ giá 500k (không có trong CSV)
SELECT 
    Name,
    Price
FROM Products
WHERE Price = 500000 AND IsActive = 1;
```

### 2. Restart Ứng Dụng

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"

# Dừng ứng dụng đang chạy (Ctrl+C trong terminal)
# Hoặc kill process
pkill -f "dotnet.*JohnHenryFashionWeb"

# Chạy lại
dotnet run
```

### 3. Kiểm Tra Trên Trình Duyệt

#### Trang John Henry (Nam):
```
http://localhost:5000/Home/JohnHenry
```
**Kỳ vọng:**
- Áo Polo: 550k - 980k
- Áo Sơ Mi: 600k - 900k
- Quần: 800k - 900k

#### Trang Freelancer (Nữ):
```
http://localhost:5000/Home/Freelancer
```
**Kỳ vọng:**
- Áo: 149k - 650k
- Chân Váy: 329k - 599k
- Đầm: 549k - 699k
- Quần: 349k - 699k

#### Các Trang Sub:
```
http://localhost:5000/Home/JohnHenryShirt       → 600k-900k
http://localhost:5000/Home/JohnHenryTrousers    → 800k-900k
http://localhost:5000/Home/JohnHenryAccessories → 250k-1,249k
http://localhost:5000/Home/FreelancerDress      → 549k-699k
http://localhost:5000/Home/FreelancerShirt      → 399k-650k
http://localhost:5000/Home/FreelancerTrousers   → 399k-699k
http://localhost:5000/Home/FreelancerSkirt      → 329k-599k
http://localhost:5000/Home/FreelancerAccessories → 149k-1,249k
```

---

## 🔍 LOGIC CỦA SCRIPT

### 1. Transaction Safety
```sql
BEGIN TRANSACTION;
-- ... các UPDATE statements ...
COMMIT TRANSACTION;
```
- Nếu có lỗi → tự động ROLLBACK
- Đảm bảo tính toàn vẹn dữ liệu

### 2. Conditional Update
```sql
UPDATE Products 
SET Price = 1500000 
WHERE Name = 'Áo Khoác Nam Cá Tính JK25FH04C-PA' 
  AND Price = 500000;  -- Chỉ update nếu giá hiện tại là 500k
```
- Chỉ cập nhật sản phẩm có giá cũ = 500,000₫
- Tránh cập nhật nhầm sản phẩm đã được điều chỉnh giá

### 3. Update by Name Matching
```sql
WHERE Name = 'Exact Product Name from CSV'
```
- Khớp chính xác tên sản phẩm
- 180 sản phẩm được cập nhật
- Các sản phẩm khác không bị ảnh hưởng

---

## 📊 DỮ LIỆU MẪU SAU KHI CẬP NHẬT

### John Henry Collection (Nam):

**Áo Khoác:**
- Áo Khoác Nam Cá Tính JK25FH04C-PA: **1.500.000₫**
- Áo Khoác Nam Cá Tính JK25FH09P-KA: **1.100.000₫**

**Áo Polo:**
- Áo Polo Nam KS25FH43C-SCCA: **700.000₫**
- Áo Polo Nam KS25SS08C-SCHE: **980.000₫**

**Áo Sơ Mi:**
- Áo Sơ Mi Nam WS25FH63P-LC: **800.000₫**
- Áo Sơ Mi Nam WS25FH78P-CL: **600.000₫**

**Quần:**
- Quần Jeans Nam JN25FH38C-RG: **900.000₫**
- Quần Khaki Nam KP25FH18C-NMSC: **850.000₫**
- Quần Tây Nam DP25FH10C-NMRG: **850.000₫**

**Phụ Kiện:**
- Giày Tây Nam SO25FH15P-DS: **1.200.000₫**
- Thắt Lưng Nam BE25FH45-HL: **600.000₫**
- Ví Nam WT25FH08-HZ: **400.000₫**
- Mũ Lưỡi Trai CA26SS06P: **299.000₫**
- Dép Quai Ngang SO25FH07P-SA: **250.000₫**

### Freelancer Collection (Nữ):

**Đầm:**
- Đầm Trắng Dài Xếp Ly FWDR25SS22C: **699.000₫**
- Đầm Dáng Suông Cổ Polo FWDR25SS26G: **549.000₫**

**Chân Váy:**
- Chân Váy Maxi Hoa FWSK23SS05G: **599.000₫**
- Chân Váy Ngắn Bất Đối Xứng FWSK25SS12C: **349.000₫**

**Áo:**
- Áo Thun Dệt Kim FWTS25FH02C: **650.000₫**
- Áo Blouse Freelancer FWBL25SS03C: **549.000₫**
- Áo Sơ Mi Nữ FWWS25SS03C: **449.000₫**
- Áo Tanktop FWTT25SS01G: **149.000₫**

**Quần:**
- Quần Jeans Cargo FWJN24FH03G: **699.000₫**
- Quần Tây Xám Ống Rộng FWDP25SS01C: **599.000₫**
- Quần Short Nữ FWSP24SS01C: **349.000₫**

**Phụ Kiện:**
- Túi Tote Nữ FWBA24SS01: **1.249.000₫**
- Thắt Lưng Nữ FWBE23SS01: **799.000₫**
- Mắt Kính Nữ FWSG23SS01G: **418.000₫**

---

## ⚠️ LƯU Ý QUAN TRỌNG

### 1. Backup Trước Khi Chạy
```sql
-- Tạo backup
BACKUP DATABASE JohnHenryFashionWeb 
TO DISK = 'path/to/backup.bak';
```

### 2. Test Trên Development Database
- Chạy script trên database test trước
- Kiểm tra kết quả
- Sau đó mới chạy trên production

### 3. Kiểm Tra Connection String
- Đảm bảo kết nối đúng database
- Không nhầm lẫn giữa Dev/Test/Production

### 4. Rollback Nếu Có Lỗi
```sql
-- Nếu có vấn đề, rollback:
ROLLBACK TRANSACTION;

-- Restore từ backup:
RESTORE DATABASE JohnHenryFashionWeb 
FROM DISK = 'path/to/backup.bak' 
WITH REPLACE;
```

---

## 🎯 KẾT QUẢ MONG ĐỢI

### Trước Khi Cập Nhật:
- ❌ Tất cả sản phẩm: **450.000₫** hoặc **500.000₫**
- ❌ Không có sự đa dạng về giá
- ❌ Không phản ánh giá trị thật của sản phẩm

### Sau Khi Cập Nhật:
- ✅ 180 sản phẩm có giá mới từ CSV
- ✅ Giá đa dạng: **149.000₫ - 1.500.000₫**
- ✅ Phản ánh đúng giá trị sản phẩm
- ✅ Các sản phẩm không trong CSV giữ nguyên giá

---

## 📞 HỖ TRỢ

### Nếu Gặp Lỗi:

**Lỗi Connection:**
```
Cannot open database "JohnHenryFashionWeb" requested by the login.
```
→ Kiểm tra connection string trong `appsettings.json`

**Lỗi Permission:**
```
The UPDATE permission was denied on the object 'Products'
```
→ Cấp quyền UPDATE cho user:
```sql
GRANT UPDATE ON Products TO [your_user];
```

**Lỗi Transaction:**
```
Transaction count after EXECUTE indicates a mismatching number of BEGIN and COMMIT statements.
```
→ Chạy lại script, đảm bảo không có lỗi syntax

### Kiểm Tra Log:
```sql
-- Xem các thay đổi gần đây
SELECT TOP 100 *
FROM Products
WHERE UpdatedAt >= DATEADD(HOUR, -1, GETDATE())
ORDER BY UpdatedAt DESC;
```

---

## ✅ CHECKLIST HOÀN THÀNH

- [ ] Backup database
- [ ] Kiểm tra connection string
- [ ] Chạy script `update_prices_from_csv.sql`
- [ ] Xác nhận số sản phẩm cập nhật = 180
- [ ] Kiểm tra query SELECT trong script
- [ ] Restart ứng dụng web
- [ ] Test trang JohnHenry - giá đa dạng
- [ ] Test trang Freelancer - giá đa dạng
- [ ] Test 8 trang sub-category
- [ ] Xác nhận giá hiển thị đúng format (1.500.000₫)
- [ ] Kiểm tra các sản phẩm không trong CSV vẫn giữ giá cũ

---

**Thời gian ước tính:** 10-15 phút
**File tham khảo:** 
- Script SQL: `/database/update_prices_from_csv.sql`
- CSV nguồn: `/database/matched_products.csv`
- Diagnosis: `/PRICE_ISSUE_DIAGNOSIS.md`
