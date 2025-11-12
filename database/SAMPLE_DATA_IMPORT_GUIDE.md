# HƯỚNG DẪN IMPORT DỮ LIỆU MẪU CHO DASHBOARD

## 📋 Tổng Quan

File `insert_sample_dashboard_data.sql` chứa dữ liệu mẫu để dashboard Admin và Seller hiển thị đẹp với đầy đủ thông số.

---

## 🎯 DỮ LIỆU SẼ ĐƯỢC IMPORT

### 1. Analytics Data (30 ngày)
- ✅ Page views (1000-3000/day)
- ✅ Unique visitors (500-1500/day)
- ✅ Conversion rate (2-5%)
- ✅ Bounce rate (30-50%)
- ✅ Average session duration
- ✅ Revenue per day
- ✅ Orders per day
- ✅ New vs returning customers

### 2. User Sessions (200 sessions)
- ✅ Session IDs
- ✅ User information
- ✅ Device type (Desktop/Mobile/Tablet)
- ✅ Browser (Chrome/Firefox/Safari/Edge)
- ✅ Location (Hanoi, HCM, Da Nang, etc.)
- ✅ Duration và pages viewed
- ✅ Conversion value

### 3. Page Views (600+ views)
- ✅ Pages visited
- ✅ Referrer sources
- ✅ Traffic sources (Google, Facebook, Direct)
- ✅ Campaign tracking
- ✅ Time on page

### 4. Conversion Events (300 events)
- ✅ Page views
- ✅ Add to cart
- ✅ Begin checkout
- ✅ Purchase completed
- ✅ Product views

### 5. Sales Reports
- ✅ 30 daily reports
- ✅ 12 weekly reports
- ✅ 6 monthly reports
- ✅ Revenue, orders, customers
- ✅ Average order value
- ✅ Top selling categories/products

### 6. Audit Logs (100 entries)
- ✅ Admin actions
- ✅ Product updates
- ✅ Order changes
- ✅ Settings modifications
- ✅ User activities

### 7. Seller Data
- ✅ Payment transactions (30 per seller)
- ✅ Settlements (3 per seller)
- ✅ Commission calculations
- ✅ Payout status

### 8. Product Performance
- ✅ View counts (100-1000)
- ✅ Ratings (3.0-5.0)
- ✅ Review counts (5-50)
- ✅ Stock movements (200 entries)

### 9. Support Tickets (50 tickets)
- ✅ Various categories
- ✅ Priority levels
- ✅ Status (open/in_progress/resolved)
- ✅ Ticket replies
- ✅ Related orders/products

### 10. Marketing Data
- ✅ Flash sales (3 campaigns)
- ✅ Email campaigns (3 campaigns)
- ✅ Campaign performance metrics

### 11. Notifications
- ✅ Order notifications
- ✅ Delivery updates
- ✅ Messages
- ✅ Per user (20 users)

---

## 🚀 CÁCH IMPORT

### Yêu Cầu
- PostgreSQL 15
- Database: johnhenry_db đã được setup
- Có data cơ bản: Users, Products, Orders, Categories

### Bước 1: Backup Database (Khuyến Nghị)
```bash
cd /Users/nguyenhuuthang/Documents/RepoGitHub/John\ Henry\ Website

# Backup trước khi import
./database/backup_database.sh
```

### Bước 2: Connect Database
```bash
psql -h localhost -U johnhenry_user -d johnhenry_db
```

### Bước 3: Import Sample Data
```bash
# From psql
\i database/insert_sample_dashboard_data.sql

# Hoặc từ terminal
psql -h localhost -U johnhenry_user -d johnhenry_db -f database/insert_sample_dashboard_data.sql
```

### Bước 4: Verify Import
```sql
-- Check analytics data
SELECT COUNT(*) FROM "AnalyticsData";
-- Expected: 30

-- Check user sessions
SELECT COUNT(*) FROM "UserSessions";
-- Expected: 200

-- Check page views
SELECT COUNT(*) FROM "PageViews";
-- Expected: 600+

-- Check sales reports
SELECT "ReportType", COUNT(*) 
FROM "SalesReports" 
GROUP BY "ReportType";
-- Expected: daily(30), weekly(12), monthly(6)

-- Check support tickets
SELECT "Status", COUNT(*) 
FROM "SupportTickets" 
GROUP BY "Status";
-- Expected: 50 total

-- Check seller settlements
SELECT COUNT(*) FROM "SellerSettlements";
-- Expected: Multiple per seller
```

