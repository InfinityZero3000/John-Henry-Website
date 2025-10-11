#!/bin/bash

# Script để chạy SQL update image paths
# Tạo: October 11, 2025

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${BLUE}=====================================${NC}"
echo -e "${BLUE}  Cập nhật đường dẫn ảnh trong DB${NC}"
echo -e "${BLUE}=====================================${NC}"
echo ""

# Database connection details
DB_HOST="localhost"
DB_PORT="5432"
DB_NAME="johnhenry_db"
DB_USER="johnhenry_user"
DB_PASSWORD="JohnHenry@2025!"

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
SQL_FILE="$SCRIPT_DIR/update_image_paths.sql"

echo -e "${BLUE}Kiểm tra file SQL...${NC}"
if [ ! -f "$SQL_FILE" ]; then
    echo -e "${RED}❌ Không tìm thấy file: $SQL_FILE${NC}"
    exit 1
fi
echo -e "${GREEN}✓ File SQL OK${NC}"
echo ""

echo -e "${BLUE}Kết nối database: ${DB_NAME}@${DB_HOST}:${DB_PORT}${NC}"
echo ""

# Run SQL script
echo -e "${BLUE}Đang chạy SQL script...${NC}"
PGPASSWORD=$DB_PASSWORD psql -h $DB_HOST -p $DB_PORT -U $DB_USER -d $DB_NAME -f "$SQL_FILE"

if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}=====================================${NC}"
    echo -e "${GREEN}✓ Cập nhật thành công!${NC}"
    echo -e "${GREEN}=====================================${NC}"
    echo ""
    echo -e "${GREEN}Đã cập nhật tất cả đường dẫn ảnh:${NC}"
    echo "  • Áo nam → ao-nam"
    echo "  • Áo nữ → ao-nu"
    echo "  • Quần nam → quan-nam"
    echo "  • Quần nữ → quan-nu"
    echo "  • Đầm nữ → dam-nu"
    echo "  • Chân váy nữ → chan-vay-nu"
    echo "  • Phụ kiện nam → phu-kien-nam"
    echo "  • Phụ kiện nữ → phu-kien-nu"
else
    echo ""
    echo -e "${RED}=====================================${NC}"
    echo -e "${RED}❌ Có lỗi xảy ra!${NC}"
    echo -e "${RED}=====================================${NC}"
    exit 1
fi
