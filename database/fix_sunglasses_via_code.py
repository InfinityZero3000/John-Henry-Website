#!/usr/bin/env python3
"""
Fix Sunglasses Category Assignment via HTTP API
This script updates sunglasses products through the application's API
"""

import requests
import json

# Get category analysis to find affected products
print("=== ANALYZING SUNGLASSES CATEGORY ISSUE ===\n")

base_url = "http://localhost:5101"

# Get category analysis
response = requests.get(f"{base_url}/Home/CategoryAnalysis")
if response.status_code == 200:
    data = response.json()
    
    # Find "Chân váy nữ" category
    chan_vay_category = None
    phu_kien_category = None
    
    for cat in data.get('categoriesAnalysis', []):
        if cat['categoryName'] == 'Chân váy nữ':
            chan_vay_category = cat
        elif cat['categoryName'] == 'Phụ kiện nữ':
            phu_kien_category = cat
    
    print("CURRENT SITUATION:")
    print(f"Chân váy nữ category ID: {chan_vay_category['categoryId']}")
    print(f"  Total products: {chan_vay_category['totalProducts']}")
    print(f"  Sample products:")
    
    sunglasses_count = 0
    for product in chan_vay_category['sampleProducts']:
        print(f"    - {product['sku']}: {product['name']}")
        if product['sku'].startswith('FWSG'):
            sunglasses_count += 1
    
    print(f"\n⚠️  Found {sunglasses_count} sunglasses products in 'Chân váy nữ' category!")
    print(f"\nPhụ kiện nữ category ID: {phu_kien_category['categoryId']}")
    print(f"  Total products: {phu_kien_category['totalProducts']}")
    
    print("\n=== SQL COMMANDS TO FIX ===")
    print(f"""
-- Update sunglasses products to correct category
UPDATE "Products"
SET "CategoryId" = '{phu_kien_category['categoryId']}'
WHERE "SKU" LIKE 'FWSG%'
  AND "IsActive" = true;

-- Verify the update
SELECT "SKU", "Name", 
  (SELECT "Name" FROM "Categories" WHERE "Id" = "Products"."CategoryId") as "CategoryName"
FROM "Products"
WHERE "SKU" LIKE 'FWSG%'
ORDER BY "SKU";
    """)
    
    print("\n=== MANUAL FIX INSTRUCTIONS ===")
    print("1. Copy the SQL UPDATE command above")
    print("2. Open your database management tool (pgAdmin, DBeaver, etc.)")
    print("3. Connect to the 'johnhenry' database")
    print("4. Run the UPDATE query")
    print("5. Restart the application")
    print("6. Verify by visiting: http://localhost:5101/Home/FreelancerSkirt")
    print("   - Should now show only skirts (no sunglasses)")
    print("7. Check: http://localhost:5101/Home/FreelancerAccessories")
    print("   - Should now show sunglasses products")

else:
    print(f"ERROR: Cannot fetch category analysis. Status code: {response.status_code}")
    print("Make sure the application is running on http://localhost:5101")