---

## 📊 DASHBOARD VIEWS

### Admin Dashboard - Sẽ Hiển Thị:

#### Overview Cards
- 📈 **Total Revenue** - Từ SalesReports
- 📦 **Total Orders** - Từ Orders + SalesReports
- 👥 **Total Customers** - Từ AspNetUsers
- 📊 **Conversion Rate** - Từ AnalyticsData

#### Charts
- **Revenue Chart** (30 days) - Line chart từ SalesReports
- **Orders Chart** (30 days) - Bar chart từ Orders
- **Traffic Sources** - Pie chart từ PageViews
- **Device Distribution** - Donut chart từ UserSessions
- **Top Products** - Table từ Products + OrderItems
- **Top Categories** - Table từ Categories + Products

#### Recent Activity
- 📝 **Recent Orders** - Từ Orders (last 10)
- 🎫 **Support Tickets** - Từ SupportTickets (pending)
- 📋 **Audit Logs** - Từ AuditLogs (last 20)

#### Analytics
- **Page Views Trend** - Từ PageViews
- **Conversion Funnel** - Từ ConversionEvents
- **Bounce Rate** - Từ AnalyticsData
- **Session Duration** - Từ UserSessions

---

### Seller Dashboard - Sẽ Hiển Thị:

#### Overview Cards
- 💰 **Total Revenue** - Từ get_seller_revenue()
- 📦 **Total Orders** - Từ Orders (seller's products)
- ⭐ **Average Rating** - Từ Products (seller's)
- 📊 **Conversion Rate** - Tính từ views/orders

#### Revenue Analytics
- **Revenue Chart** (30 days) - Line chart
- **Commission Breakdown** - Pie chart từ PaymentTransactions
- **Settlement Status** - Table từ SellerSettlements
- **Pending Payouts** - Cards từ SellerSettlements

#### Product Performance
- **Top Selling Products** - Table từ OrderItems
- **Stock Levels** - Alert cards từ Products
- **Recent Reviews** - List từ ProductReviews
- **View Analytics** - Chart từ Products.ViewCount

#### Orders Management
- **Recent Orders** - Table từ Orders
- **Order Status Distribution** - Donut chart
- **Pending Actions** - Alert cards

#### Inventory
- **Stock Movements** - Table từ StockMovements
- **Low Stock Alerts** - Cards từ Products
- **Stock Value** - Calculated from Products

---

## 🎨 SAMPLE QUERIES FOR DASHBOARD

### Admin Dashboard Queries

```sql
-- 1. Overview Stats (Last 30 days)
SELECT 
    SUM("TotalRevenue") as total_revenue,
    SUM("TotalOrders") as total_orders,
    SUM("TotalCustomers") as total_customers,
    AVG("ConversionRate") as avg_conversion_rate
FROM "SalesReports"
WHERE "ReportType" = 'daily'
AND "PeriodStart" >= CURRENT_DATE - INTERVAL '30 days';

-- 2. Revenue Trend (Daily)
SELECT 
    "PeriodStart"::DATE as date,
    "TotalRevenue" as revenue,
    "TotalOrders" as orders
FROM "SalesReports"
WHERE "ReportType" = 'daily'
ORDER BY "PeriodStart" DESC
LIMIT 30;

-- 3. Traffic Sources
SELECT 
    "Source",
    COUNT(*) as visits,
    COUNT(DISTINCT "SessionId") as sessions
FROM "PageViews"
WHERE "ViewedAt" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY "Source"
ORDER BY visits DESC;

-- 4. Device Distribution
SELECT 
    "Device",
    COUNT(*) as sessions,
    ROUND(COUNT(*) * 100.0 / SUM(COUNT(*)) OVER (), 2) as percentage
FROM "UserSessions"
WHERE "StartedAt" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY "Device";

-- 5. Top Products
SELECT 
    p."Name",
    COUNT(oi."Id") as times_ordered,
    SUM(oi."Quantity") as total_quantity,
    SUM(oi."TotalPrice") as total_revenue
FROM "Products" p
INNER JOIN "OrderItems" oi ON p."Id" = oi."ProductId"
INNER JOIN "Orders" o ON oi."OrderId" = o."Id"
WHERE o."CreatedAt" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY p."Id", p."Name"
ORDER BY total_revenue DESC
LIMIT 10;

-- 6. Pending Support Tickets
SELECT 
    "TicketNumber",
    "Subject",
    "Priority",
    "Status",
    "CreatedAt"
FROM "SupportTickets"
WHERE "Status" IN ('open', 'in_progress')
ORDER BY 
    CASE "Priority"
        WHEN 'high' THEN 1
        WHEN 'medium' THEN 2
        ELSE 3
    END,
    "CreatedAt" DESC
LIMIT 10;

-- 7. Recent Audit Logs
SELECT 
    "Action",
    "EntityType",
    "Details",
    "Timestamp"
FROM "AuditLogs"
ORDER BY "Timestamp" DESC
LIMIT 20;
```

### Seller Dashboard Queries

```sql
-- 1. Seller Revenue Stats (Using function)
SELECT * FROM get_seller_revenue(
    'seller-id-here',
    CURRENT_DATE - INTERVAL '30 days',
    CURRENT_DATE
);

-- 2. Seller Revenue Trend
SELECT 
    DATE_TRUNC('day', o."CreatedAt") as date,
    SUM(oi."TotalPrice") as revenue,
    COUNT(DISTINCT o."Id") as orders
FROM "Orders" o
INNER JOIN "OrderItems" oi ON o."Id" = oi."OrderId"
INNER JOIN "Products" p ON oi."ProductId" = p."Id"
WHERE p."SellerId" = 'seller-id-here'
AND o."CreatedAt" >= CURRENT_DATE - INTERVAL '30 days'
AND o."Status" IN ('completed', 'delivered')
GROUP BY DATE_TRUNC('day', o."CreatedAt")
ORDER BY date DESC;

-- 3. Top Selling Products
SELECT 
    p."Name",
    p."SKU",
    COUNT(oi."Id") as times_ordered,
    SUM(oi."Quantity") as total_sold,
    SUM(oi."TotalPrice") as revenue
FROM "Products" p
INNER JOIN "OrderItems" oi ON p."Id" = oi."ProductId"
INNER JOIN "Orders" o ON oi."OrderId" = o."Id"
WHERE p."SellerId" = 'seller-id-here'
AND o."CreatedAt" >= CURRENT_DATE - INTERVAL '30 days'
GROUP BY p."Id", p."Name", p."SKU"
ORDER BY revenue DESC
LIMIT 10;

-- 4. Product Performance
SELECT 
    "Name",
    "ViewCount",
    "Rating",
    "ReviewCount",
    "StockQuantity",
    "InStock"
FROM "Products"
WHERE "SellerId" = 'seller-id-here'
AND "IsActive" = TRUE
ORDER BY "ViewCount" DESC
LIMIT 20;

-- 5. Recent Orders
SELECT 
    o."OrderNumber",
    o."TotalAmount",
    o."Status",
    o."CreatedAt",
    u."UserName" as customer
FROM "Orders" o
INNER JOIN "AspNetUsers" u ON o."UserId" = u."Id"
WHERE o."Id" IN (
    SELECT DISTINCT oi."OrderId"
    FROM "OrderItems" oi
    INNER JOIN "Products" p ON oi."ProductId" = p."Id"
    WHERE p."SellerId" = 'seller-id-here'
)
ORDER BY o."CreatedAt" DESC
LIMIT 20;

-- 6. Settlement Status
SELECT 
    "SettlementNumber",
    "TotalAmount",
    "CommissionAmount",
    "NetAmount",
    "Status",
    "CreatedAt",
    "SettledAt"
FROM "SellerSettlements"
WHERE "SellerId" = 'seller-id-here'
ORDER BY "CreatedAt" DESC
LIMIT 10;

-- 7. Low Stock Alert
SELECT 
    "Name",
    "SKU",
    "StockQuantity",
    "Price"
FROM "Products"
WHERE "SellerId" = 'seller-id-here'
AND "StockQuantity" < 10
AND "IsActive" = TRUE
ORDER BY "StockQuantity" ASC;

-- 8. Recent Stock Movements
SELECT 
    sm."Type",
    sm."Quantity",
    sm."Reason",
    sm."CreatedAt",
    p."Name" as product_name
FROM "StockMovements" sm
INNER JOIN "Products" p ON sm."ProductId" = p."Id"
WHERE p."SellerId" = 'seller-id-here'
ORDER BY sm."CreatedAt" DESC
LIMIT 20;
```

---

## 🔄 RE-IMPORT (Nếu Cần)

Nếu muốn xóa và import lại:

```sql
-- WARNING: This will delete all sample data

-- Delete sample data (giữ lại real data)
DELETE FROM "AnalyticsData" WHERE "CreatedAt" >= CURRENT_DATE - INTERVAL '31 days';
DELETE FROM "UserSessions" WHERE "SessionId" LIKE 'SESSION_%';
DELETE FROM "PageViews" WHERE "SessionId" IN (SELECT "SessionId" FROM "UserSessions" WHERE "SessionId" LIKE 'SESSION_%');
DELETE FROM "ConversionEvents" WHERE "SessionId" IN (SELECT "SessionId" FROM "UserSessions" WHERE "SessionId" LIKE 'SESSION_%');
DELETE FROM "SalesReports" WHERE "GeneratedAt" >= CURRENT_DATE - INTERVAL '1 day';
DELETE FROM "AuditLogs" WHERE "Details" LIKE 'Sample%';
DELETE FROM "SupportTickets" WHERE "TicketNumber" LIKE 'TKT%';
DELETE FROM "FlashSales" WHERE "Name" IN ('Black Friday Sale', 'Cyber Monday', 'Weekend Flash Sale');
DELETE FROM "EmailCampaigns" WHERE "Name" IN ('New Arrival Announcement', 'Summer Sale Reminder', 'VIP Customer Special');

-- Then re-import
\i database/insert_sample_dashboard_data.sql
```

---

## 📈 EXPECTED RESULTS

Sau khi import, bạn sẽ thấy:

### Admin Dashboard
- ✅ Revenue chart có data 30 ngày
- ✅ Traffic analytics đầy đủ
- ✅ Device/Browser breakdown
- ✅ Top products với số liệu thực tế
- ✅ Support tickets đang pending
- ✅ Recent audit logs
- ✅ Conversion funnel complete

### Seller Dashboard
- ✅ Revenue overview với numbers
- ✅ Orders chart có data
- ✅ Product performance metrics
- ✅ Settlement history
- ✅ Stock alerts (nếu có low stock)
- ✅ Recent orders list
- ✅ Commission breakdown

---

## 🎯 CUSTOMIZATION

### Modify Data Amount

Edit file `insert_sample_dashboard_data.sql`:

```sql
-- Change number of days (default: 30)
FOR i IN 0..29 LOOP  -- Change 29 to your desired days

-- Change number of sessions (default: 200)
FROM generate_series(1, 200) i  -- Change 200 to your number

-- Change number of tickets (default: 50)
FOR i IN 1..50 LOOP  -- Change 50 to your number
```

### Adjust Ranges

```sql
-- Revenue range (default: 500k - 1.5M)
500000 + FLOOR(RANDOM() * 1000000)

-- Orders range (default: 50-150)
50 + FLOOR(RANDOM() * 100)

-- View counts (default: 100-1000)
100 + FLOOR(RANDOM() * 1000)
```

---

## 🚨 TROUBLESHOOTING

### Issue: Foreign Key Errors
```
ERROR: insert or update on table violates foreign key constraint
```

**Solution:** Đảm bảo có data cơ bản trước:
```sql
-- Check if you have basic data
SELECT COUNT(*) FROM "AspNetUsers";  -- Should be > 0
SELECT COUNT(*) FROM "Products";     -- Should be > 0
SELECT COUNT(*) FROM "Orders";       -- Should be > 0
SELECT COUNT(*) FROM "Categories";   -- Should be > 0
```

### Issue: Column Not Found
```
ERROR: column "AnalyticsData" does not exist
```

**Solution:** Chạy migrations trước:
```bash
dotnet ef database update
```

### Issue: Slow Import
Import có thể mất 1-2 phút do generate random data.

**Normal behavior:** Wait for completion message.

---

## 📞 SUPPORT

Nếu có vấn đề:
1. Check PostgreSQL logs
2. Verify tables exist
3. Check foreign key constraints
4. Review transaction logs

---

**Created:** 10/11/2025  
**Version:** 1.0  
**PostgreSQL:** 15
