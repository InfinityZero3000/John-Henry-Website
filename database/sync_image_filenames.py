#!/usr/bin/env python3
"""
Script để đồng bộ tên file ảnh từ local vào database.
Dựa trên SKU có trong tên file để match với products trong database.
"""

import os
import re
import psycopg2
from pathlib import Path

# Database connection
DB_CONFIG = {
    'host': 'localhost',
    'port': 5432,
    'database': 'johnhenry_db',
    'user': 'johnhenry_user',
    'password': 'johnhenry_password'
}

# Image folders base path
IMAGES_PATH = Path(__file__).parent.parent / 'wwwroot' / 'images'

# Folder mappings (Vietnamese to non-accented)
FOLDERS = [
    'ao-nam',
    'ao-nu', 
    'quan-nam',
    'quan-nu',
    'dam-nu',
    'chan-vay-nu',
    'phu-kien-nam',
    'phu-kien-nu'
]

def extract_sku_from_filename(filename):
    """
    Extract SKU pattern from filename.
    SKU patterns: KS25FH57C, WS24SS16P, BE25FH49, CA26SS14P, etc.
    """
    # Remove .jpg extension
    name = filename.replace('.jpg', '').replace('.png', '')
    
    # Common SKU patterns (2-4 letters + 2 digits + 2-4 letters/digits + optional suffix)
    # Examples: KS25FH57C-SC, WS24SS16P-LCRG, BE25FH49-EP
    patterns = [
        r'([A-Z]{2,4}\d{2}[A-Z]{2}\d{2}[A-Z]?)(?:-.*)?',  # KS25FH57C-SC
        r'([A-Z]{2,4}\d{2}[A-Z]{2}\d{2})(?:-.*)?',        # BE25FH49-EP
        r'([A-Z]{2}\d{2}[A-Z]{2}\d{2}[A-Z])(?:-.*)?',     # Standard format
    ]
    
    for pattern in patterns:
        match = re.search(pattern, name)
        if match:
            return match.group(1)
    
    return None

def get_all_image_files():
    """Scan all image folders and return dict of {folder: [files]}"""
    result = {}
    
    for folder in FOLDERS:
        folder_path = IMAGES_PATH / folder
        if folder_path.exists():
            files = [f for f in os.listdir(folder_path) 
                    if f.endswith(('.jpg', '.png', '.jpeg')) and not f.startswith('.')]
            result[folder] = files
            print(f"✓ Found {len(files)} images in {folder}")
    
    return result

def get_products_with_vietnamese_filenames(conn):
    """Get all products that have Vietnamese characters in FeaturedImageUrl"""
    cursor = conn.cursor()
    
    # Use single quotes inside the triple-quoted string to avoid escaping issues
    query = """
        SELECT "Id", "Name", "SKU", "FeaturedImageUrl"
        FROM "Products"
        WHERE "FeaturedImageUrl" ~ '[^\\x00-\\x7F]'
        ORDER BY "FeaturedImageUrl";
    """
    
    cursor.execute(query)
    products = cursor.fetchall()
    cursor.close()
    
    return products

def find_matching_file(product_sku, product_url, image_files_dict):
    """
    Find the matching image file for a product based on SKU.
    Returns (folder, filename) or None
    """
    # Extract folder from current URL
    # Example: /images/phu-kien-nam/Mũ Lưỡi Trai.jpg -> phu-kien-nam
    url_parts = product_url.split('/')
    if len(url_parts) < 3:
        return None
    
    folder = url_parts[2] if url_parts[1] == 'images' else url_parts[1].replace('~', '').replace('images', '').strip('/')
    
    if folder not in image_files_dict:
        return None
    
    files = image_files_dict[folder]
    
    # Try exact SKU match first
    if product_sku:
        sku_clean = product_sku.strip()
        for filename in files:
            file_sku = extract_sku_from_filename(filename)
            if file_sku and file_sku == sku_clean:
                return (folder, filename)
    
    return None

def generate_update_sql(matches):
    """Generate SQL UPDATE statements"""
    if not matches:
        return ""
    
    sql_lines = ["BEGIN;\n"]
    
    for product_id, old_url, new_url in matches:
        sql = f"""UPDATE "Products" 
SET "FeaturedImageUrl" = '{new_url}'
WHERE "Id" = '{product_id}';
"""
        sql_lines.append(sql)
    
    sql_lines.append("\nCOMMIT;\n")
    
    # Add verification
    sql_lines.append("\n-- Verification")
    sql_lines.append("\nSELECT COUNT(*) as remaining_vietnamese_filenames")
    sql_lines.append("FROM \"Products\"")
    sql_lines.append("WHERE \"FeaturedImageUrl\" ~ '[^\\x00-\\x7F]';")
    
    return '\n'.join(sql_lines)

def main():
    print("=" * 80)
    print("SYNC IMAGE FILENAMES FROM LOCAL TO DATABASE")
    print("=" * 80)
    
    # 1. Scan local image files
    print("\n[1] Scanning local image files...")
    image_files = get_all_image_files()
    total_files = sum(len(files) for files in image_files.values())
    print(f"Total files found: {total_files}")
    
    # 2. Connect to database
    print("\n[2] Connecting to database...")
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        print("✓ Connected successfully")
    except Exception as e:
        print(f"✗ Connection failed: {e}")
        return
    
    # 3. Get products with Vietnamese filenames
    print("\n[3] Getting products with Vietnamese filenames...")
    products = get_products_with_vietnamese_filenames(conn)
    print(f"Found {len(products)} products with Vietnamese filenames")
    
    # 4. Match products with local files
    print("\n[4] Matching products with local files...")
    matches = []
    not_matched = []
    
    for product_id, name, sku, old_url in products:
        result = find_matching_file(sku, old_url, image_files)
        
        if result:
            folder, filename = result
            new_url = f"/images/{folder}/{filename}"
            matches.append((product_id, old_url, new_url))
            print(f"✓ {sku:20s} -> {filename}")
        else:
            not_matched.append((product_id, name, sku, old_url))
            print(f"✗ {sku:20s} -> NOT FOUND")
    
    print(f"\n✓ Matched: {len(matches)}")
    print(f"✗ Not matched: {len(not_matched)}")
    
    # 5. Generate SQL
    if matches:
        print("\n[5] Generating SQL update script...")
        sql_content = generate_update_sql(matches)
        
        output_file = Path(__file__).parent / 'sync_image_filenames.sql'
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(sql_content)
        
        print(f"✓ SQL script saved to: {output_file}")
        print(f"  Updates: {len(matches)} products")
    
    # 6. Show not matched products
    if not_matched:
        print("\n[6] Products not matched (need manual review):")
        print("-" * 80)
        for product_id, name, sku, old_url in not_matched[:20]:  # Show first 20
            print(f"  SKU: {sku:20s} | {name[:50]}")
            print(f"       {old_url}")
        
        if len(not_matched) > 20:
            print(f"  ... and {len(not_matched) - 20} more")
    
    conn.close()
    print("\n" + "=" * 80)
    print("DONE!")
    print("=" * 80)
    
    if matches:
        print("\nNext steps:")
        print("1. Review the generated SQL file: sync_image_filenames.sql")
        print("2. Run: psql -h localhost -U johnhenry_user -d johnhenry_db -f sync_image_filenames.sql")

if __name__ == '__main__':
    main()
