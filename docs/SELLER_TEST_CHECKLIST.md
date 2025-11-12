# ✅ CHECKLIST KIỂM TRA CHỨC NĂNG SELLER

## 🎯 Mục đích
Checklist này giúp kiểm tra nhanh các chức năng seller sau khi hoàn thiện code.

---

## 📋 CHUẨN BỊ

### 1. Database
- [ ] Migration đã được apply: `20251110134612_AddSellerIdToProductsAndCoupons`
- [ ] Đã chạy script gán seller: `assign_seller_to_products.sql`
- [ ] Có ít nhất 2 sellers trong hệ thống
- [ ] Mỗi seller có ít nhất 3-5 products

### 2. Test Accounts
- [ ] Seller 1: Email/Password: ________________
- [ ] Seller 2: Email/Password: ________________
- [ ] Admin: Email/Password: ________________

---

## 🔐 AUTHENTICATION & AUTHORIZATION

### Login
- [ ] Login với tài khoản Seller thành công
- [ ] Redirect đến `/seller/dashboard` sau login
- [ ] Không thể access `/admin` routes
- [ ] Logout hoạt động đúng

---

## 📊 DASHBOARD

### Thống kê tổng quan
- [ ] **My Products Count**: Chỉ đếm products của seller hiện tại
- [ ] **My Orders Count**: Chỉ đếm orders có products của seller
- [ ] **My Revenue**: Chỉ tính từ products của seller
- [ ] **Monthly Revenue**: Đúng với tháng hiện tại

### Recent Orders
- [ ] Hiển thị 5 orders gần nhất có products của seller
- [ ] Không hiển thị orders không liên quan
- [ ] Thông tin khách hàng hiển thị đúng
- [ ] Link đến order detail hoạt động

### Top Products
- [ ] Hiển thị 5 products bán chạy nhất của seller
- [ ] Số lượng bán và revenue chính xác
- [ ] Hình ảnh sản phẩm hiển thị đúng

### Switch Seller Test
- [ ] Logout và login với Seller 2
- [ ] Dashboard hiển thị dữ liệu KHÁC HOÀN TOÀN
- [ ] Không thấy products/orders của Seller 1

---

## 📦 QUẢN LÝ SẢN PHẨM

### Danh sách sản phẩm (/seller/products)
- [ ] Chỉ hiển thị products của seller hiện tại
- [ ] Phân trang hoạt động đúng
- [ ] Tìm kiếm theo tên/SKU hoạt động
- [ ] Lọc theo category hoạt động
- [ ] Lọc theo status hoạt động

### Tạo sản phẩm mới (/seller/products/create)
- [ ] Form hiển thị đầy đủ fields
- [ ] Dropdown Categories và Brands load đúng
- [ ] Upload ảnh hoạt động
- [ ] Validation hoạt động (required fields)
- [ ] Sau khi tạo, product có `SellerId` = seller hiện tại
- [ ] Redirect về danh sách sau khi tạo thành công

### Chỉnh sửa sản phẩm (/seller/products/{id})
- [ ] Chỉ edit được products của mình
- [ ] Không thể edit products của seller khác (hiển thị error)
- [ ] Form load đúng dữ liệu hiện tại
- [ ] Upload ảnh mới hoạt động
- [ ] Giữ ảnh cũ nếu không upload mới
- [ ] Validation hoạt động
- [ ] Cập nhật thành công

### Xóa sản phẩm
- [ ] Chỉ xóa được products của mình
- [ ] Không thể xóa products của seller khác (hiển thị error)
- [ ] Confirmation trước khi xóa
- [ ] Ảnh được xóa khỏi server
- [ ] Product bị xóa khỏi database

### Switch Seller Test
- [ ] Seller 2 không thấy products của Seller 1
- [ ] Seller 2 không thể edit products của Seller 1
- [ ] URL direct access bị block với error message

---

## 📋 QUẢN LÝ ĐỜN HÀNG

