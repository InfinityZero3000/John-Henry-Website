# HƯỚNG DẪN SỬ DỤNG DATABASE FUNCTIONS, TRIGGERS & PROCEDURES

## 📋 Tổng Quan

File `triggers_functions_procedures.sql` chứa:
- **10 Functions** - Các hàm xử lý logic
- **10 Triggers** - Tự động hóa các tác vụ
- **7 Stored Procedures** - Xử lý business logic phức tạp
- **Indexes bổ sung** - Cải thiện performance

---

## 🚀 CÀI ĐẶT

### Bước 1: Kết nối Database
```bash
psql -h localhost -U johnhenry_user -d johnhenry_db
```

### Bước 2: Chạy Script
```bash
# Từ terminal
psql -h localhost -U johnhenry_user -d johnhenry_db -f database/triggers_functions_procedures.sql

# Hoặc từ psql prompt
\i database/triggers_functions_procedures.sql
```

### Bước 3: Verify Installation
```sql
-- Kiểm tra functions
SELECT proname, proargnames 
FROM pg_proc 
WHERE pronamespace = 'public'::regnamespace 
AND prokind = 'f'
ORDER BY proname;

-- Kiểm tra triggers
SELECT tgname, tgrelid::regclass 
FROM pg_trigger 
WHERE tgisinternal = false
ORDER BY tgname;

-- Kiểm tra procedures
SELECT proname, proargnames 
FROM pg_proc 
WHERE pronamespace = 'public'::regnamespace 
AND prokind = 'p'
ORDER BY proname;
```

---

## 📖 FUNCTIONS - Chi Tiết Sử Dụng

### 1. `update_updated_at_column()`
**Mục đích:** Tự động cập nhật timestamp UpdatedAt  
**Được dùng bởi:** Triggers trên Products, Categories, Orders, Brands

```sql
-- Không cần gọi trực tiếp, trigger tự động chạy
UPDATE "Products" SET "Name" = 'New Name' WHERE "Id" = '...';
-- UpdatedAt sẽ tự động được cập nhật
```

---

### 2. `calculate_product_rating(product_id)`
**Mục đích:** Tính rating trung bình của sản phẩm  
**Return:** DECIMAL(3,2) - Từ 0.00 đến 5.00

```sql
-- Tính rating của 1 sản phẩm
SELECT calculate_product_rating('123e4567-e89b-12d3-a456-426614174000'::UUID);

-- Cập nhật rating cho tất cả sản phẩm
UPDATE "Products" 
SET "Rating" = calculate_product_rating("Id");
```

**Ví dụ output:**
```
 calculate_product_rating 
--------------------------
                     4.65
```

---

### 3. `count_product_reviews(product_id)`
**Mục đích:** Đếm số reviews đã được approve  
**Return:** INTEGER

```sql
-- Đếm reviews của 1 sản phẩm
SELECT count_product_reviews('123e4567-e89b-12d3-a456-426614174000'::UUID);

-- Lấy top 10 sản phẩm có nhiều reviews nhất
SELECT 
    "Name",
    count_product_reviews("Id") as review_count
FROM "Products"
ORDER BY review_count DESC
LIMIT 10;
```

---

### 4. `get_product_final_price(product_id, quantity, coupon_code)`
**Mục đích:** Tính giá cuối cùng sau khi áp dụng coupon  
**Parameters:**
- `product_id` (UUID) - ID sản phẩm
- `quantity` (INTEGER) - Số lượng (default: 1)
- `coupon_code` (VARCHAR) - Mã giảm giá (default: NULL)

**Return:** DECIMAL(18,2)

```sql
-- Tính giá không có coupon
SELECT get_product_final_price(
    '123e4567-e89b-12d3-a456-426614174000'::UUID,
    1,
    NULL
);

-- Tính giá với coupon
SELECT get_product_final_price(
    '123e4567-e89b-12d3-a456-426614174000'::UUID,
    2,
    'SUMMER2025'
);

-- Áp dụng vào cart calculation
SELECT 
    p."Name",
    p."Price",
    get_product_final_price(p."Id", 2, 'SUMMER2025') as final_price,
    p."Price" * 2 - get_product_final_price(p."Id", 2, 'SUMMER2025') as discount_amount
FROM "Products" p
WHERE p."Id" = '123e4567-e89b-12d3-a456-426614174000'::UUID;
```

