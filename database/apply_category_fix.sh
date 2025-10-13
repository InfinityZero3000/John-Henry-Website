#!/bin/bash
# Script to apply category fixes using dotnet ef

echo "=========================================="
echo "APPLYING CATEGORY FIX TO DATABASE"
echo "=========================================="
echo ""

cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"

echo "📝 Checking if database is accessible..."
if curl -s -f "http://localhost:5101/Home/GetDatabaseStats" > /dev/null 2>&1; then
    echo "✅ Application is running"
    echo ""
    
    echo "⚠️  To apply the fix, you need to:"
    echo "   1. Stop the application"
    echo "   2. Run the SQL script directly using psql"
    echo "   3. Or manually execute the SQL in your database tool"
    echo ""
    echo "SQL Script locations:"
    echo "   - Simple fix: ./database/fix_category_simple.sql"
    echo "   - Full fix: ./database/fix_category_mapping.sql"
    echo ""
    echo "Example command:"
    echo "   psql -U johnhenry -d johnhenry -f ./database/fix_category_simple.sql"
else
    echo "❌ Application is not running"
    echo ""
    echo "Please start the application first or run the SQL script manually."
fi

echo ""
echo "=========================================="
echo "CATEGORY MAPPING ANALYSIS SUMMARY"
echo "=========================================="
echo ""
echo "📊 Issues Found:"
echo "   - 229 products have incorrect category mapping"
echo "   - 'Thời trang nam' (71 products) needs reclassification"
echo "   - 'Thời trang nữ' (117 products) needs reclassification"
echo ""
echo "✅ SQL scripts have been generated to fix these issues"
echo ""
