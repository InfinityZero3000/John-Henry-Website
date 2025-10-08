#!/usr/bin/env python3
"""
Script tự động tạo SQL seed data cho tất cả sản phẩm
dựa trên tên file ảnh trong các thư mục wwwroot/images
"""

import os
import re
from pathlib import Path

# Base path
base_path = Path("/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/wwwroot/images")

# Categories mapping
categories = {
    "Áo nam": {
        "category_id": "11111111-1111-1111-1111-111111111111",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "John Henry",
        "gender": "Nam"
    },
    "Áo nữ": {
        "category_id": "22222222-2222-2222-2222-222222222222",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "Freelancer",
        "gender": "Nữ"
    },
    "Chân váy nữ": {
        "category_id": "22222222-2222-2222-2222-222222222222",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "Freelancer",
        "gender": "Nữ"
    },
    "Đầm nữ": {
        "category_id": "22222222-2222-2222-2222-222222222222",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "Freelancer",
        "gender": "Nữ"
    },
    "Phụ kiện nam": {
        "category_id": "11111111-1111-1111-1111-111111111111",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "John Henry",
        "gender": "Nam"
    },
    "Phụ kiện nữ": {
        "category_id": "22222222-2222-2222-2222-222222222222",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "Freelancer",
        "gender": "Nữ"
    },
    "Quần nam": {
        "category_id": "11111111-1111-1111-1111-111111111111",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "John Henry",
        "gender": "Nam"
    },
    "Quần nữ": {
        "category_id": "22222222-2222-2222-2222-222222222222",
        "brand_id": "33333333-3333-3333-3333-333333333333",
        "collection": "Freelancer",
        "gender": "Nữ"
    }
}

# Price mapping based on product type
def get_price_info(product_name):
    """Get price and sale price based on product type"""
    name_lower = product_name.lower()
    
    # Áo
    if any(x in name_lower for x in ['áo thun', 'áo tanktop']):
        return (280000, 252000, 100)
    elif 'áo polo' in name_lower:
        return (420000, 378000, 70)
    elif 'áo sơ mi' in name_lower:
        return (480000, 432000, 60)
    elif 'áo blouse' in name_lower or 'áo kiểu' in name_lower:
        return (550000, 495000, 55)
    elif 'áo hoodie' in name_lower or 'áo khoác' in name_lower:
        return (1050000, 945000, 40)
    elif 'áo len' in name_lower:
        return (700000, 630000, 35)
    
    # Quần
    elif 'quần jeans' in name_lower or 'quần jean' in name_lower:
        return (650000, 585000, 50)
    elif 'quần khaki' in name_lower or 'quần tây' in name_lower:
        return (580000, 522000, 55)
    elif 'quần short' in name_lower:
        return (420000, 378000, 70)
    elif 'quần jogger' in name_lower:
        return (520000, 468000, 45)
    
    # Váy và Đầm
    elif 'chân váy' in name_lower:
        return (520000, 468000, 50)
    elif 'đầm' in name_lower:
        return (750000, 675000, 40)
    
    # Phụ kiện
    elif any(x in name_lower for x in ['dép', 'giày']):
        return (450000, 405000, 60)
    elif 'mũ' in name_lower:
        return (280000, 252000, 80)
    elif 'thắt lưng' in name_lower or 'belt' in name_lower:
        return (350000, 315000, 70)
    elif 'ví' in name_lower or 'wallet' in name_lower:
        return (550000, 495000, 50)
    elif 'túi' in name_lower or 'bag' in name_lower:
        return (680000, 612000, 45)
    elif 'mắt kính' in name_lower:
        return (450000, 405000, 55)
    
    # Default
    return (500000, 450000, 50)

def extract_sku(filename):
    """Extract SKU from filename - must be unique"""
    import hashlib
    
    # Create base SKU from filename (without .jpg)
    sku = filename.replace('.jpg', '').strip()
    
    # Remove special characters except dash, plus, spaces
    sku = re.sub(r'[^\w\s\-\+]', '', sku)
    
    # Replace spaces with dash
    sku = re.sub(r'\s+', '-', sku)
    
    # Remove multiple consecutive dashes
    sku = re.sub(r'-+', '-', sku).strip('-')
    
    # Convert to uppercase
    sku = sku.upper()
    
    # If SKU is too long, truncate and add hash suffix for uniqueness
    if len(sku) > 45:
        # Create a short hash from the original filename
        hash_suffix = hashlib.md5(filename.encode()).hexdigest()[:4].upper()
        sku = sku[:45] + '-' + hash_suffix
    
    return sku

def get_sizes(product_name, gender):
    """Get size range based on product type"""
    name_lower = product_name.lower()
    
    if gender == "Nam":
        if any(x in name_lower for x in ['áo', 'jacket', 'hoodie']):
            return "M,L,XL,XXL"
        elif 'quần' in name_lower:
            return "29,30,31,32,33"
        elif any(x in name_lower for x in ['giày', 'dép']):
            return "39,40,41,42,43"
        else:
            return "Free Size"
    else:  # Nữ
        if any(x in name_lower for x in ['áo', 'dress', 'đầm']):
            return "S,M,L,XL"
        elif 'váy' in name_lower or 'quần' in name_lower:
            return "S,M,L"
        elif any(x in name_lower for x in ['giày', 'dép']):
            return "35,36,37,38,39"
        else:
            return "Free Size"

