# Database Files - John Henry Fashion

## 📁 Files Overview

### Core Schema
- **`database_schema.sql`** (32KB) - Complete database schema với 50+ tables

### Data Import
- **`import_vietnam_addresses.sql`** (42KB) - Import địa chỉ VN (Provinces, Districts, Wards)
- **`import_hcm_full_addresses.sql`** (11KB) - Import địa chỉ TP.HCM chi tiết
- **`insert_payment_shipping_methods.sql`** (10KB) - Payment & shipping methods
- **`insert_sample_coupons.sql`** (3KB) - Sample coupons data
- **`update_product_images.sql`** (174KB) - Product images data
- **`assign_seller_to_products.sql`** (2KB) - Assign sellers to products

### ⭐ NEW: Functions, Triggers & Procedures
- **`triggers_functions_procedures.sql`** (900+ lines) - 🆕 Database automation
  - 10 Functions (calculate_product_rating, get_product_final_price, etc.)
  - 10 Triggers (auto update rating, inventory, status logging, etc.)
  - 7 Stored Procedures (process orders, settlements, reports, etc.)
  
- **`FUNCTIONS_PROCEDURES_GUIDE.md`** - 🆕 Comprehensive usage guide

### Maintenance Scripts
- **`backup_database.sh`** - Automated backup script
- **`restore_database.sh`** - Restore from backup
- **`cleanup_and_reset_shipping_methods.sql`** (3KB)
- **`remove_duplicate_shipping_methods.sql`** (2KB)
- **`verify_shipping_methods.sql`** (1KB)

### CSV Data
- **`johnhenry_products.csv`** - Product data export
- **`all_products_export.csv`** - Complete products export
- **`johnhenry,freelancer,bestseller.csv`** - Multi-store products

### Documentation
- **`DATABASE_README.md`** - General database documentation
- **`MIGRATIONS_GUIDE.md`** - Migration instructions
- **`PAYMENT_FLOW_DOCUMENTATION.md`** - Payment system docs
- **`BACKUP_RESTORE_GUIDE.md`** - Backup/restore guide
- **`FUNCTIONS_PROCEDURES_GUIDE.md`** - 🆕 Functions & procedures usage

---

## 🚀 Quick Start

### 1. Initial Setup
```bash
# Create database
createdb johnhenry_db

# Run schema
psql -d johnhenry_db -f database_schema.sql

# Import addresses
psql -d johnhenry_db -f import_vietnam_addresses.sql

# Import payment methods
psql -d johnhenry_db -f insert_payment_shipping_methods.sql
```

### 2. Deploy Functions & Triggers (NEW!)
```bash
# Deploy all functions, triggers and procedures
psql -d johnhenry_db -f triggers_functions_procedures.sql
```

### 3. Verify Installation
```bash
# Check functions
psql -d johnhenry_db -c "SELECT proname FROM pg_proc WHERE pronamespace = 'public'::regnamespace AND prokind = 'f';"

# Check triggers
psql -d johnhenry_db -c "SELECT tgname, tgrelid::regclass FROM pg_trigger WHERE tgisinternal = false;"

# Check procedures
psql -d johnhenry_db -c "SELECT proname FROM pg_proc WHERE pronamespace = 'public'::regnamespace AND prokind = 'p';"
```

---

## 📖 Documentation

### Core Docs
1. **DATABASE_README.md** - Overview và setup instructions
2. **MIGRATIONS_GUIDE.md** - How to manage migrations
3. **PAYMENT_FLOW_DOCUMENTATION.md** - Payment system architecture
4. **BACKUP_RESTORE_GUIDE.md** - Backup & restore procedures

### 🆕 NEW Documentation
5. **FUNCTIONS_PROCEDURES_GUIDE.md** - Complete guide for:
   - All 10 functions with examples
   - All 10 triggers with test cases
   - All 7 procedures with usage
   - Automation setup (cron jobs)
   - Performance monitoring
   - Troubleshooting

---

## 🔥 Key Features (NEW!)

### Functions
- ✅ `calculate_product_rating()` - Auto calculate ratings
- ✅ `get_product_final_price()` - Price with discounts
- ✅ `calculate_shipping_cost()` - Smart shipping calculation
- ✅ `get_seller_revenue()` - Revenue analytics
- ✅ `check_stock_availability()` - Inventory check

### Triggers
- ✅ Auto update product ratings on new review
- ✅ Auto update inventory on order confirmation/cancellation
- ✅ Auto log all order status changes
- ✅ Auto validate product data before save
- ✅ Auto increment coupon usage

### Procedures
- ✅ `process_order_completion()` - Complete order workflow
- ✅ `create_seller_settlement()` - Generate seller payments
- ✅ `generate_monthly_sales_report()` - Automated reporting
- ✅ `cleanup_expired_sessions()` - Maintenance
- ✅ `auto_approve_products()` - Smart auto-approval

---

## 🤖 Automation Setup

### Daily Maintenance (3 AM)
```bash
# Crontab entry
0 3 * * * psql -d johnhenry_db -c "CALL cleanup_expired_sessions(); CALL cleanup_expired_coupons();"
```

### Monthly Reports (1st day, 4 AM)
```bash
# Generate monthly reports
0 4 1 * * psql -d johnhenry_db -c "CALL generate_monthly_sales_report($(date +\%Y), $(date -d 'last month' +\%m));"
```

See `FUNCTIONS_PROCEDURES_GUIDE.md` for complete automation setup.

---

## 📊 Performance

### Indexes Added
- `idx_orders_status_created`
- `idx_orders_user_status`
- `idx_products_category_active`
- `idx_products_brand_active`
- `idx_product_reviews_product_approved`
- `idx_order_items_order_product`
- `idx_payment_transactions_seller_status`

### Optimization Tips
1. Run `ANALYZE` after bulk imports
2. Monitor function performance with `pg_stat_user_functions`
3. Use `EXPLAIN ANALYZE` for query optimization
4. Setup regular `VACUUM` jobs

---

## 🔒 Security

- All sensitive data masked in configs
- Connection strings use environment variables
- Backup scripts support encrypted backups
- Audit logging enabled via triggers
- Session cleanup automated

---

## 📞 Support

**Documentation:**
- See individual MD files in this directory
- Check `FUNCTIONS_PROCEDURES_GUIDE.md` for detailed examples

**Issues:**
- Check PostgreSQL logs: `/var/log/postgresql/`
- Review trigger execution: `SELECT * FROM pg_trigger WHERE tgisinternal = false;`
- Monitor performance: `SELECT * FROM pg_stat_user_functions;`

---

## 🔄 Updates

**Latest (10/11/2025):**
- ✅ Added 10 Functions
- ✅ Added 10 Triggers
- ✅ Added 7 Stored Procedures
- ✅ Added comprehensive guide
- ✅ Added performance indexes

**Previous:**
- Schema updates
- Address imports
- Payment methods setup
- Migration guides

---

**Last Updated:** 10/11/2025  
**PostgreSQL Version:** 15  
**ASP.NET Core Version:** 9.0
