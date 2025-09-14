// Checkout JavaScript
$(document).ready(function() {
    // Initialize checkout functionality
    initializeCheckout();
    initializeShippingCalculation();
    initializeCouponValidation();
    initializeAddressManagement();
    initializeFormValidation();
});

function initializeCheckout() {
    // Handle saved address selection
    $('#savedAddressSelect').change(function() {
        const selectedOption = $(this).find('option:selected');
        if (selectedOption.val()) {
            populateAddressFields(selectedOption);
        }
    });

    // Handle billing address toggle
    $('#useSameAddress').change(function() {
        const billingSection = $('#billingAddressSection');
        if ($(this).is(':checked')) {
            billingSection.hide();
            copyShippingToBilling();
        } else {
            billingSection.show();
        }
    });

    // Handle shipping method selection
    $('input[name="ShippingMethod"]').change(function() {
        const cost = parseFloat($(this).data('cost')) || 0;
        const deliveryTime = $(this).data('delivery-time');
        
        updateShippingFee(cost);
        updateDeliveryTime(deliveryTime);
        calculateTotal();
    });

    // Handle checkout form submission
    $('#checkoutForm').submit(function(e) {
        e.preventDefault();
        
        if (validateCheckoutForm()) {
            submitCheckoutForm();
        }
    });
}

function initializeShippingCalculation() {
    // Calculate shipping fee based on address and items
    function calculateShippingFee() {
        const shippingMethod = $('input[name="ShippingMethod"]:checked').val();
        const address = getShippingAddress();
        const items = getCartItems();
        
        if (!shippingMethod || !address.city) {
            return;
        }

        $.ajax({
            url: '/api/shipping/calculate',
            method: 'POST',
            data: {
                address: address,
                shippingMethod: shippingMethod,
                items: items
            },
            success: function(response) {
                if (response.success) {
                    updateShippingFee(response.shippingFee);
                    calculateTotal();
                }
            }
        });
    }

    // Trigger calculation when address changes
    $('#ShippingAddress_City, #ShippingAddress_District').change(calculateShippingFee);
}

function initializeCouponValidation() {
    $('#applyCouponBtn').click(function() {
        const couponCode = $('#CouponCode').val().trim();
        
        if (!couponCode) {
            showCouponMessage('Vui lòng nhập mã giảm giá', 'warning');
            return;
        }

        validateCoupon(couponCode);
    });

    // Allow enter key to apply coupon
    $('#CouponCode').keypress(function(e) {
        if (e.which === 13) {
            $('#applyCouponBtn').click();
        }
    });
}

function validateCoupon(couponCode) {
    $('#applyCouponBtn').prop('disabled', true).text('Đang kiểm tra...');
    
    const orderValue = parseFloat($('#subtotal').text().replace(/[^\d]/g, '')) || 0;
    const productIds = getCartProductIds();
    
    $.ajax({
        url: '/api/coupon/validate',
        method: 'POST',
        data: {
            couponCode: couponCode,
            orderValue: orderValue,
            productIds: productIds
        },
        success: function(response) {
            if (response.success) {
                updateDiscountAmount(response.discountAmount);
                showCouponMessage(`Áp dụng thành công! Giảm ${formatCurrency(response.discountAmount)}`, 'success');
                calculateTotal();
            } else {
                showCouponMessage(response.message || 'Mã giảm giá không hợp lệ', 'error');
            }
        },
        error: function() {
            showCouponMessage('Có lỗi xảy ra khi kiểm tra mã giảm giá', 'error');
        },
        complete: function() {
            $('#applyCouponBtn').prop('disabled', false).text('Áp dụng');
        }
    });
}

function initializeAddressManagement() {
    // Auto-complete address fields
    if (typeof google !== 'undefined' && google.maps) {
        initializeAddressAutocomplete();
    }
}

function initializeAddressAutocomplete() {
    const addressInput = document.getElementById('ShippingAddress_Address');
    
    if (addressInput) {
        const autocomplete = new google.maps.places.Autocomplete(addressInput, {
            types: ['address'],
            componentRestrictions: { country: 'VN' }
        });

        autocomplete.addListener('place_changed', function() {
            const place = autocomplete.getPlace();
            populateAddressFromPlace(place);
        });
    }
}

function initializeFormValidation() {
    // Real-time validation
    $('input[required], select[required]').blur(function() {
        validateField($(this));
    });

    // Email validation
    $('#Email').blur(function() {
        validateEmail($(this));
    });

    // Phone validation
    $('input[type="tel"]').blur(function() {
        validatePhone($(this));
    });
}