---

### 5. `calculate_shipping_cost(weight_kg, province_code, shipping_method)`
**Mục đích:** Tính phí vận chuyển  
**Parameters:**
- `weight_kg` (DECIMAL) - Trọng lượng (kg)
- `province_code` (VARCHAR) - Mã tỉnh/thành
- `shipping_method_code` (VARCHAR) - Mã phương thức vận chuyển

**Return:** DECIMAL(18,2)

```sql
-- Tính phí ship cho Hà Nội
SELECT calculate_shipping_cost(2.5, 'HN', 'standard');

-- Tính phí ship cho TP.HCM
SELECT calculate_shipping_cost(2.5, '79', 'express');

-- Tính cho nhiều tỉnh
SELECT 
    p."Code",
    p."Name",
    calculate_shipping_cost(3.0, p."Code", 'standard') as shipping_cost
FROM "Provinces" p
ORDER BY shipping_cost DESC;
```

**Province codes:**
- `HN`, `01` - Hà Nội (multiplier: 1.0)
- `SG`, `79` - TP.HCM (multiplier: 1.5)
- `DN`, `48` - Đà Nẵng (multiplier: 1.3)
- Other - Tỉnh xa (multiplier: 1.8)

---

### 6. `get_seller_commission(order_amount, seller_id)`
**Mục đích:** Tính commission platform thu từ seller  
**Return:** DECIMAL(18,2)

```sql
-- Tính commission cho 1 order
SELECT get_seller_commission(1000000, 'seller-id-here');

-- Tính tổng commission từ seller trong tháng
SELECT 
    p."SellerId",
    SUM(get_seller_commission(o."TotalAmount", p."SellerId")) as total_commission
FROM "Orders" o
INNER JOIN "OrderItems" oi ON o."Id" = oi."OrderId"
INNER JOIN "Products" p ON oi."ProductId" = p."Id"
WHERE o."CreatedAt" >= '2025-11-01'
  AND o."CreatedAt" < '2025-12-01'
GROUP BY p."SellerId";
```

**Default commission rates:**
- Bronze: 15%
- Silver: 12%
- Gold: 10%
- Platinum: 8%

---

### 7. `check_stock_availability(product_id, quantity)`
**Mục đích:** Kiểm tra còn đủ hàng không  
**Return:** BOOLEAN

```sql
-- Kiểm tra 1 sản phẩm
SELECT check_stock_availability(
    '123e4567-e89b-12d3-a456-426614174000'::UUID,
    5
);

-- Kiểm tra nhiều sản phẩm trong cart
SELECT 
    p."Name",
    p."StockQuantity",
    check_stock_availability(p."Id", 3) as can_order_3,
    check_stock_availability(p."Id", 10) as can_order_10
FROM "Products" p
WHERE p."IsActive" = TRUE
LIMIT 10;

-- Validate cart items
SELECT 
    sci."ProductId",
    sci."Quantity",
    CASE 
        WHEN check_stock_availability(sci."ProductId", sci."Quantity") 
        THEN 'Available' 
        ELSE 'Out of Stock' 
    END as status
FROM "ShoppingCartItems" sci
WHERE sci."UserId" = 'user-id-here';
```

---

### 8. `get_seller_revenue(seller_id, start_date, end_date)`
**Mục đích:** Lấy báo cáo doanh thu của seller  
**Return:** TABLE với columns:
- `total_orders` (BIGINT)
- `total_revenue` (DECIMAL)
- `total_commission` (DECIMAL)
- `net_revenue` (DECIMAL)

