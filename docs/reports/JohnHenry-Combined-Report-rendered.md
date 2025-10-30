# John Henry Fashion Web – Báo cáo Tổng hợp Kiến trúc, Tính năng & Pipelines

Phiên bản: 2025-10-24
Tác giả: Nhóm Phát triển John Henry Fashion Web

---

## Trang bìa (Executive Summary)

John Henry Fashion Web là nền tảng thương mại điện tử đa vai trò (Admin, Seller, Customer) xây dựng trên ASP.NET Core 9 (MVC), sử dụng Entity Framework Core với PostgreSQL, Authentication/Authorization bằng ASP.NET Identity (Cookie/JWT/Google OAuth), Redis cho cache phân tán, Serilog/AppInsights cho logging/telemetry, tối ưu hiệu năng bằng Response Caching/Compression và cache control static files dài hạn. Hệ thống hỗ trợ luồng mua sắm đầy đủ: duyệt sản phẩm → giỏ hàng → tạo phiên Checkout → tích hợp thanh toán (VNPay/MoMo/Stripe/COD/Bank) → xử lý return/notify → hoàn tất đơn (email/notification, trừ tồn kho, lịch sử trạng thái). Quản trị cung cấp Dashboard, Users, Inventory, Categories, Brands, Reviews, Coupons, Reports & Analytics. Hệ thống có các module bổ trợ như Blog, Marketing, Hỗ trợ (Tickets), Cấu hình hệ thống/thuế/ship/email/gateway, Quyết toán Seller.

Các mục tiêu chính:
- Đáp ứng vận hành TMĐT thực tế tại VN (VAT 10%, VNPay/MoMo, COD, ship theo phương thức)
- Bảo mật ở mức sản xuất (Identity policies, 2FA, email xác nhận, security headers, HTTPS)
- Tính mở rộng: phân lớp Services, cấu hình gateway/ship/email qua env và DB, seed dữ liệu
- Dễ triển khai: Dockerfile build multi-stage, docker-compose cho Postgres/pgAdmin

---

## Mục lục

1. Tổng quan & Công nghệ
2. Kiến trúc & Middleware Pipeline
3. Routing & Controllers Map
4. Mô hình dữ liệu & Quan hệ (ER khái quát)
5. Xác thực/Phân quyền & Các luồng Account
6. Tính năng cửa hàng: Sản phẩm, Giỏ hàng, Checkout, Thanh toán
7. Quản trị (Admin) & Người bán (Seller)
8. Hỗ trợ (Support), Marketing & Cấu hình hệ thống
9. Hiệu năng, Bảo mật & Giám sát
10. Triển khai (Docker), Môi trường & Cấu hình
11. API & Swagger
12. Pipelines chi tiết (User, Seller, Admin, System, Payment)
13. Chiến lược kiểm thử (Testing) & Đảm bảo chất lượng
14. Rủi ro & Ứng phó; Lộ trình (Roadmap)
15. Phụ lục: Danh sách Packages, Endpoint Cheatsheet

Gợi ý khi xuất sang Word: dùng pandoc với --toc để sinh mục lục tự động.

---

## 1. Tổng quan & Công nghệ

- Nền tảng: ASP.NET Core 9 (TargetFramework net9.0), MVC + Controllers + Views
- ORM & DB: EF Core 9 + Npgsql (PostgreSQL 15+), script seed shipping, EnsureCreated + Seed roles/users/blog
- AuthN/AuthZ: ASP.NET Identity, Cookie (Application/External), JWT Bearer (API), Google OAuth
- Caching & Session: MemoryCache + Redis (StackExchangeRedis), Session trước Auth
- Observability: Serilog (console + rolling file logs/john-henry-..txt), Application Insights
- Hiệu năng: ResponseCaching (filter global, 300s), ResponseCompression (Gzip/Brotli), StaticFiles cache 1 năm
- Email: SMTP (EmailSettings qua env), template trong EmailTemplates
- Thanh toán: VNPay/MoMo/Stripe/COD/Bank (IsEnabled/IsSandbox), PaymentService trung gian
- Tài liệu: Swagger (Dev), Reports Excel/PDF (EPPlus), Markdown (Markdig) khi cần
- Docker: build/publish multi-stage, runtime aspnet 9.0; docker-compose: Postgres + pgAdmin

