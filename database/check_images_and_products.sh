#!/bin/bash
# Script kiểm tra thông tin sản phẩm và ảnh

echo "=========================================="
echo "KIỂM TRA ẢNH SẢN PHẨM"
echo "=========================================="
echo ""

BASE_PATH="/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/wwwroot/images"

declare -a folders=("ao-nam" "ao-nu" "quan-nam" "quan-nu" "chan-vay-nu" "dam-nu" "phu-kien-nam" "phu-kien-nu")

total_images=0
for folder in "${folders[@]}"; do
    count=$(find "$BASE_PATH/$folder" -type f \( -iname "*.jpg" -o -iname "*.jpeg" -o -iname "*.png" -o -iname "*.gif" -o -iname "*.webp" \) 2>/dev/null | wc -l | tr -d ' ')
    echo "📁 $folder: $count ảnh"
    total_images=$((total_images + count))
done

echo ""
echo "✅ Tổng số ảnh: $total_images"
echo ""

echo "=========================================="
echo "KIỂM TRA FILE CSV"
echo "=========================================="
echo ""

CSV_PATH="/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database"

if [ -f "$CSV_PATH/johnhenry_products.csv" ]; then
    jh_count=$(($(wc -l < "$CSV_PATH/johnhenry_products.csv") - 1))
    echo "📄 johnhenry_products.csv: $jh_count sản phẩm"
else
    echo "❌ johnhenry_products.csv không tồn tại"
fi

if [ -f "$CSV_PATH/all_products_export.csv" ]; then
    all_count=$(($(wc -l < "$CSV_PATH/all_products_export.csv") - 1))
    echo "📄 all_products_export.csv: $all_count sản phẩm"
else
    echo "❌ all_products_export.csv không tồn tại"
fi

echo ""

echo "=========================================="
echo "KIỂM TRA DATABASE"
echo "=========================================="
echo ""

# Test API endpoint
if curl -s -f "http://localhost:5101/Home/GetDatabaseStats" > /dev/null 2>&1; then
    echo "✅ Website đang chạy trên http://localhost:5101"
    echo ""
    
    stats=$(curl -s "http://localhost:5101/Home/GetDatabaseStats")
    
    total=$(echo "$stats" | python3 -c "import sys, json; print(json.load(sys.stdin)['totalProducts'])" 2>/dev/null)
    active=$(echo "$stats" | python3 -c "import sys, json; print(json.load(sys.stdin)['activeProducts'])" 2>/dev/null)
    featured=$(echo "$stats" | python3 -c "import sys, json; print(json.load(sys.stdin)['featuredProducts'])" 2>/dev/null)
    freelancer=$(echo "$stats" | python3 -c "import sys, json; print(json.load(sys.stdin)['freelancerCount'])" 2>/dev/null)
    johnhenry=$(echo "$stats" | python3 -c "import sys, json; print(json.load(sys.stdin)['johnHenryCount'])" 2>/dev/null)
    
    echo "🗄️  Database Stats:"
    echo "   - Tổng sản phẩm: $total"
    echo "   - Sản phẩm active: $active"
    echo "   - Sản phẩm featured: $featured"
    echo "   - Freelancer: $freelancer"
    echo "   - John Henry: $johnhenry"
else
    echo "❌ Website không chạy hoặc không thể truy cập"
fi

echo ""
echo "=========================================="
echo "TÓM TẮT"
echo "=========================================="
echo ""
echo "📊 So sánh:"
echo "   - Ảnh có sẵn: $total_images"
if [ -n "$jh_count" ]; then
    echo "   - CSV johnhenry: $jh_count"
fi
if [ -n "$total" ]; then
    echo "   - Database: $total"
    
    if [ "$total" -lt "$jh_count" ]; then
        missing=$((jh_count - total))
        echo ""
        echo "⚠️  Database thiếu khoảng $missing sản phẩm so với CSV"
    else
        echo ""
        echo "✅ Database đã có đầy đủ sản phẩm!"
    fi
fi

echo ""
