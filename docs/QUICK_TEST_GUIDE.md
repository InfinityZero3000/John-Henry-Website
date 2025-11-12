# HƯỚNG DẪN TEST NHANH HỆ THỐNG SELLER

## ✅ Build Status: SUCCESS

**Ngày:** 10/11/2025  
**Build Time:** 5.55 giây  
**Warnings:** 0  
**Errors:** 0  

---

## 🚀 BƯỚC 1: CHUẨN BỊ DỮ LIỆU TEST

### 1.1. Tạo Seller Accounts
```sql
-- Đăng ký 2 seller accounts qua giao diện web hoặc chạy SQL:

-- Seller 1
INSERT INTO "AspNetUsers" ("Id", "Email", "UserName", "FirstName", "LastName", "IsApproved", "EmailConfirmed")
VALUES 
('seller1-guid', 'seller1@test.com', 'seller1@test.com', 'Seller', 'One', true, true);

-- Gán role Seller
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT 'seller1-guid', "Id" FROM "AspNetRoles" WHERE "Name" = 'Seller';

-- Seller 2
INSERT INTO "AspNetUsers" ("Id", "Email", "UserName", "FirstName", "LastName", "IsApproved", "EmailConfirmed")
VALUES 
('seller2-guid', 'seller2@test.com', 'seller2@test.com', 'Seller', 'Two', true, true);

-- Gán role Seller
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId")
SELECT 'seller2-guid', "Id" FROM "AspNetRoles" WHERE "Name" = 'Seller';
```

### 1.2. Gán Products cho Sellers
```sql
-- Gán một nửa sản phẩm cho Seller 1
UPDATE "Products" 
SET "SellerId" = 'seller1-guid'
WHERE "Id" IN (
    SELECT "Id" FROM "Products" 
    WHERE "SellerId" IS NULL 
    LIMIT (SELECT COUNT(*) / 2 FROM "Products" WHERE "SellerId" IS NULL)
);

-- Gán nửa còn lại cho Seller 2
UPDATE "Products" 
SET "SellerId" = 'seller2-guid'
WHERE "SellerId" IS NULL;
```

### 1.3. Verify Data
```sql
-- Check số lượng sản phẩm mỗi seller
SELECT 
    u."Email" as "SellerEmail",
    u."FirstName" || ' ' || u."LastName" as "SellerName",
    COUNT(p."Id") as "ProductCount"
FROM "Products" p
JOIN "AspNetUsers" u ON p."SellerId" = u."Id"
GROUP BY u."Id", u."Email", u."FirstName", u."LastName"
ORDER BY "ProductCount" DESC;
```

---

## 🧪 BƯỚC 2: TEST CÁC CHỨC NĂNG

### 2.1. Test Dashboard
1. Đăng nhập với seller1@test.com
2. Vào `/seller/dashboard`
3. **Kiểm tra:**
   - ✅ Số lượng sản phẩm hiển thị đúng (chỉ của seller 1)
   - ✅ Số đơn hàng đúng (chỉ đơn có sản phẩm của seller 1)
   - ✅ Doanh thu tính từ sản phẩm của seller 1
   - ✅ Top products chỉ là sản phẩm của seller 1

### 2.2. Test Products Management
1. Vào `/seller/products`
2. **Kiểm tra:**
   - ✅ Chỉ hiển thị sản phẩm của seller hiện tại
   - ✅ Tạo sản phẩm mới → tự động gán SellerId
   - ✅ Chỉnh sửa sản phẩm → chỉ sửa được sản phẩm của mình
   - ✅ Xóa sản phẩm → chỉ xóa được sản phẩm của mình

3. **Test Cross-seller Access:**
   - Thử truy cập URL edit sản phẩm của seller khác
   - Phải hiển thị lỗi "Không có quyền"

### 2.3. Test Orders
1. Vào `/seller/orders`
2. **Kiểm tra:**
   - ✅ Chỉ hiển thị đơn hàng có chứa sản phẩm của seller
   - ✅ Click vào order detail → xem được chi tiết
   - ✅ Thử URL order không có sản phẩm của mình → hiển thị lỗi

