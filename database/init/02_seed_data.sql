-- ========================================
-- SEED DATA FOR JOHN HENRY E-COMMERCE
-- ========================================

-- Insert default admin user (updated for new schema)
INSERT INTO users (email, user_name, password_hash, first_name, last_name, role, is_active) VALUES
('admin@johnhenry.com', 'admin', '$2a$11$LQJ9oFjC6BF.AxhG5LQKOeJMB2lA3h3QKOr9vXKOKV2J3lA4h5QWO', 'Admin', 'User', 'Admin', TRUE),
('seller@johnhenry.com', 'seller', '$2a$11$LQJ9oFjC6BF.AxhG5LQKOeJMB2lA3h3QKOr9vXKOKV2J3lA4h5QWO', 'Seller', 'User', 'Seller', TRUE),
('customer@johnhenry.com', 'customer', '$2a$11$LQJ9oFjC6BF.AxhG5LQKOeJMB2lA3h3QKOr9vXKOKV2J3lA4h5QWO', 'Customer', 'User', 'Customer', TRUE);

-- Insert brands
INSERT INTO brands (name, slug, description, logo_url, is_active) VALUES
('John Henry', 'john-henry', 'Thương hiệu thời trang nam cao cấp', '/images/brands/john-henry-logo.png', TRUE),
('Freelancer', 'freelancer', 'Thương hiệu thời trang nữ hiện đại', '/images/brands/freelancer-logo.png', TRUE);

-- Insert main categories (updated for new schema)
INSERT INTO categories (name, slug, description, is_active, sort_order, seo_metadata) VALUES
('Nam', 'nam', 'Thời trang nam', TRUE, 1, '{"title": "Thời trang nam - John Henry", "description": "Bộ sưu tập thời trang nam cao cấp"}'),
('Nữ', 'nu', 'Thời trang nữ', TRUE, 2, '{"title": "Thời trang nữ - Freelancer", "description": "Bộ sưu tập thời trang nữ hiện đại"}'),
('Phụ kiện', 'phu-kien', 'Phụ kiện thời trang', TRUE, 3, '{"title": "Phụ kiện thời trang", "description": "Phụ kiện thời trang cao cấp"}');

-- Insert sub-categories for Men
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Áo Nam', 'ao-nam', 'Các loại áo dành cho nam', 1, TRUE, 1),
('Quần Nam', 'quan-nam', 'Các loại quần dành cho nam', 1, TRUE, 2),
('Phụ kiện Nam', 'phu-kien-nam', 'Phụ kiện dành cho nam', 1, TRUE, 3);

-- Insert sub-categories for Women
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Áo Nữ', 'ao-nu', 'Các loại áo dành cho nữ', 2, TRUE, 1),
('Quần Nữ', 'quan-nu', 'Các loại quần dành cho nữ', 2, TRUE, 2),
('Đầm Nữ', 'dam-nu', 'Các loại đầm dành cho nữ', 2, TRUE, 3),
('Chân váy', 'chan-vay', 'Các loại chân váy', 2, TRUE, 4),
('Phụ kiện Nữ', 'phu-kien-nu', 'Phụ kiện dành cho nữ', 2, TRUE, 5);

-- Insert detailed categories for Men's clothing
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Áo Polo Nam', 'ao-polo-nam', 'Áo polo dành cho nam', 4, TRUE, 1),
('Áo Sơ Mi Nam', 'ao-so-mi-nam', 'Áo sơ mi dành cho nam', 4, TRUE, 2),
('Áo Thun Nam', 'ao-thun-nam', 'Áo thun dành cho nam', 4, TRUE, 3),
('Áo Len Nam', 'ao-len-nam', 'Áo len dành cho nam', 4, TRUE, 4),
('Áo Khoác Nam', 'ao-khoac-nam', 'Áo khoác dành cho nam', 4, TRUE, 5),
('Áo Vest - Blazer Nam', 'ao-vest-blazer-nam', 'Áo vest và blazer nam', 4, TRUE, 6);

-- Insert detailed categories for Men's pants
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Quần Jean Nam', 'quan-jean-nam', 'Quần jean dành cho nam', 5, TRUE, 1),
('Quần Tây Nam', 'quan-tay-nam', 'Quần tây dành cho nam', 5, TRUE, 2),
('Quần Short Nam', 'quan-short-nam', 'Quần short dành cho nam', 5, TRUE, 3),
('Quần Thể Thao Nam', 'quan-the-thao-nam', 'Quần thể thao nam', 5, TRUE, 4);

