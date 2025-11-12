#!/bin/bash

# VNPay Test Script
# Test VNPay integration với các test cases khác nhau

echo "🧪 VNPay Integration Test Script"
echo "=================================="
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Load .env
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
    echo -e "${GREEN}✓${NC} Loaded .env file"
else
    echo -e "${RED}✗${NC} .env file not found!"
    exit 1
fi

# Check VNPay config
echo ""
echo "📋 Current VNPay Configuration:"
echo "  TMN Code: $VNPAY_TMN_CODE"
echo "  Sandbox: $VNPAY_SANDBOX"
echo "  Payment URL: $VNPAY_PAYMENT_URL"

if [ "$VNPAY_SANDBOX" != "true" ]; then
    echo -e "${YELLOW}⚠${NC}  Warning: Not in sandbox mode!"
fi

# Test Cases
echo ""
echo "🧪 Test Cases:"
echo ""

# Test Case 1: Small Amount
echo "1. Test với số tiền nhỏ (10,000đ)"
echo "   → Kiểm tra flow thanh toán cơ bản"
echo ""

# Test Case 2: Large Amount  
echo "2. Test với số tiền lớn (5,000,000đ)"
echo "   → Kiểm tra giới hạn giao dịch"
echo ""

# Test Case 3: Special Characters
echo "3. Test với ký tự đặc biệt trong Order Info"
echo "   → Kiểm tra encoding/escaping"
echo ""

# Test URLs
echo "📱 Test Thủ Công:"
echo ""
echo "  1. Mở trình duyệt: http://localhost:5101/Checkout"
echo "  2. Chọn sản phẩm và điền thông tin"
echo "  3. Chọn phương thức: VNPay"
echo "  4. Click 'Thanh toán với VNPay'"
echo ""

# Check if server is running
if curl -s http://localhost:5101 > /dev/null; then
    echo -e "${GREEN}✓${NC} Server đang chạy"
else
    echo -e "${YELLOW}⚠${NC}  Server chưa chạy, hãy start với: dotnet run"
fi

echo ""
echo "📝 Thẻ Test VNPay Sandbox:"
echo "  Số thẻ: 9704 0000 0000 0018"
echo "  Tên: NGUYEN VAN A"
echo "  Ngày hết hạn: 03/07"
echo "  OTP: 123456"
echo ""

# API Test
echo "🔧 API Test Commands:"
echo ""
echo "# Test VNPay signature generation:"
echo "curl -X POST http://localhost:5101/api/test/vnpay/signature \\"
echo "  -H 'Content-Type: application/json' \\"
echo "  -d '{\"amount\": 10000, \"orderId\": \"TEST123\"}'"
echo ""

# Monitoring
echo "📊 Monitoring:"
echo "  Logs: tail -f logs/john-henry-$(date +%Y%m%d).txt | grep -i vnpay"
echo ""

echo "=================================="
echo "✅ Test script complete!"
echo ""
