#!/bin/bash

# Script để đồng bộ migration history với database hiện tại
# Đánh dấu tất cả migrations cũ là đã được áp dụng, trừ migration mới nhất

echo "Đang đồng bộ migration history..."

# Thêm tất cả migrations vào bảng __EFMigrationsHistory (trừ migration cuối)
# Điều này sẽ đánh dấu chúng là đã được áp dụng mà không thực sự chạy chúng

cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"

# Tạo file SQL để insert các migration đã có vào history
cat > /tmp/sync_migrations.sql << 'EOF'
-- Thêm các migration vào history nếu chưa tồn tại
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250911051832_InitialCreate', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250911051832_InitialCreate');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250911071624_AddAdminFields', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250911071624_AddAdminFields');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250911094300_AddShoppingCartItemProperties', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250911094300_AddShoppingCartItemProperties');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250912044517_AddContactMessage', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250912044517_AddContactMessage');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250912083723_AddNotifications', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250912083723_AddNotifications');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250912084430_AddSecurityEntities', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250912084430_AddSecurityEntities');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250912090829_UpdateActiveSessionFields', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250912090829_UpdateActiveSessionFields');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250912144809_AddLastLoginDateToUsers', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250912144809_AddLastLoginDateToUsers');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250917055347_AddAuditLog', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250917055347_AddAuditLog');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250918095028_AddStoreEntity', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918095028_AddStoreEntity');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250918100334_CreateStoresTableOnly', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250918100334_CreateStoresTableOnly');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250923045657_AddMissingColumns', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250923045657_AddMissingColumns');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
SELECT '20250927034723_UpdatePendingChanges', '9.0.0'
WHERE NOT EXISTS (SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20250927034723_UpdatePendingChanges');

-- Kiểm tra kết quả
SELECT * FROM "__EFMigrationsHistory" ORDER BY "MigrationId";
EOF

echo "File SQL đã được tạo tại /tmp/sync_migrations.sql"
echo ""
echo "Để áp dụng:"
echo "1. Chạy file SQL này vào database PostgreSQL của bạn"
echo "2. Sau đó chạy: dotnet ef database update"
echo ""
echo "Hoặc nếu bạn có psql, chạy:"
echo "psql -h <host> -U <user> -d <database> -f /tmp/sync_migrations.sql"
