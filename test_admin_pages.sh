#!/bin/bash

# Script test tất cả các trang admin
# Run: chmod +x test_admin_pages.sh && ./test_admin_pages.sh

BASE_URL="http://localhost:5101"
LOG_FILE="admin_pages_test_$(date +%Y%m%d_%H%M%S).log"

echo "=====================================" | tee $LOG_FILE
echo "TESTING JOHN HENRY ADMIN PAGES" | tee -a $LOG_FILE
echo "Time: $(date)" | tee -a $LOG_FILE
echo "=====================================" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

# Danh sách các trang admin cần test
declare -a pages=(
    "/admin"
    "/admin/dashboard"
    "/admin/users"
    "/admin/users?role=customer"
    "/admin/users?role=seller"
    "/admin/categories"
    "/admin/brands"
    "/admin/inventory"
    "/admin/orders"
    "/admin/orders?status=pending"
    "/admin/reviews"
    "/admin/coupons"
    "/admin/createcoupon"
    "/admin/banners"
    "/admin/blog"
    "/admin/settings"
    "/admin/approvals"
    "/admin/support"
    "/admin/marketing"
    "/admin/payments"
)

total_pages=${#pages[@]}
success_count=0
error_count=0
not_found_count=0

echo "Testing $total_pages admin pages..." | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

for page in "${pages[@]}"
do
    echo -n "Testing $page ... " | tee -a $LOG_FILE
    
    # Test page với curl
    response=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL$page" -L --max-time 10)
    
    if [ "$response" == "200" ]; then
        echo "✅ OK (200)" | tee -a $LOG_FILE
        ((success_count++))
    elif [ "$response" == "404" ]; then
        echo "❌ NOT FOUND (404)" | tee -a $LOG_FILE
        ((not_found_count++))
    elif [ "$response" == "500" ]; then
        echo "❌ ERROR (500)" | tee -a $LOG_FILE
        ((error_count++))
    elif [ "$response" == "302" ] || [ "$response" == "301" ]; then
        echo "⚠️  REDIRECT ($response)" | tee -a $LOG_FILE
        ((success_count++))
    else
        echo "⚠️  UNKNOWN ($response)" | tee -a $LOG_FILE
        ((error_count++))
    fi
    
    sleep 0.5
done

echo "" | tee -a $LOG_FILE
echo "=====================================" | tee -a $LOG_FILE
echo "SUMMARY" | tee -a $LOG_FILE
echo "=====================================" | tee -a $LOG_FILE
echo "Total pages tested: $total_pages" | tee -a $LOG_FILE
echo "✅ Success: $success_count" | tee -a $LOG_FILE
echo "❌ Errors (500): $error_count" | tee -a $LOG_FILE
echo "❌ Not Found (404): $not_found_count" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE

success_rate=$(awk "BEGIN {printf \"%.1f\", ($success_count/$total_pages)*100}")
echo "Success Rate: $success_rate%" | tee -a $LOG_FILE
echo "" | tee -a $LOG_FILE
echo "Log saved to: $LOG_FILE" | tee -a $LOG_FILE

# Highlight errors
if [ $error_count -gt 0 ] || [ $not_found_count -gt 0 ]; then
    echo "" | tee -a $LOG_FILE
    echo "⚠️  FOUND $((error_count + not_found_count)) PROBLEMATIC PAGES!" | tee -a $LOG_FILE
    echo "Check application logs for details:" | tee -a $LOG_FILE
    echo "  tail -f logs/john-henry-$(date +%Y%m%d).txt" | tee -a $LOG_FILE
fi
