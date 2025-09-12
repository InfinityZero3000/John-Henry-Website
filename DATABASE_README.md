# John Henry E-Commerce Database Setup

## 🐘 PostgreSQL Database with Docker

This project uses PostgreSQL as the primary database, containerized with Docker for easy setup and deployment.

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 9.0 SDK

### 1. Start Database
```bash
# Start PostgreSQL and pgAdmin
docker-compose up -d

# Check if containers are running
docker-compose ps
```

### 2. Access Database
- **PostgreSQL**: `localhost:5432`
- **pgAdmin**: http://localhost:8080
  - Email: `admin@johnhenry.com`
  - Password: `admin123`

### 3. Database Connection
```
Host: localhost
Port: 5432
Database: johnhenry_db
Username: johnhenry_user
Password: JohnHenry@2025!
```

### 4. Run Application
```bash
# Restore packages
dotnet restore

# Run application
dotnet run
```

## 📊 Database Schema

### Core Tables
- **users** - User accounts and authentication
- **addresses** - User shipping/billing addresses
- **categories** - Product categories (hierarchical)
- **brands** - Product brands
- **products** - Main product catalog
- **product_variants** - Product variations (size, color)
- **product_images** - Product image gallery
- **product_attributes** - Configurable attributes
- **shopping_carts** - Shopping cart items
- **wishlists** - User wishlist items

### Order Management
- **orders** - Order headers
- **order_items** - Order line items
- **order_status_history** - Order status tracking
- **coupons** - Discount coupons
- **coupon_usages** - Coupon usage tracking

### Content Management
- **blog_posts** - Blog articles
- **blog_categories** - Blog categories
- **product_reviews** - Product reviews and ratings

### System Tables
- **settings** - System configuration
- **email_templates** - Email templates
- **audit_logs** - System audit trail

## 🔧 Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# Database
DB_HOST=localhost
DB_PORT=5432
DB_NAME=johnhenry_db
DB_USER=johnhenry_user
DB_PASSWORD=JohnHenry@2025!

# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5000

# JWT
JWT_SECRET=your-secret-key
JWT_ISSUER=JohnHenryFashion
JWT_AUDIENCE=JohnHenryUsers
```

## 📈 Sample Data

The database includes sample data:
- **Admin User**: `admin@johnhenry.com` / `admin123`
- **Sample Products**: Men's and Women's clothing
- **Categories**: Complete category hierarchy
- **Blog Posts**: Sample blog articles
- **Coupons**: Test discount codes

## 🛠️ Development Commands

```bash
# Stop database
docker-compose down

# Reset database (WARNING: Deletes all data)
docker-compose down -v
docker-compose up -d

# View database logs
docker-compose logs postgres

# Access PostgreSQL CLI
docker exec -it johnhenry_postgres psql -U johnhenry_user -d johnhenry_db

# Backup database
docker exec johnhenry_postgres pg_dump -U johnhenry_user johnhenry_db > backup.sql

# Restore database
docker exec -i johnhenry_postgres psql -U johnhenry_user -d johnhenry_db < backup.sql
```

## 📊 Database Monitoring

### Performance Views
- `product_sales_summary` - Product sales analytics
- `order_summary` - Daily order summaries

### Useful Queries
```sql
-- Top selling products
SELECT * FROM product_sales_summary ORDER BY total_sold DESC LIMIT 10;

-- Recent orders
SELECT order_number, total_amount, order_status, created_at 
FROM orders ORDER BY created_at DESC LIMIT 20;

-- Category performance
SELECT c.name, COUNT(p.id) as product_count
FROM categories c
LEFT JOIN products p ON c.id = p.category_id
GROUP BY c.id, c.name
ORDER BY product_count DESC;
```

## 🔒 Security Considerations

1. **Password Security**: Uses BCrypt for password hashing
2. **JWT Tokens**: Secure authentication with configurable expiry
3. **SQL Injection**: Uses parameterized queries via Entity Framework
4. **Input Validation**: FluentValidation for data validation
5. **Audit Trail**: Complete audit logging for sensitive operations

## 🚀 Production Deployment

For production:
1. Use strong passwords
2. Configure SSL/TLS
3. Set up database backups
4. Configure monitoring
5. Use environment-specific connection strings

## 📞 Support

For database issues:
1. Check Docker container logs
2. Verify connection strings
3. Ensure PostgreSQL is running
4. Check firewall settings