---

## 2. Kiến trúc & Middleware Pipeline

Lớp ứng dụng:
- Controllers: tiếp nhận request, ánh xạ ViewModel, gọi Services/DbContext
- Services: nghiệp vụ (Cache, Email, Payment, Seo, Analytics, Reporting, Security, UserManagement...)
- Data (DbContext, Entities): ánh xạ domain; Fluent API cấu hình khóa/chỉ mục/quan hệ/decimal; seed dữ liệu
- Middleware: PerformanceMiddleware + ResponseCaching/Compression + Security headers (prod)

Trình tự Middleware chính (Program.cs):
1) Dev: Swagger, DeveloperExceptionPage | Prod: ExceptionHandler + HSTS + Security Headers
2) UseResponseCompression → PerformanceMiddleware → ResponseCaching → HTTPS Redirection
3) StaticFiles (cache dài hạn) → UseRouting → UseSession → UseAuthentication → UseAuthorization
4) Map routes: Blog, Default, API; MapStaticAssets

![diagram](mermaid-images/mermaid-ca939b4fd9fe799b42f71daddc159f3a8a9e1ff5.png){width=95%}

---

## 3. Routing & Controllers Map

Định tuyến:
- Blog: `blog/category/{slug}` → `BlogController.Category`; `blog/{slug}` → `BlogController.Details`
- Default: `{controller=Home}/{action=Index}/{id?}`
- API: `api/{controller}/{action=Index}/{id?}`

Controllers chính (tiêu biểu):
- Home, Products (ProductDetail, AddToCart, BuyNow), Cart (Index/Update/Remove/SaveSelected), Checkout (Index/CreateSession/Payment/Process/Success/Failed/Return/Notify)
- Payment (demo VNPay/MoMo/COD/Bank), Account (Login/Register/2FA/OAuth/Profile/Addresses/Security/Forgot/Reset)
- Admin (+ *.Blog/Orders/Settings): Dashboard, Users, Inventory, Categories, Brands, Reviews, Coupons, Reports, Security, Backups
- Seller & SellerProducts: dashboard và quản lý theo vai trò

---

## 4. Mô hình dữ liệu & Quan hệ

Nhóm chính:
- Catalog: Product, Category(self Parent), Brand, ProductImage, InventoryItem, StockMovement
- Order: Order, OrderItem, OrderStatusHistory, ShippingMethod
- Cart/Wishlist: ShoppingCartItem, Wishlist
- Payment: Payment, PaymentAttempt, PaymentTransaction, RefundRequest, PaymentMethod(+Config)
- Checkout: CheckoutSession, CheckoutSessionItem
- Marketing/Promo: Promotion, Coupon, CouponUsage, SystemPromotion, MarketingBanner, FlashSale
- CMS/Content: BlogPost, BlogCategory, ContactMessage, FAQ
- Users & Security: ApplicationUser(Identity), Notification, SecurityLog, PasswordHistory, ActiveSession, TwoFactorToken, RolePermission
- System Config: SystemConfiguration, ShippingConfiguration, TaxConfiguration, EmailConfiguration, PlatformFeeConfiguration
- Seller Finance: SellerSettlement, WithdrawalRequest
- Analytics/Reports: UserSession, PageView, ConversionEvent, AnalyticsData, SalesReport, ReportTemplate

ER khái quát:
![diagram](mermaid-images/mermaid-bee3c432e0f6711c54f4a94c21645a7c200eab64.png){width=95%}

---

## 5. Xác thực/Phân quyền & Luồng Account

- Vai trò: Admin, Seller, Customer (seed); Policies: RequireAdmin, RequireSeller, AdminOrSeller
- Đăng ký: có tùy chọn xác thực email bằng mã 6 số (cache Redis 10 phút)
- Đăng nhập: lockout (3 lần), 2FA, email confirm bắt buộc (config), Google OAuth (ExternalScheme) auto confirm
- Cookie & Session: Cookies HttpOnly/SameSite; Session trước Auth

Các luồng chính (sequence):
![diagram](mermaid-images/mermaid-230d112eb8a44b7f82abdc9bf8cb861b754eecb7.png){width=95%}

