#!/usr/bin/env python3
"""
Script to analyze view rendering issues for product pages.
Compares backend data counts with frontend display.
"""

import requests
import json
from bs4 import BeautifulSoup
import re

BASE_URL = "http://localhost:5101"

PAGES_TO_CHECK = {
    "JohnHenry": {
        "url": f"{BASE_URL}/Home/JohnHenry",
        "expected_pattern": "SKU NOT starting with FW",
        "category": "All John Henry products"
    },
    "Freelancer": {
        "url": f"{BASE_URL}/Home/Freelancer", 
        "expected_pattern": "SKU starting with FW",
        "category": "All Freelancer products"
    },
    "FreelancerDress": {
        "url": f"{BASE_URL}/Home/FreelancerDress",
        "expected_pattern": "SKU starting with FW + Category: Đầm nữ",
        "category": "Đầm nữ"
    },
    "FreelancerShirt": {
        "url": f"{BASE_URL}/Home/FreelancerShirt",
        "expected_pattern": "SKU starting with FW + Category: Áo nữ",
        "category": "Áo nữ"
    },
    "FreelancerTrousers": {
        "url": f"{BASE_URL}/Home/FreelancerTrousers",
        "expected_pattern": "SKU starting with FW + Category: Quần nữ",
        "category": "Quần nữ"
    },
    "FreelancerSkirt": {
        "url": f"{BASE_URL}/Home/FreelancerSkirt",
        "expected_pattern": "SKU starting with FW + Category: Chân váy nữ",
        "category": "Chân váy nữ"
    },
    "FreelancerAccessories": {
        "url": f"{BASE_URL}/Home/FreelancerAccessories",
        "expected_pattern": "SKU starting with FW + Category: Phụ kiện nữ",
        "category": "Phụ kiện nữ"
    },
    "JohnHenryShirt": {
        "url": f"{BASE_URL}/Home/JohnHenryShirt",
        "expected_pattern": "SKU NOT starting with FW + Category: Áo nam",
        "category": "Áo nam"
    },
    "JohnHenryTrousers": {
        "url": f"{BASE_URL}/Home/JohnHenryTrousers",
        "expected_pattern": "SKU NOT starting with FW + Category: Quần nam",
        "category": "Quần nam"
    },
    "JohnHenryAccessories": {
        "url": f"{BASE_URL}/Home/JohnHenryAccessories",
        "expected_pattern": "SKU NOT starting with FW + Category: Phụ kiện nam",
        "category": "Phụ kiện nam"
    }
}

