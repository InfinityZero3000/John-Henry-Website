// Payment JavaScript
let stripe;
let elements;
let cardNumber, cardExpiry, cardCvc;

$(document).ready(function() {
    initializePaymentForm();
    initializeStripe();
    initializePaymentMethods();
    initializeBankTransfer();
});

function initializePaymentForm() {
    // Handle payment form submission
    $('#paymentForm').submit(function(e) {
        e.preventDefault();
        
        if (validatePaymentForm()) {
            processPayment();
        }
    });

    // Handle terms acceptance
    $('#acceptTerms').change(function() {
        const submitBtn = $('#submitPaymentBtn');
        if ($(this).is(':checked')) {
            submitBtn.prop('disabled', false);
        } else {
            submitBtn.prop('disabled', true);
        }
    });
}

function initializeStripe() {
    // Initialize Stripe if available
    if (typeof Stripe !== 'undefined') {
        // Replace with your publishable key
        stripe = Stripe('pk_test_YOUR_STRIPE_PUBLISHABLE_KEY');
        elements = stripe.elements();

        // Create card elements
        const style = {
            base: {
                fontSize: '16px',
                color: '#424770',
                '::placeholder': {
                    color: '#aab7c4',
                },
            },
            invalid: {
                color: '#9e2146',
            },
        };

        cardNumber = elements.create('cardNumber', { style });
        cardExpiry = elements.create('cardExpiry', { style });
        cardCvc = elements.create('cardCvc', { style });

        cardNumber.mount('#stripeCardNumber');
        cardExpiry.mount('#stripeCardExpiry');
        cardCvc.mount('#stripeCardCvc');

        // Handle real-time validation errors from the card Element
        cardNumber.on('change', handleCardChange);
        cardExpiry.on('change', handleCardChange);
        cardCvc.on('change', handleCardChange);
    }
}

function handleCardChange(event) {
    const displayError = document.getElementById('card-errors');
    if (event.error) {
        displayError.textContent = event.error.message;
    } else {
        displayError.textContent = '';
    }
}

function initializePaymentMethods() {
    // Handle payment method selection
    $('input[name="SelectedPaymentMethod"]').change(function() {
        const selectedMethod = $(this).val();
        const gateway = $(this).data('gateway');
        
        // Hide all payment detail forms
        $('.payment-details-form').hide();
        
        // Show relevant form based on selection
        switch (gateway) {
            case 'stripe':
                $('#stripePaymentForm').show();
                break;
            case 'bank_transfer':
                $('#bankTransferForm').show();
                break;
            case 'cod':
                $('#codForm').show();
                break;
        }
        
        // Update button text
        updatePaymentButtonText(selectedMethod);
    });
}

function initializeBankTransfer() {
    // Handle copy bank account number
    $('.copy-btn').click(function() {
        const textToCopy = $(this).data('copy');
        copyToClipboard(textToCopy);
        
        // Show feedback
        const originalText = $(this).text();
        $(this).text('Đã copy!').prop('disabled', true);
        
        setTimeout(() => {
            $(this).text(originalText).prop('disabled', false);
        }, 2000);
    });
}

function validatePaymentForm() {
    const selectedPaymentMethod = $('input[name="SelectedPaymentMethod"]:checked').val();
    
    if (!selectedPaymentMethod) {
        showError('Vui lòng chọn phương thức thanh toán');
        return false;
    }

    // Validate terms acceptance
    if (!$('#acceptTerms').is(':checked')) {
        showError('Vui lòng đồng ý với điều khoản sử dụng');
        return false;
    }

    // Validate specific payment methods
    const gateway = $('input[name="SelectedPaymentMethod"]:checked').data('gateway');
    
    switch (gateway) {
        case 'stripe':
            return validateStripeForm();
        case 'bank_transfer':
            return validateBankTransferForm();
        default:
            return true;
    }
}

function validateStripeForm() {
    const cardholderName = $('#stripeCardholderName').val().trim();
    
    if (!cardholderName) {
        showError('Vui lòng nhập tên chủ thẻ');
        return false;
    }
    
    // Stripe elements validation will be handled by Stripe
    return true;
}

function validateBankTransferForm() {
    // Bank transfer doesn't require additional validation
    return true;
}

function processPayment() {
    const selectedPaymentMethod = $('input[name="SelectedPaymentMethod"]:checked').val();
    const gateway = $('input[name="SelectedPaymentMethod"]:checked').data('gateway');
    
    // Show loading modal
    $('#paymentLoadingModal').modal('show');
    
    switch (gateway) {
        case 'stripe':
            processStripePayment();
            break;
        default:
            processStandardPayment();
            break;
    }
}

function processStripePayment() {
    const cardholderName = $('#stripeCardholderName').val();
    
    stripe.createToken(cardNumber, {
        name: cardholderName
    }).then(function(result) {
        if (result.error) {
            // Show error to your customer
            showError(result.error.message);
            $('#paymentLoadingModal').modal('hide');
        } else {
            // Send the token to your server
            submitPaymentWithToken(result.token);
        }
    });
}

function submitPaymentWithToken(token) {
    const formData = getPaymentFormData();
    formData.stripeToken = token.id;
    
    submitPaymentForm(formData);
}

function processStandardPayment() {
    const formData = getPaymentFormData();
    submitPaymentForm(formData);
}

