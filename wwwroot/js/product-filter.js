// Product Filter JavaScript - Common functionality for all product pages
// This script handles category filtering with Vietnamese text normalization

document.addEventListener('DOMContentLoaded', function() {
    const allProducts = document.querySelectorAll('.col-xl-3');
    
    // Helper function to normalize Vietnamese text for comparison
    function normalizeVietnamese(str) {
        // Convert to lowercase and normalize Unicode
        return str.toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '') // Remove diacritics
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'd');
    }
    
    // Helper function to filter products
    function filterProducts() {
        const selectedCategory = document.querySelector('input[name="category"]:checked');
        const selectedColor = document.querySelector('input[name="color"]:checked');
        const activeSizes = document.querySelectorAll('.size-btn.active');
        
        allProducts.forEach(product => {
            let showProduct = true;
            const productName = product.querySelector('.product-name')?.textContent || '';
            
            // Category filtering
            if (selectedCategory) {
                const categoryValue = selectedCategory.value;
                showProduct = checkCategoryMatch(productName, categoryValue);
            }
            
            // Show/hide product
            product.style.display = showProduct ? '' : 'none';
        });
    }
    
    // Check if product name matches category
    function checkCategoryMatch(productName, category) {
        // Normalize product name for comparison
        const normalizedName = normalizeVietnamese(productName);
        
        const categoryKeywords = {
            'ao': ['ao'],
            'ao-polo': ['polo'],
            'ao-so-mi': ['so mi', 'somi'],
            'ao-len': ['len', 'sweater'],
            'ao-khoac': ['khoac', 'jacket'],
            'ao-thun': ['thun'],
            'quan': ['quan'],
            'quan-khaki': ['khaki'],
            'quan-jeans': ['jeans', 'jean'],
            'quan-tay': ['tay'],
            'quan-short': ['short'],
            'quan-jogger': ['jogger'],
            'phu-kien': ['mu', 'giay', 'dep', 'that lung', 'vi', 'tui', 'mat kinh'],
            'vi-nam': ['vi'],
            'that-lung': ['that lung'],
            'giay-dep': ['giay', 'dep'],
            'tui-xach': ['tui'],
            'dam': ['dam'],
            'chan-vay': ['chan vay', 'vay', 'skirt']
        };
        
        const keywords = categoryKeywords[category];
        if (!keywords) return true;
        
        // Check if any keyword matches (case-insensitive, accent-insensitive)
        return keywords.some(keyword => {
            const normalizedKeyword = normalizeVietnamese(keyword);
            return normalizedName.includes(normalizedKeyword);
        });
    }

    // Color filter functionality
    const colorRadios = document.querySelectorAll('input[name="color"]');
    colorRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            console.log('Color selected:', this.value);
            filterProducts();
        });
    });

    // Category filter functionality  
    const categoryRadios = document.querySelectorAll('input[name="category"]');
    categoryRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            console.log('Category selected:', this.value);
            filterProducts();
        });
    });

    // Size button functionality
    const sizeBtns = document.querySelectorAll('.size-btn');
    sizeBtns.forEach(btn => {
        btn.addEventListener('click', function() {
            // Toggle active state
            this.classList.toggle('active');
            if (this.classList.contains('active')) {
                this.classList.remove('btn-outline-secondary');
                this.classList.add('btn-primary');
            } else {
                this.classList.remove('btn-primary');
                this.classList.add('btn-outline-secondary');
            }
            console.log('Size selected:', this.textContent);
        });
    });

    // Clear filters functionality
    const clearFiltersBtn = document.querySelector('.clear-filters');
    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', function() {
            // Clear radio buttons
            const radios = document.querySelectorAll('input[type="radio"]');
            radios.forEach(radio => {
                radio.checked = false;
            });

            // Clear size selections
            sizeBtns.forEach(btn => {
                btn.classList.remove('active', 'btn-primary');
                btn.classList.add('btn-outline-secondary');
            });

            // Clear price inputs
            const priceInputs = document.querySelectorAll('input[type="number"]');
            priceInputs.forEach(input => {
                if (input.getAttribute('value') === '0') {
                    input.value = '0';
                } else {
                    input.value = '3000000';
                }
            });

            // Reset price range slider
            const priceRange = document.getElementById('priceRange');
            if (priceRange) {
                priceRange.value = '1500000';
            }

            // Show all products
            allProducts.forEach(product => {
                product.style.display = '';
            });

            console.log('Filters cleared');
        });
    }

    // Price range slider functionality
    const priceRange = document.getElementById('priceRange');
    const minPriceInput = document.querySelector('input[value="0"]');
    const maxPriceInput = document.querySelector('input[value="3000000"]');

    if (priceRange && minPriceInput && maxPriceInput) {
        priceRange.addEventListener('input', function() {
            const value = parseInt(this.value);
            maxPriceInput.value = value;
        });

        minPriceInput.addEventListener('input', function() {
            const value = parseInt(this.value);
            if (value < parseInt(priceRange.max)) {
                priceRange.min = value;
            }
        });

        maxPriceInput.addEventListener('input', function() {
            const value = parseInt(this.value);
            if (value > 0) {
                priceRange.value = value;
            }
        });
    }

    // Wishlist functionality from homepage
    const wishlistBtns = document.querySelectorAll('.wishlist-btn');
    wishlistBtns.forEach(btn => {
        btn.addEventListener('click', function(e) {
            e.preventDefault();
            const productId = this.getAttribute('data-wishlist-product-id');
            if (productId) {
                toggleWishlist(productId, this);
            }
        });
    });

    // Sort functionality
    const sortSelect = document.getElementById('sort-select');
    if (sortSelect) {
        sortSelect.addEventListener('change', function() {
            console.log('Sort by:', this.value);
            // Add sorting functionality here
        });
    }

    // Filter section toggle functionality
    const filterTitles = document.querySelectorAll('.filter-section h5');
    filterTitles.forEach(title => {
        title.addEventListener('click', function() {
            const icon = this.querySelector('i');
            const content = this.nextElementSibling;
            
            if (content.style.display === 'none') {
                content.style.display = 'block';
                icon.classList.remove('fa-chevron-right');
                icon.classList.add('fa-chevron-down');
            } else {
                content.style.display = 'none';
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-right');
            }
        });
    });

    // Subcategory dropdown functionality
    const expandIcons = document.querySelectorAll('.expand-icon');
    expandIcons.forEach(icon => {
        icon.addEventListener('click', function(e) {
            e.stopPropagation(); // Prevent triggering parent label click
            const targetId = this.getAttribute('data-target');
            const subcategoriesDiv = document.getElementById(targetId);
            
            if (subcategoriesDiv) {
                if (subcategoriesDiv.classList.contains('show')) {
                    // Close dropdown with smooth animation
                    subcategoriesDiv.style.maxHeight = '0';
                    subcategoriesDiv.style.opacity = '0';
                    this.classList.remove('expanded');
                    
                    setTimeout(() => {
                        subcategoriesDiv.classList.remove('show');
                        subcategoriesDiv.style.display = 'none';
                    }, 300);
                } else {
                    // Close all other dropdowns first
                    document.querySelectorAll('.subcategories.show').forEach(div => {
                        div.style.maxHeight = '0';
                        div.style.opacity = '0';
                        setTimeout(() => {
                            div.classList.remove('show');
                            div.style.display = 'none';
                        }, 300);
                    });
                    document.querySelectorAll('.expand-icon.expanded').forEach(otherIcon => {
                        otherIcon.classList.remove('expanded');
                    });
                    
                    // Open this dropdown with smooth animation
                    subcategoriesDiv.style.display = 'block';
                    subcategoriesDiv.classList.add('show');
                    this.classList.add('expanded');
                    
                    // Trigger reflow for smooth animation
                    subcategoriesDiv.offsetHeight;
                    subcategoriesDiv.style.maxHeight = '300px';
                    subcategoriesDiv.style.opacity = '1';
                }
            }
        });
    });

    // Handle subcategory selection
    const subcategoryInputs = document.querySelectorAll('.subcategory-item input[type="radio"]');
    subcategoryInputs.forEach(input => {
        input.addEventListener('change', function() {
            if (this.checked) {
                // Uncheck parent category radio button
                const parentCategory = this.closest('.filter-option').querySelector('input[type="radio"]');
                if (parentCategory) {
                    parentCategory.checked = false;
                }
                console.log('Subcategory selected:', this.value);
                // Trigger filter
                filterProducts();
            }
        });
    });
});

// Wishlist function (to be defined globally or in _Layout.cshtml)
function toggleWishlist(productId, button) {
    const icon = button.querySelector('i');
    
    if (icon.classList.contains('far')) {
        icon.classList.remove('far');
        icon.classList.add('fas');
        button.style.color = '#dc3545';
    } else {
        icon.classList.remove('fas');
        icon.classList.add('far');
        button.style.color = '';
    }
}
