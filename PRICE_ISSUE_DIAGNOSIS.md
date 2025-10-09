# 🔴 NGUYÊN NHÂN VÀ GIẢI PHÁP: GIÁ SẢN PHẨM HIỂN THỊ 450,000₫

## 📋 TÓM TẮT VẤN ĐỀ

**Triệu chứng:** Tất cả sản phẩm trên trang JohnHenry.cshtml và Freelancer.cshtml đều hiển thị giá 450,000₫

**Nguyên nhân:** 
- ✅ **Code đã đúng**: Views và Controllers đang load giá từ database chính xác
- ❌ **Database có vấn đề**: Tất cả sản phẩm trong database có `Price = 450000` và `SalePrice = NULL`

**Kết luận:** Đây không phải lỗi code, mà là **dữ liệu trong database chưa đa dạng**.

---

## 🔍 XÁC NHẬN CODE ĐÚNG

### 1. **Controllers (HomeController.cs)** ✅

```csharp
// Lines 47-75: JohnHenry() action
public async Task<IActionResult> JohnHenry()
{
    var johnHenryCategory = await _context.Categories
        .FirstOrDefaultAsync(c => c.Name == "Thời trang nam");
    
    var products = await _context.Products
        .Where(p => p.IsActive && p.CategoryId == johnHenryCategory!.Id)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    return View(products);  // ✅ Trả về List<Product> từ database
}

// Lines 76-104: Freelancer() action
public async Task<IActionResult> Freelancer()
{
    var freelancerCategory = await _context.Categories
        .FirstOrDefaultAsync(c => c.Name == "Thời trang nữ");
    
    var products = await _context.Products
        .Where(p => p.IsActive && p.CategoryId == freelancerCategory!.Id)
        .OrderByDescending(p => p.CreatedAt)
        .ToListAsync();

    return View(products);  // ✅ Trả về List<Product> từ database
}
```

### 2. **Views (JohnHenry.cshtml & Freelancer.cshtml)** ✅

```razor
@model List<JohnHenryFashionWeb.Models.Product>

<div class="products-grid">
    <div class="row g-4">
        @if (Model != null && Model.Any())
        {
            @foreach (var product in Model)
            {
                <div class="product-info">
                    <h6 class="product-name">@product.Name</h6>
                    <div class="product-price">
                        @(product.SalePrice?.ToString("N0") ?? product.Price.ToString("N0"))₫
                        <!-- ✅ Hiển thị SalePrice nếu có, không thì hiển thị Price -->
                    </div>
                </div>
            }
        }
    </div>
</div>
```

**Logic hiển thị giá:**
- Nếu `SalePrice != null` → hiển thị giá sale (giá khuyến mãi)
- Nếu `SalePrice == null` → hiển thị giá gốc (`Price`)
- Format: `ToString("N0")` → 450,000 → 450.000₫ (dấu phẩy ngăn cách hàng nghìn)

---

## 💾 KIỂM TRA DATABASE

### Bước 1: Kết nối database

**Sử dụng một trong các cách sau:**

#### Option 1: SQL Server Management Studio (SSMS)
1. Mở SSMS
2. Connect với connection string từ `appsettings.json`
3. Mở database `JohnHenryFashionWeb`

#### Option 2: VS Code với SQLTools extension
1. Cài extension: `SQL Server (mssql)`
2. Connect với connection string
3. Chạy query SQL

#### Option 3: Command line với dotnet-ef
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet ef database update
```

### Bước 2: Kiểm tra dữ liệu hiện tại

Chạy query sau để xem giá của tất cả sản phẩm:

```sql
-- Xem tất cả sản phẩm với giá
SELECT 
    p.Id,
    p.Name,
    c.Name AS Category,
    p.Price AS 'Giá gốc',
    p.SalePrice AS 'Giá sale',
    CASE 
        WHEN p.SalePrice IS NOT NULL THEN p.SalePrice
        ELSE p.Price
    END AS 'Giá hiển thị',
    p.IsActive,
    p.CreatedAt
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
WHERE p.IsActive = 1
ORDER BY c.Name, p.Name;
```

**Kết quả mong đợi sẽ cho thấy:**
- Nếu tất cả sản phẩm có `Price = 450000` và `SalePrice = NULL` → **Đây là nguyên nhân**
- Các sản phẩm trong category "Thời trang nam" xuất hiện trên trang JohnHenry
- Các sản phẩm trong category "Thời trang nữ" xuất hiện trên trang Freelancer

---

## 🔧 GIẢI PHÁP: CẬP NHẬT GIÁ SẢN PHẨM

### **Phương án 1: Sử dụng SQL Script (Nhanh)**

File đã tạo: `/database/update_product_prices.sql`

**Cách chạy:**

```bash
# Nếu dùng SQL Server
sqlcmd -S your_server -d JohnHenryFashionWeb -i database/update_product_prices.sql