### Danh sách đơn hàng (/seller/orders)
- [ ] ⚠️ **TODO**: Cần implement filter theo seller
- [ ] Chỉ hiển thị orders có products của seller
- [ ] Phân trang hoạt động
- [ ] Tìm kiếm hoạt động
- [ ] Lọc theo status hoạt động
- [ ] Lọc theo date range hoạt động

### Chi tiết đơn hàng (/seller/orders/{id})
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Chỉ hiển thị nếu order có products của seller
- [ ] Hiển thị đầy đủ thông tin
- [ ] Chỉ hiển thị order items thuộc về seller
- [ ] Không hiển thị items của sellers khác (nếu có)

### Cập nhật trạng thái
- [ ] ⚠️ **TODO**: Cần implement với filter
- [ ] Chỉ cập nhật được orders của mình
- [ ] Dropdown status hoạt động
- [ ] Notes được lưu đúng

---

## 📊 TỒN KHO (INVENTORY)

### Danh sách tồn kho (/seller/inventory)
- [ ] ✅ Chỉ hiển thị products của seller
- [ ] Tìm kiếm hoạt động
- [ ] Filter "Low Stock" hoạt động
- [ ] Hiển thị đúng số lượng tồn kho
- [ ] Sắp xếp theo quantity tăng dần

### Cập nhật tồn kho
- [ ] ✅ Chỉ cập nhật được products của mình
- [ ] ✅ Không cập nhật được products của seller khác
- [ ] Modal/Form hiển thị đúng
- [ ] Validation số lượng
- [ ] Cập nhật real-time
- [ ] Hiển thị success message

---

## 🎫 QUẢN LÝ MÃ GIẢM GIÁ

### Danh sách coupons (/seller/coupons)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Hiển thị coupons của seller + system coupons (SellerId = NULL)
- [ ] Phân trang hoạt động
- [ ] Tìm kiếm hoạt động
- [ ] Lọc theo status hoạt động

### Tạo coupon mới
- [ ] ⚠️ **TODO**: Cần set SellerId
- [ ] Form validation hoạt động
- [ ] Các loại discount (percentage, fixed) hoạt động
- [ ] Date picker hoạt động
- [ ] Sau khi tạo, coupon có `SellerId` = seller hiện tại

### Chỉnh sửa coupon
- [ ] ⚠️ **TODO**: Cần check ownership
- [ ] Chỉ edit được coupons của mình
- [ ] Không edit được system coupons
- [ ] Không edit được coupons của seller khác

### Xóa coupon
- [ ] ⚠️ **TODO**: Cần check ownership
- [ ] Chỉ xóa được coupons của mình
- [ ] Confirmation trước khi xóa

---

## ⭐ QUẢN LÝ ĐÁNH GIÁ

### Danh sách reviews (/seller/reviews)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Chỉ hiển thị reviews của products thuộc seller
- [ ] Statistics chính xác
- [ ] Phân trang hoạt động
- [ ] Filter theo rating hoạt động
- [ ] Filter theo status hoạt động

### Phê duyệt/Từ chối review
- [ ] ⚠️ **TODO**: Cần check ownership
- [ ] Chỉ approve/reject reviews của products mình
- [ ] Button approve hoạt động
- [ ] Button reject hoạt động
- [ ] Status update real-time

---

## 💰 DOANH THU & BÁO CÁO

### Sales Report (/seller/sales)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Chỉ tính revenue từ products của seller
- [ ] Date range picker hoạt động
- [ ] Biểu đồ hiển thị đúng
- [ ] Số liệu chính xác

### Commissions (/seller/commissions)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Tính commission từ sales của seller
- [ ] Monthly breakdown chính xác
- [ ] Commission rate hiển thị đúng

### Reports (/seller/reports)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Tất cả metrics filter theo seller
- [ ] Charts render đúng
- [ ] Export functionality (nếu có)

---

## 👥 QUẢN LÝ KHÁCH HÀNG