```sql
-- Doanh thu seller trong tháng 11/2025
SELECT * FROM get_seller_revenue(
    'seller-id-here',
    '2025-11-01'::TIMESTAMP,
    '2025-11-30'::TIMESTAMP
);

-- Doanh thu tất cả sellers
SELECT 
    u."UserName",
    r.*
FROM "AspNetUsers" u
CROSS JOIN LATERAL get_seller_revenue(
    u."Id",
    '2025-11-01'::TIMESTAMP,
    '2025-11-30'::TIMESTAMP
) r
WHERE u."Id" IN (
    SELECT DISTINCT "SellerId" FROM "Products" WHERE "SellerId" IS NOT NULL
)
ORDER BY r.net_revenue DESC;
```

**Example output:**
```
 total_orders | total_revenue | total_commission | net_revenue 
--------------+---------------+------------------+-------------
           25 |   15000000.00 |      2250000.00  | 12750000.00
```

---

### 9. `calculate_order_discount(subtotal, coupon_code)`
**Mục đích:** Tính số tiền discount cho order  
**Return:** DECIMAL(18,2)

```sql
-- Tính discount
SELECT calculate_order_discount(500000, 'SUMMER2025');

-- Áp dụng vào order summary
SELECT 
    500000 as subtotal,
    calculate_order_discount(500000, 'SUMMER2025') as discount,
    500000 - calculate_order_discount(500000, 'SUMMER2025') as final_total;
```

---

### 10. `generate_order_number()`
**Mục đích:** Tạo mã order unique  
**Return:** VARCHAR(50) - Format: ORD + YYYYMMDD + 4 số random

```sql
-- Generate order number
SELECT generate_order_number();

-- Example output: ORD202511100123

-- Sử dụng khi tạo order mới
INSERT INTO "Orders" (
    "Id", "OrderNumber", "UserId", "TotalAmount", ...
) VALUES (
    gen_random_uuid(),
    generate_order_number(),
    'user-id',
    1000000,
    ...
);
```

---

## 🔔 TRIGGERS - Chi Tiết

### 1. `update_products_timestamp`
**Bảng:** Products  
**Event:** BEFORE UPDATE  
**Hành động:** Tự động cập nhật UpdatedAt

### 2. `update_categories_timestamp`
**Bảng:** Categories  
**Event:** BEFORE UPDATE

### 3. `update_orders_timestamp`
**Bảng:** Orders  
**Event:** BEFORE UPDATE

### 4. `update_brands_timestamp`
**Bảng:** Brands  
**Event:** BEFORE UPDATE

---

### 5. `update_product_rating_on_review` ⭐
**Bảng:** ProductReviews  
**Event:** AFTER INSERT OR UPDATE  
**Hành động:** Tự động cập nhật Rating và ReviewCount của Product khi có review mới

```sql
-- Test trigger
INSERT INTO "ProductReviews" (
    "Id", "ProductId", "UserId", "Rating", "Comment", "IsApproved"
) VALUES (
    gen_random_uuid(),
    'product-id',
    'user-id',
    5,
    'Excellent product!',
    TRUE
);

-- Product Rating và ReviewCount sẽ tự động được cập nhật
```

---

### 6. `update_inventory_trigger` ⭐⭐⭐
**Bảng:** Orders  
**Event:** AFTER UPDATE  
**Hành động:** 
- Giảm stock khi order status = 'confirmed'
- Hoàn trả stock khi order status = 'cancelled'

```sql
-- Test: Confirm order
UPDATE "Orders" 
SET "Status" = 'confirmed' 
WHERE "Id" = 'order-id';
-- Stock sẽ tự động giảm

-- Test: Cancel order
UPDATE "Orders" 
SET "Status" = 'cancelled' 
WHERE "Id" = 'order-id';
-- Stock sẽ tự động hoàn trả
```

---

### 7. `log_order_status_trigger` ⭐
**Bảng:** Orders  
**Event:** AFTER UPDATE  
**Hành động:** Log mọi thay đổi order status vào OrderStatusHistories

```sql
-- Test
UPDATE "Orders" 
SET "Status" = 'shipping' 
WHERE "Id" = 'order-id';

-- Check log
SELECT * FROM "OrderStatusHistories" 
WHERE "OrderId" = 'order-id' 
ORDER BY "CreatedAt" DESC;
```

