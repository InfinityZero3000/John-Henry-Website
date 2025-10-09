#!/bin/bash
# Script để tự động chạy SQL insert products vào PostgreSQL

set -e  # Exit on error

echo "=========================================="
echo "John Henry & Freelancer Products Importer"
echo "=========================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
SQL_FILE="$SCRIPT_DIR/insert_products_from_csv.sql"

echo "📁 Project Root: $PROJECT_ROOT"
echo "📄 SQL File: $SQL_FILE"
echo ""

# Check if SQL file exists
if [ ! -f "$SQL_FILE" ]; then
    echo -e "${RED}❌ Error: SQL file not found!${NC}"
    echo "Please run 'python3 generate_product_inserts.py' first"
    exit 1
fi

# Read connection string from appsettings.json
APPSETTINGS="$PROJECT_ROOT/appsettings.json"

if [ ! -f "$APPSETTINGS" ]; then
    echo -e "${RED}❌ Error: appsettings.json not found!${NC}"
    exit 1
fi

echo "🔍 Reading database connection from appsettings.json..."

# Extract connection string (basic parsing)
CONN_STRING=$(grep -A 2 "DefaultConnection" "$APPSETTINGS" | grep -o '".*"' | tr -d '"' | tail -1)

if [ -z "$CONN_STRING" ]; then
    echo -e "${RED}❌ Error: Could not find DefaultConnection in appsettings.json${NC}"
    exit 1
fi

echo "✓ Connection string found"
echo ""

# Parse connection string to extract components
# Format: Host=localhost;Port=5432;Database=johnhenry_db;Username=postgres;Password=xxx

DB_HOST=$(echo "$CONN_STRING" | grep -o 'Host=[^;]*' | cut -d'=' -f2)
DB_PORT=$(echo "$CONN_STRING" | grep -o 'Port=[^;]*' | cut -d'=' -f2)
DB_NAME=$(echo "$CONN_STRING" | grep -o 'Database=[^;]*' | cut -d'=' -f2)
DB_USER=$(echo "$CONN_STRING" | grep -o 'Username=[^;]*' | cut -d'=' -f2)
DB_PASS=$(echo "$CONN_STRING" | grep -o 'Password=[^;]*' | cut -d'=' -f2)

# Default values if not found - use current user
DB_HOST=${DB_HOST:-localhost}
DB_PORT=${DB_PORT:-5432}
DB_NAME=${DB_NAME:-johnhenry_db}
DB_USER=${DB_USER:-$(whoami)}

echo "📊 Database Information:"
echo "   Host: $DB_HOST"
echo "   Port: $DB_PORT"
echo "   Database: $DB_NAME"
echo "   User: $DB_USER"
echo ""

# Confirm before proceeding
echo -e "${YELLOW}⚠️  This will insert 903 products into the database.${NC}"
echo -e "${YELLOW}   Make sure you have a backup if needed!${NC}"
echo ""
read -p "Continue? (y/N) " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Cancelled."
    exit 0
fi

echo ""
echo "🚀 Starting import..."
echo ""

# Build psql connection string (only export password if set)
if [ -n "$DB_PASS" ]; then
    PGPASSWORD="$DB_PASS"
    export PGPASSWORD
fi

# Run SQL file
if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$SQL_FILE" 2>&1; then
    echo ""
    echo -e "${GREEN}✅ Import completed successfully!${NC}"
    echo ""
    
    # Show statistics
    echo "📈 Verifying import..."
    echo ""
    
    # Count products
    PRODUCT_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c 'SELECT COUNT(*) FROM "Products";' 2>/dev/null | tr -d ' ')
    echo "   Total Products: $PRODUCT_COUNT"
    
    # Count categories
    CATEGORY_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c 'SELECT COUNT(*) FROM "Categories";' 2>/dev/null | tr -d ' ')
    echo "   Total Categories: $CATEGORY_COUNT"
    
    # Count brands
    BRAND_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c 'SELECT COUNT(*) FROM "Brands";' 2>/dev/null | tr -d ' ')
    echo "   Total Brands: $BRAND_COUNT"
    
    echo ""
    echo "✨ All done!"
    
else
    echo ""
    echo -e "${RED}❌ Import failed!${NC}"
    echo "Please check the error messages above."
    exit 1
fi

unset PGPASSWORD
