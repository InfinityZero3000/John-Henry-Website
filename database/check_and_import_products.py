#!/usr/bin/env python3
"""
Script để kiểm tra và import sản phẩm còn thiếu vào database
"""

import csv
import os
import psycopg2
from datetime import datetime
import sys

# Thông tin kết nối database
DB_CONFIG = {
    'host': 'localhost',
    'port': '5432',
    'database': 'johnhenry',
    'user': 'johnhenry',
    'password': 'johnhenry123'
}

def connect_db():
    """Kết nối đến database PostgreSQL"""
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        return conn
    except Exception as e:
        print(f"❌ Không thể kết nối database: {e}")
        sys.exit(1)

def get_category_mapping(conn):
    """Lấy mapping từ tên thư mục ảnh sang category ID"""
    cursor = conn.cursor()
    
    # Tạo mapping từ folder name sang category
    folder_to_category = {
        'ao-nam': 'Áo nam',
        'ao-nu': 'Áo nữ', 
        'quan-nam': 'Quần nam',
        'quan-nu': 'Quần nữ',
        'chan-vay-nu': 'Chân váy nữ',
        'dam-nu': 'Đầm nữ',
        'phu-kien-nam': 'Phụ kiện nam',
        'phu-kien-nu': 'Phụ kiện nữ'
    }
    
    category_map = {}
    for folder, cat_name in folder_to_category.items():
        cursor.execute("""
            SELECT "Id", "Name" FROM "Categories" 
            WHERE LOWER("Name") LIKE %s OR "Slug" LIKE %s
            LIMIT 1
        """, (f'%{cat_name.lower()}%', f'%{folder}%'))
        result = cursor.fetchone()
        if result:
            category_map[folder] = result[0]
            print(f"✓ Mapping: {folder} -> {cat_name} (ID: {result[0]})")
    
    cursor.close()
    return category_map, folder_to_category

def get_brand_id(conn, brand_name='John Henry'):
    """Lấy brand ID"""
    cursor = conn.cursor()
    cursor.execute('SELECT "Id" FROM "Brands" WHERE "Name" = %s LIMIT 1', (brand_name,))
    result = cursor.fetchone()
    cursor.close()
    
    if result:
        print(f"✓ Brand '{brand_name}' ID: {result[0]}")
        return result[0]
    else:
        print(f"❌ Không tìm thấy brand '{brand_name}'")
        return None

def get_existing_skus(conn):
    """Lấy danh sách SKU đã có trong database"""
    cursor = conn.cursor()
    cursor.execute('SELECT "SKU" FROM "Products"')
    existing_skus = set(row[0] for row in cursor.fetchall())
    cursor.close()
    print(f"✓ Database hiện có {len(existing_skus)} sản phẩm")
    return existing_skus

def scan_product_images(base_path='/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/wwwroot/images'):
    """Scan các thư mục ảnh và lấy thông tin SKU"""
    folders = ['ao-nam', 'ao-nu', 'quan-nam', 'quan-nu', 'chan-vay-nu', 'dam-nu', 'phu-kien-nam', 'phu-kien-nu']
    image_products = {}
    
    for folder in folders:
        folder_path = os.path.join(base_path, folder)
        if not os.path.exists(folder_path):
            print(f"⚠️  Folder không tồn tại: {folder_path}")
            continue
        
        files = [f for f in os.listdir(folder_path) 
                if f.lower().endswith(('.jpg', '.jpeg', '.png', '.gif', '.webp'))]
        
        print(f"✓ {folder}: {len(files)} ảnh")
        
        for file in files:
            # Extract SKU from filename (assuming format: SKU.jpg hoặc SKU-1.jpg)
            sku = file.split('.')[0].split('-')[0] if '-' in file else file.split('.')[0]
            sku = sku.upper()
            
            if sku not in image_products:
                image_products[sku] = {
                    'folder': folder,
                    'images': []
                }
            image_products[sku]['images'].append(file)
    
    return image_products