---

### 8. `increment_coupon_usage_trigger`
**Bảng:** Orders  
**Event:** AFTER INSERT  
**Hành động:** Tăng usage count của coupon và log vào CouponUsages

---

### 9. `validate_product_trigger` ⭐⭐
**Bảng:** Products  
**Event:** BEFORE INSERT OR UPDATE  
**Hành động:**
- Validate price > 0
- Validate sale_price < price
- Validate stock >= 0
- Auto-update InStock status
- Auto-generate slug

```sql
-- Test validation
INSERT INTO "Products" (...) VALUES (..., -100, ...);
-- Sẽ raise exception: "Product price must be greater than 0"

INSERT INTO "Products" (..., "Price" = 100000, "SalePrice" = 150000, ...);
-- Sẽ raise exception: "Sale price must be less than regular price"
```

---

## 📦 STORED PROCEDURES - Chi Tiết

### 1. `process_order_completion(order_id)` ⭐⭐⭐
**Mục đích:** Xử lý hoàn tất order  
**Hành động:**
1. Cập nhật order status = 'completed'
2. Tạo payment transactions cho sellers
3. Log audit

```sql
-- Process order
CALL process_order_completion('123e4567-e89b-12d3-a456-426614174000'::UUID);

-- Verify
SELECT * FROM "Orders" WHERE "Id" = '123e4567...';
SELECT * FROM "PaymentTransactions" WHERE "OrderId" = '123e4567...';
```

---

### 2. `create_seller_settlement(seller_id, start_date, end_date)` ⭐⭐⭐
**Mục đích:** Tạo settlement (thanh toán) cho seller  
**Hành động:**
1. Tính tổng doanh thu trong kỳ
2. Tính commission
3. Tạo SellerSettlement record
4. Link payment transactions

```sql
-- Tạo settlement cho tháng 11/2025
CALL create_seller_settlement(
    'seller-id-here',
    '2025-11-01 00:00:00'::TIMESTAMP,
    '2025-11-30 23:59:59'::TIMESTAMP
);

-- Check settlement
SELECT * FROM "SellerSettlements" 
WHERE "SellerId" = 'seller-id-here' 
ORDER BY "CreatedAt" DESC 
LIMIT 1;
```

---

### 3. `cleanup_expired_sessions()`
**Mục đích:** Xóa sessions hết hạn  
**Nên chạy:** Daily (cron job)

```sql
-- Manual cleanup
CALL cleanup_expired_sessions();

-- Output: "Cleaned up 123 expired sessions"
```

**Setup cron job:**
```bash
# Crontab entry - chạy hàng ngày lúc 3AM
0 3 * * * psql -U johnhenry_user -d johnhenry_db -c "CALL cleanup_expired_sessions();"
```

---

### 4. `generate_monthly_sales_report(year, month)` ⭐⭐
**Mục đích:** Tạo báo cáo doanh thu tháng

```sql
-- Generate report cho tháng 11/2025
CALL generate_monthly_sales_report(2025, 11);

-- View report
SELECT * FROM "SalesReports" 
WHERE "ReportType" = 'monthly' 
AND "PeriodStart" >= '2025-11-01' 
ORDER BY "PeriodStart" DESC;
```

---

### 5. `auto_approve_products()`
**Mục đích:** Tự động duyệt sản phẩm từ sellers uy tín

```sql
-- Auto approve
CALL auto_approve_products();

-- Output: "Auto-approved 15 products"
```

**Setup cron job:**
```bash
# Chạy mỗi giờ
0 * * * * psql -U johnhenry_user -d johnhenry_db -c "CALL auto_approve_products();"
```

---

### 6. `archive_old_orders(days_old)`
**Mục đích:** Archive orders cũ

```sql
-- Archive orders cũ hơn 1 năm
CALL archive_old_orders(365);

-- Archive orders cũ hơn 2 năm
CALL archive_old_orders(730);
```

---

### 7. `recalculate_all_product_ratings()`
**Mục đích:** Recalculate ratings cho tất cả products  
**Nên chạy:** Khi cần sync lại data