---

## 6. Tính năng cửa hàng: Sản phẩm → Giỏ → Checkout → Thanh toán

Sản phẩm: chi tiết, ảnh, SKU, Price/SalePrice, tồn kho, liên quan theo danh mục; review (phê duyệt admin)

Giỏ hàng (Authorize): thêm/cập nhật/xóa; kiểm tra tồn kho; lưu danh sách chọn; sidebar partial

Checkout: tạo CheckoutSession(+Items), tính phí ship (ShippingMethod, min order free), VAT 10%, discount (Promotion/Coupon: %/định lượng, min order, max discount, usage limit), Total; chọn payment/shipping; tạo Order trước khi thanh toán; cập nhật inventory sau thanh toán.

Thanh toán: PaymentService điều phối VNPay/MoMo/Stripe/COD/Bank; hỗ trợ Redirect URL hoặc xử lý trực tiếp; Return/Notify cập nhật trạng thái.

Sequence rút gọn:
![diagram](mermaid-images/mermaid-849cb411e0ffc5b72a56a5d7f5a10dcc5d4ac78d.png){width=95%}

---

## 7. Quản trị (Admin) & Người bán (Seller)

Admin: Dashboard KPI/biểu đồ, Orders gần đây, Users mới, cảnh báo pending; Users (lọc/phân trang/roles/status/reset), Inventory (low/out/in-stock), Categories/Brands/Reviews/Coupons, Reports Excel/PDF (lịch), Analytics realtime (khung), System (logs, security, backups, configs).

Seller: Dashboard doanh thu/đơn theo shop; quản lý sản phẩm/ảnh/giá/tồn; ProductApproval (nếu bật); xem đơn theo shop; SellerSettlement & WithdrawalRequest.

---

## 8. Hỗ trợ, Marketing & Cấu hình hệ thống

- Support: SupportTicket, TicketReply, Dispute (số, trạng thái, liên kết order/product, phân công admin)
- Marketing: SystemPromotion, MarketingBanner, FlashSale, EmailCampaign, PushNotificationCampaign
- System Config: SystemConfiguration, ShippingConfiguration, TaxConfiguration, EmailConfiguration, PaymentGatewayConfiguration, PlatformFeeConfiguration

---

## 9. Hiệu năng, Bảo mật & Giám sát

- Hiệu năng: ResponseCaching, Compression, Static cache 1 năm, Redis cache; PerformanceMiddleware đo lường
- Bảo mật: password policy mạnh, lockout, RequireConfirmedEmail/2FA, HTTPS, HSTS, security headers, SameSite/HttpOnly
- Giám sát: Serilog file+console, ApplicationInsights (telemetry), SecurityLog & ActiveSession

Đề xuất nâng cao: rate limiting, CSP header, bảo vệ webhook (chữ ký), idempotency, hàng đợi nền (email/report), cache catalog (ETag), precompute báo cáo.

---

## 10. Triển khai (Docker), Môi trường & Cấu hình

- Dockerfile: restore → build → publish → runtime aspnet 9.0
- docker-compose: Postgres + PGAdmin (port 5432/8080), volumes dữ liệu
- .env: DB_*, REDIS_CONNECTION, JWT_*, GOOGLE_*, EMAIL_*, BASE_URL, SITE_NAME, PAYMENT_* (VNPay/MoMo/Stripe), flags IsEnabled/IsSandbox

---

## 11. API & Swagger

- Bật AddEndpointsApiExplorer, AddSwaggerGen; dùng Swagger/UI trong Development
- API convention route: `api/{controller}/{action}/{id?}`

---

## 12. Pipelines chi tiết

### 12.1 System Pipeline (khởi động)
1) Load .env → override appsettings
2) Đăng ký services (DbContext, Identity, Auth, Cache, Session, Services, HttpClient, Swagger, AppInsights, Compression/Caching)
3) Build app → Seed (roles, admin/seller users, blog posts, shipping methods)
4) Middleware chain theo môi trường
5) Map routes

### 12.2 User Pipeline
- Register → (email code optional) → Confirm → Role Customer
- Login → lockout/2FA → redirect role-based
- Profile/Security/Addresses/Orders
- Shopping: ProductDetail → Cart → Checkout (Session) → Payment → Success

