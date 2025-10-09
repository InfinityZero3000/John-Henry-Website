#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script để phân loại sản phẩm từ CSV và tạo SQL insert statements
Dựa vào SKU và tên sản phẩm để xác định category
"""

import csv
import os
import uuid
import re
from datetime import datetime
from typing import Dict, List, Tuple

# Định nghĩa categories với keywords và SKU patterns
CATEGORIES = {
    # Women's items (check first due to FW prefix)
    'Áo nữ': {
        'keywords': [
            'áo sơ mi nữ', 'áo thun nữ', 'áo polo nữ', 'áo khoác nữ',
            'áo blouse', 'áo kiểu nữ', 'áo blazer nữ', 'áo vest nữ',
            'áo len nữ', 'croptop', 'tank top nữ', 'tanktop nữ', 'freelancer',
        ],
        'sku_patterns': [
            r'^FWWS\d{2}',   # FWWS25FH = Áo sơ mi nữ
            r'^FWTS\d{2}',   # FWTS25FH = Áo thun nữ
            r'^FWKS\d{2}',   # FWKS25FH = Áo polo nữ
            r'^FWJK\d{2}',   # FWJK24FH = Áo khoác nữ
            r'^FWBZ\d{2}',   # FWBZ24FH = Áo blazer nữ
            r'^FWBL\d{2}',   # FWBL25FH = Áo blouse nữ
            r'^FWSW\d{2}',   # FWSW22FH = Áo len nữ
            r'^FWTT\d{2}',   # FWTT = Tank top nữ
        ],
        'exclude': []
    },
    'Quần nữ': {
        'keywords': [
            'quần tây nữ', 'quần âu nữ', 'quần short nữ', 'quần jeans nữ',
            'quần baggy',
        ],
        'sku_patterns': [
            r'^FWDP\d{2}',   # FWDP25FH = Quần tây nữ
            r'^FWSP\d{2}',   # FWSP25SS = Quần short nữ
            r'^FWJN\d{2}',   # FWJN25SS = Quần jeans nữ
        ],
        'exclude': []
    },
    'Chân váy nữ': {
        'keywords': [
            'chân váy', 'váy', 'skirt', 'mini skirt', 'midi', 'quần váy',
        ],
        'sku_patterns': [
            r'^FWSK\d{2}',   # FWSK25SS = Chân váy nữ
        ],
        'exclude': []
    },
    'Đầm nữ': {
        'keywords': [
            'đầm', 'dress', 'váy đầm',
        ],
        'sku_patterns': [
            r'^FWDR\d{2}',   # FWDR25SS = Đầm nữ
        ],
        'exclude': []
    },
    'Phụ kiện nữ': {
        'keywords': [
            'mắt kính nữ', 'túi nữ', 'balo nữ', 'ví nữ', 'thắt lưng nữ',
        ],
        'sku_patterns': [
            r'^FWSG\d{2}',   # FWSG = Mắt kính nữ
            r'^FWBE\d{2}',   # FWBE = Thắt lưng nữ
        ],
        'exclude': []
    },
    
    # Men's clothing (check before accessories)
    'Áo nam': {
        'keywords': [
            'áo sơ mi nam', 'áo thun nam', 'áo polo nam', 'áo khoác nam',
            'áo vest nam', 'áo blazer nam', 'áo len nam', 'sweater nam',
            'áo tanktop', 'tank top',
        ],
        'sku_patterns': [
            r'^WS\d{2}',     # WS25FH, WS24SS = Áo sơ mi nam
            r'^TS\d{2}',     # TS25FH, TS24SS = Áo thun nam
            r'^KS\d{2}',     # KS25FH, KS24SS = Áo polo nam
            r'^JK\d{2}',     # JK25FH, JK24SS = Áo khoác nam
            r'^BZ\d{2}',     # BZ25FH, BZ24SS = Áo blazer nam
            r'^SW\d{2}',     # SW25FH, SW24SS = Áo len nam
            r'^SS\d{2}',     # SS24TS = Áo thun set
            r'^TK\d{2}',     # TK23SS = Tank top nam
        ],
        'exclude': ['fwws', 'fwts', 'fwks', 'fwjk', 'fwbz', 'fwsw', 'fwbl']
    },
    'Quần nam': {
        'keywords': [
            'quần tây nam', 'quần âu nam', 'quần short nam', 'quần jeans nam',
            'quần kaki nam', 'quần chinos',
        ],
        'sku_patterns': [
            r'^DP\d{2}',     # DP25ES, DP24FH = Quần tây nam
            r'^SP\d{2}',     # SP25SS, SP24SS = Quần short nam
            r'^JN\d{2}',     # JN24FH, JN23SS = Quần jeans nam
            r'^KP\d{2}',     # KP25SS, KP24FH = Quần kaki nam
        ],
        'exclude': ['fwdp', 'fwsp', 'fwjn']
    },
    
    # Accessories (check last)
    'Phụ kiện nam': {
        'keywords': [
            'thắt lưng', 'cà vạt', 'belt', 'tie', 'balo', 'ví', 'vớ',
            'mắt kính', 'nón', 'giày', 'dép', 'sandal', 'đồng hồ',
        ],
        'sku_patterns': [
            r'^BE\d{2}',     # BE25FH = Thắt lưng (Belt)
            r'^CT\d{2}',     # CT24FH = Cà vạt (Cravat/Tie)
            r'^CB\d{2}',     # CB22FH = Cà vạt
            r'^BA\d{2}',     # BA24FH = Balo
            r'^WT\d{2}',     # WT25FH = Đồng hồ (Watch)
            r'^SK\d{2}',     # SK24FH = Vớ (Sock)
            r'^SO\d{2}',     # SO25FH = Giày/Sandal
            r'^SG\d{2}',     # SG24FH = Mắt kính (Sunglasses) - nam
            r'^CA\d{2}',     # CA26SS = Mũ/Nón (Cap)
        ],
        'exclude': ['fwbe', 'fwsg', 'nữ']
    },
}

# Mapping category to folder in wwwroot/images
CATEGORY_TO_FOLDER = {
    'Áo nam': 'Áo nam',
    'Áo nữ': 'Áo nữ',
    'Quần nam': 'Quần nam',
    'Quần nữ': 'Quần nữ',
    'Chân váy nữ': 'Chân váy nữ',
    'Đầm nữ': 'Đầm nữ',
    'Phụ kiện nam': 'Phụ kiện nam',
    'Phụ kiện nữ': 'Phụ kiện nữ',
}

def determine_category(sku: str, name: str) -> str:
    """Xác định category dựa vào SKU và tên sản phẩm"""
    sku_upper = sku.upper()
    name_lower = name.lower()
    
    # Duyệt qua các categories theo thứ tự (women first)
    for category_name, category_info in CATEGORIES.items():
        # Check exclude patterns first
        exclude_patterns = category_info.get('exclude', [])
        is_excluded = False
        for exclude in exclude_patterns:
            if exclude.lower() in sku_upper.lower() or exclude.lower() in name_lower:
                is_excluded = True
                break
        
        if is_excluded:
            continue
        
        # Check SKU patterns
        sku_patterns = category_info.get('sku_patterns', [])
        for pattern in sku_patterns:
            if re.match(pattern, sku_upper):
                return category_name
        
        # Check keywords in product name
        keywords = category_info.get('keywords', [])
        for keyword in keywords:
            if keyword.lower() in name_lower:
                return category_name
    
    # Fallback logic nếu không match được
    if 'đầm' in name_lower or 'dress' in name_lower:
        return 'Đầm nữ'
    elif 'váy' in name_lower or 'skirt' in name_lower:
        return 'Chân váy nữ'
    elif 'quần' in name_lower and ('nữ' in name_lower or sku_upper.startswith('FW')):
        return 'Quần nữ'
    elif 'quần' in name_lower:
        return 'Quần nam'
    elif 'áo' in name_lower and ('nữ' in name_lower or sku_upper.startswith('FW')):
        return 'Áo nữ'
    elif 'áo' in name_lower:
        return 'Áo nam'
    elif any(keyword in name_lower for keyword in ['thắt lưng', 'cà vạt', 'belt', 'balo', 'ví', 'vớ', 'giày', 'dép']):
        if 'nữ' in name_lower or sku_upper.startswith('FW'):
            return 'Phụ kiện nữ'
        return 'Phụ kiện nam'
    
    # Default
    return 'Áo nam'

def determine_brand(sku: str, name: str) -> str:
    """Xác định brand dựa vào SKU hoặc tên"""
    # Freelancer thường có SKU bắt đầu bằng FW
    if sku.startswith('FW'):
        return 'Freelancer'
    
    # Check trong tên
    name_lower = name.lower()
    if 'freelancer' in name_lower:
        return 'Freelancer'
    
    # Mặc định là John Henry
    return 'John Henry'

def generate_slug(name: str) -> str:
    """Tạo slug từ tên sản phẩm"""
    import re
    
    # Vietnamese to ASCII mapping
    vietnamese_map = {
        'à': 'a', 'á': 'a', 'ả': 'a', 'ã': 'a', 'ạ': 'a',
        'ă': 'a', 'ằ': 'a', 'ắ': 'a', 'ẳ': 'a', 'ẵ': 'a', 'ặ': 'a',
        'â': 'a', 'ầ': 'a', 'ấ': 'a', 'ẩ': 'a', 'ẫ': 'a', 'ậ': 'a',
        'è': 'e', 'é': 'e', 'ẻ': 'e', 'ẽ': 'e', 'ẹ': 'e',
        'ê': 'e', 'ề': 'e', 'ế': 'e', 'ể': 'e', 'ễ': 'e', 'ệ': 'e',
        'ì': 'i', 'í': 'i', 'ỉ': 'i', 'ĩ': 'i', 'ị': 'i',
        'ò': 'o', 'ó': 'o', 'ỏ': 'o', 'õ': 'o', 'ọ': 'o',
        'ô': 'o', 'ồ': 'o', 'ố': 'o', 'ổ': 'o', 'ỗ': 'o', 'ộ': 'o',
        'ơ': 'o', 'ờ': 'o', 'ớ': 'o', 'ở': 'o', 'ỡ': 'o', 'ợ': 'o',
        'ù': 'u', 'ú': 'u', 'ủ': 'u', 'ũ': 'u', 'ụ': 'u',
        'ư': 'u', 'ừ': 'u', 'ứ': 'u', 'ử': 'u', 'ữ': 'u', 'ự': 'u',
        'ỳ': 'y', 'ý': 'y', 'ỷ': 'y', 'ỹ': 'y', 'ỵ': 'y',
        'đ': 'd',
        'À': 'A', 'Á': 'A', 'Ả': 'A', 'Ã': 'A', 'Ạ': 'A',
        'Ă': 'A', 'Ằ': 'A', 'Ắ': 'A', 'Ẳ': 'A', 'Ẵ': 'A', 'Ặ': 'A',
        'Â': 'A', 'Ầ': 'A', 'Ấ': 'A', 'Ẩ': 'A', 'Ẫ': 'A', 'Ậ': 'A',
        'È': 'E', 'É': 'E', 'Ẻ': 'E', 'Ẽ': 'E', 'Ẹ': 'E',
        'Ê': 'E', 'Ề': 'E', 'Ế': 'E', 'Ể': 'E', 'Ễ': 'E', 'Ệ': 'E',
        'Ì': 'I', 'Í': 'I', 'Ỉ': 'I', 'Ĩ': 'I', 'Ị': 'I',
        'Ò': 'O', 'Ó': 'O', 'Ỏ': 'O', 'Õ': 'O', 'Ọ': 'O',
        'Ô': 'O', 'Ồ': 'O', 'Ố': 'O', 'Ổ': 'O', 'Ỗ': 'O', 'Ộ': 'O',
        'Ơ': 'O', 'Ờ': 'O', 'Ớ': 'O', 'Ở': 'O', 'Ỡ': 'O', 'Ợ': 'O',
        'Ù': 'U', 'Ú': 'U', 'Ủ': 'U', 'Ũ': 'U', 'Ụ': 'U',
        'Ư': 'U', 'Ừ': 'U', 'Ứ': 'U', 'Ử': 'U', 'Ữ': 'U', 'Ự': 'U',
        'Ỳ': 'Y', 'Ý': 'Y', 'Ỷ': 'Y', 'Ỹ': 'Y', 'Ỵ': 'Y',
        'Đ': 'D',
    }
    
    # Convert Vietnamese characters
    slug = name.lower()
    for viet, ascii_char in vietnamese_map.items():
        slug = slug.replace(viet, ascii_char)
    
    # Remove special characters and replace spaces with hyphens
    slug = re.sub(r'[^a-z0-9\s-]', '', slug)
    slug = re.sub(r'\s+', '-', slug)
    slug = re.sub(r'-+', '-', slug)
    slug = slug.strip('-')
    
    return slug

def check_image_exists(sku: str, category: str, base_path: str) -> Tuple[bool, str]:
    """Kiểm tra xem file ảnh có tồn tại không"""
    folder = CATEGORY_TO_FOLDER.get(category, category)
    image_folder = os.path.join(base_path, 'wwwroot', 'images', folder)
    
    # Thử các extension khác nhau
    extensions = ['.jpg', '.jpeg', '.png', '.webp', '.JPG', '.JPEG', '.PNG']
    
    for ext in extensions:
        image_filename = f"{sku}{ext}"
        image_path = os.path.join(image_folder, image_filename)
        
        if os.path.exists(image_path):
            return True, f"/images/{folder}/{image_filename}"
    
    return False, f"/images/default-product.jpg"

def read_csv_and_generate_sql(csv_file: str, base_path: str) -> Tuple[List[Dict], str]:
    """Đọc CSV và tạo SQL insert statements"""
    
    products = []
    categories_set = set()
    brands_set = set()
    
    # Read CSV
    with open(csv_file, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        
        for row in reader:
            sku = row['sku'].strip()
            name = row['name'].strip()
            price = float(row['price'].replace(',', ''))
            
            # Determine category and brand
            category = determine_category(sku, name)
            brand = determine_brand(sku, name)
            
            categories_set.add(category)
            brands_set.add(brand)
            
            # Generate slug
            slug = generate_slug(name)
            
            # Check image
            has_image, image_url = check_image_exists(sku, category, base_path)
            
            product = {
                'id': str(uuid.uuid4()),
                'sku': sku,
                'name': name,
                'slug': slug,
                'price': price,
                'category': category,
                'brand': brand,
                'image_url': image_url,
                'has_image': has_image,
            }
            
            products.append(product)
    
    # Generate SQL
    sql_parts = []
    
    # Header
    sql_parts.append("-- =====================================================")
    sql_parts.append("-- SQL Script để thêm sản phẩm John Henry & Freelancer")
    sql_parts.append(f"-- Tạo ngày: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    sql_parts.append(f"-- Tổng số sản phẩm: {len(products)}")
    sql_parts.append("-- =====================================================")
    sql_parts.append("")
    
    # Categories section
    sql_parts.append("-- 1. Tạo Categories (nếu chưa tồn tại)")
    sql_parts.append("-- =====================================================")
    
    category_ids = {}
    for category in sorted(categories_set):
        cat_id = str(uuid.uuid4())
        category_ids[category] = cat_id
        cat_slug = generate_slug(category)
        
        sql_parts.append(f"""