```sql
-- Recalculate
CALL recalculate_all_product_ratings();

-- Output: "Recalculated ratings for 1500 products"
```

---

### 8. `cleanup_expired_coupons()`
**Mục đích:** Deactivate coupons hết hạn

```sql
-- Cleanup
CALL cleanup_expired_coupons();

-- Output: "Deactivated 23 expired coupons"
```

---

## 🤖 AUTOMATION - Cron Jobs Setup

### 1. Daily Tasks (3 AM)
```bash
#!/bin/bash
# /path/to/daily_maintenance.sh

psql -U johnhenry_user -d johnhenry_db << EOF
-- Cleanup expired sessions
CALL cleanup_expired_sessions();

-- Cleanup expired coupons
CALL cleanup_expired_coupons();

-- Archive old orders (older than 1 year)
CALL archive_old_orders(365);
EOF
```

**Crontab:**
```
0 3 * * * /path/to/daily_maintenance.sh
```

---

### 2. Hourly Tasks
```bash
#!/bin/bash
# /path/to/hourly_maintenance.sh

psql -U johnhenry_user -d johnhenry_db << EOF
-- Auto approve products
CALL auto_approve_products();
EOF
```

**Crontab:**
```
0 * * * * /path/to/hourly_maintenance.sh
```

---

### 3. Monthly Tasks (1st day of month, 4 AM)
```bash
#!/bin/bash
# /path/to/monthly_reports.sh

YEAR=$(date +%Y)
LAST_MONTH=$(date -d "last month" +%m)

psql -U johnhenry_user -d johnhenry_db << EOF
-- Generate monthly sales report
CALL generate_monthly_sales_report($YEAR, $LAST_MONTH);

-- Recalculate all ratings
CALL recalculate_all_product_ratings();
EOF
```

**Crontab:**
```
0 4 1 * * /path/to/monthly_reports.sh
```

---

## 🎯 USE CASES - Ví Dụ Thực Tế

### Use Case 1: Checkout Process
```sql
-- 1. Check stock availability
SELECT 
    sci."ProductId",
    sci."Quantity",
    check_stock_availability(sci."ProductId", sci."Quantity") as available
FROM "ShoppingCartItems" sci
WHERE sci."UserId" = 'user-id';

-- 2. Calculate prices
SELECT 
    sci."ProductId",
    p."Name",
    get_product_final_price(sci."ProductId", sci."Quantity", 'COUPON123') as price
FROM "ShoppingCartItems" sci
JOIN "Products" p ON sci."ProductId" = p."Id"
WHERE sci."UserId" = 'user-id';

-- 3. Calculate shipping
SELECT calculate_shipping_cost(5.0, '79', 'express');

-- 4. Create order (trigger sẽ tự động giảm stock)
INSERT INTO "Orders" (...) VALUES (...);
```

---

### Use Case 2: Seller Dashboard
```sql
-- 1. Get seller revenue
SELECT * FROM get_seller_revenue(
    'seller-id',
    date_trunc('month', CURRENT_DATE),
    CURRENT_DATE
);

-- 2. Get pending settlements
SELECT * FROM "SellerSettlements"
WHERE "SellerId" = 'seller-id'
AND "Status" = 'pending'
ORDER BY "CreatedAt" DESC;

-- 3. Get product performance
SELECT 
    p."Name",
    p."Rating",
    p."ReviewCount",
    p."ViewCount",
    p."StockQuantity"
FROM "Products" p
WHERE p."SellerId" = 'seller-id'
ORDER BY p."ViewCount" DESC
LIMIT 10;
```

---

