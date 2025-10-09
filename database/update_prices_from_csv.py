#!/usr/bin/env python3
"""Script để update giá sản phẩm từ CSV vào database"""
import csv
import psycopg2
import re
from pathlib import Path

DB_CONFIG = {
    'host': 'localhost',
    'port': 5432,
    'database': 'johnhenry_db',
    'user': 'johnhenry_user',
    'password': 'JohnHenry@2025!'
}

def extract_sku_from_csv_name(name):
    """Extract SKU từ tên trong CSV"""
    patterns = [
        r'([A-Z]{2}\d{2}[A-Z]{2}\d{2}[A-Z]?-[A-Z]{2,4})',  # JK25FH04C-PA
        r'([A-Z]{4}\d{2}[A-Z]{2}\d{2,3}[A-Z])',  # FWDR25SS014G
        r'([A-Z]{2}\d{2}[A-Z]{2}\d{2})',  # CA26SS06P
    ]
    
    for pattern in patterns:
        match = re.search(pattern, name)
        if match:
            return match.group(1)
    return None

def main():
    csv_file = Path(__file__).parent / 'johnhenry_products_final.csv'
    output_file = Path(__file__).parent / 'matched_products.csv'
    
    print(f"📂 Đọc file: {csv_file.name}")
    
    # Đọc CSV
    csv_products = []
    with open(csv_file, 'r', encoding='utf-8') as f:
        reader = csv.DictReader(f)
        for row in reader:
            sku = row.get('sku', '').strip()
            if sku:
                csv_products.append({
                    'name': row.get('name', ''),
                    'sku': sku,
                    'price': row.get('price', '0'),
                    'sale_price': ''  # CSV không có sale price
                })
    
    print(f"   Tìm thấy {len(csv_products)} sản phẩm trong CSV")
    
    # Kết nối database
    print("\n🔗 Kết nối database...")
    conn = psycopg2.connect(**DB_CONFIG)
    cur = conn.cursor()
    
    # Lấy tất cả SKU từ database
    cur.execute('SELECT "Id", "SKU", "Name", "Price", "SalePrice" FROM "Products"')
    db_products = {row[1]: row for row in cur.fetchall()}
    
    print(f"   Database có {len(db_products)} sản phẩm")
    
    # Tìm SKU trùng khớp
    matched = []
    updates = []
    
    for csv_prod in csv_products:
        sku = csv_prod['sku']
        if sku in db_products:
            db_id, db_sku, db_name, db_price, db_sale = db_products[sku]
            
            # Parse giá từ CSV (format: "449000")
            price_str = csv_prod['price'].strip()
            
            try:
                new_price = float(price_str) if price_str else 500000
                new_sale = None  # CSV không có sale price, giữ nguyên
                
                matched.append({
                    'SKU': sku,
                    'CSV_Name': csv_prod['name'],
                    'DB_Name': db_name,
                    'Old_Price': float(db_price),
                    'New_Price': new_price
                })
                
                # Chỉ update nếu giá khác
                if float(db_price) != new_price:
                    updates.append((db_id, new_price))
                    
            except ValueError as e:
                print(f"⚠️  Lỗi parse giá cho {sku}: {e}")
    
    print(f"\n✅ Tìm thấy {len(matched)} SKU trùng khớp")
    print(f"📝 Có {len(updates)} sản phẩm cần update giá")
    
    # Ghi file matched products
    if matched:
        with open(output_file, 'w', encoding='utf-8', newline='') as f:
            writer = csv.DictWriter(f, fieldnames=[
                'SKU', 'CSV_Name', 'DB_Name', 
                'Old_Price', 'New_Price'
            ])
            writer.writeheader()
            writer.writerows(matched)
        
        print(f"\n💾 Đã lưu file: {output_file.name}")
    
    # Update database
    if updates:
        print(f"\n🔄 Đang update {len(updates)} sản phẩm...")
        
        for i, (prod_id, price) in enumerate(updates, 1):
            cur.execute(
                'UPDATE "Products" SET "Price" = %s WHERE "Id" = %s',
                (price, prod_id)
            )
            if i % 20 == 0:
                print(f"   Đã update {i}/{len(updates)}...")
        
        conn.commit()
        print(f"\n✅ Đã update giá cho {len(updates)} sản phẩm!")
        
        # Show một vài ví dụ
        print("\n📊 Một số ví dụ đã update:")
        for item in matched[:5]:
            print(f"   {item['SKU']}: {item['Old_Price']:,.0f}₫ → {item['New_Price']:,.0f}₫")
    else:
        print("\n✅ Tất cả giá đã được cập nhật!")
    
    cur.close()
    conn.close()

if __name__ == '__main__':
    main()
