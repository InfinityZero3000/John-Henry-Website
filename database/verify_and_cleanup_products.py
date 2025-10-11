#!/usr/bin/env python3
"""
Script để:
1. Kiểm tra các products trong database có ảnh tương ứng hay không
2. Xóa products không có ảnh
3. Tạo báo cáo chi tiết về các vấn đề
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

# Folder mappings
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

def get_all_image_files():
    """Lấy tất cả file ảnh từ local folders."""
    all_files = {}
    total = 0
    
    print("\n[1] Scanning local image files...")
    for folder in FOLDERS:
        folder_path = IMAGES_PATH / folder
        if folder_path.exists():
            files = [f for f in os.listdir(folder_path) if f.lower().endswith(('.jpg', '.jpeg', '.png'))]
            all_files[folder] = files
            print(f"    ✓ Found {len(files)} images in {folder}")
            total += len(files)
        else:
            print(f"    ✗ Folder not found: {folder}")
            all_files[folder] = []
    
    print(f"\n    Total: {total} image files")
    return all_files

def extract_sku_from_filename(filename):
    """Extract SKU từ filename."""
    # Remove extension
    name = filename.replace('.jpg', '').replace('.jpeg', '').replace('.png', '').replace('.JPG', '')
    
    # Try different patterns
    patterns = [
        r'([A-Z]{2,4}\d{2}[A-Z]{2}\d{2}[A-Z]?)(?:-.*)?',  # KS25FH57C-SC
        r'([A-Z]{2,4}\d{2}[A-Z]{2}\d{2})(?:-.*)?',        # BE25FH49-EP
    ]
    
    for pattern in patterns:
        match = re.match(pattern, name, re.IGNORECASE)
        if match:
            return match.group(1).upper()
    
    return None

def normalize_filename(filename):
    """Normalize filename để so sánh (lowercase, remove extension)."""
    return filename.lower().replace('.jpg', '').replace('.jpeg', '').replace('.png', '')

def check_image_exists(featured_image_url, all_files):
    """
    Kiểm tra xem image có tồn tại thực sự hay không.
    Returns: (exists: bool, folder: str, actual_filename: str or None, issue: str or None)
    """
    if not featured_image_url:
        return (False, None, None, "Empty FeaturedImageUrl")
    
    # Parse URL: /images/ao-nam/KS25FH57C-SC.jpg
    # hoặc: ~/images/ao-nam/KS25FH57C-SC.jpg
    # hoặc: /home/~/images/ao-nam/KS25FH57C-SC.jpg
    
    # Clean URL
    url = featured_image_url.replace('~/images/', '').replace('/images/', '').replace('/home/~/images/', '')
    
    # Check if it has Vietnamese characters
    if re.search(r'[À-ỹ]', url):
        return (False, None, None, f"Vietnamese characters in URL: {url}")
    
    # Split folder/filename
    parts = url.split('/')
    if len(parts) != 2:
        return (False, None, None, f"Invalid URL format: {featured_image_url}")
    
    folder, filename = parts
    
    # Normalize for comparison
    normalized_filename = normalize_filename(filename)
    
    # Check if folder exists
    if folder not in all_files:
        return (False, folder, None, f"Folder '{folder}' not in list")
    
    # Check exact match (case-sensitive)
    if filename in all_files[folder]:
        return (True, folder, filename, None)
    
    # Check case-insensitive match
    for actual_file in all_files[folder]:
        if normalize_filename(actual_file) == normalized_filename:
            return (False, folder, actual_file, f"Case mismatch: DB has '{filename}' but file is '{actual_file}'")
    
    return (False, folder, None, f"File not found: {filename}")

def get_all_products(conn):
    """Lấy tất cả products từ database."""
    cursor = conn.cursor()
    cursor.execute("""
        SELECT "Id", "SKU", "Name", "FeaturedImageUrl", "CategoryId"
        FROM "Products"
        ORDER BY "CreatedAt" DESC
    """)
    
    columns = [desc[0] for desc in cursor.description]
    products = []
    for row in cursor.fetchall():
        products.append(dict(zip(columns, row)))
    
    cursor.close()
    return products

def analyze_products(products, all_files):
    """
    Phân tích products và categorize vào các nhóm:
    - valid: có ảnh, đúng case
    - case_issue: có ảnh nhưng sai case
    - not_found: không tìm thấy ảnh
    - vietnamese: còn dấu tiếng Việt
    """
    valid = []
    case_issues = []
    not_found = []
    vietnamese = []
    
    print("\n[2] Analyzing products...")
    
    for product in products:
        product_id = product['Id']
        sku = product['SKU']
        name = product['Name']
        url = product['FeaturedImageUrl']
        
        exists, folder, actual_file, issue = check_image_exists(url, all_files)
        
        product_info = {
            'id': product_id,
            'sku': sku,
            'name': name,
            'url': url,
            'folder': folder,
            'actual_file': actual_file,
            'issue': issue
        }
        
        if exists:
            valid.append(product_info)
        elif issue and 'Vietnamese' in issue:
            vietnamese.append(product_info)
        elif issue and 'Case mismatch' in issue:
            case_issues.append(product_info)
        else:
            not_found.append(product_info)
    
    print(f"    ✓ Valid products: {len(valid)}")
    print(f"    ⚠ Case issues: {len(case_issues)}")
    print(f"    ⚠ Vietnamese names: {len(vietnamese)}")
    print(f"    ✗ Not found: {len(not_found)}")
    
    return {
        'valid': valid,
        'case_issues': case_issues,
        'vietnamese': vietnamese,
        'not_found': not_found
    }

def generate_report(analysis):
    """Tạo báo cáo chi tiết."""
    report = []
    report.append("\n" + "="*80)
    report.append("PRODUCT IMAGE VERIFICATION REPORT")
    report.append("="*80)
    
    # Summary
    total = sum(len(v) for v in analysis.values())
    report.append(f"\nTotal products: {total}")
    report.append(f"  ✓ Valid (has image, correct case): {len(analysis['valid'])}")
    report.append(f"  ⚠ Case issues (image exists but wrong case): {len(analysis['case_issues'])}")
    report.append(f"  ⚠ Vietnamese characters in filename: {len(analysis['vietnamese'])}")
    report.append(f"  ✗ Image not found: {len(analysis['not_found'])}")
    
    # Case Issues Detail
    if analysis['case_issues']:
        report.append("\n" + "-"*80)
        report.append("CASE MISMATCH ISSUES (can be fixed by updating database):")
        report.append("-"*80)
        for i, p in enumerate(analysis['case_issues'][:20], 1):  # Show first 20
            report.append(f"\n{i}. SKU: {p['sku']}")
            report.append(f"   Name: {p['name'][:60]}")
            report.append(f"   Issue: {p['issue']}")
        if len(analysis['case_issues']) > 20:
            report.append(f"\n   ... and {len(analysis['case_issues']) - 20} more")
    
    # Vietnamese Issues Detail
    if analysis['vietnamese']:
        report.append("\n" + "-"*80)
        report.append("VIETNAMESE FILENAME ISSUES (should be fixed or deleted):")
        report.append("-"*80)
        for i, p in enumerate(analysis['vietnamese'], 1):
            report.append(f"\n{i}. SKU: {p['sku']}")
            report.append(f"   Name: {p['name'][:60]}")
            report.append(f"   URL: {p['url']}")
    
    # Not Found Detail
    if analysis['not_found']:
        report.append("\n" + "-"*80)
        report.append("IMAGE NOT FOUND (should be deleted from database):")
        report.append("-"*80)
        for i, p in enumerate(analysis['not_found'][:30], 1):  # Show first 30
            report.append(f"\n{i}. SKU: {p['sku']}")
            report.append(f"   Name: {p['name'][:60]}")
            report.append(f"   Issue: {p['issue']}")
        if len(analysis['not_found']) > 30:
            report.append(f"\n   ... and {len(analysis['not_found']) - 30} more")
    
    report_text = "\n".join(report)
    print(report_text)
    
    # Save to file
    report_file = Path(__file__).parent / 'product_verification_report.txt'
    with open(report_file, 'w', encoding='utf-8') as f:
        f.write(report_text)
    print(f"\n[Report saved to: {report_file}]")
    
    return report_text

def generate_fix_sql(analysis):
    """Tạo SQL để fix case issues và delete products without images."""
    sql_lines = []
    sql_lines.append("-- SQL Script to Fix Case Issues and Delete Products Without Images")
    sql_lines.append("-- Generated by verify_and_cleanup_products.py")
    sql_lines.append("")
    sql_lines.append("BEGIN;")
    sql_lines.append("")
    
    # Fix case issues
    if analysis['case_issues']:
        sql_lines.append("-- Fix Case Mismatch Issues")
        sql_lines.append(f"-- Total: {len(analysis['case_issues'])} products")
        sql_lines.append("")
        
        for p in analysis['case_issues']:
            # Create correct URL with actual filename
            correct_url = f"/images/{p['folder']}/{p['actual_file']}"
            sql_lines.append(f"-- {p['sku']}: {p['name'][:50]}")
            sql_lines.append(f"UPDATE \"Products\" SET \"FeaturedImageUrl\" = '{correct_url}' WHERE \"Id\" = '{p['id']}';")
            sql_lines.append("")
    
    # Delete products without images
    to_delete = analysis['vietnamese'] + analysis['not_found']
    if to_delete:
        sql_lines.append("-- Delete Products Without Images")
        sql_lines.append(f"-- Total: {len(to_delete)} products")
        sql_lines.append("")
        
        for p in to_delete:
            sql_lines.append(f"-- {p['sku']}: {p['name'][:50]}")
            sql_lines.append(f"DELETE FROM \"Products\" WHERE \"Id\" = '{p['id']}';")
            sql_lines.append("")
    
    sql_lines.append("COMMIT;")
    sql_lines.append("")
    sql_lines.append("-- Verification query")
    sql_lines.append("SELECT COUNT(*) as total_products FROM \"Products\";")
    
    sql_text = "\n".join(sql_lines)
    
    # Save to file
    sql_file = Path(__file__).parent / 'fix_and_cleanup_products.sql'
    with open(sql_file, 'w', encoding='utf-8') as f:
        f.write(sql_text)
    
    print(f"\n[3] SQL script generated: {sql_file}")
    print(f"    - Fix case issues: {len(analysis['case_issues'])} products")
    print(f"    - Delete without images: {len(to_delete)} products")
    
    return sql_file

def main():
    print("="*80)
    print("PRODUCT IMAGE VERIFICATION & CLEANUP")
    print("="*80)
    
    # Get all image files
    all_files = get_all_image_files()
    
    # Connect to database
    print("\n[Connecting to database...]")
    conn = psycopg2.connect(**DB_CONFIG)
    
    # Get all products
    products = get_all_products(conn)
    print(f"    ✓ Loaded {len(products)} products from database")
    
    # Analyze
    analysis = analyze_products(products, all_files)
    
    # Generate report
    generate_report(analysis)
    
    # Generate SQL fix script
    sql_file = generate_fix_sql(analysis)
    
    conn.close()
    
    print("\n" + "="*80)
    print("NEXT STEPS:")
    print("="*80)
    print("1. Review the report: product_verification_report.txt")
    print("2. Review the SQL script: fix_and_cleanup_products.sql")
    print("3. Execute SQL script:")
    print(f"   psql -h localhost -U johnhenry_user -d johnhenry_db -f {sql_file.name}")
    print("="*80)

if __name__ == '__main__':
    main()
