# Báo Cáo Cấu Trúc Cơ Sở Dữ Liệu John Henry Fashion Web

## Tổng Quan
Cơ sở dữ liệu John Henry Fashion Web được thiết kế để hỗ trợ một hệ thống thương mại điện tử hoàn chỉnh với các tính năng quản lý sản phẩm, đơn hàng, người dùng, thanh toán và phân tích.

## Phân Loại Các Bảng Theo Chức Năng

### 1. 👤 Hệ Thống Người Dùng & Xác Thực (User & Authentication)

#### Bảng Chính:
- **AspNetUsers** → **Người Dùng ASP.NET Identity**
- **users** → **Người Dùng (Custom)**
- **AspNetRoles** → **Vai Trò Hệ Thống**
- **AspNetUserRoles** → **Phân Quyền Người Dùng**
- **AspNetUserClaims** → **Quyền Người Dùng**
- **AspNetRoleClaims** → **Quyền Vai Trò**
- **AspNetUserLogins** → **Đăng Nhập Ngoài**
- **AspNetUserTokens** → **Token Người Dùng**

#### Bảng Liên Quan:
- **Addresses** → **Địa Chỉ Giao Hàng**
- **addresses** → **Địa Chỉ (Custom)**
- **PasswordHistories** → **Lịch Sử Mật Khẩu**
- **TwoFactorTokens** → **Token Xác Thực 2 Bước**

### 2. 🔐 Bảo Mật & Phiên Làm Việc (Security & Sessions)

- **ActiveSessions** → **Phiên Đang Hoạt Động**
- **UserSessions** → **Phiên Người Dùng**
- **SecurityLogs** → **Nhật Ký Bảo Mật**
- **audit_logs** → **Nhật Ký Kiểm Toán**

### 3. 🛍️ Quản Lý Sản Phẩm (Product Management)

#### Bảng Sản Phẩm:
- **Products** → **Sản Phẩm**
- **products** → **Sản Phẩm (Custom)**
- **Categories** → **Danh Mục Sản Phẩm**
- **categories** → **Danh Mục (Custom)**
- **Brands** → **Thương Hiệu**
- **brands** → **Thương Hiệu (Custom)**

#### Bảng Thuộc Tính:
- **product_variants** → **Biến Thể Sản Phẩm**
- **product_attributes** → **Thuộc Tính Sản Phẩm**
- **product_attribute_values** → **Giá Trị Thuộc Tính**
- **product_variant_attributes** → **Thuộc Tính Biến Thể**

#### Bảng Hình Ảnh & Đánh Giá:
- **ProductImages** → **Hình Ảnh Sản Phẩm**
- **product_images** → **Hình Ảnh (Custom)**
- **ProductReviews** → **Đánh Giá Sản Phẩm**
- **product_reviews** → **Đánh Giá (Custom)**
- **review_helpful_votes** → **Bình Chọn Đánh Giá Hữu Ích**

### 4. 🛒 Giỏ Hàng & Mua Sắm (Shopping Cart)

- **ShoppingCartItems** → **Sản Phẩm Trong Giỏ**
- **shopping_carts** → **Giỏ Hàng (Custom)**
- **Wishlists** → **Danh Sách Yêu Thích**
- **wishlists** → **Yêu Thích (Custom)**

### 5. 📦 Quản Lý Đơn Hàng (Order Management)

#### Bảng Đơn Hàng:
- **Orders** → **Đơn Hàng**
- **orders** → **Đơn Hàng (Custom)**
- **OrderItems** → **Chi Tiết Đơn Hàng**
- **order_items** → **Chi Tiết (Custom)**
- **OrderStatusHistories** → **Lịch Sử Trạng Thái**
- **order_status_history** → **Lịch Sử (Custom)**

#### Bảng Thanh Toán:
- **CheckoutSessions** → **Phiên Thanh Toán**
- **CheckoutSessionItems** → **Sản Phẩm Thanh Toán**

### 6. 💳 Hệ Thống Thanh Toán (Payment System)

- **Payments** → **Thanh Toán**
- **PaymentMethods** → **Phương Thức Thanh Toán**
- **PaymentAttempts** → **Lần Thử Thanh Toán**
- **RefundRequests** → **Yêu Cầu Hoàn Tiền**

### 7. 🚚 Vận Chuyển & Kho Hàng (Shipping & Inventory)

- **ShippingMethods** → **Phương Thức Giao Hàng**
- **InventoryItems** → **Kho Hàng**
- **StockMovements** → **Biến Động Kho**

### 8. 🎫 Khuyến Mãi & Giảm Giá (Promotions & Coupons)

- **Coupons** → **Phiếu Giảm Giá**
- **coupons** → **Giảm Giá (Custom)**
- **coupon_usages** → **Lịch Sử Sử Dụng Coupon**
- **Promotions** → **Chương Trình Khuyến Mãi**

### 9. 📝 Blog & Nội Dung (Blog & Content)

- **BlogPosts** → **Bài Viết Blog**
- **blog_posts** → **Bài Viết (Custom)**
- **BlogCategories** → **Danh Mục Blog**
- **blog_categories** → **Danh Mục (Custom)**

### 10. 📞 Liên Hệ & Thông Báo (Contact & Notifications)