def read_csv_products(csv_path):
    """Đọc dữ liệu từ CSV file"""
    products = {}
    
    if not os.path.exists(csv_path):
        print(f"❌ File CSV không tồn tại: {csv_path}")
        return products
    
    with open(csv_path, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        for row in reader:
            sku = row['sku'].strip().upper()
            products[sku] = {
                'name': row['name'].strip(),
                'price': float(row['price'].replace(',', ''))
            }
    
    print(f"✓ CSV file có {len(products)} sản phẩm")
    return products

def import_missing_products(conn, csv_products, image_products, category_map, folder_to_category, brand_id):
    """Import các sản phẩm còn thiếu"""
    existing_skus = get_existing_skus(conn)
    cursor = conn.cursor()
    
    imported = 0
    skipped = 0
    errors = 0
    
    # Default category nếu không xác định được
    cursor.execute('SELECT "Id" FROM "Categories" LIMIT 1')
    default_category_id = cursor.fetchone()[0]
    
    for sku, product_info in csv_products.items():
        if sku in existing_skus:
            skipped += 1
            continue
        
        # Tìm category dựa trên image folder
        category_id = default_category_id
        featured_image = None
        
        if sku in image_products:
            folder = image_products[sku]['folder']
            category_id = category_map.get(folder, default_category_id)
            if image_products[sku]['images']:
                featured_image = f"~/images/{folder}/{image_products[sku]['images'][0]}"
        
        try:
            # Generate slug
            slug = product_info['name'].lower().replace(' ', '-')
            slug = ''.join(c for c in slug if c.isalnum() or c == '-')
            
            now = datetime.utcnow()
            
            cursor.execute("""
                INSERT INTO "Products" 
                ("Id", "Name", "Slug", "SKU", "Price", "StockQuantity", "InStock", 
                 "IsFeatured", "IsActive", "Rating", "ReviewCount", "CategoryId", "BrandId",
                 "FeaturedImageUrl", "CreatedAt", "UpdatedAt", "Status", "ManageStock")
                VALUES 
                (gen_random_uuid(), %s, %s, %s, %s, 100, true, false, true, 0, 0, %s, %s, %s, %s, %s, 'active', true)
            """, (
                product_info['name'],
                slug,
                sku,
                product_info['price'],
                category_id,
                brand_id,
                featured_image,
                now,
                now
            ))
            
            imported += 1
            print(f"✓ Imported: {sku} - {product_info['name']}")
            
        except Exception as e:
            errors += 1
            print(f"❌ Error importing {sku}: {e}")
    
    conn.commit()
    cursor.close()
    
    return imported, skipped, errors

def main():
    print("=" * 80)
    print("KIỂM TRA VÀ IMPORT SẢN PHẨM VÀO DATABASE")
    print("=" * 80)
    print()
    
    # 1. Kiểm tra số ảnh trong các folder
    print("📁 KIỂM TRA ẢNH SẢN PHẨM:")
    print("-" * 80)
    image_products = scan_product_images()
    print(f"\n✓ Tổng số SKU từ ảnh: {len(image_products)}")
    print()
    
    # 2. Đọc CSV files
    print("📄 KIỂM TRA DỮ LIỆU CSV:")
    print("-" * 80)
    csv_path = '/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/johnhenry_products.csv'
    csv_products = read_csv_products(csv_path)
    print()
    
    # 3. Kết nối database và kiểm tra
    print("🗄️  KẾT NỐI DATABASE:")
    print("-" * 80)
    conn = connect_db()
    print("✓ Kết nối database thành công")
    
    existing_skus = get_existing_skus(conn)
    print()
    
    # 4. Lấy category và brand mapping
    print("🏷️  LẤY THÔNG TIN CATEGORIES VÀ BRANDS:")
    print("-" * 80)
    category_map, folder_to_category = get_category_mapping(conn)
    brand_id = get_brand_id(conn)
    print()
    
    # 5. Tìm sản phẩm còn thiếu
    print("🔍 PHÂN TÍCH DỮ LIỆU:")
    print("-" * 80)
    missing_skus = set(csv_products.keys()) - existing_skus
    print(f"✓ CSV có: {len(csv_products)} sản phẩm")
    print(f"✓ Database có: {len(existing_skus)} sản phẩm")
    print(f"✓ Còn thiếu: {len(missing_skus)} sản phẩm")
    print()
    
    if missing_skus:
        print(f"📝 Danh sách SKU còn thiếu (10 đầu tiên):")
        for i, sku in enumerate(list(missing_skus)[:10]):
            print(f"  {i+1}. {sku}")
        if len(missing_skus) > 10:
            print(f"  ... và {len(missing_skus) - 10} SKU khác")
        print()
        
        # 6. Import sản phẩm
        response = input("❓ Bạn có muốn import các sản phẩm còn thiếu? (y/n): ")
        if response.lower() == 'y':
            print()
            print("📥 IMPORT SẢN PHẨM:")
            print("-" * 80)
            imported, skipped, errors = import_missing_products(
                conn, csv_products, image_products, category_map, folder_to_category, brand_id
            )
            
            print()
            print("=" * 80)
            print("KẾT QUẢ IMPORT:")
            print(f"  ✓ Imported: {imported} sản phẩm")
            print(f"  ⊘ Skipped: {skipped} sản phẩm (đã tồn tại)")
            print(f"  ✗ Errors: {errors} sản phẩm")
            print("=" * 80)
    else:
        print("✓ Không có sản phẩm thiếu. Database đã đầy đủ!")
    
    conn.close()
    print()
    print("✓ Hoàn thành!")

if __name__ == "__main__":
    main()
