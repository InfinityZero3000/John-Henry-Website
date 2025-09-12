// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Product interaction functionality
document.addEventListener('DOMContentLoaded', function() {
    // Initialize cart count on page load
    updateCartCount();
    
    // Initialize wishlist states for products on the page
    initializeWishlistStates();
});

// Add to Wishlist functionality
function addToWishlist(productId, buttonElement) {
    if (!isUserAuthenticated()) {
        showAuthRequiredModal();
        return;
    }

    const originalIcon = buttonElement.innerHTML;
    buttonElement.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
    buttonElement.disabled = true;

    fetch('/Products/AddToWishlist', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify(productId)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            buttonElement.innerHTML = '<i class="fas fa-heart text-danger"></i>';
            buttonElement.onclick = () => removeFromWishlist(productId, buttonElement);
            showToast('Success', data.message, 'success');
        } else {
            buttonElement.innerHTML = originalIcon;
            showToast('Error', data.message, 'error');
        }
    })
    .catch(error => {
        buttonElement.innerHTML = originalIcon;
        showToast('Error', 'An error occurred while adding to wishlist', 'error');
        console.error('Error:', error);
    })
    .finally(() => {
        buttonElement.disabled = false;
    });
}

// Remove from Wishlist functionality
function removeFromWishlist(productId, buttonElement) {
    const originalIcon = buttonElement.innerHTML;
    buttonElement.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
    buttonElement.disabled = true;

    fetch('/Products/RemoveFromWishlist', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify(productId)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            buttonElement.innerHTML = '<i class="far fa-heart"></i>';
            buttonElement.onclick = () => addToWishlist(productId, buttonElement);
            showToast('Success', data.message, 'success');
        } else {
            buttonElement.innerHTML = originalIcon;
            showToast('Error', data.message, 'error');
        }
    })
    .catch(error => {
        buttonElement.innerHTML = originalIcon;
        showToast('Error', 'An error occurred while removing from wishlist', 'error');
        console.error('Error:', error);
    })
    .finally(() => {
        buttonElement.disabled = false;
    });
}

// Add to Cart functionality
function addToCart(productId, buttonElement, quantity = 1, size = null, color = null) {
    if (!isUserAuthenticated()) {
        showAuthRequiredModal();
        return;
    }

    const originalText = buttonElement.innerHTML;
    buttonElement.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Adding...';
    buttonElement.disabled = true;

    const requestData = {
        productId: productId,
        quantity: quantity
    };

    if (size) requestData.size = size;
    if (color) requestData.color = color;

    fetch('/Products/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: JSON.stringify(requestData)
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            buttonElement.innerHTML = '<i class="fas fa-check"></i> Added!';
            updateCartCount(data.cartCount);
            showToast('Success', data.message, 'success');
            
            // Reset button after 2 seconds
            setTimeout(() => {
                buttonElement.innerHTML = originalText;
                buttonElement.disabled = false;
            }, 2000);
        } else {
            buttonElement.innerHTML = originalText;
            showToast('Error', data.message, 'error');
            buttonElement.disabled = false;
        }
    })
    .catch(error => {
        buttonElement.innerHTML = originalText;
        showToast('Error', 'An error occurred while adding to cart', 'error');
        console.error('Error:', error);
        buttonElement.disabled = false;
    });
}

// Update cart count in navigation
function updateCartCount(count = null) {
    if (count !== null) {
        updateCartCountDisplay(count);
    } else {
        // Fetch current cart count
        fetch('/Products/GetCartCount', {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.json())
        .then(data => {
            updateCartCountDisplay(data.cartCount);
        })
        .catch(error => {
            console.error('Error fetching cart count:', error);
        });
    }
}

function updateCartCountDisplay(count) {
    const cartCountElements = document.querySelectorAll('.cart-count');
    cartCountElements.forEach(element => {
        element.textContent = count;
        element.style.display = count > 0 ? 'inline' : 'none';
    });
}

// Initialize wishlist states for products on current page
function initializeWishlistStates() {
    const wishlistButtons = document.querySelectorAll('[data-wishlist-product-id]');
    
    wishlistButtons.forEach(button => {
        const productId = button.getAttribute('data-wishlist-product-id');
        
        fetch(`/Products/IsInWishlist?productId=${productId}`, {
            method: 'GET',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
        .then(response => response.json())
        .then(data => {
            if (data.isInWishlist) {
                button.innerHTML = '<i class="fas fa-heart text-danger"></i>';
                button.onclick = () => removeFromWishlist(productId, button);
            } else {
                button.innerHTML = '<i class="far fa-heart"></i>';
                button.onclick = () => addToWishlist(productId, button);
            }
        })
        .catch(error => {
            console.error('Error checking wishlist status:', error);
            // Default to not in wishlist
            button.innerHTML = '<i class="far fa-heart"></i>';
            button.onclick = () => addToWishlist(productId, button);
        });
    });
}

// Check if user is authenticated
function isUserAuthenticated() {
    // Check if there's an authentication indicator in the DOM
    return document.querySelector('.user-authenticated') !== null || 
           document.querySelector('[data-user-authenticated="true"]') !== null;
}

// Show authentication required modal
function showAuthRequiredModal() {
    // Create and show a modal for authentication requirement
    const modalHtml = `
        <div class="modal fade" id="authRequiredModal" tabindex="-1" aria-labelledby="authRequiredModalLabel" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="authRequiredModalLabel">Login Required</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p>You need to login to add items to your wishlist or cart.</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <a href="/Account/Login" class="btn btn-primary">Login</a>
                        <a href="/Account/Register" class="btn btn-outline-primary">Register</a>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    // Remove existing modal if any
    const existingModal = document.getElementById('authRequiredModal');
    if (existingModal) {
        existingModal.remove();
    }
    
    // Add modal to page
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('authRequiredModal'));
    modal.show();
}

// Toast notification system
function showToast(title, message, type = 'info') {
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '9999';
        document.body.appendChild(toastContainer);
    }
    
    const toastId = 'toast-' + Date.now();
    const bgClass = type === 'success' ? 'bg-success' : type === 'error' ? 'bg-danger' : 'bg-info';
    
    const toastHtml = `
        <div id="${toastId}" class="toast ${bgClass} text-white" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header ${bgClass} text-white border-0">
                <strong class="me-auto">${title}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;
    
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    
    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: 5000
    });
    
    toast.show();
    
    // Remove toast element after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function() {
        toastElement.remove();
    });
}