def get_colors(product_name):
    """Get color options"""
    # Extract color from name if present
    colors = []
    name_lower = product_name.lower()
    
    color_map = {
        'trắng': 'Trắng',
        'đen': 'Đen',
        'xanh': 'Xanh navy',
        'xám': 'Xám',
        'nâu': 'Nâu',
        'be': 'Be',
        'hồng': 'Hồng',
    }
    
    for key, value in color_map.items():
        if key in name_lower:
            colors.append(value)
    
    if not colors:
        # Default colors
        colors = ['Trắng', 'Đen', 'Xám']
    
    return ','.join(colors)

def generate_product_sql(product_name, sku, image_path, category_info):
    """Generate SQL INSERT statement for a product"""
    price, sale_price, stock = get_price_info(product_name)
    sizes = get_sizes(product_name, category_info["gender"])
    colors = get_colors(product_name)
    
    # Create slug
    slug = product_name.lower()
    slug = re.sub(r'[àáạảãâầấậẩẫăằắặẳẵ]', 'a', slug)
    slug = re.sub(r'[èéẹẻẽêềếệểễ]', 'e', slug)
    slug = re.sub(r'[ìíịỉĩ]', 'i', slug)
    slug = re.sub(r'[òóọỏõôồốộổỗơờớợởỡ]', 'o', slug)
    slug = re.sub(r'[ùúụủũưừứựửữ]', 'u', slug)
    slug = re.sub(r'[ỳýỵỷỹ]', 'y', slug)
    slug = re.sub(r'[đ]', 'd', slug)
    slug = re.sub(r'[^a-z0-9\s-]', '', slug)
    slug = re.sub(r'\s+', '-', slug)
    slug = re.sub(r'-+', '-', slug).strip('-')
    
    # Generate description
    description = f"{product_name} - Sản phẩm thuộc bộ sưu tập {category_info['collection']}, thiết kế hiện đại, chất lượng cao cấp."
    short_desc = product_name
    
    sql = f"""(
    gen_random_uuid(),
    '{product_name}',
    '{slug}',
    '{description}',
    '{short_desc}',
    '{sku}',
    {price}, {sale_price}, {stock}, true, true,
    '{image_path}',
    '{sizes}', '{colors}', 'Cotton',
    false, true, 'active', 0, 0, 0,
    '{category_info["category_id"]}'::uuid, '{category_info["brand_id"]}'::uuid, 
    seed_datetime, seed_datetime
)"""
    
    return sql

def main():
    """Main function to generate SQL"""
    output_file = "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/seed_all_products.sql"
    
    with open(output_file, 'w', encoding='utf-8') as f:
        f.write("""-- ========================================
-- Script thêm TẤT CẢ sản phẩm (189 sản phẩm)
-- Auto-generated from image files
-- ========================================

DO $$ 
DECLARE
    seed_datetime timestamp := '2025-01-01 00:00:00'::timestamp;
BEGIN

""")
        
        total_products = 0
        
        for folder_name, category_info in categories.items():
            folder_path = base_path / folder_name
            
            if not folder_path.exists():
                print(f"Folder không tồn tại: {folder_path}")
                continue
            
            # Get all image files
            image_files = sorted([f for f in os.listdir(folder_path) if f.endswith('.jpg')])
            
            if not image_files:
                print(f"Không có file ảnh trong: {folder_path}")
                continue
            
            f.write(f"\n-- ============ {folder_name.upper()} ({len(image_files)} sản phẩm) ============\n")
            f.write('INSERT INTO "Products" (\n')
            f.write('    "Id", "Name", "Slug", "Description", "ShortDescription", "SKU",\n')
            f.write('    "Price", "SalePrice", "StockQuantity", "ManageStock", "InStock",\n')
            f.write('    "FeaturedImageUrl", "Size", "Color", "Material",\n')
            f.write('    "IsFeatured", "IsActive", "Status", "ViewCount", "Rating", "ReviewCount",\n')
            f.write('    "CategoryId", "BrandId", "CreatedAt", "UpdatedAt"\n')
            f.write(') VALUES\n')
            
            product_sqls = []
            for img_file in image_files:
                # Extract product name from filename
                product_name = img_file.replace('.jpg', '').strip()
                sku = extract_sku(product_name)
                image_path = f"/images/{folder_name}/{img_file}"
                
                sql = generate_product_sql(product_name, sku, image_path, category_info)
                product_sqls.append(sql)
                total_products += 1
            
            f.write(',\n'.join(product_sqls))
            f.write(';\n\n')
        
        f.write("""END $$;

-- Kiểm tra kết quả
SELECT 'Đã thêm thành công """ + str(total_products) + """ sản phẩm' as status;
SELECT COUNT(*) as total_products FROM "Products";
""")
    
    print(f"✅ Đã tạo file SQL: {output_file}")
    print(f"📊 Tổng số sản phẩm: {total_products}")

if __name__ == "__main__":
    main()