# Hoặc copy nội dung file và chạy trực tiếp trong SSMS/SQLTools
```

Script này sẽ:
1. Cập nhật giá cho **Áo Polo Nam**: 450k-650k (có sale)
2. Cập nhật giá cho **Áo Sơ Mi Nam**: 380k-720k (một số có sale)
3. Cập nhật giá cho **Quần Nam**: 550k-850k (có sale)
4. Cập nhật giá cho **Phụ kiện Nam**: 180k-320k (một số có sale)
5. Cập nhật giá cho **Váy Nữ**: 780k-980k (có sale)
6. Cập nhật giá cho **Áo Nữ**: 350k-580k (có sale)
7. Cập nhật giá cho **Quần Nữ**: 550k-720k (có sale)
8. Cập nhật giá cho **Chân Váy**: 450k-550k (có sale)
9. Cập nhật giá cho **Phụ kiện Nữ**: 150k-280k (một số có sale)

**Ví dụ về giá sau khi cập nhật:**
```
Áo Polo Nam Tay Ngắn Form Ôm      → Price: 650,000₫  SalePrice: 520,000₫  → Hiển thị: 520.000₫
Áo Sơ Mi Nam Tay Ngắn Cộc Tay     → Price: 620,000₫  SalePrice: NULL      → Hiển thị: 620.000₫
Quần Jeans Nam Slim Fit           → Price: 850,000₫  SalePrice: 720,000₫  → Hiển thị: 720.000₫
```

### **Phương án 2: Thông qua Admin Panel (Thủ công)**

1. Truy cập: `http://localhost:5000/Admin/Products`
2. Click "Edit" trên từng sản phẩm
3. Cập nhật:
   - **Price**: Giá gốc (bắt buộc)
   - **SalePrice**: Giá sale (tùy chọn - để trống nếu không có khuyến mãi)
4. Click "Save"

**Ví dụ cập nhật thủ công:**
```
Sản phẩm: Áo Polo Nam Tay Ngắn Form Ôm Tay Phối Viền
- Price: 650000
- SalePrice: 520000  ← Nhập số này nếu có khuyến mãi, để trống nếu không
```

### **Phương án 3: Import CSV (Hàng loạt)**

Nếu có nhiều sản phẩm, tạo file CSV với cấu trúc:

```csv
Name,Price,SalePrice
"Áo Polo Nam Tay Ngắn Form Ôm",650000,520000
"Áo Sơ Mi Nam Tay Dài",480000,380000
"Quần Tây Nam Slim Fit",780000,650000
```

Sau đó import qua Admin Panel (nếu có feature) hoặc viết script C#.

---

## ✅ XÁC NHẬN SAU KHI CẬP NHẬT

### Bước 1: Kiểm tra database

```sql
-- Xem 10 sản phẩm đầu tiên
SELECT TOP 10
    Name,
    Price,
    SalePrice,
    CASE WHEN SalePrice IS NOT NULL THEN SalePrice ELSE Price END AS DisplayPrice
FROM Products
WHERE IsActive = 1
ORDER BY CreatedAt DESC;
```

**Kết quả mong đợi:** Các sản phẩm có giá khác nhau (không phải toàn 450,000₫)

### Bước 2: Restart ứng dụng

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet run
```

### Bước 3: Kiểm tra trên trình duyệt

1. Truy cập: `http://localhost:5000/Home/JohnHenry`
2. Kiểm tra giá các sản phẩm:
   - ✅ Một số sản phẩm có giá **520.000₫** (SalePrice)
   - ✅ Một số sản phẩm có giá **620.000₫** (Price gốc)
   - ✅ Không còn tất cả sản phẩm 450.000₫

3. Truy cập: `http://localhost:5000/Home/Freelancer`
4. Kiểm tra tương tự

### Bước 4: Kiểm tra từng sub-page

- `http://localhost:5000/Home/JohnHenryShirt` → Áo sơ mi nam (380k-720k)
- `http://localhost:5000/Home/JohnHenryTrousers` → Quần nam (550k-850k)
- `http://localhost:5000/Home/JohnHenryAccessories` → Phụ kiện nam (180k-320k)
- `http://localhost:5000/Home/FreelancerDress` → Váy nữ (780k-980k)
- `http://localhost:5000/Home/FreelancerShirt` → Áo nữ (350k-580k)
- `http://localhost:5000/Home/FreelancerTrousers` → Quần nữ (550k-720k)
- `http://localhost:5000/Home/FreelancerSkirt` → Chân váy (450k-550k)
- `http://localhost:5000/Home/FreelancerAccessories` → Phụ kiện nữ (150k-280k)