INSERT INTO "Categories" ("Id", "Name", "Slug", "Description", "IsActive", "SortOrder", "CreatedAt", "UpdatedAt")
SELECT '{cat_id}', '{category}', '{cat_slug}', 'Danh mục {category}', true, 0, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM "Categories" WHERE "Slug" = '{cat_slug}');
""")
    
    sql_parts.append("")
    sql_parts.append("-- 2. Tạo Brands (nếu chưa tồn tại)")
    sql_parts.append("-- =====================================================")
    
    brand_ids = {}
    for brand in sorted(brands_set):
        brand_id = str(uuid.uuid4())
        brand_ids[brand] = brand_id
        brand_slug = generate_slug(brand)
        
        sql_parts.append(f"""
INSERT INTO "Brands" ("Id", "Name", "Slug", "Description", "IsActive", "CreatedAt", "UpdatedAt")
SELECT '{brand_id}', '{brand}', '{brand_slug}', 'Thương hiệu {brand}', true, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM "Brands" WHERE "Slug" = '{brand_slug}');
""")
    
    sql_parts.append("")
    sql_parts.append("-- 3. Thêm sản phẩm")
    sql_parts.append("-- =====================================================")
    
    for product in products:
        cat_slug = generate_slug(product['category'])
        brand_slug = generate_slug(product['brand'])
        
        # Escape single quotes in name
        safe_name = product['name'].replace("'", "''")
        
        sql_parts.append(f"""