-- Insert detailed categories for Women's clothing
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Áo Blouse Nữ', 'ao-blouse-nu', 'Áo blouse dành cho nữ', 7, TRUE, 1),
('Áo T-Shirt Nữ', 'ao-t-shirt-nu', 'Áo t-shirt dành cho nữ', 7, TRUE, 2),
('Áo Sơ Mi Nữ', 'ao-so-mi-nu', 'Áo sơ mi dành cho nữ', 7, TRUE, 3),
('Áo Polo Nữ', 'ao-polo-nu', 'Áo polo dành cho nữ', 7, TRUE, 4),
('Áo Blazer Nữ', 'ao-blazer-nu', 'Áo blazer dành cho nữ', 7, TRUE, 5),
('Áo Len Nữ', 'ao-len-nu', 'Áo len dành cho nữ', 7, TRUE, 6);

-- Insert detailed categories for Women's pants
INSERT INTO categories (name, slug, description, parent_category_id, is_active, sort_order) VALUES
('Quần Jean Nữ', 'quan-jean-nu', 'Quần jean dành cho nữ', 8, TRUE, 1),
('Quần Tây Nữ', 'quan-tay-nu', 'Quần tây dành cho nữ', 8, TRUE, 2),
('Quần Short Nữ', 'quan-short-nu', 'Quần short dành cho nữ', 8, TRUE, 3),
('Quần Legging', 'quan-legging', 'Quần legging', 8, TRUE, 4);

-- Remove product attributes and variants section as we simplified the schema
-- Products now use simple size/color fields instead of complex variants