- **ContactMessages** → **Tin Nhắn Liên Hệ**
- **Notifications** → **Thông Báo**
- **email_templates** → **Mẫu Email**

### 11. 📊 Phân Tích & Báo Cáo (Analytics & Reports)

- **AnalyticsData** → **Dữ Liệu Phân Tích**
- **PageViews** → **Lượt Xem Trang**
- **ConversionEvents** → **Sự Kiện Chuyển Đổi**
- **SalesReports** → **Báo Cáo Bán Hàng**
- **ReportTemplates** → **Mẫu Báo Cáo**

### 12. ⚙️ Cấu Hình Hệ Thống (System Configuration)

- **settings** → **Cài Đặt Hệ Thống**
- **__EFMigrationsHistory** → **Lịch Sử Migration Entity Framework**

## Mối Liên Hệ Giữa Các Bảng

### 🔗 Liên Kết Chính

#### 1. Người Dùng (User Relationships)
```
AspNetUsers ──┬── Orders (nhiều đơn hàng)
              ├── ShoppingCartItems (giỏ hàng)
              ├── Wishlists (yêu thích)
              ├── ProductReviews (đánh giá)
              ├── Addresses (địa chỉ)
              └── UserSessions (phiên làm việc)
```

#### 2. Sản Phẩm (Product Relationships)
```
Products ──┬── ProductImages (hình ảnh)
           ├── ProductReviews (đánh giá)
           ├── product_variants (biến thể)
           ├── OrderItems (trong đơn hàng)
           ├── ShoppingCartItems (trong giỏ)
           └── InventoryItems (kho hàng)

Categories ── Products (phân loại)
Brands ─── Products (thương hiệu)
```

#### 3. Đơn Hàng (Order Relationships)
```
Orders ──┬── OrderItems (chi tiết sản phẩm)
         ├── OrderStatusHistories (lịch sử trạng thái)
         ├── Payments (thanh toán)
         └── ShippingMethods (vận chuyển)

OrderItems ── Products (sản phẩm được đặt)
```

#### 4. Thanh Toán (Payment Relationships)
```
Orders ── Payments ──┬── PaymentMethods (phương thức)
                     ├── PaymentAttempts (lần thử)
                     └── RefundRequests (hoàn tiền)
```

## Cách Hoạt Động Của Hệ Thống

### 🔄 Quy Trình Mua Hàng

1. **Đăng Ký/Đăng Nhập**
   - User tạo tài khoản trong `AspNetUsers`
   - Thông tin bổ sung lưu trong `users`
   - Địa chỉ lưu trong `Addresses`

2. **Duyệt Sản Phẩm**
   - Sản phẩm hiển thị từ `Products`
   - Phân loại theo `Categories` và `Brands`
   - Hình ảnh từ `ProductImages`
   - Đánh giá từ `ProductReviews`

3. **Thêm Vào Giỏ Hàng**
   - Lưu trong `ShoppingCartItems`
   - Hoặc `Wishlists` (yêu thích)

4. **Thanh Toán**
   - Tạo `CheckoutSessions`
   - Chi tiết trong `CheckoutSessionItems`
   - Tạo `Orders` và `OrderItems`

5. **Xử Lý Đơn Hàng**
   - Cập nhật `OrderStatusHistories`
   - Xử lý `Payments`
   - Cập nhật `InventoryItems`
   - Ghi `StockMovements`

### 🔐 Bảo Mật & Giám Sát

1. **Xác Thực**
   - ASP.NET Identity system
   - Two-factor authentication (`TwoFactorTokens`)
   - Password history (`PasswordHistories`)

2. **Phiên Làm Việc**
   - Track trong `ActiveSessions`
   - Log trong `UserSessions`

3. **Audit & Security**
   - Mọi hoạt động ghi vào `audit_logs`
   - Security events trong `SecurityLogs`

### 📊 Phân Tích & Báo Cáo

1. **Thu Thập Dữ Liệu**
   - Page views → `PageViews`
   - User actions → `ConversionEvents`
   - Sales data → `AnalyticsData`

2. **Báo Cáo**
   - Template trong `ReportTemplates`
   - Kết quả trong `SalesReports`

## Đặc Điểm Thiết Kế

### ✅ Ưu Điểm
- **Dual Structure**: Có cả bảng ASP.NET Identity và custom tables
- **Comprehensive**: Bao phủ đầy đủ chức năng e-commerce
- **Scalable**: Thiết kế có thể mở rộng
- **Audit Trail**: Đầy đủ log và tracking
- **Security**: Nhiều lớp bảo mật

### ⚠️ Lưu Ý
- **Duplicate Tables**: Một số bảng có 2 phiên bản (Pascal và snake_case)
- **Complex Relations**: Nhiều mối liên hệ phức tạp cần quản lý
- **Data Consistency**: Cần đảm bảo đồng bộ giữa các bảng

## Kết Luận

Cơ sở dữ liệu John Henry Fashion Web được thiết kế hoàn chỉnh cho một hệ thống thương mại điện tử quy mô lớn với đầy đủ tính năng từ quản lý sản phẩm, đơn hàng, thanh toán đến phân tích và bảo mật.