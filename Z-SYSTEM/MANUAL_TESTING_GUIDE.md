# HƯỚNG DẪN TEST MANUAL - CHECKOUT & PAYMENT FLOWS

**Ngày tạo:** 26/10/2025  
**Phiên bản:** 1.0  
**Trạng thái:** Ready for Testing

---

## MỤC LỤC
1. [Chuẩn bị môi trường test](#1-chuẩn-bị-môi-trường-test)
2. [Test COD Flow](#2-test-cod-flow)
3. [Test Bank Transfer Flow](#3-test-bank-transfer-flow)
4. [Test VNPay Flow](#4-test-vnpay-flow)
5. [Test MoMo Flow](#5-test-momo-flow)
6. [Test Stripe Flow](#6-test-stripe-flow)
7. [Test Error Scenarios](#7-test-error-scenarios)
8. [Test Session Expiry](#8-test-session-expiry)
9. [Checklist Validation](#9-checklist-validation)

---

## 1. CHUẨN BỊ MÔI TRƯỜNG TEST

### 1.1. Prerequisites
- [x] Application đang chạy: `http://localhost:5101`
- [ ] Database seeded với PaymentMethods records
- [ ] User account để login (hoặc test với guest checkout)
- [ ] Có ít nhất 1 sản phẩm trong cart

### 1.2. Kiểm tra Payment Methods trong Database
```sql
-- Chạy query để xem payment methods có sẵn
SELECT code, display_name, is_active, sort_order, service_fee
FROM "PaymentMethods" 
WHERE is_active = true
ORDER BY sort_order;
```

**Expected Result:**
| code | display_name | is_active | sort_order | service_fee |
|------|--------------|-----------|------------|-------------|
| cod | Thanh toán khi nhận hàng | true | 1 | 0 |
| bank_transfer | Chuyển khoản ngân hàng | true | 2 | 0 |
| vnpay | VNPay | true | 3 | 0 |
| momo | MoMo | true | 4 | 0 |
| stripe | Stripe | true | 5 | 0 |

### 1.3. Chuẩn bị Test Data
**Sản phẩm test:**
- Tên: Test Product
- Giá: 500,000 VND
- Số lượng: 1

**Thông tin giao hàng:**
- Họ tên: Nguyễn Văn A
- Email: test@example.com
- SĐT: 0987654321
- Địa chỉ: 123 Test Street, Ward 1, District 1, Ho Chi Minh City

---

## 2. TEST COD FLOW (Cash on Delivery)

### 2.1. Test Steps

#### Bước 1: Add product to cart
1. Vào trang sản phẩm: `http://localhost:5101/Products`
2. Chọn 1 sản phẩm
3. Click "Thêm vào giỏ hàng"
4. ✅ **Verify:** Toast notification "Đã thêm vào giỏ hàng"

#### Bước 2: Go to checkout
1. Click icon giỏ hàng
2. Click "Thanh toán"
3. ✅ **Verify:** Redirect to `/Checkout`

#### Bước 3: Fill shipping information
1. Điền đầy đủ thông tin giao hàng
2. Chọn shipping method (ví dụ: STANDARD)
3. ✅ **Verify:** Shipping fee được tính và hiển thị
4. ✅ **Verify:** Auto discount 5% hiển thị nếu order >= 500k
5. ✅ **Verify:** Total amount được tính đúng

#### Bước 4: Create checkout session
1. Click "Đặt hàng"
2. ✅ **Verify:** Loading modal hiển thị
3. ✅ **Verify:** Redirect to `/Checkout/Payment?sessionId=xxx`

#### Bước 5: Select COD payment method
1. Chọn radio button "Thanh toán khi nhận hàng"
2. ✅ **Verify:** COD form hiển thị với alert warning về số tiền cần chuẩn bị
3. ✅ **Verify:** Button text = "Xác nhận đặt hàng"

#### Bước 6: Confirm terms and submit
1. Check "Đồng ý với điều khoản sử dụng"
2. Click "Xác nhận đặt hàng"
3. ✅ **Verify:** Loading modal hiển thị
4. ✅ **Verify:** AJAX call to `/Checkout/ProcessPayment`

#### Bước 7: Verify success page
1. ✅ **Verify:** Redirect to `/Checkout/Success?orderId=xxx`
2. ✅ **Verify:** Progress steps hiển thị completed
3. ✅ **Verify:** Order number hiển thị
4. ✅ **Verify:** Payment method = "Thanh toán khi nhận hàng"
5. ✅ **Verify:** COD instruction hiển thị: "Vui lòng chuẩn bị số tiền chính xác..."
6. ✅ **Verify:** Order items list hiển thị đúng
7. ✅ **Verify:** Total amount hiển thị đúng

#### Bước 8: Verify database
```sql
-- Kiểm tra Order được tạo
SELECT 
    order_number, 
    payment_method, 
    payment_status, 
    total_amount, 
    created_at
FROM "Orders" 
ORDER BY created_at DESC 
LIMIT 1;
```

**Expected Result:**
- payment_method = 'cod'
- payment_status = 'pending'
- total_amount = (expected amount)

```sql
-- Kiểm tra OrderItems
SELECT 
    product_id, 
    quantity, 
    unit_price, 
    total_price
FROM "OrderItems" 
WHERE order_id = (SELECT id FROM "Orders" ORDER BY created_at DESC LIMIT 1);
```

#### Bước 9: Verify cart cleared
1. Click icon giỏ hàng
2. ✅ **Verify:** Cart rỗng (0 items)

---

## 3. TEST BANK TRANSFER FLOW

### 3.1. Test Steps (tương tự COD, khác biệt ở steps sau)

#### Bước 5: Select Bank Transfer payment method
1. Chọn radio button "Chuyển khoản ngân hàng"
2. ✅ **Verify:** Bank Transfer form hiển thị
3. ✅ **Verify:** 2 tài khoản ngân hàng hiển thị:
   - Vietcombank
   - Techcombank
4. ✅ **Verify:** Transfer note hiển thị: "Thanh toan don hang [SessionId]"
5. ✅ **Verify:** Copy button có sẵn cho account numbers
6. ✅ **Verify:** Button text = "Xác nhận chuyển khoản"

#### Bước 5.1: Test copy functionality
1. Click "Copy" button bên cạnh account number
2. ✅ **Verify:** Button text changes to "Đã copy!"
3. ✅ **Verify:** Account number được copy vào clipboard

#### Bước 7: Verify success page
- ✅ **Verify:** Bank Transfer instruction hiển thị
- ✅ **Verify:** Bank account info hiển thị
- ✅ **Verify:** Transfer content hiển thị

---

## 4. TEST VNPAY FLOW

### 4.1. Prerequisites
- VNPay Sandbox account
- TmnCode và HashSecret configured trong appsettings.json

### 4.2. Test Steps

#### Bước 5: Select VNPay payment method
1. Chọn radio button "VNPay"
2. ✅ **Verify:** VNPay logo hiển thị
3. ✅ **Verify:** Button text = "Thanh toán với VNPay"

#### Bước 6: Submit payment
1. Check terms
2. Click "Thanh toán với VNPay"
3. ✅ **Verify:** Loading modal hiển thị
4. ✅ **Verify:** Redirect to VNPay sandbox URL
5. ✅ **Verify:** URL contains vnp_TmnCode, vnp_Amount, vnp_OrderInfo, vnp_SecureHash

#### Bước 7: Complete payment on VNPay
1. Nhập thông tin test card (VNPay sandbox)
2. Click "Thanh toán"
3. ✅ **Verify:** VNPay callback to `/Checkout/PaymentReturn`
4. ✅ **Verify:** Query parameters: vnp_ResponseCode, vnp_TransactionNo

#### Bước 8: Verify success/failed redirect
**Success case (vnp_ResponseCode = "00"):**
- ✅ **Verify:** Redirect to `/Checkout/Success?orderId=xxx`
- ✅ **Verify:** payment_status = 'completed'

**Failed case (vnp_ResponseCode != "00"):**
- ✅ **Verify:** Redirect to `/Checkout/Failed?orderId=xxx`
- ✅ **Verify:** payment_status = 'failed'
- ✅ **Verify:** Error message hiển thị

---

## 5. TEST MOMO FLOW

### 5.1. Prerequisites
- MoMo Sandbox account
- PartnerCode, AccessKey, SecretKey configured

### 5.2. Test Steps (tương tự VNPay)

#### Bước 5: Select MoMo
1. Chọn "MoMo"
2. ✅ **Verify:** MoMo logo hiển thị
3. ✅ **Verify:** Button text = "Thanh toán với MoMo"

#### Bước 6-8: Similar to VNPay
- Test redirect to MoMo gateway
- Complete payment on MoMo
- Verify callback và redirect

---

## 6. TEST STRIPE FLOW

### 6.1. Prerequisites
- Stripe Sandbox account
- PublishableKey và SecretKey configured
- Stripe.js loaded trong Payment.cshtml

### 6.2. Test Steps

#### Bước 5: Select Stripe payment method
1. Chọn radio button "Stripe"
2. ✅ **Verify:** Stripe form hiển thị
3. ✅ **Verify:** Card input fields hiển thị:
   - Card holder name (text input)
   - Card number (Stripe Element)
   - Expiry date (Stripe Element)
   - CVC (Stripe Element)
4. ✅ **Verify:** Button text = "Thanh toán bằng thẻ"

#### Bước 6: Fill card information
**Stripe Test Card:**
- Number: `4242 4242 4242 4242`
- Expiry: Any future date (e.g., 12/25)
- CVC: Any 3 digits (e.g., 123)
- Name: Test User

1. Điền card holder name
2. ✅ **Verify:** Required field validation
3. Điền card number vào Stripe Element
4. ✅ **Verify:** Real-time validation
5. Điền expiry và CVC
6. ✅ **Verify:** Stripe Elements styling đúng

#### Bước 7: Submit payment
1. Check terms
2. Click "Thanh toán bằng thẻ"
3. ✅ **Verify:** Stripe.createToken() được gọi
4. ✅ **Verify:** Token được tạo thành công
5. ✅ **Verify:** AJAX call với stripeToken parameter

#### Bước 8: Verify success
- ✅ **Verify:** Redirect to Success page
- ✅ **Verify:** payment_status = 'completed'
- ✅ **Verify:** transaction_id có giá trị

---

## 7. TEST ERROR SCENARIOS

### 7.1. Invalid Session ID
**Test:**
1. Truy cập URL: `/Checkout/Payment?sessionId=invalid-guid`
2. ✅ **Expected:** Error page hoặc redirect to Checkout

### 7.2. Session Expired
**Test:**
1. Tạo checkout session
2. Đợi > 1 giờ (hoặc manually update ExpiresAt trong DB)
3. Try to complete payment
4. ✅ **Expected:** Error "Session đã hết hạn"

### 7.3. Invalid Order ID
**Test:**
1. Truy cập URL: `/Checkout/Success?orderId=invalid-guid`
2. ✅ **Expected:** 404 hoặc error message

### 7.4. Payment Gateway Timeout
**Test:**
1. Tắt internet connection (hoặc use invalid API keys)
2. Try to pay với VNPay/MoMo
3. ✅ **Expected:** Error message "Lỗi kết nối..."

### 7.5. Missing Required Fields
**Test:**
1. Leave shipping info empty
2. Click "Đặt hàng"
3. ✅ **Expected:** Validation errors hiển thị

### 7.6. Missing Payment Method Selection
**Test:**
1. Go to Payment page
2. Don't select any payment method
3. Click submit
4. ✅ **Expected:** Error "Vui lòng chọn phương thức thanh toán"

### 7.7. Terms Not Accepted
**Test:**
1. Select payment method
2. Don't check "Đồng ý điều khoản"
3. ✅ **Expected:** Submit button disabled

### 7.8. Double Submission
**Test:**
1. Click submit button
2. Quickly click again
3. ✅ **Expected:** Second click prevented by `isSubmitting` flag

---

## 8. TEST SESSION EXPIRY

### 8.1. Session Timeout (1 hour)
**Manual Test:**
```sql
-- Update session expiry to past time
UPDATE "CheckoutSessions" 
SET expires_at = NOW() - INTERVAL '1 minute'
WHERE id = 'your-session-id';
```

**Test:**
1. Try to submit payment
2. ✅ **Expected:** Error "Phiên thanh toán đã hết hạn"

### 8.2. Session Warning (Future Enhancement)
⚠️ **TODO:** Implement countdown timer
- Show warning 5 minutes before expiry
- Auto-extend session when user is active

---

## 9. CHECKLIST VALIDATION

### 9.1. UI/UX Checks
- [ ] Progress steps display correctly (3 steps)
- [ ] Payment method icons/logos load
- [ ] Responsive design works on mobile
- [ ] Loading modal prevents user interaction
- [ ] Success/Failed pages have proper styling
- [ ] Copy to clipboard functionality works
- [ ] Alert notifications display and auto-dismiss

### 9.2. Functional Checks
- [ ] Cart items persist during checkout
- [ ] Shipping fee calculation correct
- [ ] Auto discount (5% >= 500k) applied
- [ ] Tiered discount for SUPER_EXPRESS applied
- [ ] Tax calculation correct
- [ ] Total amount matches breakdown
- [ ] Order number generated uniquely
- [ ] Payment status updated correctly
- [ ] Cart cleared after successful order

### 9.3. Security Checks
- [ ] CSRF token present in forms
- [ ] Payment signatures verified (VNPay, MoMo)
- [ ] SQL injection prevented
- [ ] XSS attacks prevented
- [ ] Session hijacking prevented
- [ ] Double submission prevented
- [ ] Page unload warning when payment processing

### 9.4. Database Validation
- [ ] Order record created
- [ ] OrderItems records created
- [ ] PaymentAttempt logged
- [ ] CheckoutSession marked as used
- [ ] User cart cleared (if DB-based)

### 9.5. Integration Checks
- [ ] Email notification sent (if configured)
- [ ] SMS notification sent for COD (if configured)
- [ ] Analytics tracking fires (Google Analytics)
- [ ] Payment gateway webhooks handled
- [ ] Refund flow works (if implemented)

---

## 10. EXPECTED RESULTS SUMMARY

### COD Flow
| Step | Expected Result | Status |
|------|-----------------|--------|
| Payment method selection | COD form visible | ⬜ |
| Submit payment | Direct to Success | ⬜ |
| Payment status | 'pending' | ⬜ |
| Cart cleared | Yes | ⬜ |

### Bank Transfer Flow
| Step | Expected Result | Status |
|------|-----------------|--------|
| Payment method selection | Bank accounts visible | ⬜ |
| Copy account number | Clipboard updated | ⬜ |
| Submit payment | Direct to Success | ⬜ |
| Payment status | 'pending' | ⬜ |

### VNPay Flow
| Step | Expected Result | Status |
|------|-----------------|--------|
| Payment method selection | VNPay logo visible | ⬜ |
| Submit payment | Redirect to VNPay | ⬜ |
| Complete payment | Callback to PaymentReturn | ⬜ |
| Success case | Redirect to Success | ⬜ |
| Payment status | 'completed' | ⬜ |

### Stripe Flow
| Step | Expected Result | Status |
|------|-----------------|--------|
| Payment method selection | Card form visible | ⬜ |
| Fill card info | Stripe validation works | ⬜ |
| Submit payment | Token created | ⬜ |
| Payment processed | Direct to Success | ⬜ |
| Payment status | 'completed' | ⬜ |

---

## 11. KNOWN ISSUES / LIMITATIONS

### 11.1. Current Issues
1. ⚠️ **API Keys Empty:** Cần configure real API keys trong appsettings.json
2. ⚠️ **Session Expiry Warning:** Chưa có UI countdown timer
3. ⚠️ **Email Notifications:** Cần configure SMTP settings
4. ⚠️ **Bank Account Info:** Account numbers trong appsettings.json đang empty

### 11.2. Future Enhancements
- [ ] Add session countdown timer with visual indicator
- [ ] Implement auto-extend session on user activity
- [ ] Add payment retry mechanism for failed transactions
- [ ] Support installment payments
- [ ] Add order tracking page
- [ ] Implement refund flow UI
- [ ] Support multiple currencies
- [ ] Add payment method fees display

---

## 12. TEST REPORT TEMPLATE

### Test Execution Report
**Tester:** _______________  
**Date:** _______________  
**Environment:** Development / Staging / Production  
**Browser:** Chrome / Firefox / Safari / Edge  

### Test Results Summary
| Test Case | Result | Notes |
|-----------|--------|-------|
| COD Flow | ⬜ Pass ⬜ Fail | |
| Bank Transfer Flow | ⬜ Pass ⬜ Fail | |
| VNPay Flow | ⬜ Pass ⬜ Fail | |
| MoMo Flow | ⬜ Pass ⬜ Fail | |
| Stripe Flow | ⬜ Pass ⬜ Fail | |
| Error Scenarios | ⬜ Pass ⬜ Fail | |
| Session Expiry | ⬜ Pass ⬜ Fail | |

### Issues Found
| ID | Severity | Description | Steps to Reproduce | Status |
|----|----------|-------------|-------------------|--------|
| | | | | |

### Screenshots
- Attach screenshots for any issues found
- Include browser console errors if applicable

---

**End of Manual Testing Guide**