-- Insert sample products for Men (updated for new schema)
INSERT INTO products (sku, name, slug, description, short_description, category_id, brand_id, price, sale_price, stock_quantity, featured_image_url, gallery_images, size, color, material, is_active, is_featured, status, seo_metadata) VALUES
('JH-MH-001', 'Áo Hoodie Nam Tay Dài Premium', 'ao-hoodie-nam-tay-dai-premium', 'Áo hoodie nam tay dài chất liệu cotton cao cấp, thiết kế hiện đại, phù hợp cho mọi hoạt động hàng ngày.', 'Áo hoodie nam tay dài chất liệu premium', 12, 1, 800000, 720000, 50, '/images/Áo nam/Áo Hoodie Nam Tay Dài TS25FH22C-LS.jpg', ARRAY['/images/Áo nam/Áo Hoodie Nam Tay Dài TS25FH22C-LS.jpg'], 'S,M,L,XL', 'Đen,Xám,Navy', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Hoodie Nam Premium", "description": "Áo hoodie nam chất liệu cotton cao cấp"}'),
('JH-MJ-001', 'Áo Khoác Nam Cá Tính', 'ao-khoac-nam-ca-tinh', 'Áo khoác nam thiết kế cá tính, chất liệu cao cấp, phù hợp cho nhiều dịp khác nhau.', 'Áo khoác nam cá tính cao cấp', 12, 1, 1500000, 1350000, 30, '/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH04C-PA.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH04C-PA.jpg'], 'M,L,XL', 'Đen,Navy,Xám', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Áo Khoác Nam Cá Tính", "description": "Áo khoác nam thiết kế cá tính"}'),
('JH-MS-001', 'Áo Len Nam Tay Dài Form Vừa', 'ao-len-nam-tay-dai-form-vua', 'Áo len nam tay dài form vừa, chất liệu wool cao cấp, giữ ấm tốt.', 'Áo len nam tay dài form vừa', 11, 1, 890000, NULL, 40, '/images/Áo nam/Áo Len Nam Tay Dài Form Vừa SW25FH04M-LB.jpg', ARRAY['/images/Áo nam/Áo Len Nam Tay Dài Form Vừa SW25FH04M-LB.jpg'], 'S,M,L,XL', 'Be,Xám,Navy', '100% Wool', TRUE, FALSE, 'active', '{"title": "Áo Len Nam Form Vừa", "description": "Áo len nam chất liệu wool cao cấp"}');

-- Insert sample products for Women (updated for new schema)
INSERT INTO products (sku, name, slug, description, short_description, category_id, brand_id, price, sale_price, stock_quantity, featured_image_url, gallery_images, size, color, material, is_active, is_featured, status, seo_metadata) VALUES
('FL-WB-001', 'Áo Blouse Nữ Thời Trang', 'ao-blouse-nu-thoi-trang', 'Áo blouse nữ thiết kế thời trang, chất liệu mềm mại, phù hợp cho công sở.', 'Áo blouse nữ thời trang công sở', 18, 2, 750000, 675000, 45, '/images/Áo nữ/Áo Blouse Nữ Thời Trang FWBL25SS01C.jpg', ARRAY['/images/Áo nữ/Áo Blouse Nữ Thời Trang FWBL25SS01C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Silk blend', TRUE, TRUE, 'active', '{"title": "Áo Blouse Nữ Thời Trang", "description": "Áo blouse nữ cho công sở"}'),
('FL-WP-001', 'Áo Polo Nữ Cổ Lưới', 'ao-polo-nu-co-luoi', 'Áo polo nữ cổ lưới thiết kế độc đáo, thoáng mát và thoải mái.', 'Áo polo nữ cổ lưới thoáng mát', 21, 2, 680000, NULL, 35, '/images/Áo nữ/Áo Polo Nữ Cổ Lưới FWKS25SS11G.jpg', ARRAY['/images/Áo nữ/Áo Polo Nữ Cổ Lưới FWKS25SS11G.jpg'], 'S,M,L', 'Trắng,Xanh,Hồng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nữ Cổ Lưới", "description": "Áo polo nữ thoáng mát"}'),
('FL-WS-001', 'Áo Sơ Mi Nữ Tay Ngắn', 'ao-so-mi-nu-tay-ngan', 'Áo sơ mi nữ tay ngắn phong cách hiện đại, phù hợp cho nhiều dịp.', 'Áo sơ mi nữ tay ngắn hiện đại', 20, 2, 890000, 801000, 25, '/images/Áo nữ/Áo Sơ Mi Nữ Tay Ngắn FWSH25SS03C.jpg', ARRAY['/images/Áo nữ/Áo Sơ Mi Nữ Tay Ngắn FWSH25SS03C.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Be', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Áo Sơ Mi Nữ Tay Ngắn", "description": "Áo sơ mi nữ hiện đại"}'),
('FL-WT-001', 'Áo Thun Nữ Basic Form Rộng', 'ao-thun-nu-basic-form-rong', 'Áo thun nữ basic form rộng, thoải mái và dễ phối đồ.', 'Áo thun nữ basic thoải mái', 19, 2, 450000, NULL, 60, '/images/Áo nữ/Áo Thun Nữ Basic Form Rộng FWTS25SS04C.jpg', ARRAY['/images/Áo nữ/Áo Thun Nữ Basic Form Rộng FWTS25SS04C.jpg'], 'S,M,L,XL,XXL', 'Trắng,Đen,Xám,Hồng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Thun Nữ Basic", "description": "Áo thun nữ basic form rộng"}');

-- Remove product variants and variant attributes sections
-- (These are not needed with simplified schema)

-- Insert blog categories
INSERT INTO blog_categories (name, slug, description, is_active, sort_order) VALUES
('Tin tức', 'tin-tuc', 'Tin tức về thương hiệu và sản phẩm', TRUE, 1),
('Xu hướng thời trang', 'xu-huong-thoi-trang', 'Các xu hướng thời trang mới nhất', TRUE, 2),
('Hướng dẫn phối đồ', 'huong-dan-phoi-do', 'Hướng dẫn cách phối đồ đẹp', TRUE, 3),
('Sự kiện', 'su-kien', 'Các sự kiện của thương hiệu', TRUE, 4);

-- Insert blog posts (updated for new schema)
INSERT INTO blog_posts (title, slug, content, excerpt, featured_image_url, author_id, category_id, status, published_at, is_published, is_featured, seo_metadata) VALUES
('Grand Opening John Henry Cầu Giấy', 'grand-opening-john-henry-cau-giay', 'Chính thức có mặt tại 355 Cầu Giấy, P. Cầu Giấy, TP. Hà Nội, JOHN HENRY mang đến một không gian mua sắm hoàn toàn mới...', 'Chính thức có mặt tại 355 Cầu Giấy, P. Cầu Giấy, TP. Hà Nội, JOHN HENRY mang đến một không gian mua sắm...', '/images/Banner/banner-man-main.jpg', '1', 4, 'published', NOW() - INTERVAL '2 days', TRUE, TRUE, '{"title": "Grand Opening John Henry Cầu Giấy", "description": "Khai trương cửa hàng John Henry tại Cầu Giấy"}'),
('New Drop Jeans Collection', 'new-drop-jeans-collection', 'Muốn để vibe nữ văn thoái mái cả ngày? Quần jeans ống rộng darkwash mới của Freelancer giúp nàng g...', 'Muốn để vibe nữ văn thoái mái cả ngày? Quần jeans ống rộng darkwash mới của Freelancer...', '/images/Banner/banner-women-1.jpg', '1', 1, 'published', NOW() - INTERVAL '3 days', TRUE, FALSE, '{"title": "New Drop Jeans Collection", "description": "Bộ sưu tập jeans mới"}'),
('Cool Style Easy Wear', 'cool-style-easy-wear', 'Chỉ cần một chiếc áo polo basic, một chiếc quần short năng động, thêm đôi giày là đã có ng...', 'Chỉ cần một chiếc áo polo basic, một chiếc quần short năng động, thêm đôi giày...', '/images/Banner/banner-man-0.jpg', '1', 3, 'published', NOW() - INTERVAL '5 days', TRUE, FALSE, '{"title": "Cool Style Easy Wear", "description": "Hướng dẫn phối đồ casual"}');

-- Insert sample coupons (updated for new schema)
INSERT INTO coupons (code, name, description, type, value, min_order_amount, usage_limit, start_date, end_date, is_active) VALUES
('WELCOME10', 'Chào mừng khách hàng mới', 'Giảm 10% cho đơn hàng đầu tiên', 'percentage', 10.00, 500000, 1000, NOW() - INTERVAL '30 days', NOW() + INTERVAL '30 days', TRUE),
('SUMMER50', 'Khuyến mãi mùa hè', 'Giảm 50,000đ cho đơn hàng từ 1 triệu', 'fixed_amount', 50000.00, 1000000, 500, NOW() - INTERVAL '15 days', NOW() + INTERVAL '15 days', TRUE),
('FREESHIP', 'Miễn phí vận chuyển', 'Miễn phí vận chuyển cho đơn hàng từ 800k', 'fixed_amount', 30000.00, 800000, NULL, NOW() - INTERVAL '7 days', NOW() + INTERVAL '60 days', TRUE);

-- Insert system settings
INSERT INTO settings (key, value, description, type, is_public) VALUES
('site_name', 'John Henry Fashion', 'Tên website', 'string', TRUE),
('site_description', 'Thời trang cao cấp dành cho nam và nữ', 'Mô tả website', 'string', TRUE),
('currency', 'VND', 'Đơn vị tiền tệ', 'string', TRUE),
('tax_rate', '10', 'Thuế VAT (%)', 'number', FALSE),
('shipping_fee', '30000', 'Phí vận chuyển mặc định', 'number', TRUE),
('free_shipping_threshold', '800000', 'Miễn phí ship từ số tiền', 'number', TRUE),
('contact_email', 'contact@johnhenry.com', 'Email liên hệ', 'string', TRUE),
('contact_phone', '1900-xxxx', 'Số điện thoại liên hệ', 'string', TRUE),
('facebook_url', 'https://facebook.com/johnhenryvn', 'Facebook URL', 'string', TRUE),
('instagram_url', 'https://instagram.com/johnhenryvn', 'Instagram URL', 'string', TRUE);

-- Insert email templates (updated for new schema)
INSERT INTO email_templates (name, subject, body, type, is_active) VALUES
('order_confirmation', 'Xác nhận đơn hàng #{order_number}', 'Chào {customer_name},\n\nCảm ơn bạn đã đặt hàng tại John Henry. Đơn hàng #{order_number} của bạn đã được xác nhận.\n\nThông tin đơn hàng:\n{order_details}\n\nTổng tiền: {total_amount}\n\nCảm ơn bạn!', 'order_confirmation', TRUE),
('welcome_email', 'Chào mừng bạn đến với John Henry', 'Chào {customer_name},\n\nChào mừng bạn đến với gia đình John Henry! Cảm ơn bạn đã đăng ký tài khoản.\n\nHãy khám phá bộ sưu tập thời trang cao cấp của chúng tôi.\n\nTrân trọng,\nTeam John Henry', 'welcome', TRUE),
('order_status_update', 'Cập nhật tình trạng đơn hàng #{order_number}', 'Chào {customer_name},\n\nĐơn hàng #{order_number} của bạn đã được cập nhật tình trạng: {order_status}\n\n{order_details}\n\nCảm ơn bạn!', 'order_status_update', TRUE);

-- ========================================
-- INSERT DATA FOR NEW TABLES
-- ========================================

-- Insert payment methods
INSERT INTO payment_methods (name, code, description, icon_url, is_active, requires_redirect, min_amount, max_amount, sort_order) VALUES
('Thanh toán khi nhận hàng', 'cod', 'Thanh toán bằng tiền mặt khi nhận hàng', '/images/payment/cod.png', TRUE, FALSE, 0, 10000000, 1),
('Chuyển khoản ngân hàng', 'bank_transfer', 'Chuyển khoản qua ngân hàng', '/images/payment/bank.png', TRUE, FALSE, 0, NULL, 2),
('MoMo', 'momo', 'Thanh toán qua ví điện tử MoMo', '/images/payment/momo.png', TRUE, TRUE, 10000, 20000000, 3),
('VNPay', 'vnpay', 'Thanh toán qua VNPay', '/images/payment/vnpay.png', TRUE, TRUE, 10000, 20000000, 4),
('ZaloPay', 'zalopay', 'Thanh toán qua ZaloPay', '/images/payment/zalopay.png', TRUE, TRUE, 10000, 20000000, 5);

-- Insert shipping methods
INSERT INTO shipping_methods (name, code, description, cost, estimated_days, is_active, min_order_amount, max_weight, sort_order) VALUES
('Giao hàng tiêu chuẩn', 'standard', 'Giao hàng trong 3-5 ngày làm việc', 30000, 4, TRUE, 0, 5000, 1),
('Giao hàng nhanh', 'express', 'Giao hàng trong 1-2 ngày làm việc', 50000, 2, TRUE, 0, 3000, 2),
('Giao hàng miễn phí', 'free', 'Miễn phí giao hàng cho đơn từ 800k', 0, 5, TRUE, 800000, 5000, 3);

-- Insert sample notifications
INSERT INTO notifications (user_id, title, message, type, action_url, is_read, created_at) VALUES
('1', 'Chào mừng đến với John Henry', 'Cảm ơn bạn đã đăng ký tài khoản. Hãy khám phá bộ sưu tập thời trang của chúng tôi!', 'welcome', '/products', FALSE, NOW() - INTERVAL '1 day'),
('1', 'Sản phẩm mới đã có sẵn', 'Bộ sưu tập mùa đông mới đã ra mắt với nhiều mẫu áo len đẹp.', 'product', '/products/new', FALSE, NOW() - INTERVAL '2 hours');

-- Insert sample security logs
INSERT INTO security_logs (user_id, event_type, description, ip_address, user_agent, created_at) VALUES
('1', 'LoginSuccess', 'Đăng nhập thành công', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', NOW() - INTERVAL '1 hour'),
('1', 'PasswordChange', 'Thay đổi mật khẩu thành công', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', NOW() - INTERVAL '1 day');

-- Insert sample active sessions
INSERT INTO active_sessions (session_id, user_id, ip_address, user_agent, device_type, location, is_active, expires_at, created_at) VALUES
('sess_12345', '1', '192.168.1.100', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'Desktop', 'Hà Nội, Việt Nam', TRUE, NOW() + INTERVAL '7 days', NOW() - INTERVAL '1 hour');

-- Insert sample contact messages
INSERT INTO contact_messages (name, email, phone, subject, message, is_read, created_at) VALUES
('Nguyễn Văn A', 'customer@example.com', '0123456789', 'Hỏi về sản phẩm', 'Tôi muốn hỏi về chất liệu của áo hoodie nam.', FALSE, NOW() - INTERVAL '2 hours'),
('Trần Thị B', 'customer2@example.com', '0987654321', 'Phản hồi dịch vụ', 'Dịch vụ giao hàng rất tốt, cảm ơn shop!', TRUE, NOW() - INTERVAL '1 day');

-- ========================================
-- SAMPLE ANALYTICS DATA
-- ========================================

-- Insert sample user sessions
INSERT INTO user_sessions (session_id, user_id, start_time, end_time, duration, user_agent, ip_address, is_active, device_type, browser, operating_system, country, city) VALUES
('sess_analytics_1', '1', NOW() - INTERVAL '2 hours', NOW() - INTERVAL '1 hour', 60, 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', FALSE, 'Desktop', 'Chrome', 'Windows', 'Vietnam', 'Hà Nội'),
('sess_analytics_2', NULL, NOW() - INTERVAL '4 hours', NOW() - INTERVAL '3 hours', 45, 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)', '192.168.1.101', FALSE, 'Mobile', 'Safari', 'iOS', 'Vietnam', 'TP.HCM');

-- Insert sample page views
INSERT INTO page_views (user_id, session_id, page, referrer, user_agent, ip_address, time_on_page, source, medium, campaign, viewed_at) VALUES
('1', 'sess_analytics_1', '/products', 'https://google.com', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', '192.168.1.100', 120, 'google', 'organic', NULL, NOW() - INTERVAL '2 hours'),
(NULL, 'sess_analytics_2', '/products/ao-hoodie-nam-tay-dai-premium', '/products', 'Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)', '192.168.1.101', 180, 'direct', NULL, NULL, NOW() - INTERVAL '3 hours');

-- Insert sample conversion events
INSERT INTO conversion_events (user_id, session_id, conversion_type, value, source, medium, converted_at) VALUES
('1', 'sess_analytics_1', 'purchase', 800000, 'google', 'organic', NOW() - INTERVAL '1 hour');

-- Insert sample analytics data
INSERT INTO analytics_data (event_type, entity_id, user_id, session_id, source, medium, data) VALUES
('product_view', '1', '1', 'sess_analytics_1', 'google', 'organic', '{"product_name": "Áo Hoodie Nam Tay Dài Premium", "category": "Áo Nam"}'),
('add_to_cart', '1', '1', 'sess_analytics_1', 'google', 'organic', '{"product_id": 1, "quantity": 1, "price": 800000}');

-- ========================================
-- UPDATED VIEWS FOR NEW SCHEMA
-- ========================================

-- Drop old views if they exist
DROP VIEW IF EXISTS product_sales_summary;
DROP VIEW IF EXISTS order_summary;

-- Create updated views for reporting
CREATE VIEW product_sales_summary AS
SELECT 
    p.id,
    p.name,
    p.sku,
    p.price,
    p.sale_price,
    COALESCE(SUM(oi.quantity), 0) as total_sold,
    COALESCE(SUM(oi.total_price), 0) as total_revenue,
    p.stock_quantity,
    p.rating,
    p.review_count,
    c.name as category_name,
    b.name as brand_name,
    p.status,
    p.is_featured
FROM products p
LEFT JOIN order_items oi ON p.id = oi.product_id
LEFT JOIN orders o ON oi.order_id = o.id AND o.status NOT IN ('cancelled', 'returned')
LEFT JOIN categories c ON p.category_id = c.id
LEFT JOIN brands b ON p.brand_id = b.id
GROUP BY p.id, p.name, p.sku, p.price, p.sale_price, p.stock_quantity, p.rating, p.review_count, c.name, b.name, p.status, p.is_featured;

CREATE VIEW order_summary AS
SELECT 
    DATE(created_at) as order_date,
    COUNT(*) as total_orders,
    SUM(total_amount) as total_revenue,
    AVG(total_amount) as avg_order_value,
    COUNT(CASE WHEN status = 'delivered' THEN 1 END) as completed_orders,
    COUNT(CASE WHEN status = 'pending' THEN 1 END) as pending_orders,
    COUNT(CASE WHEN status = 'cancelled' THEN 1 END) as cancelled_orders
FROM orders
GROUP BY DATE(created_at)
ORDER BY order_date DESC;

CREATE VIEW daily_analytics AS
SELECT 
    DATE(created_at) as analytics_date,
    event_type,
    COUNT(*) as event_count,
    COUNT(DISTINCT user_id) as unique_users,
    COUNT(DISTINCT session_id) as unique_sessions
FROM analytics_data
GROUP BY DATE(created_at), event_type
ORDER BY analytics_date DESC, event_type;

CREATE VIEW user_engagement_summary AS
SELECT 
    us.user_id,
    COUNT(DISTINCT us.session_id) as total_sessions,
    AVG(us.duration) as avg_session_duration,
    COUNT(DISTINCT pv.page) as pages_visited,
    COUNT(ce.id) as conversions,
    SUM(ce.value) as total_conversion_value
FROM user_sessions us
LEFT JOIN page_views pv ON us.session_id = pv.session_id
LEFT JOIN conversion_events ce ON us.session_id = ce.session_id
WHERE us.user_id IS NOT NULL
GROUP BY us.user_id;