### 2.4. Test Coupons
1. Vào `/seller/coupons`
2. **Kiểm tra:**
   - ✅ Chỉ hiển thị coupon của seller
   - ✅ Tạo coupon mới → tự động gán SellerId
   - ✅ Edit coupon → chỉ sửa được của mình
   - ✅ Delete coupon → chỉ xóa được của mình

3. **Test Cross-seller Access:**
   - Đăng nhập seller 2
   - Thử edit coupon của seller 1 (thay ID trong URL)
   - Phải hiển thị lỗi "Không có quyền"

### 2.5. Test Reviews
1. Vào `/seller/reviews`
2. **Kiểm tra:**
   - ✅ Chỉ hiển thị reviews của sản phẩm seller
   - ✅ Statistics chính xác
   - ✅ Approve review → chỉ approve được review sản phẩm của mình
   - ✅ Reject review → chỉ reject được review sản phẩm của mình

### 2.6. Test Sales & Commissions
1. Vào `/seller/sales`
2. **Kiểm tra:**
   - ✅ Doanh thu tính từ sản phẩm của seller
   - ✅ Chart hiển thị đúng data
   - ✅ Filter theo date range hoạt động

3. Vào `/seller/commissions`
4. **Kiểm tra:**
   - ✅ Total sales từ sản phẩm của seller
   - ✅ Commission tính đúng (15%)
   - ✅ Monthly data chính xác

### 2.7. Test Customers
1. Vào `/seller/customers`
2. **Kiểm tra:**
   - ✅ Chỉ khách hàng đã mua sản phẩm của seller
   - ✅ Total spent tính từ sản phẩm của seller
   - ✅ Top customers đúng
   - ✅ New customers chính xác

### 2.8. Test Reports
1. Vào `/seller/reports`
2. **Kiểm tra:**
   - ✅ Total revenue từ sản phẩm của seller
   - ✅ Products count chỉ của seller
   - ✅ Charts hiển thị data của seller
   - ✅ Filter date range hoạt động

### 2.9. Test Product Performance
1. Vào `/seller/product-performance`
2. **Kiểm tra:**
   - ✅ Chỉ hiển thị sản phẩm của seller
   - ✅ Top products đúng
   - ✅ Low performing products đúng
   - ✅ Metrics chính xác

### 2.10. Test Analytics
1. Vào `/seller/analytics`
2. **Kiểm tra:**
   - ✅ Top selling products từ seller
   - ✅ Revenue calculations đúng

---

## 🔒 BƯỚC 3: TEST BẢO MẬT

### 3.1. Cross-Seller Access Test
**Mục tiêu:** Đảm bảo Seller A không thể xem/sửa dữ liệu của Seller B

**Test Case 1: Product Edit**
```
1. Đăng nhập seller1@test.com
2. Lấy ID một sản phẩm của seller 2
3. Truy cập URL: /seller/products/edit/{seller2-product-id}
4. Expected: Redirect hoặc error "Không có quyền"
```

**Test Case 2: Coupon Edit**
```
1. Đăng nhập seller1@test.com
2. Lấy ID một coupon của seller 2
3. Truy cập URL: /seller/coupons/edit/{seller2-coupon-id}
4. Expected: Error "Bạn không có quyền chỉnh sửa mã giảm giá này!"
```

**Test Case 3: Review Approve**
```
1. Đăng nhập seller1@test.com
2. Lấy ID một review của sản phẩm seller 2
3. Gọi API: POST /seller/reviews/approve/{seller2-review-id}
4. Expected: JSON { success: false, message: "Bạn không có quyền..." }
```

### 3.2. Authentication Test
**Test Case: Unauthenticated Access**
```
1. Logout khỏi hệ thống
2. Truy cập URL: /seller/dashboard
3. Expected: Redirect to /Account/Login
```

**Test Case: Wrong Role Access**
```
1. Đăng nhập với customer account
2. Truy cập URL: /seller/dashboard
3. Expected: 403 Forbidden hoặc redirect
```

---

## 📊 BƯỚC 4: TEST DỮ LIỆU CHÍNH XÁC