function populateAddressFields(selectedOption) {
    $('#ShippingAddress_FullName').val(selectedOption.data('fullname') || '');
    $('#ShippingAddress_PhoneNumber').val(selectedOption.data('phone') || '');
    $('#ShippingAddress_Address').val(selectedOption.data('address') || '');
    $('#ShippingAddress_Ward').val(selectedOption.data('ward') || '');
    $('#ShippingAddress_District').val(selectedOption.data('district') || '');
    $('#ShippingAddress_City').val(selectedOption.data('city') || '');
    $('#ShippingAddress_PostalCode').val(selectedOption.data('postalcode') || '');
}

function copyShippingToBilling() {
    $('#BillingAddress_FullName').val($('#ShippingAddress_FullName').val());
    $('#BillingAddress_PhoneNumber').val($('#ShippingAddress_PhoneNumber').val());
    $('#BillingAddress_Address').val($('#ShippingAddress_Address').val());
    $('#BillingAddress_Ward').val($('#ShippingAddress_Ward').val());
    $('#BillingAddress_District').val($('#ShippingAddress_District').val());
    $('#BillingAddress_City').val($('#ShippingAddress_City').val());
    $('#BillingAddress_PostalCode').val($('#ShippingAddress_PostalCode').val());
}

function updateShippingFee(fee) {
    const formattedFee = fee > 0 ? formatCurrency(fee) : 'Miễn phí';
    $('#shippingFee').text(formattedFee);
    
    // Store the numeric value for calculation
    $('#shippingFee').data('amount', fee);
}

function updateDiscountAmount(amount) {
    if (amount > 0) {
        $('#discountAmount').text('-' + formatCurrency(amount));
        $('#discountRow').show();
    } else {
        $('#discountRow').hide();
    }
    
    // Store the numeric value for calculation
    $('#discountAmount').data('amount', amount);
}

function calculateTotal() {
    const subtotal = parseFloat($('#subtotal').text().replace(/[^\d]/g, '')) || 0;
    const shippingFee = parseFloat($('#shippingFee').data('amount')) || 0;
    const discountAmount = parseFloat($('#discountAmount').data('amount')) || 0;
    const tax = subtotal * 0.1; // 10% VAT
    
    const total = subtotal + shippingFee + tax - discountAmount;
    
    $('#tax').text(formatCurrency(tax));
    $('#total').text(formatCurrency(total));
}

function getShippingAddress() {
    return {
        fullName: $('#ShippingAddress_FullName').val(),
        phoneNumber: $('#ShippingAddress_PhoneNumber').val(),
        address: $('#ShippingAddress_Address').val(),
        ward: $('#ShippingAddress_Ward').val(),
        district: $('#ShippingAddress_District').val(),
        city: $('#ShippingAddress_City').val(),
        postalCode: $('#ShippingAddress_PostalCode').val()
    };
}

function getBillingAddress() {
    if ($('#useSameAddress').is(':checked')) {
        return getShippingAddress();
    }
    
    return {
        fullName: $('#BillingAddress_FullName').val(),
        phoneNumber: $('#BillingAddress_PhoneNumber').val(),
        address: $('#BillingAddress_Address').val(),
        ward: $('#BillingAddress_Ward').val(),
        district: $('#BillingAddress_District').val(),
        city: $('#BillingAddress_City').val(),
        postalCode: $('#BillingAddress_PostalCode').val()
    };
}

function getCartItems() {
    // This should be populated from the server or stored in data attributes
    return window.cartItems || [];
}

function getCartProductIds() {
    return getCartItems().map(item => item.productId);
}

function validateCheckoutForm() {
    let isValid = true;
    
    // Validate required fields
    $('input[required], select[required]').each(function() {
        if (!validateField($(this))) {
            isValid = false;
        }
    });

    // Validate email
    if (!validateEmail($('#Email'))) {
        isValid = false;
    }

    // Validate shipping method
    if (!$('input[name="ShippingMethod"]:checked').length) {
        showError('Vui lòng chọn phương thức vận chuyển');
        isValid = false;
    }

    return isValid;
}

function validateField($field) {
    const value = $field.val().trim();
    const fieldName = $field.closest('.form-group').find('label').text().replace('*', '').trim();
    
    if (!value) {
        showFieldError($field, `${fieldName} là bắt buộc`);
        return false;
    }
    
    clearFieldError($field);
    return true;
}

