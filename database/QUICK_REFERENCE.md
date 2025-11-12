# 🚀 QUICK REFERENCE - Database Functions & Procedures
*John Henry Fashion - PostgreSQL 15*

---

## 📌 MOST USED FUNCTIONS

### 1️⃣ Calculate Final Price with Coupon
```sql
SELECT get_product_final_price(
    product_id::UUID,
    quantity::INTEGER,
    'COUPON_CODE'
);

-- Example
SELECT get_product_final_price(
    '123e4567-e89b-12d3-a456-426614174000'::UUID,
    2,
    'SUMMER2025'
);
-- Returns: 450000.00
```

### 2️⃣ Calculate Shipping Cost
```sql
SELECT calculate_shipping_cost(
    weight_kg::DECIMAL,
    province_code::VARCHAR,
    shipping_method::VARCHAR
);

-- Example
SELECT calculate_shipping_cost(2.5, '79', 'standard');
-- Returns: 65000.00 (65k VND)
```

### 3️⃣ Check Stock Availability
```sql
SELECT check_stock_availability(
    product_id::UUID,
    quantity::INTEGER
);

-- Example
SELECT check_stock_availability(
    '123e4567-e89b-12d3-a456-426614174000'::UUID,
    5
);
-- Returns: true or false
```

### 4️⃣ Get Seller Revenue Report
```sql
SELECT * FROM get_seller_revenue(
    seller_id::VARCHAR,
    start_date::TIMESTAMP,
    end_date::TIMESTAMP
);

-- Example: This month's revenue
SELECT * FROM get_seller_revenue(
    'seller-id-here',
    date_trunc('month', CURRENT_DATE),
    CURRENT_DATE
);
```

---

## 🔔 AUTO TRIGGERS (No need to call manually)

### ✅ Product Rating Auto-Update
```sql
-- Just insert a review, rating updates automatically
INSERT INTO "ProductReviews" (..., "IsApproved" = TRUE);
-- → Product "Rating" and "ReviewCount" auto-updated ✨
```

### ✅ Inventory Auto-Update
```sql
-- Confirm order
UPDATE "Orders" SET "Status" = 'confirmed' WHERE "Id" = '...';
-- → Stock quantity auto-decreased ✨

-- Cancel order
UPDATE "Orders" SET "Status" = 'cancelled' WHERE "Id" = '...';
-- → Stock quantity auto-restored ✨
```

### ✅ Order Status Logging
```sql
-- Change order status
UPDATE "Orders" SET "Status" = 'shipping' WHERE "Id" = '...';
-- → Automatically logged in "OrderStatusHistories" ✨
```

### ✅ Timestamp Auto-Update
```sql
-- Update any product
UPDATE "Products" SET "Name" = 'New Name' WHERE "Id" = '...';
-- → "UpdatedAt" automatically set to CURRENT_TIMESTAMP ✨
```

---

## 📦 ESSENTIAL PROCEDURES

### 1️⃣ Complete an Order
```sql
CALL process_order_completion(order_id::UUID);

-- Example
CALL process_order_completion('123e4567-e89b-12d3-a456-426614174000'::UUID);
-- ✅ Sets status to 'completed'
-- ✅ Creates payment transactions for sellers
-- ✅ Logs to audit
```

### 2️⃣ Create Seller Settlement
```sql
CALL create_seller_settlement(
    seller_id::VARCHAR,
    start_date::TIMESTAMP,
    end_date::TIMESTAMP
);

-- Example: Monthly settlement
CALL create_seller_settlement(
    'seller-id-here',
    '2025-11-01'::TIMESTAMP,
    '2025-11-30'::TIMESTAMP
);
-- ✅ Calculates revenue & commission
-- ✅ Creates settlement record
-- ✅ Links payment transactions
```

### 3️⃣ Generate Monthly Report
```sql
CALL generate_monthly_sales_report(year::INTEGER, month::INTEGER);

-- Example
CALL generate_monthly_sales_report(2025, 11);
-- ✅ Creates report in "SalesReports" table
```

---

## 🔧 MAINTENANCE PROCEDURES

### Daily Cleanup (Run at 3 AM)
```sql
-- Clean expired sessions
CALL cleanup_expired_sessions();

-- Clean expired coupons
CALL cleanup_expired_coupons();
```

### Weekly/Monthly
```sql
-- Recalculate all product ratings (if needed)
CALL recalculate_all_product_ratings();

-- Archive old orders (older than 1 year)
CALL archive_old_orders(365);
```

---

## 💡 COMMON USE CASES