### 12.3 Seller Pipeline
- Sign-in (Seller) → Dashboard
- Product CRUD → (Approval flow nếu bật)
- View Orders theo shop → cập nhật tồn/ trạng thái
- Finance: PaymentTransaction → SellerSettlement → WithdrawalRequest

### 12.4 Admin Pipeline
- Dashboard → Users/Inventory/Categories/Brands/Reviews/Coupons → Reports/Analytics → System(Security, Backups)

### 12.5 Payment Pipeline (trạng thái)

![diagram](mermaid-images/mermaid-633a6781ee2f44701e9eb66a20030a3a4d766678.png){width=95%}

Lưu ý bảo mật: verify signature cho Return/Notify, idempotency xử lý webhook, log chi tiết, đối chiếu số tiền/đơn.

---

## 13. Kiểm thử & Đảm bảo chất lượng

- Unit tests: Services (Payment calculation, Promotions, Shipping fee), Controllers (Cart/Checkout minimal)
- Integration tests: Checkout → Order → Payment (mock gateway), Auth (login/2FA), Repositories/DbContext (in-memory/pg test)
- UI/End-to-end: kịch bản UAT cho giỏ→checkout→payment; đăng ký→email code→login→2FA
- Hiệu năng: load test endpoints sản phẩm/checkout; cache-hit ratio; response time mục tiêu < 200ms cho trang chính
- Bảo mật: kiểm thử lockout, CSRF (form tokens), XSS (encode output), SSRF/SQLi (EF safe), headers (CSP nếu áp dụng)

---

## 14. Rủi ro & Ứng phó; Lộ trình

Rủi ro:
- Tích hợp gateway thật (chữ ký/webhook) → cần môi trường sandbox và test case phong phú
- Dữ liệu tồn kho không đồng bộ khi thanh toán chậm → dùng hàng đợi/lock optimistic
- Tải cao trang danh mục/sản phẩm → áp dụng cache + pagination + CQRS nếu cần

Lộ trình đề xuất:
1) Hoàn thiện PaymentService thực (VNPay/MoMo/Stripe) + bảo vệ webhook
2) Bổ sung tests (unit/integration/e2e) ≥ 60% vùng quan trọng
3) CI/CD (build, test, docker build, scan) + giám sát ApplicationInsights dashboard
4) Tối ưu SEO/Cache catalog + CDN static
5) Mở rộng Seller UI/flow phê duyệt + settlement UI

---

## 15. Phụ lục

### 15.1 Packages chính
- Microsoft.EntityFrameworkCore.* 9.0.1; Npgsql.EntityFrameworkCore.PostgreSQL 9.0.1
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 9.0.0; Authentication.JwtBearer 9.0.0; Authentication.Google 9.0.9
- Microsoft.Extensions.Caching.StackExchangeRedis 9.0.0; Serilog.*; Swashbuckle.AspNetCore 6.8.1; EPPlus; Markdig; QRCoder; DotNetEnv

### 15.2 Endpoint Cheatsheet
- User: /Account/Login, /Account/Register, /Account/Addresses, /Account/Security
- Products: /Products/ProductDetail/{id}
- Cart: /Cart, /Cart/UpdateQuantity, /Cart/RemoveItem, /Cart/SaveSelectedItems
- Checkout: /Checkout (GET), /Checkout/CreateSession (POST), /Checkout/Payment, /Checkout/ProcessPayment, /Checkout/Success
- Payment demo: /Payment/Checkout, /Payment/VNPay/Return, /Payment/MoMo/Return
- Blog: /blog/{slug}, /blog/category/{slug}
- Admin: /admin/dashboard, /admin/users, /admin/inventory, /admin/reports, /admin/categories, /admin/brands, /admin/reviews, /admin/coupons

---

Ghi chú xuất bản: Tài liệu này kết hợp đầy đủ nội dung Kiến trúc & Tính năng (Report A) và Pipelines (Report B), kèm bổ sung chi tiết kỹ thuật, kiểm thử, bảo mật, DevOps và roadmap. Khi xuất sang Word, dùng tùy chọn --toc và --number-sections để mục lục và đánh số tự động.
