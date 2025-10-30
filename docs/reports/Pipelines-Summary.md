# Pipelines Summary – John Henry Fashion Web

Phiên bản: 2025-10-24

Tài liệu tóm tắt pipelines (quy trình) chính dành cho: User, Seller, Admin, Hệ thống và Thanh toán. Nội dung súc tích để tham khảo nhanh.

## 1) User Pipeline (Khách hàng)

- Đăng ký: nhập thông tin → (tùy chọn) xác thực email 6 số (Redis, 10 phút) → hoàn tất
- Đăng nhập: email + mật khẩu → lockout sau 3 lần sai → (nếu bật) 2FA → định tuyến theo role
- Google OAuth: lấy email/ tên → tạo user nếu chưa có → auto email confirmed → đăng nhập
- Tài khoản: hồ sơ, đổi/đặt mật khẩu, địa chỉ (mặc định), bảo mật (phiên/nhật ký)

Mua sắm:
1. Product Detail → AddToCart/BuyNow
2. Cart (cập nhật/xóa/tổng tiền)
3. Checkout: tạo CheckoutSession(+Items), tính ship/thuế/discount/total
4. Chọn thanh toán → tạo Order(pending) → PaymentService → Gateway/Direct
5. Return/Notify → CompleteOrder (email/notify, trừ tồn, status history)

## 2) Seller Pipeline (Người bán)

- Dashboard Seller: thống kê đơn/doanh thu shop (role Seller)
- Quản lý sản phẩm: thêm/sửa/xóa, SKU, tồn kho, ảnh, giá
- Phê duyệt sản phẩm (nếu bật): tạo ProductApproval → Admin duyệt/từ chối → cập nhật
- Đơn hàng theo shop: cập nhật trạng thái/ghi nhận tồn kho
- Tài chính: PaymentTransaction → SellerSettlement → WithdrawalRequest

## 3) Admin Pipeline

- Dashboard: KPI (doanh thu, đơn, khách), order gần đây, user mới, cảnh báo
- Users: filter/paging/roles/enable/disable/reset password
- Inventory: low/out/in-stock; Category/Brand/Review/Coupon
- Reports & Analytics: sinh Excel/PDF, chart, realtime (khung), export
- System: logs, security, cấu hình (ship/email/payment/tax/fees) qua model

## 4) System Pipeline (Khởi động & Bảo mật)

Khởi động:
1. Load .env → Configure services (EF/Identity/Auth/Cache/Session/Serilog/AI/Swagger)
2. EnsureCreated → Seed roles (Admin/Seller/Customer), admin user, blog, shipping methods
3. Middleware: Dev( Swagger/DevException ) | Prod( ExceptionHandler/HSTS + security headers )
4. ResponseCompression/ResponseCaching/StaticFiles cache/HTTPS/Session/AuthZ
5. Map routes (Blog, Default, API)

Bảo mật:
- Password policy mạnh, lockout, RequireConfirmedEmail/2FA
- Security headers (prod), HTTPS, SameSite/HttpOnly cookie
- SecurityLog/ActiveSession cho audit

## 5) Payment Pipeline (Chi tiết)

Thành phần: CheckoutController + PaymentService + Gateways (VNPay/MoMo/Stripe) + COD/Bank.

Chuẩn:
1. CreateSession → tính ship/tax/discount → Session active
2. ProcessPayment → (tạo Order pending) → gọi PaymentService
3. Nếu online → trả PaymentUrl → redirect gateway; nếu COD/Bank → trả trực tiếp
4. Return/Notify → xác minh → CompleteOrder: set paid/processing; email/notify; trừ tồn; OrderStatusHistory

Trạng thái đơn (rút gọn): pending → processing → completed | cancelled

## 6) Checklist nhanh theo vai trò

- User
  - [ ] Đăng ký/Đăng nhập/Google/2FA
  - [ ] Thêm giỏ/BuyNow/Checkout/Thanh toán
  - [ ] Xem đơn/Email/Thông báo
- Seller
  - [ ] Quản lý sản phẩm/tồn kho
  - [ ] Phê duyệt (nếu bật)
  - [ ] Xem đơn của shop
- Admin
  - [ ] Dashboard/KPI/Cảnh báo
  - [ ] Users/Inventory/Categories/Brands/Reviews/Coupons
  - [ ] Reports/Analytics/Export
  - [ ] System/Security/Configs

## 7) Endpoint tham khảo nhanh

- User: /Account/Login, /Account/Register, /Account/Addresses, /Account/Security
- Products: /Products/ProductDetail/{id}
- Cart: /Cart, /Cart/UpdateQuantity, /Cart/RemoveItem
- Checkout: /Checkout (GET), /Checkout/CreateSession (POST), /Checkout/Payment, /Checkout/ProcessPayment, /Checkout/Success
- Payment demo: /Payment/Checkout, /Payment/VNPay/Return, /Payment/MoMo/Return
- Blog: /blog/{slug}, /blog/category/{slug}
- Admin: /admin/dashboard, /admin/users, /admin/inventory, /admin/reports, /admin/categories, /admin/brands
