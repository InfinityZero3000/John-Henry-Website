#!/usr/bin/env python3
"""
Auto-fix script for view rendering issues.
Adds proper @foreach loops to display products from @Model.
"""

import re
import os
from pathlib import Path

VIEW_FOLDER = Path("/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/Views/Home")

# Template for product rendering (from working Freelancer.cshtml)
PRODUCT_LOOP_TEMPLATE = '''                    <div class="products-grid">
                        <div class="row g-4">
                            @if (Model != null && Model.Any())
                            {
                                @foreach (var product in Model)
                                {
                                    <div class="col-xl-3 col-lg-4 col-md-6 col-sm-6">
                                        <div class="product-card-new">
                                            <div class="product-image-container">
                                                <a href="@Url.Action("ProductDetail", "Home", new { id = product.Id })">
                                                    <img src="@product.FeaturedImageUrl" alt="@product.Name" class="product-image">
                                                </a>
                                                @if (product.IsNew)
                                                {
                                                    <span class="new-badge">MỚI</span>
                                                }
                                            </div>
                                            <div class="product-info">
                                                <a href="@Url.Action("ProductDetail", "Home", new { id = product.Id })" style="text-decoration: none; color: inherit;">
                                                    <h6 class="product-name">@product.Name</h6>
                                                </a>
                                                <div class="product-price-actions">
                                                    <div class="product-price">
                                                        <span class="current-price">@product.Price.ToString("N0")₫</span>
                                                    </div>
                                                    <div class="product-actions">
                                                        <button class="action-btn wishlist-btn" title="Yêu thích" data-wishlist-product-id="@product.Id">
                                                            <i class="far fa-heart"></i>
                                                        </button>
                                                        <button class="action-btn cart-btn" title="Thêm vào giỏ" onclick="addToCart('@product.Id', this)">
                                                            <i class="fas fa-shopping-bag"></i>
                                                        </button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                }
                            }
                            else
                            {
                                <div class="col-12 text-center py-5">
                                    <p class="text-muted">Không có sản phẩm nào</p>
                                </div>
                            }
                        </div>
                    </div>
                    
                    @* Pagination *@
                    @Html.Partial("_Pagination")'''

def find_product_section(content):
    """Find the section where products are rendered."""
    # Look for product-grid or products section
    patterns = [
        (r'<div class="products-grid">.*?</div>\s*</div>', 'products-grid'),
        (r'<div class="row g-4">.*?@Html\.Partial\("_Pagination"\)', 'row with pagination'),
        (r'<!-- Products Section -->.*?<!-- End Products Section -->', 'commented section')
    ]
    
    for pattern, name in patterns:
        match = re.search(pattern, content, re.DOTALL)
        if match:
            return match.span(), name
    
    return None, None

def backup_file(filepath):
    """Create a backup of the file."""
    backup_path = filepath.parent / f"{filepath.stem}_BACKUP{filepath.suffix}"
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    with open(backup_path, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f"  ✓ Backup created: {backup_path.name}")
    return backup_path

def fix_view_file(filename):
    """Fix a view file by replacing hardcoded products with @foreach loop."""
    filepath = VIEW_FOLDER / filename
    
    if not filepath.exists():
        print(f"  ❌ File not found: {filename}")
        return False
    
    print(f"\n{'='*80}")
    print(f"Fixing: {filename}")
    print(f"{'='*80}")
    
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Backup original file
    backup_path = backup_file(filepath)
    
    # Find product section
    section_span, section_type = find_product_section(content)
    
    if section_span:
        print(f"  ✓ Found product section: {section_type}")
        print(f"  ✓ Section location: lines {content[:section_span[0]].count(chr(10))} - {content[:section_span[1]].count(chr(10))}")
        
        # Replace the section with proper template
        new_content = content[:section_span[0]] + PRODUCT_LOOP_TEMPLATE + content[section_span[1]:]
        
        # Write fixed content
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(new_content)
        
        print(f"  ✅ Fixed! Replaced {section_span[1] - section_span[0]} characters with proper @foreach loop")
        return True
    else:
        print(f"  ⚠️  Could not automatically locate product section")
        print(f"  📝 Manual fix instructions:")
        print(f"     1. Find the section with hardcoded <div class=\"product-card-new\">")
        print(f"     2. Replace entire section with @foreach loop from Freelancer.cshtml")
        print(f"     3. Ensure @Model is used to loop through products")
        return False

def main():
    print("="*80)
    print("AUTO-FIX VIEW RENDERING ISSUES")
    print("="*80)
    print(f"\nWorking directory: {VIEW_FOLDER}")
    
    files_to_fix = [
        "FreelancerDress.cshtml",
        "FreelancerShirt.cshtml",
        # Skip these for now as they need different fixes
        # "FreelancerSkirt.cshtml",
        # "FreelancerAccessories.cshtml"
    ]
    
    print(f"\nWill fix {len(files_to_fix)} files:\n")
    
    results = {
        "success": [],
        "failed": [],
        "skipped": []
    }
    
    for filename in files_to_fix:
        try:
            if fix_view_file(filename):
                results["success"].append(filename)
            else:
                results["failed"].append(filename)
        except Exception as e:
            print(f"  ❌ ERROR: {e}")
            results["failed"].append(filename)
    
    # Summary
    print(f"\n{'='*80}")
    print("SUMMARY")
    print(f"{'='*80}")
    print(f"\n✅ Successfully fixed: {len(results['success'])} files")
    for f in results['success']:
        print(f"   - {f}")
    
    if results['failed']:
        print(f"\n❌ Failed to fix: {len(results['failed'])} files")
        for f in results['failed']:
            print(f"   - {f}")
    
    print(f"\n{'='*80}")
    print("NEXT STEPS")
    print(f"{'='*80}")
    print("\n1. Restart the application")
    print("2. Test the fixed pages:")
    for f in results['success']:
        page_name = f.replace('.cshtml', '')
        print(f"   - http://localhost:5101/Home/{page_name}")
    print("3. If issues persist, check browser console for errors")
    print("4. Original files backed up with _BACKUP suffix")
    
    print("\n" + "="*80)

if __name__ == "__main__":
    main()
