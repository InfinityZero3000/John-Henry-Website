#!/usr/bin/env python3
"""
Script tự động tạo SQL UPDATE statements từ file CSV
Giúp cập nhật giá sản phẩm một cách tự động

Usage:
    python generate_update_sql.py
    python generate_update_sql.py --input custom_prices.csv --output custom_update.sql
"""

import csv
import sys
from pathlib import Path
from datetime import datetime
from typing import List, Dict

def read_csv(file_path: str) -> List[Dict[str, str]]:
    """Đọc file CSV và trả về list of dictionaries"""
    products = []
    
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            reader = csv.DictReader(f)
            for row in reader:
                products.append(row)
        
        print(f"✅ Đã đọc {len(products)} sản phẩm từ {file_path}")
        return products
    except FileNotFoundError:
        print(f"❌ Không tìm thấy file: {file_path}")
        sys.exit(1)
    except Exception as e:
        print(f"❌ Lỗi khi đọc file: {e}")
        sys.exit(1)

def categorize_products(products: List[Dict[str, str]]) -> Dict[str, List[Dict[str, str]]]:
    """Phân loại sản phẩm theo category"""
    categories = {
        'Áo Khoác Nam': [],
        'Áo Len Nam': [],
        'Áo Polo Nam': [],
        'Áo Polo Nữ': [],
        'Áo Sơ Mi Nam': [],
        'Áo Sơ Mi Nữ': [],
        'Áo Thun Nam': [],
        'Áo Thun Nữ': [],
        'Áo Blouse Nữ': [],
        'Chân Váy': [],
        'Đầm Nữ': [],
        'Quần Tây Nam': [],
        'Quần Tây Nữ': [],
        'Quần Jeans Nam': [],
        'Quần Jeans Nữ': [],
        'Quần Khaki Nam': [],
        'Quần Short Nam': [],
        'Quần Short Nữ': [],
        'Thắt Lưng Nam': [],
        'Thắt Lưng Nữ': [],
        'Ví Nam': [],
        'Giày Dép Nam': [],
        'Mũ Nam': [],
        'Túi Xách Nữ': [],
        'Mắt Kính Nữ': []
    }
    
    for product in products:
        name = product['DB_Name']
        
        # Phân loại dựa vào tên sản phẩm
        if 'Áo Khoác' in name and 'FW' not in product['SKU']:
            categories['Áo Khoác Nam'].append(product)
        elif 'Áo Len' in name:
            categories['Áo Len Nam'].append(product)
        elif 'Polo' in name and 'FW' not in product['SKU']:
            categories['Áo Polo Nam'].append(product)
        elif 'Polo' in name and 'FW' in product['SKU']:
            categories['Áo Polo Nữ'].append(product)
        elif 'Sơ Mi' in name and 'FW' not in product['SKU']:
            categories['Áo Sơ Mi Nam'].append(product)
        elif ('Sơ Mi' in name or 'Blouse' not in name) and 'FW' in product['SKU'] and 'WS' in product['SKU']:
            categories['Áo Sơ Mi Nữ'].append(product)
        elif 'Blouse' in name:
            categories['Áo Blouse Nữ'].append(product)
        elif 'Áo Thun' in name and 'FW' not in product['SKU']:
            categories['Áo Thun Nam'].append(product)
        elif ('Áo Thun' in name or 'Tanktop' in name) and 'FW' in product['SKU']:
            categories['Áo Thun Nữ'].append(product)
        elif 'Chân Váy' in name or 'Mini Skirt' in name or 'Váy' in name and 'Đầm' not in name:
            categories['Chân Váy'].append(product)
        elif 'Đầm' in name:
            categories['Đầm Nữ'].append(product)
        elif 'Quần Tây' in name and 'FW' not in product['SKU']:
            categories['Quần Tây Nam'].append(product)
        elif 'Quần Tây' in name and 'FW' in product['SKU']:
            categories['Quần Tây Nữ'].append(product)
        elif 'Jeans' in name and 'FW' not in product['SKU']:
            categories['Quần Jeans Nam'].append(product)
        elif 'Jeans' in name and 'FW' in product['SKU']:
            categories['Quần Jeans Nữ'].append(product)
        elif 'Khaki' in name or 'Jogger' in name:
            categories['Quần Khaki Nam'].append(product)
        elif 'Short' in name and 'FW' not in product['SKU']:
            categories['Quần Short Nam'].append(product)
        elif 'Short' in name and 'FW' in product['SKU']:
            categories['Quần Short Nữ'].append(product)
        elif 'Thắt Lưng' in name and 'FW' not in product['SKU']:
            categories['Thắt Lưng Nam'].append(product)
        elif 'Thắt Lưng' in name and 'FW' in product['SKU']:
            categories['Thắt Lưng Nữ'].append(product)
        elif 'Ví' in name:
            categories['Ví Nam'].append(product)
        elif 'Dép' in name or 'Giày' in name:
            categories['Giày Dép Nam'].append(product)
        elif 'Mũ' in name:
            categories['Mũ Nam'].append(product)
        elif 'Túi' in name:
            categories['Túi Xách Nữ'].append(product)
        elif 'Mắt Kính' in name:
            categories['Mắt Kính Nữ'].append(product)
    
    # Loại bỏ categories rỗng
    return {k: v for k, v in categories.items() if v}

