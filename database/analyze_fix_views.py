#!/usr/bin/env python3
"""
Script to analyze and fix view files that have rendering issues.
Identifies hardcoded products and generates fix recommendations.
"""

import re
import os

VIEW_FOLDER = "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/Views/Home"

VIEWS_TO_FIX = {
    "FreelancerDress.cshtml": {
        "issue": "Only 2 products rendered out of 89",
        "expected_model": "List<Product>",
        "should_have_loop": True
    },
    "FreelancerShirt.cshtml": {
        "issue": "Only 4 products rendered out of 185",
        "expected_model": "List<Product>",
        "should_have_loop": True
    },
    "FreelancerSkirt.cshtml": {
        "issue": "36 hardcoded products, backend returns 0",
        "expected_model": "List<Product>",
        "should_have_loop": True
    },
    "FreelancerAccessories.cshtml": {
        "issue": "2 hardcoded products, backend returns 0",
        "expected_model": "List<Product>",
        "should_have_loop": True
    }
}

def analyze_view_file(filename):
    """Analyze a view file for potential issues."""
    filepath = os.path.join(VIEW_FOLDER, filename)
    
    if not os.path.exists(filepath):
        return None
    
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    analysis = {
        "filename": filename,
        "has_model_declaration": bool(re.search(r'@model\s+', content)),
        "has_foreach_loop": bool(re.search(r'@foreach\s*\(', content)),
        "has_hardcoded_divs": content.count('<div class="product-card-new">'),
        "has_model_usage": bool(re.search(r'@Model', content)),
        "line_count": len(content.split('\n'))
    }
    
    # Look for hardcoded product structures
    hardcoded_patterns = [
        r'<div class="product-card-new">.*?</div>',
        r'<img src="/images/.*?" alt="',
        r'<h5>.*?</h5>'
    ]
    
    analysis["potential_hardcoded_sections"] = []
    for pattern in hardcoded_patterns:
        matches = re.findall(pattern, content, re.DOTALL)
        if len(matches) > 0 and len(matches) < 10:  # Likely hardcoded if few matches
            analysis["potential_hardcoded_sections"].append({
                "pattern": pattern,
                "count": len(matches)
            })
    
    return analysis

def generate_fix_instructions(filename, analysis, issue_info):
    """Generate fix instructions for a view file."""
    print(f"\n{'='*80}")
    print(f"FIX INSTRUCTIONS FOR: {filename}")
    print(f"{'='*80}")
    print(f"Issue: {issue_info['issue']}")
    print(f"\nCurrent State:")
    print(f"  - Has @model declaration: {analysis['has_model_declaration']}")
    print(f"  - Has @foreach loop: {analysis['has_foreach_loop']}")
    print(f"  - Uses @Model: {analysis['has_model_usage']}")
    print(f"  - Hardcoded product divs: {analysis['has_hardcoded_divs']}")
    print(f"  - Total lines: {analysis['line_count']}")
    
    if analysis['potential_hardcoded_sections']:
        print(f"\n  Potential hardcoded sections:")
        for section in analysis['potential_hardcoded_sections']:
            print(f"    - {section['pattern'][:50]}... : {section['count']} matches")
    
    print(f"\n{'='*80}")
    print("SOLUTION:")
    print(f"{'='*80}")
    
    if not analysis['has_foreach_loop']:
        print("\n❌ CRITICAL: View is missing @foreach loop!")
        print("\nThe view should have a loop like:")
        print("""
@foreach (var product in Model)
{
    <div class="product-card-new">
        <a href="/Home/ProductDetail/@product.Id">
            <img src="@product.FeaturedImageUrl" alt="@product.Name" />
        </a>
        <div class="product-info">
            <h5>@product.Name</h5>
            <p class="price">@product.Price.ToString("N0")đ</p>
            <p class="sku">SKU: @product.SKU</p>
        </div>
    </div>
}
        """)
    
    if analysis['has_hardcoded_divs'] > 0 and analysis['has_hardcoded_divs'] < 10:
        print(f"\n⚠️ WARNING: Found {analysis['has_hardcoded_divs']} hardcoded product divs")
        print("These should be replaced with a @foreach loop over @Model")
    
    if not analysis['has_model_usage']:
        print("\n⚠️ WARNING: View is not using @Model")
        print("Make sure to loop through @Model to display products from backend")
    
    print("\n" + "="*80)
    print("STEPS TO FIX:")
    print("="*80)
    print("1. Locate the hardcoded product cards in the view")
    print("2. Delete all hardcoded product divs")
    print("3. Add @foreach (var product in Model) { ... } loop")
    print("4. Inside loop, create product card using product properties")
    print("5. Test the page to ensure all products are displayed")
    
    return True

def main():
    print("="*80)
    print("VIEW RENDERING FIX ANALYZER")
    print("="*80)
    print(f"\nAnalyzing {len(VIEWS_TO_FIX)} view files...\n")
    
    for filename, issue_info in VIEWS_TO_FIX.items():
        analysis = analyze_view_file(filename)
        
        if analysis:
            generate_fix_instructions(filename, analysis, issue_info)
        else:
            print(f"\n❌ ERROR: Could not find {filename}")
    
    print("\n" + "="*80)
    print("SUMMARY")
    print("="*80)
    print("\nCommon issues found:")
    print("1. Views have hardcoded product HTML instead of @foreach loops")
    print("2. Views are not using @Model to access backend data")
    print("3. Product cards are static, not dynamically generated")
    
    print("\nRecommended approach:")
    print("1. Look at working views (Freelancer.cshtml, JohnHenry.cshtml)")
    print("2. Copy the @foreach loop structure from working views")
    print("3. Apply the same pattern to problematic views")
    print("4. Ensure all views use @Model consistently")
    
    print("\n" + "="*80)

if __name__ == "__main__":
    main()
