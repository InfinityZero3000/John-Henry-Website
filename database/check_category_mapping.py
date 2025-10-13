#!/usr/bin/env python3
"""
Script để kiểm tra category mapping và tạo SQL update script
"""

import csv
import os

def read_db_export(filename):
    """Đọc sản phẩm từ database export"""
    products = []
    try:
        with open(filename, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            for row in reader:
                products.append({
                    'sku': row['SKU'].strip(),
                    'name': row['Name'].strip(),
                    'category': row['category'].strip(),
                    'brand': row['brand'].strip()
                })
    except Exception as e:
        print(f"Error reading {filename}: {e}")
    return products

def detect_correct_category_from_sku(sku, name):
    """Phát hiện category đúng dựa trên SKU và tên"""
    sku_upper = sku.upper()
    name_lower = name.lower()
    
    # Women's products (FW prefix)
    if sku_upper.startswith('FW'):
        if any(x in sku_upper for x in ['BL', 'BLOUSE']):
            return 'Áo nữ'
        elif any(x in sku_upper for x in ['TS', 'TSHIRT']):
            return 'Áo nữ'
        elif 'KS' in sku_upper or 'polo' in name_lower:
            return 'Áo nữ'
        elif 'WS' in sku_upper or 'ws' in name_lower:
            return 'Áo nữ'
        elif any(x in name_lower for x in ['đầm', 'dam']):
            return 'Đầm nữ'
        elif any(x in name_lower for x in ['váy', 'vay', 'skirt', 'chân váy']):
            return 'Chân váy nữ'
        elif any(x in name_lower for x in ['quần', 'quan', 'pants', 'jeans']):
            return 'Quần nữ'
        elif 'BZ' in sku_upper or 'blazer' in name_lower or 'khoác' in name_lower:
            return 'Áo nữ'
        elif 'JK' in sku_upper or 'jacket' in name_lower:
            return 'Áo nữ'
        elif any(x in sku_upper for x in ['BE', 'BELT']) or 'thắt lưng' in name_lower or 'belt' in name_lower:
            return 'Phụ kiện nữ'
        elif any(x in sku_upper for x in ['BA', 'BAG']) or 'balo' in name_lower or 'bag' in name_lower or 'túi' in name_lower:
            return 'Phụ kiện nữ'
        elif 'SG' in sku_upper or 'mắt kính' in name_lower or 'kính' in name_lower:
            return 'Phụ kiện nữ'
        elif 'SP' in sku_upper or 'sandal' in name_lower or 'giày' in name_lower:
            return 'Phụ kiện nữ'
        else:
            return 'Áo nữ'  # Default for FW
    
    # Men's products
    elif 'WS' in sku_upper and any(x in name_lower for x in ['sơ mi', 'so mi', 'shirt']):
        return 'Áo nam'
    elif 'KS' in sku_upper or ('polo' in name_lower and 'nam' in name_lower):
        return 'Áo nam'
    elif 'TS' in sku_upper or ('áo thun' in name_lower and 'nam' in name_lower):
        return 'Áo nam'
    elif 'BZ' in sku_upper or 'blazer' in name_lower or 'vest' in name_lower:
        return 'Áo nam'
    elif 'JK' in sku_upper or ('khoác' in name_lower and 'nam' in name_lower):
        return 'Áo nam'
    elif any(x in sku_upper for x in ['DP', 'KP']) or ('quần' in name_lower and 'nam' in name_lower):
        return 'Quần nam'
    elif any(x in sku_upper for x in ['BE', 'BELT']) or 'thắt lưng' in name_lower:
        return 'Phụ kiện nam'
    elif any(x in sku_upper for x in ['CT', 'TIE']) or 'cà vạt' in name_lower or 'cavat' in name_lower:
        return 'Phụ kiện nam'
    elif 'BA' in sku_upper or 'balo' in name_lower:
        return 'Phụ kiện nam'
    elif 'WT' in sku_upper or 'áo gió' in name_lower or 'windbreaker' in name_lower:
        return 'Áo nam'
    
    # Default fallback
    if 'nam' in name_lower:
        return 'Áo nam'
    elif 'nữ' in name_lower or 'nu' in name_lower:
        return 'Áo nữ'
    
    return None

def main():
    print("=" * 80)
    print("KIỂM TRA VÀ FIX CATEGORY MAPPING")
    print("=" * 80)
    print()
    
    db_export_path = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/all_products_export.csv'
    
    print("📄 Đọc database export...")
    products = read_db_export(db_export_path)
    print(f"   ✓ Đọc được {len(products)} sản phẩm")
    print()
    
    # Phân tích category hiện tại
    print("📊 PHÂN TÍCH CATEGORY HIỆN TẠI:")
    print("-" * 80)
    category_count = {}
    for product in products:
        cat = product['category']
        category_count[cat] = category_count.get(cat, 0) + 1
    
    for cat, count in sorted(category_count.items()):
        print(f"   {cat}: {count} sản phẩm")
    print()
    
    # Tìm sản phẩm có category sai
    print("🔍 KIỂM TRA CATEGORY MAPPING:")
    print("-" * 80)
    
    mismatched = []
    for product in products:
        current_cat = product['category']
        correct_cat = detect_correct_category_from_sku(product['sku'], product['name'])
        
        if correct_cat and current_cat != correct_cat:
            mismatched.append({
                'sku': product['sku'],
                'name': product['name'],
                'current': current_cat,
                'correct': correct_cat
            })
    
    if not mismatched:
        print("✅ Tất cả sản phẩm đã có category đúng!")
        return
    
    print(f"⚠️  Tìm thấy {len(mismatched)} sản phẩm có category sai:")
    print()
    
    # Group by correction needed
    corrections = {}
    for item in mismatched:
        key = (item['current'], item['correct'])
        if key not in corrections:
            corrections[key] = []
        corrections[key].append(item)
    
    for (current, correct), items in corrections.items():
        print(f"   {current} → {correct}: {len(items)} sản phẩm")
        for item in items[:3]:
            print(f"      - {item['sku']}: {item['name'][:50]}")
        if len(items) > 3:
            print(f"      ... và {len(items) - 3} sản phẩm khác")
        print()
    
    # Tạo SQL script
    output_file = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/fix_category_mapping.sql'
    
    print(f"📝 Tạo SQL script: {output_file}")
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("-- SQL Script to fix category mapping\n")
        f.write("-- Auto-generated\n")
        f.write(f"-- Total products to update: {len(mismatched)}\n")
        f.write("\n")
        f.write("DO $$\n")
        f.write("DECLARE\n")
        
        # Collect all categories needed
        categories_needed = set()
        for item in mismatched:
            categories_needed.add(item['correct'])
        
        for cat_name in sorted(categories_needed):
            var_name = cat_name.lower().replace(' ', '_')
            var_name = var_name.replace('á', 'a').replace('ă', 'a').replace('â', 'a')
            var_name = var_name.replace('đ', 'd')
            var_name = var_name.replace('é', 'e').replace('ê', 'e')
            var_name = var_name.replace('í', 'i')
            var_name = var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
            var_name = var_name.replace('ú', 'u').replace('ư', 'u')
            var_name = var_name.replace('ý', 'y')
            f.write(f"    v_{var_name}_id UUID;\n")
        
        f.write("BEGIN\n")
        f.write("    -- Get Category IDs\n")
        
        for cat_name in sorted(categories_needed):
            var_name = cat_name.lower().replace(' ', '_')
            var_name = var_name.replace('á', 'a').replace('ă', 'a').replace('â', 'a')
            var_name = var_name.replace('đ', 'd')
            var_name = var_name.replace('é', 'e').replace('ê', 'e')
            var_name = var_name.replace('í', 'i')
            var_name = var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
            var_name = var_name.replace('ú', 'u').replace('ư', 'u')
            var_name = var_name.replace('ý', 'y')
            f.write(f'    SELECT "Id" INTO v_{var_name}_id FROM "Categories" WHERE "Name" = \'{cat_name}\' LIMIT 1;\n')
        
        f.write("\n")
        f.write("    -- Update product categories\n")
        
        # Group updates by category change
        for (current, correct), items in sorted(corrections.items()):
            var_name = correct.lower().replace(' ', '_')
            var_name = var_name.replace('á', 'a').replace('ă', 'a').replace('â', 'a')
            var_name = var_name.replace('đ', 'd')
            var_name = var_name.replace('é', 'e').replace('ê', 'e')
            var_name = var_name.replace('í', 'i')
            var_name = var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
            var_name = var_name.replace('ú', 'u').replace('ư', 'u')
            var_name = var_name.replace('ý', 'y')
            
            f.write(f"\n    -- Update: {current} → {correct} ({len(items)} products)\n")
            f.write(f'    UPDATE "Products"\n')
            f.write(f'    SET "CategoryId" = v_{var_name}_id, "UpdatedAt" = NOW()\n')
            f.write(f'    WHERE "SKU" IN (\n')
            
            for i, item in enumerate(items):
                f.write(f"        '{item['sku']}'")
                if i < len(items) - 1:
                    f.write(",\n")
                else:
                    f.write("\n")
            
            f.write(f'    );\n')
            f.write(f'    RAISE NOTICE \'Updated % products from {current} to {correct}\', ROW_COUNT;\n')
        
        f.write("\n")
        f.write("    RAISE NOTICE 'Category mapping fix completed!';\n")
        f.write("END $$;\n")
        f.write("\n")
        f.write("-- Verify updates\n")
        f.write('SELECT c."Name" as category, COUNT(*) as product_count\n')
        f.write('FROM "Products" p\n')
        f.write('JOIN "Categories" c ON p."CategoryId" = c."Id"\n')
        f.write('GROUP BY c."Name"\n')
        f.write('ORDER BY c."Name";\n')
    
    print(f"✅ SQL script đã được tạo!")
    print(f"   File: {output_file}")
    print()
    print("📝 Để áp dụng fix, chạy lệnh:")
    print(f"   psql -U johnhenry -d johnhenry -f {output_file}")
    print()

if __name__ == "__main__":
    main()
