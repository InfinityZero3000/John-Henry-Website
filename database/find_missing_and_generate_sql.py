#!/usr/bin/env python3
"""
Script để tìm sản phẩm thiếu và tạo SQL import script
"""

import csv
import sys

def read_csv_products(filename):
    """Đọc sản phẩm từ CSV"""
    products = {}
    try:
        with open(filename, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            for row in reader:
                sku = row['sku'].strip().upper()
                # Chỉ lấy sản phẩm unique (nếu trùng, lấy cái đầu tiên)
                if sku not in products:
                    products[sku] = {
                        'name': row['name'].strip(),
                        'price': float(row['price'].replace(',', ''))
                    }
    except Exception as e:
        print(f"Error reading {filename}: {e}", file=sys.stderr)
    return products

def read_db_export(filename):
    """Đọc sản phẩm từ database export"""
    skus = set()
    try:
        with open(filename, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            for row in reader:
                sku = row['SKU'].strip().upper()
                skus.add(sku)
    except Exception as e:
        print(f"Error reading {filename}: {e}", file=sys.stderr)
    return skus

def generate_slug(name):
    """Generate slug from product name"""
    import unicodedata
    import re
    
    # Remove Vietnamese accents
    name = unicodedata.normalize('NFKD', name)
    name = name.encode('ASCII', 'ignore').decode('ASCII')
    
    # Convert to lowercase and replace spaces with hyphens
    slug = name.lower()
    slug = re.sub(r'[^a-z0-9\s-]', '', slug)
    slug = re.sub(r'[\s]+', '-', slug)
    slug = re.sub(r'-+', '-', slug)
    slug = slug.strip('-')
    
    return slug

def detect_category_from_sku(sku, name):
    """Phát hiện category dựa trên SKU prefix"""
    sku_upper = sku.upper()
    name_lower = name.lower()
    
    # Women's products (FW prefix)
    if sku_upper.startswith('FW'):
        if 'BL' in sku_upper or 'blouse' in name_lower or 'áo kiểu' in name_lower or 'áo sát nách' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'TS' in sku_upper or 'áo thun' in name_lower or 't-shirt' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'KS' in sku_upper or 'polo' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'WS' in sku_upper or 'áo sơ mi' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'DR' in sku_upper or 'đầm' in name_lower or 'dam' in name_lower or 'váy dài' in name_lower:
            return 'dam-nu', 'Đầm nữ'
        elif 'SK' in sku_upper or 'váy' in name_lower or 'skirt' in name_lower or 'chân váy' in name_lower:
            return 'chan-vay-nu', 'Chân váy nữ'
        elif 'DP' in sku_upper or 'quần' in name_lower or 'pants' in name_lower or 'jeans' in name_lower:
            return 'quan-nu', 'Quần nữ'
        elif 'BZ' in sku_upper or 'blazer' in name_lower or 'áo khoác' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'JK' in sku_upper or 'jacket' in name_lower:
            return 'ao-nu', 'Áo nữ'
        elif 'BE' in sku_upper or 'thắt lưng' in name_lower or 'belt' in name_lower:
            return 'phu-kien-nu', 'Phụ kiện nữ'
        elif 'BA' in sku_upper or 'balo' in name_lower or 'bag' in name_lower or 'túi' in name_lower:
            return 'phu-kien-nu', 'Phụ kiện nữ'
        elif 'SG' in sku_upper or 'mắt kính' in name_lower or 'kính' in name_lower:
            return 'phu-kien-nu', 'Phụ kiện nữ'
        else:
            return 'ao-nu', 'Áo nữ'  # Default for FW
    
    # Men's products
    elif 'WS' in sku_upper or 'áo sơ mi' in name_lower:
        return 'ao-nam', 'Áo nam'
    elif 'KS' in sku_upper or 'polo' in name_lower:
        return 'ao-nam', 'Áo nam'
    elif 'TS' in sku_upper or 'áo thun' in name_lower:
        return 'ao-nam', 'Áo nam'
    elif 'BZ' in sku_upper or 'blazer' in name_lower or 'vest' in name_lower:
        return 'ao-nam', 'Áo nam'
    elif 'JK' in sku_upper or 'jacket' in name_lower or 'áo khoác' in name_lower:
        return 'ao-nam', 'Áo nam'
    elif 'DP' in sku_upper or 'KP' in sku_upper or 'quần' in name_lower:
        return 'quan-nam', 'Quần nam'
    elif 'BE' in sku_upper or 'thắt lưng' in name_lower or 'belt' in name_lower:
        return 'phu-kien-nam', 'Phụ kiện nam'
    elif 'CT' in sku_upper or 'cà vạt' in name_lower or 'tie' in name_lower or 'cavat' in name_lower:
        return 'phu-kien-nam', 'Phụ kiện nam'
    elif 'BA' in sku_upper or 'balo' in name_lower:
        return 'phu-kien-nam', 'Phụ kiện nam'
    
    # Default
    return None, None

def main():
    print("=" * 80)
    print("TÌM SẢN PHẨM THIẾU VÀ TẠO SQL IMPORT SCRIPT")
    print("=" * 80)
    print()
    
    # Đọc dữ liệu
    csv_path = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/johnhenry_products.csv'
    db_export_path = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/all_products_export.csv'
    
    print("📄 Đọc file CSV...")
    csv_products = read_csv_products(csv_path)
    print(f"   ✓ CSV có {len(csv_products)} sản phẩm unique")
    
    print("📄 Đọc database export...")
    db_skus = read_db_export(db_export_path)
    print(f"   ✓ Database có {len(db_skus)} SKU")
    print()
    
    # Tìm sản phẩm thiếu
    missing_skus = set(csv_products.keys()) - db_skus
    
    print(f"🔍 Phân tích:")
    print(f"   - SKU trong CSV: {len(csv_products)}")
    print(f"   - SKU trong DB: {len(db_skus)}")
    print(f"   - SKU thiếu: {len(missing_skus)}")
    print()
    
    if not missing_skus:
        print("✅ Database đã có đầy đủ sản phẩm từ CSV!")
        print()
        
        # Check if DB has more products
        extra_skus = db_skus - set(csv_products.keys())
        if extra_skus:
            print(f"ℹ️  Database có thêm {len(extra_skus)} SKU không có trong CSV:")
            for sku in sorted(list(extra_skus)[:10]):
                print(f"   - {sku}")
            if len(extra_skus) > 10:
                print(f"   ... và {len(extra_skus) - 10} SKU khác")
        return
    
    print(f"📝 Danh sách {len(missing_skus)} SKU thiếu:")
    for sku in sorted(missing_skus):
        product = csv_products[sku]
        print(f"   - {sku}: {product['name']} ({product['price']:,.0f} VND)")
    print()
    
    # Tạo SQL script
    output_file = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/import_missing_products.sql'
    
    print(f"📝 Tạo SQL script: {output_file}")
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("-- SQL Script to import missing products\n")
        f.write("-- Generated automatically\n")
        f.write("-- Auto-generated\n")
        f.write("\n")
        f.write("-- First, get category and brand IDs (you may need to adjust these)\n")
        f.write("DO $$\n")
        f.write("DECLARE\n")
        f.write("    v_john_henry_brand_id UUID;\n")
        f.write("    v_freelancer_brand_id UUID;\n")
        
        # Collect all categories needed
        categories_needed = set()
        for sku in sorted(missing_skus):
            product = csv_products[sku]
            folder, cat_name = detect_category_from_sku(sku, product['name'])
            if cat_name:
                categories_needed.add(cat_name)
        
        for cat_name in sorted(categories_needed):
            var_name = cat_name.lower().replace(' ', '_').replace('á', 'a').replace('ă', 'a').replace('ầ', 'a').replace('ấ', 'a')
            var_name = var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
            var_name = var_name.replace('ú', 'u').replace('ư', 'u')
            var_name = var_name.replace('é', 'e').replace('ê', 'e')
            var_name = var_name.replace('í', 'i')
            f.write(f"    v_{var_name}_category_id UUID;\n")
        
        f.write("BEGIN\n")
        f.write("    -- Get Brand IDs\n")
        f.write('    SELECT "Id" INTO v_john_henry_brand_id FROM "Brands" WHERE "Name" = \'John Henry\' LIMIT 1;\n')
        f.write('    SELECT "Id" INTO v_freelancer_brand_id FROM "Brands" WHERE "Name" = \'Freelancer\' LIMIT 1;\n')
        f.write("\n")
        f.write("    -- Get Category IDs\n")
        
        for cat_name in sorted(categories_needed):
            var_name = cat_name.lower().replace(' ', '_').replace('á', 'a').replace('ă', 'a').replace('ầ', 'a').replace('ấ', 'a')
            var_name = var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
            var_name = var_name.replace('ú', 'u').replace('ư', 'u')
            var_name = var_name.replace('é', 'e').replace('ê', 'e')
            var_name = var_name.replace('í', 'i')
            f.write(f'    SELECT "Id" INTO v_{var_name}_category_id FROM "Categories" WHERE "Name" = \'{cat_name}\' LIMIT 1;\n')
        
        f.write("\n")
        f.write("    -- Insert missing products\n")
        
        for sku in sorted(missing_skus):
            product = csv_products[sku]
            slug = generate_slug(product['name'])
            folder, cat_name = detect_category_from_sku(sku, product['name'])
            
            # Determine brand
            brand_var = 'v_freelancer_brand_id' if sku.startswith('FW') else 'v_john_henry_brand_id'
            
            # Determine category variable
            if cat_name:
                cat_var_name = cat_name.lower().replace(' ', '_').replace('á', 'a').replace('ă', 'a').replace('ầ', 'a').replace('ấ', 'a')
                cat_var_name = cat_var_name.replace('ó', 'o').replace('ô', 'o').replace('ơ', 'o')
                cat_var_name = cat_var_name.replace('ú', 'u').replace('ư', 'u')
                cat_var_name = cat_var_name.replace('é', 'e').replace('ê', 'e')
                cat_var_name = cat_var_name.replace('í', 'i')
                category_var = f'v_{cat_var_name}_category_id'
            else:
                category_var = 'v_ao_nam_category_id'  # default
            
            # Image path
            image_path = f'~/images/{folder}/{sku}.jpg' if folder else None
            image_clause = f"'{image_path}'" if image_path else 'NULL'
            
            product_name = product['name'].replace("'", "''")
            f.write(f"\n    -- {sku}: {product['name']}\n")
            f.write(f"    INSERT INTO \"Products\" (\n")
            f.write(f"        \"Id\", \"Name\", \"Slug\", \"SKU\", \"Price\",\n")
            f.write(f"        \"StockQuantity\", \"InStock\", \"IsFeatured\", \"IsActive\",\n")
            f.write(f"        \"Rating\", \"ReviewCount\", \"CategoryId\", \"BrandId\",\n")
            f.write(f"        \"FeaturedImageUrl\", \"CreatedAt\", \"UpdatedAt\", \"Status\", \"ManageStock\"\n")
            f.write(f"    ) VALUES (\n")
            f.write(f"        gen_random_uuid(),\n")
            f.write(f"        '{product_name}'::text,\n")
            f.write(f"        '{slug}'::text,\n")
            f.write(f"        '{sku}'::text,\n")
            f.write(f"        {product['price']}::numeric,\n")
            f.write(f"        100,  -- StockQuantity\n")
            f.write(f"        true, -- InStock\n")
            f.write(f"        false, -- IsFeatured\n")
            f.write(f"        true,  -- IsActive\n")
            f.write(f"        0,     -- Rating\n")
            f.write(f"        0,     -- ReviewCount\n")
            f.write(f"        {category_var},\n")
            f.write(f"        {brand_var},\n")
            f.write(f"        {image_clause},\n")
            f.write(f"        NOW(), -- CreatedAt\n")
            f.write(f"        NOW(), -- UpdatedAt\n")
            f.write(f"        'active'::text,\n")
            f.write(f"        true   -- ManageStock\n")
            f.write(f"    ) ON CONFLICT (\"SKU\") DO NOTHING;\n")
        
        f.write("\n")
        f.write("    RAISE NOTICE 'Import completed!';\n")
        f.write("END $$;\n")
        f.write("\n")
        f.write("-- Verify import\n")
        f.write("SELECT COUNT(*) as imported_count FROM \"Products\" WHERE \"SKU\" IN (\n")
        for i, sku in enumerate(sorted(missing_skus)):
            f.write(f"    '{sku}'")
            if i < len(missing_skus) - 1:
                f.write(",\n")
            else:
                f.write("\n")
        f.write(");\n")
    
    print(f"✅ SQL script đã được tạo!")
    print(f"   File: {output_file}")
    print()
    print("📝 Để import, chạy lệnh:")
    print(f"   psql -U johnhenry -d johnhenry -f {output_file}")
    print()

if __name__ == "__main__":
    main()