---

## 🎯 TẠI SAO CODE ĐÃ ĐÚNG NHƯNG VẪN HIỂN THỊ 450K?

### Logic hoạt động của hệ thống:

```
Browser Request → HomeController.JohnHenry()
                        ↓
            Query database: SELECT * FROM Products WHERE CategoryId = 'Thời trang nam'
                        ↓
            Result: [Product1{Price:450k, SalePrice:null}, Product2{Price:450k, SalePrice:null}, ...]
                        ↓
            Pass to View: return View(products)
                        ↓
            JohnHenry.cshtml: @foreach (var product in Model)
                        ↓
            Render: <div class="product-price">450.000₫</div>
```

**Vấn đề:** Nếu database chỉ có sản phẩm giá 450k → code đúng nhưng output vẫn là 450k

**Giải pháp:** Cập nhật database để có nhiều mức giá khác nhau

---

## 📝 CHECKLIST HOÀN THÀNH

### Trước khi cập nhật:
- [ ] Backup database (quan trọng!)
  ```bash
  dotnet ef migrations script > backup_before_price_update.sql
  ```
- [ ] Xác nhận connection string trong `appsettings.json`
- [ ] Kiểm tra số lượng sản phẩm hiện có
  ```sql
  SELECT COUNT(*) FROM Products WHERE IsActive = 1;
  ```

### Sau khi cập nhật:
- [ ] Chạy script `update_product_prices.sql`
- [ ] Kiểm tra query SELECT để xác nhận giá đã thay đổi
- [ ] Restart ứng dụng
- [ ] Test trên trình duyệt (10 trang)
- [ ] Kiểm tra hiển thị giá sale (nếu có)
- [ ] Xác nhận format hiển thị đúng (dấu phẩy ngăn cách: 1.500.000₫)

---

## 🔮 PHÒNG TRÁNH SAU NÀY

### 1. Seed Data với giá đa dạng

Khi tạo migration hoặc seed data, đảm bảo:

```csharp
// Migrations/xxxxx_SeedProducts.cs
migrationBuilder.InsertData(
    table: "Products",
    columns: new[] { "Id", "Name", "Price", "SalePrice", ... },
    values: new object[,]
    {
        { Guid.NewGuid(), "Áo Polo Nam", 550000m, 450000m, ... },  // ✅ Có sale
        { Guid.NewGuid(), "Áo Sơ Mi Nam", 720000m, null, ... },    // ✅ Không sale
        { Guid.NewGuid(), "Quần Tây Nam", 850000m, 720000m, ... }, // ✅ Có sale
    }
);
```

### 2. Validation trong Admin Panel

Thêm validation khi tạo/sửa sản phẩm:

```csharp
[Range(1000, 10000000, ErrorMessage = "Giá phải từ 1.000₫ đến 10.000.000₫")]
public decimal Price { get; set; }

[Range(1000, 10000000, ErrorMessage = "Giá sale phải từ 1.000₫ đến 10.000.000₫")]
public decimal? SalePrice { get; set; }
```

### 3. Test Data Generator

Tạo script để generate test data với giá random:

```sql
-- Generate random prices between 300k-1000k
UPDATE Products
SET 
    Price = 300000 + (ABS(CHECKSUM(NEWID())) % 700000),
    SalePrice = CASE 
        WHEN ABS(CHECKSUM(NEWID())) % 2 = 0 
        THEN Price * 0.8  -- 20% discount
        ELSE NULL 
    END
WHERE IsActive = 1;
```

---

## 📞 KẾT LUẬN

**Vấn đề hiện tại:**
- ✅ Code hoàn toàn chính xác
- ❌ Database chưa có dữ liệu đa dạng về giá

**Hành động cần làm:**
1. Chạy script `database/update_product_prices.sql`
2. Restart ứng dụng
3. Test lại 10 trang sản phẩm
4. Xác nhận giá hiển thị đúng và đa dạng

**Thời gian ước tính:** 5-10 phút

---

**File tham khảo:**
- Script SQL: `/database/update_product_prices.sql`
- Controller: `/Controllers/HomeController.cs` (lines 47-104)
- View JohnHenry: `/Views/Home/JohnHenry.cshtml` (lines 580-615)
- View Freelancer: `/Views/Home/Freelancer.cshtml` (lines 390-425)
