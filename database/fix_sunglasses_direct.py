#!/usr/bin/env python3
"""
Direct database fix for sunglasses category assignment
Moves FWSG* products from "Chân váy nữ" to "Phụ kiện nữ"
"""

import psycopg2
import sys

# Database connection parameters
DB_HOST = "localhost"
DB_PORT = "5432"
DB_NAME = "johnhenry"
DB_USER = "nguyenhuuthang"  # macOS username

# Category IDs from CategoryAnalysis
CHAN_VAY_CATEGORY_ID = "047f1f96-6947-4233-afa2-cb4b991953e5"
PHU_KIEN_CATEGORY_ID = "2357d59d-ed57-4d6e-b435-696aa680dd60"

print("=== FIX SUNGLASSES CATEGORY ASSIGNMENT ===\n")

try:
    # Connect to database
    print(f"Connecting to database '{DB_NAME}' as user '{DB_USER}'...")
    conn = psycopg2.connect(
        host=DB_HOST,
        port=DB_PORT,
        dbname=DB_NAME,
        user=DB_USER
    )
    cursor = conn.cursor()
    print("✅ Connected successfully!\n")
    
    # Check current state
    print("BEFORE FIX:")
    cursor.execute("""
        SELECT "SKU", "Name", "CategoryId",
            (SELECT "Name" FROM "Categories" WHERE "Id" = "Products"."CategoryId") as "CategoryName"
        FROM "Products"
        WHERE "SKU" LIKE 'FWSG%' AND "IsActive" = true
        ORDER BY "SKU"
    """)
    
    products = cursor.fetchall()
    print(f"Found {len(products)} sunglasses products:")
    for sku, name, cat_id, cat_name in products:
        status = "❌ WRONG" if cat_id == CHAN_VAY_CATEGORY_ID else "✅ OK"
        print(f"  {status} {sku}: {name}")
        print(f"       Category: {cat_name} ({cat_id})")
    
    # Count products in each category
    print("\nCategory counts BEFORE fix:")
    cursor.execute("""
        SELECT c."Name", COUNT(p."Id") as "Count"
        FROM "Categories" c
        LEFT JOIN "Products" p ON p."CategoryId" = c."Id" AND p."IsActive" = true
        WHERE c."Name" IN ('Chân váy nữ', 'Phụ kiện nữ')
        GROUP BY c."Name"
        ORDER BY c."Name"
    """)
    for cat_name, count in cursor.fetchall():
        print(f"  {cat_name}: {count} products")
    
    # Perform the fix
    print("\n" + "="*50)
    print("APPLYING FIX...")
    print("="*50 + "\n")
    
    cursor.execute("""
        UPDATE "Products"
        SET "CategoryId" = %s
        WHERE "SKU" LIKE 'FWSG%%' AND "IsActive" = true
        RETURNING "SKU", "Name"
    """, (PHU_KIEN_CATEGORY_ID,))
    
    updated_products = cursor.fetchall()
    print(f"✅ Updated {len(updated_products)} products:")
    for sku, name in updated_products:
        print(f"  - {sku}: {name}")
    
    # Commit the changes
    conn.commit()
    print("\n✅ Changes committed to database!")
    
    # Verify the fix
    print("\n" + "="*50)
    print("AFTER FIX:")
    print("="*50 + "\n")
    
    cursor.execute("""
        SELECT "SKU", "Name", "CategoryId",
            (SELECT "Name" FROM "Categories" WHERE "Id" = "Products"."CategoryId") as "CategoryName"
        FROM "Products"
        WHERE "SKU" LIKE 'FWSG%' AND "IsActive" = true
        ORDER BY "SKU"
    """)
    
    products = cursor.fetchall()
    print(f"All {len(products)} sunglasses products:")
    for sku, name, cat_id, cat_name in products:
        status = "✅" if cat_id == PHU_KIEN_CATEGORY_ID else "❌"
        print(f"  {status} {sku}: {name}")
        print(f"       Category: {cat_name}")
    
    # Count products in each category after fix
    print("\nCategory counts AFTER fix:")
    cursor.execute("""
        SELECT c."Name", COUNT(p."Id") as "Count"
        FROM "Categories" c
        LEFT JOIN "Products" p ON p."CategoryId" = c."Id" AND p."IsActive" = true
        WHERE c."Name" IN ('Chân váy nữ', 'Phụ kiện nữ')
        GROUP BY c."Name"
        ORDER BY c."Name"
    """)
    for cat_name, count in cursor.fetchall():
        print(f"  {cat_name}: {count} products")
    
    cursor.close()
    conn.close()
    
    print("\n" + "="*50)
    print("SUCCESS! Next steps:")
    print("="*50)
    print("1. Restart the application to clear cache")
    print("2. Visit: http://localhost:5101/Home/FreelancerSkirt")
    print("   → Should show 33 skirts (no sunglasses)")
    print("3. Visit: http://localhost:5101/Home/FreelancerAccessories")
    print("   → Should show 5 products (2 existing + 3 sunglasses)")
    print("\n✅ FIX COMPLETED SUCCESSFULLY!")
    
except psycopg2.Error as e:
    print(f"❌ Database error: {e}")
    print(f"\nTroubleshooting:")
    print(f"1. Make sure PostgreSQL is running")
    print(f"2. Check database name: {DB_NAME}")
    print(f"3. Check username: {DB_USER}")
    print(f"4. You may need to create a PostgreSQL user:")
    print(f"   createuser -s {DB_USER}")
    sys.exit(1)
    
except Exception as e:
    print(f"❌ Unexpected error: {e}")
    sys.exit(1)