### Use Case 1: Shopping Cart Checkout
```sql
-- 1. Check stock for all items
SELECT 
    sci."ProductId",
    check_stock_availability(sci."ProductId", sci."Quantity") as available
FROM "ShoppingCartItems" sci
WHERE sci."UserId" = 'user-id';

-- 2. Calculate total with coupon
SELECT 
    SUM(get_product_final_price(
        sci."ProductId", 
        sci."Quantity", 
        'COUPON123'
    )) as total
FROM "ShoppingCartItems" sci
WHERE sci."UserId" = 'user-id';

-- 3. Add shipping cost
SELECT 
    <total_from_step_2> + 
    calculate_shipping_cost(5.0, '79', 'standard') as final_total;
```

### Use Case 2: Product Details Page
```sql
-- Get product with calculated info
SELECT 
    p."Name",
    p."Price",
    p."SalePrice",
    calculate_product_rating(p."Id") as rating,
    count_product_reviews(p."Id") as review_count,
    check_stock_availability(p."Id", 1) as in_stock
FROM "Products" p
WHERE p."Id" = 'product-id';
```

### Use Case 3: Seller Dashboard
```sql
-- Get current month revenue
SELECT * FROM get_seller_revenue(
    'seller-id',
    date_trunc('month', CURRENT_DATE),
    CURRENT_DATE
);

-- Get pending settlements
SELECT * FROM "SellerSettlements"
WHERE "SellerId" = 'seller-id' 
AND "Status" = 'pending'
ORDER BY "CreatedAt" DESC;
```

---

## 🎯 QUICK TESTS

### Test Function
```sql
-- Test price calculation
SELECT get_product_final_price(
    (SELECT "Id" FROM "Products" LIMIT 1),
    1,
    NULL
);
```

### Test Trigger
```sql
-- Test rating auto-update
INSERT INTO "ProductReviews" (
    "Id", "ProductId", "UserId", "Rating", "IsApproved"
) VALUES (
    gen_random_uuid(),
    (SELECT "Id" FROM "Products" LIMIT 1),
    (SELECT "Id" FROM "AspNetUsers" LIMIT 1),
    5,
    TRUE
);

-- Check if rating updated
SELECT "Rating", "ReviewCount" 
FROM "Products" 
WHERE "Id" = <product_id_used_above>;
```

### Test Procedure
```sql
-- Test cleanup
CALL cleanup_expired_sessions();
-- Check output: "Cleaned up X expired sessions"
```

---

## ⚡ PERFORMANCE TIPS

### Use Indexes
```sql
-- These indexes are already created:
-- idx_orders_status_created
-- idx_products_category_active
-- idx_product_reviews_product_approved
```

### Check Function Performance
```sql
SELECT 
    funcname,
    calls,
    total_time,
    avg_time
FROM pg_stat_user_functions
ORDER BY total_time DESC
LIMIT 10;
```

### Analyze After Bulk Operations
```sql
ANALYZE "Products";
ANALYZE "Orders";
ANALYZE "ProductReviews";
```

---

## 🚨 TROUBLESHOOTING

### Function Not Found?
```sql
-- Check if exists
SELECT proname FROM pg_proc 
WHERE proname = 'function_name';

-- Recreate
\i database/triggers_functions_procedures.sql
```

### Trigger Not Firing?
```sql
-- Check status
SELECT tgname, tgenabled 
FROM pg_trigger 
WHERE tgrelid = 'table_name'::regclass;

-- Re-enable if disabled
ALTER TABLE "table_name" ENABLE TRIGGER trigger_name;
```

### Performance Issue?
```sql
-- Check for locks
SELECT * FROM pg_locks WHERE NOT granted;

-- Check slow queries
SELECT * FROM pg_stat_statements
ORDER BY mean_exec_time DESC
LIMIT 10;
```

---

## 📚 FULL DOCUMENTATION

👉 **See `FUNCTIONS_PROCEDURES_GUIDE.md` for:**
- Complete function signatures
- All parameters explained
- More examples
- Automation setup
- Advanced use cases

---

## 🔗 CHEAT SHEET

| Need to... | Use this... |
|------------|-------------|
| Calculate price with discount | `get_product_final_price()` |
| Calculate shipping | `calculate_shipping_cost()` |
| Check if in stock | `check_stock_availability()` |
| Get seller revenue | `get_seller_revenue()` |
| Complete order | `process_order_completion()` |
| Create settlement | `create_seller_settlement()` |
| Generate report | `generate_monthly_sales_report()` |
| Clean sessions | `cleanup_expired_sessions()` |
| Recalc ratings | `recalculate_all_product_ratings()` |

---

**Version:** 1.0  
**Last Updated:** 10/11/2025  
**PostgreSQL:** 15