### Customers List (/seller/customers)
- [ ] ⚠️ **TODO**: Cần implement filter
- [ ] Chỉ hiển thị customers đã mua products của seller
- [ ] Top customers chính xác
- [ ] New customers chính xác
- [ ] Statistics chính xác

---

## ⚙️ CÀI ĐẶT

### Profile (/seller/profile)
- [ ] Hiển thị thông tin seller đúng
- [ ] Update thông tin hoạt động
- [ ] Validation hoạt động

### Settings (/seller/settings)
- [ ] ⚠️ **TODO**: Cần persist vào database
- [ ] Form hiển thị đúng
- [ ] Save settings hoạt động
- [ ] Load settings từ database

---

## 🏪 QUẢN LÝ CỬA HÀNG

### Store Management (/seller/store-management)
- [ ] Hiển thị thông tin store
- [ ] Inventory hiển thị đúng
- [ ] Settings hiển thị đúng
- [ ] Statistics chính xác

### Store Settings (/seller/store/settings)
- [ ] Update store info hoạt động
- [ ] Additional settings hoạt động

---

## 🔔 THÔNG BÁO

### Notifications (/seller/notifications)
- [ ] Hiển thị notifications của seller
- [ ] Mark as read hoạt động
- [ ] Mark all as read hoạt động
- [ ] Filter theo type hoạt động
- [ ] Unread count chính xác

---

## 🧪 EDGE CASES

### Seller không có products
- [ ] Dashboard hiển thị 0s thay vì error
- [ ] Products page hiển thị empty state
- [ ] Không có crash/error

### Seller không có orders
- [ ] Dashboard hiển thị 0 orders
- [ ] Orders page hiển thị empty state
- [ ] Revenue = 0

### Products với SellerId = NULL
- [ ] Admin có thể quản lý
- [ ] Sellers không thấy trong danh sách của mình
- [ ] Không thể edit/delete

### Concurrent Access
- [ ] 2 sellers login cùng lúc
- [ ] Mỗi người chỉ thấy dữ liệu của mình
- [ ] Không có data leak giữa sellers

---

## 🚨 SECURITY TESTING

### Authorization
- [ ] Seller không access được `/admin` routes
- [ ] Seller không xem được products của seller khác
- [ ] Direct URL access bị block với error
- [ ] API endpoints có authorization

### Data Isolation
- [ ] Seller A không thấy dữ liệu của Seller B
- [ ] Dashboard stats chính xác cho từng seller
- [ ] Queries có WHERE clause filter `SellerId`

### SQL Injection
- [ ] Search inputs được sanitize
- [ ] No SQL injection vulnerabilities

---

## 📱 RESPONSIVE & UI

### Desktop (>1200px)
- [ ] Layout hiển thị đúng
- [ ] Sidebar navigation hoạt động
- [ ] Tables responsive

### Tablet (768px - 1200px)
- [ ] Layout adapt đúng
- [ ] Navigation collapsible
- [ ] Forms usable

### Mobile (<768px)
- [ ] Mobile menu hoạt động
- [ ] Tables scroll horizontal
- [ ] Forms stack vertical

---

## 📊 PERFORMANCE

### Load Times
- [ ] Dashboard load < 2s
- [ ] Products list load < 2s
- [ ] Các pages khác load < 3s

### Database Queries
- [ ] Không có N+1 query problems
- [ ] Indexes được sử dụng (check với EXPLAIN)
- [ ] Pagination giảm load

---

## ✅ SUMMARY

**Hoàn thành:** _____ / _____
**Lỗi tìm thấy:** _____
**Cần fix:** _____

### Lỗi nghiêm trọng (Critical)
1. _______________________________________
2. _______________________________________

### Lỗi quan trọng (High)
1. _______________________________________
2. _______________________________________

### Lỗi thông thường (Medium)
1. _______________________________________
2. _______________________________________

### Cải tiến (Low)
1. _______________________________________
2. _______________________________________

---

**Tester:** ___________________
**Ngày test:** _______________
**Browser:** _________________
**Database:** ________________