def generate_sql(products: List[Dict[str, str]], output_file: str):
    """Generate SQL UPDATE statements"""
    
    # Phân loại sản phẩm
    categorized = categorize_products(products)
    
    with open(output_file, 'w', encoding='utf-8') as f:
        # Header
        f.write("-- " + "="*53 + "\n")
        f.write(f"-- CẬP NHẬT GIÁ SẢN PHẨM - Generated {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
        f.write("-- " + "="*53 + "\n")
        f.write(f"-- Tổng số: {len(products)} sản phẩm\n")
        f.write("-- " + "="*53 + "\n\n")
        
        f.write("BEGIN TRANSACTION;\n\n")
        f.write("PRINT 'Bắt đầu cập nhật giá sản phẩm...';\n\n")
        
        total_count = 0
        
        # Generate UPDATE cho từng category
        for category, items in categorized.items():
            if not items:
                continue
                
            f.write(f"-- {category} ({len(items)} sản phẩm)\n")
            
            for product in items:
                db_name = product['DB_Name']
                old_price = float(product['Old_Price'])
                new_price = float(product['New_Price'])
                
                # Format giá (loại bỏ phần thập phân nếu là .0)
                old_price_str = f"{int(old_price)}" if old_price.is_integer() else f"{old_price}"
                new_price_str = f"{int(new_price)}" if new_price.is_integer() else f"{new_price}"
                
                sql = f"UPDATE Products SET Price = {new_price_str} WHERE Name = '{db_name}' AND Price = {old_price_str};\n"
                f.write(sql)
                total_count += 1
            
            f.write("\n")
        
        # Footer
        f.write("-- Kiểm tra số lượng cập nhật\n")
        f.write("DECLARE @UpdatedCount INT;\n")
        f.write("SELECT @UpdatedCount = @@ROWCOUNT;\n\n")
        f.write("PRINT '----------------------------------------';\n")
        f.write("PRINT 'Số sản phẩm đã cập nhật: ' + CAST(@UpdatedCount AS VARCHAR(10));\n")
        f.write("PRINT '----------------------------------------';\n\n")
        
        # Query kiểm tra
        f.write("-- Xem trước kết quả\n")
        f.write("SELECT TOP 20\n")
        f.write("    p.Name,\n")
        f.write("    c.Name AS Category,\n")
        f.write("    p.Price,\n")
        f.write("    p.SalePrice,\n")
        f.write("    p.UpdatedAt\n")
        f.write("FROM Products p\n")
        f.write("INNER JOIN Categories c ON p.CategoryId = c.Id\n")
        f.write("WHERE p.UpdatedAt >= DATEADD(MINUTE, -5, GETDATE())\n")
        f.write("    AND p.IsActive = 1\n")
        f.write("ORDER BY p.UpdatedAt DESC;\n\n")
        
        # Statistics
        f.write("-- Thống kê giá theo danh mục\n")
        f.write("SELECT \n")
        f.write("    c.Name AS 'Danh mục',\n")
        f.write("    COUNT(*) AS 'Số sản phẩm',\n")
        f.write("    MIN(p.Price) AS 'Giá thấp nhất',\n")
        f.write("    MAX(p.Price) AS 'Giá cao nhất',\n")
        f.write("    AVG(p.Price) AS 'Giá trung bình'\n")
        f.write("FROM Products p\n")
        f.write("INNER JOIN Categories c ON p.CategoryId = c.Id\n")
        f.write("WHERE p.IsActive = 1\n")
        f.write("GROUP BY c.Name\n")
        f.write("ORDER BY c.Name;\n\n")
        
        f.write("COMMIT TRANSACTION;\n")
        f.write("PRINT '';\n")
        f.write("PRINT '✅ CẬP NHẬT THÀNH CÔNG!';\n")
        f.write(f"PRINT '✅ Tổng cộng: {total_count} sản phẩm';\n")
    
    print(f"✅ Đã tạo SQL script: {output_file}")
    print(f"   - Tổng số UPDATE statements: {total_count}")
    print(f"   - Số categories: {len(categorized)}")

def main():
    """Main function"""
    import argparse
    
    parser = argparse.ArgumentParser(description='Generate SQL UPDATE từ CSV')
    parser.add_argument('--input', '-i', 
                       default='database/matched_products.csv',
                       help='Đường dẫn file CSV input')
    parser.add_argument('--output', '-o',
                       default='database/update_prices_generated.sql',
                       help='Đường dẫn file SQL output')
    
    args = parser.parse_args()
    
    print("🚀 Bắt đầu generate SQL script...")
    print(f"   Input: {args.input}")
    print(f"   Output: {args.output}")
    print()
    
    # Đọc CSV
    products = read_csv(args.input)
    
    # Generate SQL
    generate_sql(products, args.output)
    
    print()
    print("✅ HOÀN THÀNH!")
    print()
    print("📝 Các bước tiếp theo:")
    print(f"   1. Kiểm tra file: {args.output}")
    print("   2. Chạy script trong SQL Server Management Studio")
    print("   3. Hoặc: sqlcmd -S SERVER -d DB -i " + args.output)
    print()

if __name__ == '__main__':
    main()
