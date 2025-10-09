# Database Migrations Guide

## ⚠️ Important Note

**Không còn sử dụng file SQL init!**

Thư mục `init/` đã bị xóa vì các file SQL schema không khớp với C# models hiện tại:
- SQL dùng `INTEGER` cho ID
- C# Models dùng `GUID` cho ID
- Có conflict về bảng users (SQL có bảng riêng, C# dùng ASP.NET Identity)

## ✅ Sử dụng EF Core Migrations

### 1. Kiểm tra trạng thái migrations

```bash
dotnet ef migrations list
```

### 2. Tạo migration mới (khi có thay đổi models)

```bash
dotnet ef migrations add TenMigration
```

### 3. Apply migrations lên database

```bash
dotnet ef database update
```

### 4. Apply migration cụ thể

```bash
dotnet ef database update TenMigration
```

### 5. Xóa migration cuối cùng (chưa apply)

```bash
dotnet ef migrations remove
```

### 6. Tạo SQL script từ migrations

```bash
# Tạo script cho tất cả migrations
dotnet ef migrations script -o Migrations/migration_script.sql

# Tạo script từ migration A đến B
dotnet ef migrations script FromMigration ToMigration -o script.sql

# Tạo script idempotent (có thể chạy nhiều lần)
dotnet ef migrations script --idempotent -o script.sql
```

## 📋 Migrations hiện có

Hiện tại project có các migrations:
1. `20250911051832_InitialCreate` - Tạo database ban đầu
2. `20250911071624_AddAdminFields` - Thêm fields admin
3. `20250911094300_AddShoppingCartItemProperties` - Thêm properties cho shopping cart
4. `20250912044517_AddContactMessage` - Thêm bảng contact messages
5. `20250912083723_AddNotifications` - Thêm bảng notifications
6. `20250912084430_AddSecurityEntities` - Thêm bảng security
7. `20250912090829_UpdateActiveSessionFields` - Cập nhật session fields
8. `20250912144809_AddLastLoginDateToUsers` - Thêm last login date
9. `20250917055347_AddAuditLog` - Thêm audit log
10. `20250918095028_AddStoreEntity` - Thêm entity Store
11. `20250918100334_CreateStoresTableOnly` - Tạo bảng stores
12. `20250923045657_AddMissingColumns` - Thêm columns thiếu
13. `20250927034723_UpdatePendingChanges` - Cập nhật pending changes
14. `20251005132333_AddSocialMediaWebsiteToStore` - Thêm social media
15. `20251008070843_AddSellerIdToProduct` - Thêm seller ID cho product

## 🔄 Setup Database mới

### Cách 1: Sử dụng Migrations (Khuyên dùng)

```bash
# 1. Đảm bảo connection string đúng trong appsettings.json
# 2. Apply tất cả migrations
dotnet ef database update

# 3. Seed data (nếu cần)
# Chạy ứng dụng, nó sẽ tự động seed data
dotnet run
```

### Cách 2: Sử dụng SQL Script

```bash
# 1. Tạo SQL script từ migrations
dotnet ef migrations script -o database/setup_database.sql

# 2. Chạy script trên PostgreSQL
psql -U postgres -d johnhenry_db -f database/setup_database.sql
```

## 🔧 Troubleshooting

### Migration conflict
```bash
# Xóa database và tạo lại
dotnet ef database drop
dotnet ef database update
```

### Reset migrations
```bash
# Xóa tất cả migrations và tạo lại từ đầu
# CẢNH BÁO: Sẽ mất hết migration history
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Xem SQL của migration
```bash
dotnet ef migrations script 20250911051832_InitialCreate 20250911071624_AddAdminFields
```

## 📝 Best Practices

1. **Luôn tạo migration khi thay đổi models**
   ```bash
   dotnet ef migrations add DescriptiveName
   ```

2. **Test migration trên dev database trước**
   ```bash
   dotnet ef database update --connection "DevConnectionString"
   ```

3. **Backup database trước khi apply migration**
   ```bash
   ./database/backup_database.sh
   ```

4. **Sử dụng migration script cho production**
   ```bash
   dotnet ef migrations script --idempotent -o production_update.sql
   ```

5. **Không xóa migration đã apply vào production**

6. **Đặt tên migration rõ ràng**
   - ✅ `AddEmailToUser`
   - ✅ `UpdateProductPriceType`
   - ❌ `Update1`
   - ❌ `Fix`

## 🔗 Useful Commands

```bash
# Xem thông tin DbContext
dotnet ef dbcontext info

# Tạo code từ database có sẵn (reverse engineer)
dotnet ef dbcontext scaffold "ConnectionString" Npgsql.EntityFrameworkCore.PostgreSQL -o Models

# Optimize DbContext
dotnet ef dbcontext optimize
```

## 📚 Tài liệu tham khảo

- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL with EF Core](https://www.npgsql.org/efcore/)