-- {product['name']} ({product['sku']})
INSERT INTO "Products" (
    "Id", "Name", "Slug", "SKU", "Price", "StockQuantity", 
    "ManageStock", "InStock", "FeaturedImageUrl", "IsFeatured", 
    "IsActive", "Status", "ViewCount", "Rating", "ReviewCount", 
    "CategoryId", "BrandId", "CreatedAt", "UpdatedAt"
)
SELECT 
    '{product['id']}',
    '{safe_name}',
    '{product['slug']}-{product['sku'].lower()}',
    '{product['sku']}',
    {product['price']},
    100,
    true,
    true,
    '{product['image_url']}',
    false,
    true,
    'active',
    0,
    0,
    0,
    (SELECT "Id" FROM "Categories" WHERE "Slug" = '{cat_slug}' LIMIT 1),
    (SELECT "Id" FROM "Brands" WHERE "Slug" = '{brand_slug}' LIMIT 1),
    CURRENT_TIMESTAMP,
    CURRENT_TIMESTAMP
WHERE NOT EXISTS (SELECT 1 FROM "Products" WHERE "SKU" = '{product['sku']}');
""")
    
    sql_parts.append("")
    sql_parts.append("-- =====================================================")
    sql_parts.append("-- Hoàn tất!")
    sql_parts.append(f"-- Tổng số categories: {len(categories_set)}")
    sql_parts.append(f"-- Tổng số brands: {len(brands_set)}")
    sql_parts.append(f"-- Tổng số products: {len(products)}")
    sql_parts.append("-- =====================================================")
    
    return products, '\n'.join(sql_parts)

def generate_summary_report(products: List[Dict]) -> str:
    """Tạo báo cáo tóm tắt"""
    report = []
    
    report.append("=" * 80)
    report.append("BÁO CÁO PHÂN LOẠI SẢN PHẨM")
    report.append("=" * 80)
    report.append("")
    
    # Count by category
    category_counts = {}
    for product in products:
        cat = product['category']
        category_counts[cat] = category_counts.get(cat, 0) + 1
    
    report.append("1. THỐNG KÊ THEO DANH MỤC:")
    report.append("-" * 80)
    for cat, count in sorted(category_counts.items(), key=lambda x: x[1], reverse=True):
        report.append(f"   {cat:30s}: {count:4d} sản phẩm")
    
    # Count by brand
    brand_counts = {}
    for product in products:
        brand = product['brand']
        brand_counts[brand] = brand_counts.get(brand, 0) + 1
    
    report.append("")
    report.append("2. THỐNG KÊ THEO THƯƠNG HIỆU:")
    report.append("-" * 80)
    for brand, count in sorted(brand_counts.items()):
        report.append(f"   {brand:30s}: {count:4d} sản phẩm")
    
    # Image status
    has_image_count = sum(1 for p in products if p['has_image'])
    no_image_count = len(products) - has_image_count
    
    report.append("")
    report.append("3. TRẠNG THÁI HÌNH ẢNH:")
    report.append("-" * 80)
    report.append(f"   Có hình ảnh:    {has_image_count:4d} sản phẩm ({has_image_count*100//len(products)}%)")
    report.append(f"   Chưa có ảnh:    {no_image_count:4d} sản phẩm ({no_image_count*100//len(products)}%)")
    
    # Products without images
    if no_image_count > 0:
        report.append("")
        report.append("4. SẢN PHẨM CHƯA CÓ HÌNH ẢNH:")
        report.append("-" * 80)
        for product in products:
            if not product['has_image']:
                report.append(f"   - {product['sku']:20s}: {product['name']}")
    
    report.append("")
    report.append("=" * 80)
    
    return '\n'.join(report)

def main():
    """Main function"""
    # Paths
    script_dir = os.path.dirname(os.path.abspath(__file__))
    base_path = os.path.dirname(script_dir)
    csv_file = os.path.join(script_dir, 'johnhenry_products.csv')
    
    print(f"Đang xử lý file: {csv_file}")
    print(f"Base path: {base_path}")
    print("")
    
    # Generate SQL
    products, sql_content = read_csv_and_generate_sql(csv_file, base_path)
    
    # Save SQL file
    output_sql = os.path.join(script_dir, 'insert_products_from_csv.sql')
    with open(output_sql, 'w', encoding='utf-8') as f:
        f.write(sql_content)
    
    print(f"✓ Đã tạo file SQL: {output_sql}")
    
    # Generate and save report
    report = generate_summary_report(products)
    output_report = os.path.join(script_dir, 'PRODUCT_CLASSIFICATION_REPORT.md')
    with open(output_report, 'w', encoding='utf-8') as f:
        f.write(report)
    
    print(f"✓ Đã tạo báo cáo: {output_report}")
    print("")
    print(report)

if __name__ == '__main__':
    main()