### Use Case 3: Admin Analytics
```sql
-- 1. Monthly revenue
SELECT * FROM "SalesReports"
WHERE "ReportType" = 'monthly'
ORDER BY "PeriodStart" DESC
LIMIT 12;

-- 2. Top sellers
SELECT 
    u."UserName",
    r.total_orders,
    r.total_revenue,
    r.net_revenue
FROM "AspNetUsers" u
CROSS JOIN LATERAL get_seller_revenue(
    u."Id",
    '2025-01-01'::TIMESTAMP,
    '2025-12-31'::TIMESTAMP
) r
WHERE r.total_revenue > 0
ORDER BY r.total_revenue DESC
LIMIT 20;

-- 3. Commission summary
SELECT 
    SUM(total_commission) as platform_revenue
FROM (
    SELECT get_seller_commission(o."TotalAmount", p."SellerId") as total_commission
    FROM "Orders" o
    JOIN "OrderItems" oi ON o."Id" = oi."OrderId"
    JOIN "Products" p ON oi."ProductId" = p."Id"
    WHERE o."Status" IN ('completed', 'delivered')
    AND o."CreatedAt" >= '2025-01-01'
) subquery;
```

---

## 🔧 MAINTENANCE

### Check Function Performance
```sql
-- List all functions với execution time
SELECT 
    schemaname,
    funcname,
    calls,
    total_time,
    self_time,
    avg_time
FROM pg_stat_user_functions
ORDER BY total_time DESC;

-- Reset stats
SELECT pg_stat_reset();
```

---

### Check Trigger Performance
```sql
-- Disable a trigger temporarily
ALTER TABLE "Products" DISABLE TRIGGER validate_product_trigger;

-- Re-enable
ALTER TABLE "Products" ENABLE TRIGGER validate_product_trigger;

-- Drop a trigger
DROP TRIGGER IF EXISTS trigger_name ON table_name;
```

---

### Update Functions
```sql
-- Drop and recreate
DROP FUNCTION IF EXISTS function_name(param_types);
-- Then run CREATE OR REPLACE FUNCTION ...

-- Or just CREATE OR REPLACE (recommended)
CREATE OR REPLACE FUNCTION function_name(...)
...
```

---

## 📊 MONITORING

### Daily Checks
```sql
-- Check failed procedures
SELECT * FROM pg_stat_statements
WHERE query LIKE '%CALL%'
AND calls > 0
ORDER BY mean_exec_time DESC;

-- Check trigger execution
SELECT 
    tgname,
    tgrelid::regclass,
    tgenabled
FROM pg_trigger
WHERE tgisinternal = false
ORDER BY tgname;

-- Check for locks
SELECT * FROM pg_locks
WHERE NOT granted;
```

---

## 🚨 TROUBLESHOOTING

### Common Issues

**1. Function not found**
```sql
-- Check if function exists
SELECT proname, proargtypes 
FROM pg_proc 
WHERE proname = 'function_name';

-- Recreate function
\i database/triggers_functions_procedures.sql
```

**2. Trigger not firing**
```sql
-- Check trigger status
SELECT tgname, tgenabled 
FROM pg_trigger 
WHERE tgrelid = 'table_name'::regclass;

-- Re-enable trigger
ALTER TABLE table_name ENABLE TRIGGER trigger_name;
```

**3. Performance issues**
```sql
-- Analyze tables
ANALYZE "Products";
ANALYZE "Orders";
ANALYZE "OrderItems";

-- Rebuild indexes
REINDEX TABLE "Products";
```

---

## 📝 NOTES

1. **PostgreSQL specific:** Tất cả code trong file này chỉ chạy trên PostgreSQL, không tương thích với SQL Server hoặc MySQL.

2. **Testing:** Test thoroughly trên development environment trước khi deploy lên production.

3. **Backup:** Luôn backup database trước khi chạy procedures lần đầu.

4. **Monitoring:** Setup monitoring cho function/procedure execution times.

5. **Permissions:** Đảm bảo database user có quyền CREATE FUNCTION, CREATE TRIGGER, CREATE PROCEDURE.

---

## 🔗 REFERENCES

- PostgreSQL Documentation: https://www.postgresql.org/docs/15/
- Functions: https://www.postgresql.org/docs/15/xfunc.html
- Triggers: https://www.postgresql.org/docs/15/trigger-definition.html
- Procedures: https://www.postgresql.org/docs/15/sql-createprocedure.html

---

**Last Updated:** 10/11/2025  
**Version:** 1.0  
**Author:** GitHub Copilot
