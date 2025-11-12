# 🧪 VNPay Testing Guide

## 📋 Tổng Quan

Guide này hướng dẫn bạn test VNPay sandbox trước khi chuyển sang production.

---

## ✅ Checklist Trước Khi Test

- [x] VNPay credentials đã được config trong `.env`
- [x] `VNPAY_SANDBOX=true` (đang ở chế độ test)
- [x] Server đang chạy (http://localhost:5101)
- [ ] Đã có sản phẩm trong giỏ hàng
- [ ] Đã điền thông tin checkout

---

## 🚀 Bước 1: Start Server

```bash
cd "/Users/nguyenhuuthang/Documents/RepoGitHub/John Henry Website"
dotnet run
```

Đợi server khởi động xong, bạn sẽ thấy:
```
Now listening on: http://localhost:5101
```

---

## 🛒 Bước 2: Tạo Đơn Hàng Test

### 2.1. Thêm Sản Phẩm Vào Giỏ

1. Mở trình duyệt: http://localhost:5101
2. Browse sản phẩm
3. Click "Thêm vào giỏ hàng" cho 1-2 sản phẩm
4. Click icon giỏ hàng → "Thanh toán"

### 2.2. Điền Thông Tin Checkout

**Form bên trái (Thông tin giao hàng):**
- ✅ Họ tên: `Nguyễn Văn Test`
- ✅ Email: `test@example.com`
- ✅ Số điện thoại: `0901234567`
- ✅ Địa chỉ: `123 Test Street`
- ✅ Tỉnh/Thành phố: Chọn `Hồ Chí Minh`
- ✅ Quận/Huyện: Chọn `Quận 1`
- ✅ Phường/Xã: Chọn `Phường Bến Nghé`
- ✅ Ghi chú: `Test order VNPay`

**Bên phải (Chọn phương thức):**
- 📦 Phương thức vận chuyển: `Giao hàng tiêu chuẩn`
- 💳 Phương thức thanh toán: `VNPay`

### 2.3. Review Đơn Hàng

Kiểm tra tóm tắt bên phải:
- ✅ Sản phẩm đúng
- ✅ Số lượng đúng
- ✅ Tổng tiền đúng
- ✅ Phí vận chuyển đúng

Click **"Đặt hàng"**

---

## 💳 Bước 3: Thanh Toán VNPay Sandbox

### 3.1. Redirect Sang VNPay

Sau khi click "Đặt hàng", bạn sẽ được redirect sang trang payment:
- URL: `http://localhost:5101/Checkout/Payment?sessionId=...`
- Thấy thông tin đơn hàng
- Thấy nút **"Thanh toán với VNPay"**

Click nút này → redirect sang VNPay sandbox.

### 3.2. Trang VNPay Sandbox

URL sẽ là: `https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?...`

**⚠️ LÀM THEO ĐÚNG THỨ TỰ:**

#### Option 1: Thanh Toán Thành Công ✅

1. Chọn ngân hàng: **NCB** (Ngân hàng TMCP Quốc Dân)
2. Click "Tiếp tục"
3. Nhập thông tin thẻ test:
   ```
   Số thẻ:        9704 0000 0000 0018
   Tên chủ thẻ:   NGUYEN VAN A
   Ngày hết hạn:  03/07
   ```
4. Click "Tiếp tục"
5. Nhập OTP: `123456`
6. Click "Tiếp tục"

**Kết quả mong đợi:**
- ✅ Thanh toán thành công
- ✅ Redirect về: `http://localhost:5101/Payment/VNPayReturn?...`
- ✅ Thấy message: "Thanh toán thành công"
- ✅ Order status: `Paid`

#### Option 2: Thanh Toán Thất Bại ❌ (để test error handling)

1. Chọn ngân hàng: **NCB**
2. Nhập thẻ test thất bại:
   ```
   Số thẻ:        9704 0000 0000 0026
   Tên chủ thẻ:   NGUYEN VAN A
   Ngày hết hạn:  03/07
   ```
3. Nhập OTP: `123456`

**Kết quả mong đợi:**
- ❌ Thanh toán thất bại
- ↩️ Redirect về với error message
- 📝 Order status: `Pending` (không đổi)

#### Option 3: Hủy Giao Dịch ⏹️

1. Click nút **"Quay lại"** trên trang VNPay
2. Hoặc click **"Hủy giao dịch"**

**Kết quả mong đợi:**
- ⏹️ Giao dịch bị hủy
- ↩️ Redirect về payment page
- 📝 Order status: `Pending`

---

## 🔍 Bước 4: Kiểm Tra Kết Quả

### 4.1. Check Trong Browser

**Sau khi thanh toán thành công:**
- ✅ URL: `http://localhost:5101/Payment/VNPayReturn?vnp_Amount=...`
- ✅ Hiển thị: "Đơn hàng #XXXXX đã được thanh toán thành công"
- ✅ Button: "Xem đơn hàng"

### 4.2. Check Database

```bash
# Connect PostgreSQL
psql -U your_username -d johnhenry

# Check order
SELECT 
    id,
    order_number,
    total_amount,
    payment_status,
    payment_method,
    created_at
FROM "Orders"
ORDER BY created_at DESC
LIMIT 5;
```

**Expected result:**
```
 id | order_number | total_amount | payment_status | payment_method |     created_at
----+--------------+--------------+----------------+----------------+--------------------
  1 | ORD20241107  |    500000.00 | Paid           | VNPay          | 2024-11-07 10:30:00
```

### 4.3. Check Logs

```bash
# Monitor logs realtime
tail -f logs/john-henry-$(date +%Y%m%d).txt | grep -i vnpay

# Or view recent VNPay logs
grep -i vnpay logs/john-henry-$(date +%Y%m%d).txt | tail -20
```

**Expected log entries:**
```
[INFO] VNPay: Creating payment URL for order ORD20241107
[INFO] VNPay: Payment URL generated successfully
[INFO] VNPay: Received callback for order ORD20241107
[INFO] VNPay: Signature validated successfully
[INFO] VNPay: Payment successful, amount: 500000
[INFO] VNPay: Order ORD20241107 updated to Paid
```

---

## 🧪 Test Cases Bổ Sung

### Test Case 1: Số Tiền Nhỏ
```
Amount: 10,000 VND
Card: 9704 0000 0000 0018
Expected: Success
```

### Test Case 2: Số Tiền Lớn
```
Amount: 50,000,000 VND
Card: 9704 0000 0000 0018
Expected: Success (sandbox không limit)
```

### Test Case 3: Multiple Attempts
```
1. Tạo order
2. Cancel payment
3. Thử lại với order ID cũ
Expected: Vẫn cho phép thanh toán
```

### Test Case 4: Timeout
```
1. Tạo order
2. Đợi trên trang VNPay > 15 phút
3. Timeout tự động
Expected: Redirect về với error message
```

---

## 🐛 Troubleshooting

### Lỗi 1: "Invalid Signature"

**Triệu chứng:**
- Redirect về với message "Chữ ký không hợp lệ"

**Nguyên nhân:**
- `VNPAY_HASH_SECRET` sai
- Query parameters bị modify

**Fix:**
1. Kiểm tra `.env`:
   ```bash
   grep VNPAY_HASH_SECRET .env
   ```
2. Đảm bảo không có spaces/newlines
3. Restart server

### Lỗi 2: "Order Not Found"

**Triệu chứng:**
- Callback về nhưng không tìm thấy order

**Nguyên nhân:**
- Session expired
- OrderId không match

**Fix:**
1. Check session timeout trong `appsettings.json`
2. Check order creation logs

### Lỗi 3: Redirect Loop

**Triệu chứng:**
- Redirect liên tục giữa site và VNPay

**Nguyên nhân:**
- Return URL không đúng
- Missing callback handler

**Fix:**
1. Check `VNPAY_RETURN_URL` trong `.env`
2. Ensure route exists: `/Payment/VNPayReturn`

### Lỗi 4: "Amount Mismatch"

**Triệu chứng:**
- VNPay báo số tiền không khớp

**Nguyên nhân:**
- Amount calculation sai
- Currency conversion issue

**Fix:**
1. Check amount × 100 (VNPay dùng đơn vị VND nhỏ nhất)
2. Verify:
   ```csharp
   int vnpayAmount = (int)(totalAmount * 100);
   ```

---

## 📊 Expected Flow Diagram

```
[User]
  │
  ↓ 1. Add to cart
[Product Page]
  │
  ↓ 2. Checkout
[Checkout Form]
  │
  ↓ 3. Fill info + Select VNPay
[Payment Page]
  │
  ↓ 4. Click "Thanh toán"
[VNPay Sandbox]
  │
  ↓ 5a. Success
  ├─→ [VNPayReturn]
  │     ↓
  │   [Success Page]
  │
  ↓ 5b. Failure
  └─→ [VNPayReturn]
        ↓
      [Error Page]
```

---

## ✅ Success Criteria

Sau khi test xong, bạn nên thấy:

- ✅ Có thể tạo order với payment method VNPay
- ✅ Redirect sang VNPay sandbox thành công
- ✅ Thanh toán với test card thành công
- ✅ Callback trả về đúng kết quả
- ✅ Order status update sang `Paid`
- ✅ Database có record đầy đủ
- ✅ Logs không có error
- ✅ User experience mượt mà

---

## 🚀 Chuyển Sang Production

Khi test thành công, làm theo `PAYMENT_GATEWAY_SETUP.md`:

1. Đăng ký VNPay merchant account (7-14 ngày)
2. Nhận production credentials
3. Update `.env`:
   ```bash
   VNPAY_SANDBOX=false
   VNPAY_TMN_CODE=<your_prod_code>
   VNPAY_HASH_SECRET=<your_prod_secret>
   VNPAY_PAYMENT_URL=https://pay.vnpay.vn/vpcpay.html
   ```
4. Test lại với số tiền nhỏ (10,000 VND)
5. Monitor logs carefully
6. Go live! 🎉

---

## 📞 Support

**VNPay Sandbox Issues:**
- Email: support@vnpay.vn
- Hotline: 1900 555 577
- Docs: https://sandbox.vnpayment.vn/apis

**Code Issues:**
- Check logs first
- Review `Services/PaymentService.cs`
- Debug `Controllers/PaymentController.cs`

---

**Last Updated:** 2024-11-07  
**Status:** ✅ Sandbox Ready  
**Next Step:** Register production account