function getPaymentFormData() {
    const selectedPaymentMethod = $('input[name="SelectedPaymentMethod"]:checked').val();
    
    return {
        sessionId: $('#SessionId').val(),
        paymentMethod: selectedPaymentMethod,
        paymentMethodId: $('input[name="PaymentMethodId"]').val(),
        selectedBankAccount: $('#selectedBankAccount').val(),
        transferNote: $('#transferNote').val()
    };
}

function submitPaymentForm(formData) {
    $.ajax({
        url: $('#paymentForm').attr('action'),
        method: 'POST',
        data: formData,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            $('#paymentLoadingModal').modal('hide');
            
            if (response.success) {
                if (response.redirectUrl) {
                    // Redirect to payment gateway or success page
                    window.location.href = response.redirectUrl;
                } else {
                    showSuccess(response.message || 'Thanh toán thành công');
                }
            } else {
                showError(response.message || 'Thanh toán thất bại');
            }
        },
        error: function(xhr) {
            $('#paymentLoadingModal').modal('hide');
            
            let errorMessage = 'Có lỗi xảy ra khi xử lý thanh toán';
            
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            
            showError(errorMessage);
        }
    });
}

function updatePaymentButtonText(paymentMethod) {
    const buttonText = getPaymentButtonText(paymentMethod);
    $('#paymentButtonText').text(buttonText);
}

function getPaymentButtonText(paymentMethod) {
    switch (paymentMethod) {
        case 'vnpay':
            return 'Thanh toán với VNPay';
        case 'momo':
            return 'Thanh toán với MoMo';
        case 'stripe':
            return 'Thanh toán bằng thẻ';
        case 'cod':
            return 'Xác nhận đặt hàng';
        case 'bank_transfer':
            return 'Xác nhận chuyển khoản';
        default:
            return 'Hoàn tất thanh toán';
    }
}

function copyToClipboard(text) {
    if (navigator.clipboard && window.isSecureContext) {
        // Use the Clipboard API when available
        navigator.clipboard.writeText(text);
    } else {
        // Fallback for older browsers
        const textArea = document.createElement('textarea');
        textArea.value = text;
        textArea.style.position = 'fixed';
        textArea.style.left = '-999999px';
        textArea.style.top = '-999999px';
        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();
        
        try {
            document.execCommand('copy');
        } catch (err) {
            console.error('Error copying text: ', err);
        }
        
        document.body.removeChild(textArea);
    }
}

function showError(message) {
    // Create and show error alert
    const alertHtml = `
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            <i class="fas fa-exclamation-circle me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    // Insert at the top of the form
    $('#paymentForm').prepend(alertHtml);
    
    // Scroll to top
    $('html, body').animate({ scrollTop: 0 }, 300);
    
    // Auto-remove after 10 seconds
    setTimeout(() => {
        $('.alert').fadeOut();
    }, 10000);
}

function showSuccess(message) {
    // Create and show success alert
    const alertHtml = `
        <div class="alert alert-success alert-dismissible fade show" role="alert">
            <i class="fas fa-check-circle me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    // Insert at the top of the form
    $('#paymentForm').prepend(alertHtml);
    
    // Scroll to top
    $('html, body').animate({ scrollTop: 0 }, 300);
}

// Handle page unload warning for payment in progress
window.addEventListener('beforeunload', function(e) {
    if ($('#paymentLoadingModal').hasClass('show')) {
        e.preventDefault();
        e.returnValue = 'Thanh toán đang được xử lý. Bạn có chắc chắn muốn rời khỏi trang?';
        return e.returnValue;
    }
});

// Prevent double submission
let isSubmitting = false;

$('#paymentForm').submit(function(e) {
    if (isSubmitting) {
        e.preventDefault();
        return false;
    }
    
    isSubmitting = true;
    
    // Reset flag after 30 seconds to allow retry if needed
    setTimeout(() => {
        isSubmitting = false;
    }, 30000);
});

// Security: Disable right-click and certain keyboard shortcuts on payment page
$(document).ready(function() {
    // Disable right-click context menu
    $(document).on('contextmenu', function(e) {
        e.preventDefault();
        return false;
    });
    
    // Disable certain keyboard shortcuts
    $(document).keydown(function(e) {
        // Disable F12, Ctrl+Shift+I, Ctrl+Shift+J, Ctrl+U
        if (e.keyCode === 123 || 
            (e.ctrlKey && e.shiftKey && (e.keyCode === 73 || e.keyCode === 74)) ||
            (e.ctrlKey && e.keyCode === 85)) {
            e.preventDefault();
            return false;
        }
    });
});

// Payment method specific validations
function validatePaymentMethodSpecific(gateway) {
    switch (gateway) {
        case 'vnpay':
            return validateVNPayPayment();
        case 'momo':
            return validateMoMoPayment();
        case 'stripe':
            return validateStripePayment();
        default:
            return true;
    }
}

function validateVNPayPayment() {
    // VNPay specific validation if needed
    return true;
}

function validateMoMoPayment() {
    // MoMo specific validation if needed
    return true;
}

function validateStripePayment() {
    // This will be handled by Stripe Elements
    return true;
}

// Auto-focus on first payment method
$(document).ready(function() {
    const firstPaymentMethod = $('input[name="SelectedPaymentMethod"]:first');
    if (firstPaymentMethod.length) {
        firstPaymentMethod.focus();
    }
});
