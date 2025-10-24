/**
 * Buy Now Functionality
 * Allows users to purchase a single product immediately without adding to cart
 */

/**
 * Quick buy now from product cards
 * @param {string} productSku - Product SKU
 * @param {string} productName - Product name for display
 * @param {HTMLElement} buttonElement - Button that was clicked
 */
function buyNowQuick(productSku, productName, buttonElement) {
    console.log('🛒 Buy Now Quick - SKU:', productSku, 'Name:', productName);
    
    // Validate product SKU
    if (!productSku || productSku === '' || productSku === 'undefined') {
        console.error('❌ Invalid product SKU:', productSku);
        showToastNotification('Sản phẩm không hợp lệ', 'error');
        return;
    }

    // Show loading state
    const originalContent = buttonElement.innerHTML;
    buttonElement.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
    buttonElement.disabled = true;

    // Prepare form data with default values
    const formData = new FormData();
    formData.append('productId', productSku);
    formData.append('quantity', 1); // Default quantity
    formData.append('size', 'M'); // Default size
    formData.append('color', ''); // No color selection

    console.log('📤 Sending BuyNow request...');

    // Send buy now request
    fetch('/Products/BuyNow', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        console.log('📥 Response status:', response.status);
        return response.json();
    })
    .then(data => {
        console.log('📥 Response data:', data);
        
        if (data.success) {
            console.log('✅ Buy Now successful, redirecting...');
            showToastNotification('Chuyển đến trang thanh toán...', 'success');
            
            // Redirect to checkout after short delay
            setTimeout(() => {
                window.location.href = data.redirectUrl || '/Cart/Checkout';
            }, 500);
        } else {
            console.error('❌ Buy Now failed:', data.message);
            showToastNotification(data.message || 'Không thể mua ngay. Vui lòng thử lại.', 'error');
            
            // Restore button
            buttonElement.innerHTML = originalContent;
            buttonElement.disabled = false;
        }
    })
    .catch(error => {
        console.error('❌ Buy Now error:', error);
        showToastNotification('Có lỗi xảy ra. Vui lòng thử lại.', 'error');
        
        // Restore button
        buttonElement.innerHTML = originalContent;
        buttonElement.disabled = false;
    });
}

/**
 * Buy now with options (from detail page)
 * @param {string} productSku - Product SKU
 * @param {number} quantity - Quantity to purchase
 * @param {string} size - Selected size
 * @param {string} color - Selected color
 */
function buyNowWithOptions(productSku, quantity, size, color) {
    console.log('🛒 Buy Now with Options - SKU:', productSku, 'Qty:', quantity, 'Size:', size, 'Color:', color);
    
    // Validate inputs
    if (!productSku || productSku === '' || productSku === 'undefined') {
        console.error('❌ Invalid product SKU:', productSku);
        showToastNotification('Sản phẩm không hợp lệ', 'error');
        return;
    }

    if (!quantity || quantity <= 0) {
        console.error('❌ Invalid quantity:', quantity);
        showToastNotification('Số lượng không hợp lệ', 'error');
        return;
    }

    // Prepare form data
    const formData = new FormData();
    formData.append('productId', productSku);
    formData.append('quantity', quantity);
    formData.append('size', size || 'M');
    formData.append('color', color || '');

    console.log('📤 Sending BuyNow request with options...');

    // Send buy now request
    return fetch('/Products/BuyNow', {
        method: 'POST',
        body: formData
    })
    .then(response => {
        console.log('📥 Response status:', response.status);
        return response.json();
    })
    .then(data => {
        console.log('📥 Response data:', data);
        
        if (data.success) {
            console.log('✅ Buy Now successful');
            return { success: true, redirectUrl: data.redirectUrl };
        } else {
            console.error('❌ Buy Now failed:', data.message);
            throw new Error(data.message || 'Không thể mua ngay');
        }
    });
}

/**
 * Show toast notification
 * @param {string} message - Message to display
 * @param {string} type - Type: success, error, warning, info
 */
function showToastNotification(message, type = 'success') {
    // Check if global showToast function exists (from other scripts)
    if (typeof showToast === 'function') {
        showToast(message, type);
        return;
    }

    // Fallback: Create simple toast
    let toastContainer = document.getElementById('toastContainer');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toastContainer';
        toastContainer.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999;';
        document.body.appendChild(toastContainer);
    }

    const toast = document.createElement('div');
    const iconMap = {
        success: 'check-circle',
        error: 'times-circle',
        warning: 'exclamation-triangle',
        info: 'info-circle'
    };
    const colorMap = {
        success: 'success',
        error: 'danger',
        warning: 'warning',
        info: 'info'
    };

    toast.className = `alert alert-${colorMap[type]} alert-dismissible fade show`;
    toast.style.cssText = 'min-width: 300px; box-shadow: 0 4px 12px rgba(0,0,0,0.15); animation: slideInRight 0.3s ease;';
    toast.innerHTML = `
        <strong><i class="fas fa-${iconMap[type]} me-2"></i></strong>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    toastContainer.appendChild(toast);

    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// CSS Animation for toast
if (!document.getElementById('buyNowStyles')) {
    const style = document.createElement('style');
    style.id = 'buyNowStyles';
    style.textContent = `
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        .buy-now-overlay {
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            background: linear-gradient(to top, rgba(0,0,0,0.7) 0%, transparent 100%);
            padding: 15px;
            opacity: 0;
            transition: opacity 0.3s ease;
            display: flex;
            justify-content: center;
            align-items: flex-end;
        }

        .product-card-new:hover .buy-now-overlay {
            opacity: 1;
        }

        .buy-now-btn {
            font-weight: 600;
            font-size: 0.85rem;
            padding: 8px 20px;
            border-radius: 25px;
            box-shadow: 0 4px 12px rgba(220, 53, 69, 0.4);
            transition: all 0.3s ease;
            border: none;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .buy-now-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(220, 53, 69, 0.6);
        }

        .buy-now-btn:active {
            transform: translateY(0);
        }

        .buy-now-btn i {
            animation: pulse 2s infinite;
        }

        @keyframes pulse {
            0%, 100% {
                opacity: 1;
            }
            50% {
                opacity: 0.6;
            }
        }
    `;
    document.head.appendChild(style);
}

console.log('✅ Buy Now functionality loaded');