### 4.1. Verify Dashboard Statistics
```sql
-- Kiểm tra số sản phẩm của seller
SELECT COUNT(*) FROM "Products" WHERE "SellerId" = 'seller1-guid';

-- Kiểm tra doanh thu thực tế
SELECT SUM(oi."TotalPrice")
FROM "OrderItems" oi
JOIN "Orders" o ON oi."OrderId" = o."Id"
JOIN "Products" p ON oi."ProductId" = p."Id"
WHERE p."SellerId" = 'seller1-guid'
AND o."Status" = 'completed';

-- Kiểm tra số đơn hàng
SELECT COUNT(DISTINCT o."Id")
FROM "Orders" o
JOIN "OrderItems" oi ON o."Id" = oi."OrderId"
JOIN "Products" p ON oi."ProductId" = p."Id"
WHERE p."SellerId" = 'seller1-guid';
```

### 4.2. Compare với giao diện
- So sánh kết quả SQL với số liệu trên dashboard
- Phải khớp 100%

---

## ✅ CHECKLIST HOÀN THÀNH

### Phase 1: Basic Tests
- [ ] Dashboard hiển thị đúng statistics
- [ ] Products list chỉ sản phẩm của seller
- [ ] Create product tự động gán SellerId
- [ ] Edit product có ownership check
- [ ] Delete product có ownership check

### Phase 2: Orders & Coupons
- [ ] Orders list filter đúng
- [ ] Order detail có ownership check
- [ ] Coupons list filter đúng
- [ ] Create coupon tự động gán SellerId
- [ ] Edit/Delete coupon có ownership check

### Phase 3: Reviews & Analytics
- [ ] Reviews filter đúng
- [ ] Approve/Reject có ownership check
- [ ] Sales data chính xác
- [ ] Commissions tính đúng
- [ ] Customers list đúng

### Phase 4: Reports
- [ ] Reports data chính xác
- [ ] Product performance đúng
- [ ] Analytics charts đúng

### Phase 5: Security
- [ ] Cross-seller access bị block
- [ ] Unauthenticated access redirect login
- [ ] Wrong role access forbidden
- [ ] All error messages user-friendly

---

## 🐛 NẾU GẶP LỖI

### Lỗi: "Không tìm thấy sản phẩm"
**Nguyên nhân:** Products chưa có SellerId  
**Giải pháp:** Chạy SQL assign seller to products

### Lỗi: "Doanh thu = 0"
**Nguyên nhân:** Chưa có orders completed  
**Giải pháp:** Tạo test orders hoặc update status orders hiện có

### Lỗi: "Không có quyền"
**Nguyên nhân:** Đúng như mong đợi - đây là tính năng bảo mật  
**Giải pháp:** Không cần fix

### Lỗi: Build failed
**Nguyên nhân:** Thiếu using statements hoặc syntax error  
**Giải pháp:** 
```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet build JohnHenryFashionWeb.csproj
# Xem error details và fix
```

---

## 📝 GHI CHÚ

### Điểm Quan Trọng:
1. **SellerId không được null** cho products mới
2. **Mọi query đều filter theo seller**
3. **Ownership verification trước mọi edit/delete**
4. **Error messages phải user-friendly**

### Performance Tips:
1. Database đã có indexes trên SellerId
2. Sử dụng `.Include()` để reduce queries
3. `.ToListAsync()` cho async operations
4. Filter ở database level, không filter ở application level

### Monitoring:
- Theo dõi response time của các trang
- Check database query performance
- Monitor error logs trong `/logs/`

---

## 🎉 KẾT LUẬN

Sau khi hoàn thành tất cả test cases:
- ✅ Hệ thống đã được nâng cấp hoàn toàn
- ✅ Mỗi seller chỉ xem được dữ liệu của mình
- ✅ Bảo mật và phân quyền chặt chẽ
- ✅ Không còn hardcoded values
- ✅ Database integration hoàn chỉnh

**Next Steps:**
1. Deploy lên staging environment
2. UAT testing với real users
3. Performance testing với large dataset
4. Production deployment

---

**Tài liệu tham khảo:**
- `/docs/SELLER_UPGRADE_SUMMARY.md` - Tóm tắt nâng cấp
- `/docs/SELLER_TESTING_GUIDE.md` - Hướng dẫn chi tiết
- `/docs/SELLER_TEST_CHECKLIST.md` - Checklist đầy đủ
