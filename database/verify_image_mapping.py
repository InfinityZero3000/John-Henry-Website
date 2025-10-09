#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script kiểm tra mapping giữa SKU sản phẩm và file ảnh
"""

import os
import re
from collections import defaultdict

# Đường dẫn
script_dir = os.path.dirname(os.path.abspath(__file__))
base_path = os.path.dirname(script_dir)
sql_file = os.path.join(script_dir, "insert_products_from_csv.sql")
images_base = os.path.join(base_path, "wwwroot", "images")

# Các thư mục chứa ảnh
image_folders = [
    "Áo nam", "Áo nữ", "Quần nam", "Quần nữ", 
    "Chân váy nữ", "Đầm nữ", "Phụ kiện nam", "Phụ kiện nữ"
]

print("=" * 80)
print("KIỂM TRA MAPPING GIỮA SKU VÀ FILE ẢNH")
print("=" * 80)
print()

# 1. Đọc tất cả SKU từ SQL file
print("📄 Đang đọc SKU từ file SQL...")
skus_in_sql = set()
sku_to_name = {}
sku_to_image = {}

with open(sql_file, 'r', encoding='utf-8') as f:
    lines = f.readlines()
    
    for i, line in enumerate(lines):
        # Tìm dòng comment có SKU và tên sản phẩm
        # Format: -- Tên sản phẩm (SKU)
        if line.strip().startswith('--') and '(' in line and ')' in line:
            match = re.search(r'--\s*(.+?)\s*\(([A-Z0-9\-]+)\)', line)
            if match:
                product_name = match.group(1).strip()
                sku = match.group(2).strip()
                skus_in_sql.add(sku)
                sku_to_name[sku] = product_name
        
        # Tìm FeaturedImageUrl trong INSERT statement
        if '"FeaturedImageUrl"' in line or 'FeaturedImageUrl' in line:
            # Look for image path in next few lines
            for j in range(i, min(i+5, len(lines))):
                img_match = re.search(r"'/images/([^']+)'", lines[j])
                if img_match:
                    image_path = img_match.group(1)
                    # Extract SKU from previous lines
                    for k in range(max(0, i-10), i):
                        sku_match = re.search(r"'([A-Z0-9\-]+)',.*--.*SKU", lines[k])
                        if sku_match:
                            sku = sku_match.group(1)
                            sku_to_image[sku] = image_path
                            break
                    break

print(f"   Tìm thấy {len(skus_in_sql)} SKU trong file SQL")
print()

# 2. Đọc tất cả file ảnh
print("🖼️  Đang quét các thư mục ảnh...")
image_files = {}  # category -> list of filenames
all_image_skus = set()
sku_to_category = {}
sku_to_filename = {}

for folder in image_folders:
    folder_path = os.path.join(images_base, folder)
    if not os.path.exists(folder_path):
        print(f"   ⚠️  Thư mục không tồn tại: {folder}")
        continue
    
    files = [f for f in os.listdir(folder_path) 
             if f.lower().endswith(('.jpg', '.jpeg', '.png', '.webp')) 
             and not f.startswith('.')]
    image_files[folder] = files
    
    # Extract SKU from filename (remove extension)
    for filename in files:
        sku = os.path.splitext(filename)[0]
        all_image_skus.add(sku)
        sku_to_category[sku] = folder
        sku_to_filename[sku] = filename
    
    print(f"   {folder:20s}: {len(files):4d} files")

print(f"\n   Tổng số file ảnh: {len(all_image_skus)}")
print()

# 3. So sánh
print("🔍 PHÂN TÍCH KẾT QUẢ:")
print("=" * 80)
print()

# SKU có trong SQL nhưng không có ảnh
missing_images = skus_in_sql - all_image_skus
if missing_images:
    print(f"❌ SKU KHÔNG CÓ ẢNH: {len(missing_images)} sản phẩm")
    print("-" * 80)
    for sku in sorted(missing_images)[:30]:  # Show first 30
        product_name = sku_to_name.get(sku, "N/A")
        print(f"   - {sku:30s} | {product_name}")
    if len(missing_images) > 30:
        print(f"   ... và {len(missing_images) - 30} SKU khác")
    print()
else:
    print("✅ TẤT CẢ SKU TRONG SQL ĐỀU CÓ ẢNH!")
    print()

# Ảnh có nhưng không có trong SQL
extra_images = all_image_skus - skus_in_sql
if extra_images:
    print(f"⚠️  FILE ẢNH KHÔNG CÓ TRONG SQL: {len(extra_images)} files")
    print("-" * 80)
    
    # Group by category
    by_category = defaultdict(list)
    for sku in extra_images:
        if sku in sku_to_category:
            by_category[sku_to_category[sku]].append(sku)
    
    for category, skus in sorted(by_category.items()):
        print(f"\n   {category}: ({len(skus)} files)")
        for sku in sorted(skus)[:15]:  # Show first 15 per category
            filename = sku_to_filename.get(sku, "")
            print(f"      - {sku:30s} ({filename})")
        if len(skus) > 15:
            print(f"      ... và {len(skus) - 15} ảnh khác")
    print()
else:
    print("✅ TẤT CẢ ẢNH ĐỀU CÓ TRONG SQL!")
    print()

# 4. Kiểm tra case-sensitive issues
print("🔤 KIỂM TRA VẤN ĐỀ CHỮ HOA/THƯỜNG:")
print("=" * 80)

case_issues = []
skus_lower = {sku.lower(): sku for sku in skus_in_sql}
images_lower = {sku.lower(): sku for sku in all_image_skus}

for sku_lower, sku_sql in skus_lower.items():
    if sku_lower in images_lower:
        sku_img = images_lower[sku_lower]
        if sku_sql != sku_img:
            case_issues.append((sku_sql, sku_img))

if case_issues:
    print(f"⚠️  Tìm thấy {len(case_issues)} trường hợp khác biệt chữ hoa/thường:")
    print("-" * 80)
    for sql_sku, img_sku in case_issues[:20]:
        print(f"   SQL: {sql_sku:30s} <-> Ảnh: {img_sku}")
    if len(case_issues) > 20:
        print(f"   ... và {len(case_issues) - 20} trường hợp khác")
    print()
else:
    print("✅ Không có vấn đề về chữ hoa/thường")
    print()

# 5. Thống kê chi tiết
print("📊 THỐNG KÊ CHI TIẾT:")
print("=" * 80)
matched_count = len(skus_in_sql & all_image_skus)
print(f"   Tổng SKU trong SQL:        {len(skus_in_sql):4d}")
print(f"   Tổng file ảnh có sẵn:      {len(all_image_skus):4d}")
print(f"   Số SKU khớp hoàn toàn:     {matched_count:4d}")
if len(skus_in_sql) > 0:
    print(f"   Tỷ lệ khớp:                {matched_count * 100.0 / len(skus_in_sql):.1f}%")
print()

# 6. Kiểm tra một số SKU cụ thể
print("🔍 KIỂM TRA MẪU:")
print("=" * 80)
sample_skus = list(skus_in_sql)[:5]
for sku in sample_skus:
    has_image = "✅" if sku in all_image_skus else "❌"
    category = sku_to_category.get(sku, "N/A")
    name = sku_to_name.get(sku, "N/A")
    img_path = sku_to_image.get(sku, "N/A")
    
    print(f"\n   SKU: {sku}")
    print(f"   Tên: {name}")
    print(f"   Có ảnh: {has_image}")
    if sku in all_image_skus:
        print(f"   Danh mục: {category}")
        print(f"   File: {sku_to_filename.get(sku, 'N/A')}")
    print(f"   Path trong SQL: {img_path}")

print()
print("=" * 80)
print("HOÀN THÀNH!")
print("=" * 80)

# Save report to file
report_file = os.path.join(script_dir, "IMAGE_MAPPING_REPORT.txt")
with open(report_file, 'w', encoding='utf-8') as f:
    f.write("=" * 80 + "\n")
    f.write("BÁO CÁO KIỂM TRA MAPPING SKU VÀ FILE ẢNH\n")
    f.write("=" * 80 + "\n\n")
    
    f.write(f"Tổng SKU trong SQL: {len(skus_in_sql)}\n")
    f.write(f"Tổng file ảnh: {len(all_image_skus)}\n")
    f.write(f"Số SKU khớp: {matched_count}\n")
    if len(skus_in_sql) > 0:
        f.write(f"Tỷ lệ khớp: {matched_count * 100.0 / len(skus_in_sql):.1f}%\n\n")
    
    if missing_images:
        f.write("\n" + "=" * 80 + "\n")
        f.write(f"SKU KHÔNG CÓ ẢNH ({len(missing_images)} sản phẩm):\n")
        f.write("=" * 80 + "\n")
        for sku in sorted(missing_images):
            product_name = sku_to_name.get(sku, "N/A")
            f.write(f"{sku:30s} | {product_name}\n")
    
    if extra_images:
        f.write("\n" + "=" * 80 + "\n")
        f.write(f"FILE ẢNH KHÔNG CÓ TRONG SQL ({len(extra_images)} files):\n")
        f.write("=" * 80 + "\n")
        by_category = defaultdict(list)
        for sku in extra_images:
            if sku in sku_to_category:
                by_category[sku_to_category[sku]].append(sku)
        
        for category, skus in sorted(by_category.items()):
            f.write(f"\n{category}: ({len(skus)} files)\n")
            for sku in sorted(skus):
                filename = sku_to_filename.get(sku, "")
                f.write(f"   {sku:30s} ({filename})\n")

print(f"\n📄 Báo cáo chi tiết đã được lưu vào: {report_file}")