function validateEmail($emailField) {
    const email = $emailField.val().trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    if (!email) {
        showFieldError($emailField, 'Email là bắt buộc');
        return false;
    }
    
    if (!emailRegex.test(email)) {
        showFieldError($emailField, 'Địa chỉ email không hợp lệ');
        return false;
    }
    
    clearFieldError($emailField);
    return true;
}

function validatePhone($phoneField) {
    const phone = $phoneField.val().trim();
    const phoneRegex = /^[\d\s\-\+\(\)]+$/;
    
    if (phone && !phoneRegex.test(phone)) {
        showFieldError($phoneField, 'Số điện thoại không hợp lệ');
        return false;
    }
    
    clearFieldError($phoneField);
    return true;
}

function showFieldError($field, message) {
    $field.addClass('is-invalid');
    
    let errorElement = $field.siblings('.text-danger');
    if (!errorElement.length) {
        errorElement = $('<span class="text-danger"></span>');
        $field.after(errorElement);
    }
    
    errorElement.text(message);
}

function clearFieldError($field) {
    $field.removeClass('is-invalid');
    $field.siblings('.text-danger').remove();
}

function showCouponMessage(message, type) {
    const messageElement = $('#couponMessage');
    const alertClass = type === 'success' ? 'alert-success' : 
                     type === 'warning' ? 'alert-warning' : 'alert-danger';
    
    messageElement
        .removeClass('alert-success alert-warning alert-danger')
        .addClass(`alert ${alertClass}`)
        .text(message)
        .show();
    
    // Auto-hide after 5 seconds
    setTimeout(() => {
        messageElement.fadeOut();
    }, 5000);
}

function submitCheckoutForm() {
    const submitBtn = $('#continueToPaymentBtn');
    const originalText = submitBtn.text();
    
    // Show loading state
    submitBtn.prop('disabled', true)
             .addClass('btn-loading')
             .html('<i class="fas fa-spinner fa-spin me-2"></i>Đang xử lý...');

    const formData = {
        email: $('#Email').val(),
        shippingAddress: getShippingAddress(),
        billingAddress: getBillingAddress(),
        useSameAddressForBilling: $('#useSameAddress').is(':checked'),
        shippingMethod: $('input[name="ShippingMethod"]:checked').val(),
        couponCode: $('#CouponCode').val(),
        notes: $('#Notes').val()
    };

    $.ajax({
        url: $('#checkoutForm').attr('action'),
        method: 'POST',
        data: formData,
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function(response) {
            if (response.success) {
                // Redirect to payment page
                window.location.href = `/Checkout/Payment?sessionId=${response.sessionId}`;
            } else {
                showError(response.message || 'Có lỗi xảy ra khi tạo phiên thanh toán');
            }
        },
        error: function(xhr) {
            let errorMessage = 'Có lỗi xảy ra khi xử lý yêu cầu';
            
            if (xhr.responseJSON && xhr.responseJSON.message) {
                errorMessage = xhr.responseJSON.message;
            }
            
            showError(errorMessage);
        },
        complete: function() {
            // Reset button state
            submitBtn.prop('disabled', false)
                     .removeClass('btn-loading')
                     .text(originalText);
        }
    });
}

function showError(message) {
    // You can implement a toast notification or modal here
    alert(message);
}

function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND',
        minimumFractionDigits: 0
    }).format(amount).replace('₫', '₫');
}

function populateAddressFromPlace(place) {
    if (!place.address_components) return;
    
    let address = '';
    let ward = '';
    let district = '';
    let city = '';
    let postalCode = '';
    
    for (const component of place.address_components) {
        const types = component.types;
        
        if (types.includes('street_number')) {
            address = component.long_name + ' ' + address;
        } else if (types.includes('route')) {
            address += component.long_name;
        } else if (types.includes('sublocality_level_1') || types.includes('ward')) {
            ward = component.long_name;
        } else if (types.includes('administrative_area_level_2') || types.includes('district')) {
            district = component.long_name;
        } else if (types.includes('administrative_area_level_1') || types.includes('city')) {
            city = component.long_name;
        } else if (types.includes('postal_code')) {
            postalCode = component.long_name;
        }
    }
    
    $('#ShippingAddress_Address').val(address.trim());
    $('#ShippingAddress_Ward').val(ward);
    $('#ShippingAddress_District').val(district);
    $('#ShippingAddress_City').val(city);
    $('#ShippingAddress_PostalCode').val(postalCode);
}