def analyze_page(page_name, page_info):
    """Analyze a single page for rendering issues."""
    print(f"\n{'='*80}")
    print(f"Analyzing: {page_name}")
    print(f"URL: {page_info['url']}")
    print(f"Expected: {page_info['expected_pattern']}")
    print(f"{'='*80}")
    
    try:
        # Fetch the page
        response = requests.get(page_info['url'], timeout=10)
        response.raise_for_status()
        
        html_content = response.text
        soup = BeautifulSoup(html_content, 'html.parser')
        
        # Extract total products from backend (ViewBag.TotalProducts)
        total_match = re.search(r'Tổng:\s*(\d+)\s*sản phẩm', html_content)
        backend_total = int(total_match.group(1)) if total_match else 0
        
        # Count product cards in HTML
        product_cards = soup.find_all('div', class_='product-card-new')
        frontend_count = len(product_cards)
        
        # Extract SKUs from product cards
        skus_in_html = []
        for card in product_cards:
            # Try to find SKU in various places
            sku_elem = card.find('p', class_='sku') or card.find('span', class_='sku')
            if sku_elem:
                sku_text = sku_elem.get_text(strip=True)
                sku_match = re.search(r'SKU:\s*([A-Z0-9\-]+)', sku_text)
                if sku_match:
                    skus_in_html.append(sku_match.group(1))
            
            # Also check data attributes
            if card.get('data-sku'):
                skus_in_html.append(card.get('data-sku'))
        
        # Check for hardcoded products
        is_hardcoded = False
        if frontend_count > 0 and frontend_count < 10:
            is_hardcoded = True
        
        # Analyze pagination
        pagination = soup.find('div', class_='pagination') or soup.find('nav', {'aria-label': 'Page navigation'})
        has_pagination = pagination is not None
        
        # Extract current page and total pages
        current_page = 1
        total_pages = 1
        if pagination:
            active_page = pagination.find('a', class_='active') or pagination.find('span', class_='active')
            if active_page:
                current_page = int(active_page.get_text(strip=True))
            
            page_links = pagination.find_all('a', href=True)
            page_numbers = [int(link.get_text(strip=True)) for link in page_links if link.get_text(strip=True).isdigit()]
            if page_numbers:
                total_pages = max(page_numbers)
        
        # Calculate expected products on first page (40 per page)
        expected_on_page1 = min(40, backend_total)
        
        # Determine status
        status = "✅ OK"
        issues = []
        
        if backend_total == 0:
            status = "⚠️ NO DATA"
            issues.append("Backend returns 0 products")
        elif frontend_count == 0:
            status = "❌ CRITICAL"
            issues.append(f"Backend has {backend_total} products but frontend shows 0")
        elif frontend_count < expected_on_page1:
            status = "❌ RENDERING ISSUE"
            issues.append(f"Expected {expected_on_page1} products on page 1, but only {frontend_count} rendered")
        elif is_hardcoded:
            status = "⚠️ POSSIBLY HARDCODED"
            issues.append(f"Only {frontend_count} products shown, might be hardcoded sample data")
        
        # Check SKU patterns
        if skus_in_html:
            if "FW" in page_info['expected_pattern']:
                non_fw_skus = [sku for sku in skus_in_html if not sku.startswith('FW')]
                if non_fw_skus:
                    issues.append(f"Found {len(non_fw_skus)} SKUs NOT starting with FW: {non_fw_skus[:3]}")
            else:
                fw_skus = [sku for sku in skus_in_html if sku.startswith('FW')]
                if fw_skus:
                    issues.append(f"Found {len(fw_skus)} SKUs starting with FW: {fw_skus[:3]}")
        
        # Print results
        print(f"\nStatus: {status}")
        print(f"Backend Total: {backend_total} products")
        print(f"Frontend Displayed: {frontend_count} products")
        print(f"Expected on Page 1: {expected_on_page1} products")
        print(f"Current Page: {current_page}/{total_pages}")
        print(f"Has Pagination: {'Yes' if has_pagination else 'No'}")
        
        if skus_in_html:
            print(f"SKUs Found: {len(skus_in_html)}")
            print(f"Sample SKUs: {skus_in_html[:5]}")
        else:
            print("SKUs Found: None (Could not extract SKUs from HTML)")
        
        if issues:
            print(f"\n⚠️ Issues Found:")
            for i, issue in enumerate(issues, 1):
                print(f"  {i}. {issue}")
        
        return {
            "page": page_name,
            "status": status,
            "backend_total": backend_total,
            "frontend_count": frontend_count,
            "expected": expected_on_page1,
            "issues": issues,
            "skus": skus_in_html[:10]
        }
        
    except requests.exceptions.RequestException as e:
        print(f"❌ ERROR: Failed to fetch page - {e}")
        return {
            "page": page_name,
            "status": "❌ ERROR",
            "error": str(e)
        }
    except Exception as e:
        print(f"❌ ERROR: {e}")
        return {
            "page": page_name,
            "status": "❌ ERROR",
            "error": str(e)
        }

def main():
    print("=" * 80)
    print("PRODUCT PAGE RENDERING ANALYSIS")
    print("=" * 80)
    print(f"Checking {len(PAGES_TO_CHECK)} pages...")
    
    results = []
    
    for page_name, page_info in PAGES_TO_CHECK.items():
        result = analyze_page(page_name, page_info)
        results.append(result)
    
    # Summary
    print("\n" + "=" * 80)
    print("SUMMARY")
    print("=" * 80)
    
    ok_pages = [r for r in results if r['status'] == "✅ OK"]
    warning_pages = [r for r in results if "⚠️" in r['status']]
    error_pages = [r for r in results if "❌" in r['status']]
    
    print(f"\n✅ OK: {len(ok_pages)} pages")
    for r in ok_pages:
        print(f"   - {r['page']}: {r['frontend_count']}/{r['backend_total']} products")
    
    print(f"\n⚠️ WARNING: {len(warning_pages)} pages")
    for r in warning_pages:
        print(f"   - {r['page']}: {r.get('frontend_count', 0)}/{r.get('backend_total', 0)} products")
        if 'issues' in r:
            for issue in r['issues']:
                print(f"     • {issue}")
    
    print(f"\n❌ ERROR: {len(error_pages)} pages")
    for r in error_pages:
        print(f"   - {r['page']}")
        if 'issues' in r:
            for issue in r['issues']:
                print(f"     • {issue}")
        if 'error' in r:
            print(f"     • {r['error']}")
    
    # Save detailed report
    report_file = "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website/database/VIEW_RENDERING_REPORT.json"
    with open(report_file, 'w', encoding='utf-8') as f:
        json.dump(results, f, indent=2, ensure_ascii=False)
    
    print(f"\n📄 Detailed report saved to: {report_file}")
    
    # Recommendations
    print("\n" + "=" * 80)
    print("RECOMMENDATIONS")
    print("=" * 80)
    
    if error_pages or warning_pages:
        print("\nTo fix rendering issues:")
        print("1. Check if views are using @Model to loop through products")
        print("2. Verify @foreach loops are correctly implemented")
        print("3. Remove any hardcoded sample products in the views")
        print("4. Ensure Controller is passing correct Model to View")
        print("5. Check JavaScript filters are not hiding products")
    else:
        print("\n✅ All pages are rendering correctly!")

if __name__ == "__main__":
    main()
