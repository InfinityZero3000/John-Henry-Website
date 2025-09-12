-- ========================================
-- SEED DATA FOR JOHN HENRY E-COMMERCE
-- ========================================

-- Insert default admin user
INSERT INTO users (email, password_hash, first_name, last_name, role, is_active, is_email_verified) VALUES
('admin@johnhenry.com', '$2a$11$LQJ9oFjC6BF.AxhG5LQKOeJMB2lA3h3QKOr9vXKOKV2J3lA4h5QWO', 'Admin', 'User', 'Admin', TRUE, TRUE),
('staff@johnhenry.com', '$2a$11$LQJ9oFjC6BF.AxhG5LQKOeJMB2lA3h3QKOr9vXKOKV2J3lA4h5QWO', 'Staff', 'User', 'Staff', TRUE, TRUE);

-- Insert brands
INSERT INTO brands (name, slug, description, is_active) VALUES
('John Henry', 'john-henry', 'Thương hiệu thời trang nam cao cấp', TRUE),
('Freelancer', 'freelancer', 'Thương hiệu thời trang nữ hiện đại', TRUE);

-- Insert main categories
INSERT INTO categories (name, slug, description, is_active, sort_order) VALUES
('Nam', 'nam', 'Thời trang nam', TRUE, 1),
('Nữ', 'nu', 'Thời trang nữ', TRUE, 2),
('Phụ kiện', 'phu-kien', 'Phụ kiện thời trang', TRUE, 3);

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

-- Insert product attributes
INSERT INTO product_attributes (name, display_name, type, is_required, is_filterable, sort_order) VALUES
('size', 'Kích thước', 'select', TRUE, TRUE, 1),
('color', 'Màu sắc', 'color', TRUE, TRUE, 2),
('material', 'Chất liệu', 'select', FALSE, TRUE, 3),
('fit', 'Kiểu dáng', 'select', FALSE, TRUE, 4);

-- Insert size values
INSERT INTO product_attribute_values (attribute_id, value, display_value, sort_order) VALUES
(1, 'XS', 'XS', 1),
(1, 'S', 'S', 2),
(1, 'M', 'M', 3),
(1, 'L', 'L', 4),
(1, 'XL', 'XL', 5),
(1, 'XXL', 'XXL', 6);

-- Insert color values
INSERT INTO product_attribute_values (attribute_id, value, display_value, color_code, sort_order) VALUES
(2, 'black', 'Đen', '#000000', 1),
(2, 'white', 'Trắng', '#FFFFFF', 2),
(2, 'red', 'Đỏ', '#CE193A', 3),
(2, 'blue', 'Xanh dương', '#0066CC', 4),
(2, 'navy', 'Xanh navy', '#001f3f', 5),
(2, 'gray', 'Xám', '#808080', 6),
(2, 'beige', 'Be', '#F5F5DC', 7),
(2, 'brown', 'Nâu', '#8B4513', 8);

-- Insert material values
INSERT INTO product_attribute_values (attribute_id, value, display_value, sort_order) VALUES
(3, 'cotton', '100% Cotton', 1),
(3, 'polyester', '100% Polyester', 2),
(3, 'cotton-blend', 'Cotton pha', 3),
(3, 'wool', '100% Wool', 4),
(3, 'silk', '100% Silk', 5),
(3, 'linen', '100% Linen', 6);

-- Insert fit values
INSERT INTO product_attribute_values (attribute_id, value, display_value, sort_order) VALUES
(4, 'slim', 'Slim fit', 1),
(4, 'regular', 'Regular fit', 2),
(4, 'loose', 'Loose fit', 3),
(4, 'oversize', 'Oversize', 4);

-- Insert sample products for Men
INSERT INTO products (sku, name, slug, description, short_description, category_id, brand_id, price, compare_price, stock_quantity, is_active, is_featured, tags) VALUES
('JH-MH-001', 'Áo Hoodie Nam Tay Dài Premium', 'ao-hoodie-nam-tay-dai-premium', 'Áo hoodie nam tay dài chất liệu cotton cao cấp, thiết kế hiện đại, phù hợp cho mọi hoạt động hàng ngày.', 'Áo hoodie nam tay dài chất liệu premium', 12, 1, 800000, 1000000, 50, TRUE, TRUE, ARRAY['hoodie', 'nam', 'tay-dai']),
('JH-MJ-001', 'Áo Khoác Nam Cá Tính', 'ao-khoac-nam-ca-tinh', 'Áo khoác nam thiết kế cá tính, chất liệu cao cấp, phù hợp cho nhiều dịp khác nhau.', 'Áo khoác nam cá tính cao cấp', 12, 1, 1500000, 1800000, 30, TRUE, TRUE, ARRAY['khoac', 'nam', 'ca-tinh']),
('JH-MS-001', 'Áo Len Nam Tay Dài Form Vừa', 'ao-len-nam-tay-dai-form-vua', 'Áo len nam tay dài form vừa, chất liệu wool cao cấp, giữ ấm tốt.', 'Áo len nam tay dài form vừa', 11, 1, 890000, 1100000, 40, TRUE, FALSE, ARRAY['len', 'nam', 'form-vua']);

-- Insert sample products for Women
INSERT INTO products (sku, name, slug, description, short_description, category_id, brand_id, price, compare_price, stock_quantity, is_active, is_featured, tags) VALUES
('FL-WB-001', 'Áo Blouse Nữ Thời Trang', 'ao-blouse-nu-thoi-trang', 'Áo blouse nữ thiết kế thời trang, chất liệu mềm mại, phù hợp cho công sở.', 'Áo blouse nữ thời trang công sở', 18, 2, 750000, 900000, 45, TRUE, TRUE, ARRAY['blouse', 'nu', 'cong-so']),
('FL-WP-001', 'Áo Polo Nữ Cổ Lưới', 'ao-polo-nu-co-luoi', 'Áo polo nữ cổ lưới thiết kế độc đáo, thoáng mát và thoải mái.', 'Áo polo nữ cổ lưới thoáng mát', 21, 2, 680000, 800000, 35, TRUE, FALSE, ARRAY['polo', 'nu', 'co-luoi']),
('FL-WS-001', 'Áo Sơ Mi Nữ Tay Ngắn', 'ao-so-mi-nu-tay-ngan', 'Áo sơ mi nữ tay ngắn phong cách hiện đại, phù hợp cho nhiều dịp.', 'Áo sơ mi nữ tay ngắn hiện đại', 20, 2, 890000, 1000000, 25, TRUE, TRUE, ARRAY['so-mi', 'nu', 'tay-ngan']),
('FL-WT-001', 'Áo Thun Nữ Basic Form Rộng', 'ao-thun-nu-basic-form-rong', 'Áo thun nữ basic form rộng, thoải mái và dễ phối đồ.', 'Áo thun nữ basic thoải mái', 19, 2, 450000, 550000, 60, TRUE, FALSE, ARRAY['thun', 'nu', 'basic']);

-- Insert product images
INSERT INTO product_images (product_id, image_url, alt_text, sort_order, is_main) VALUES
-- Áo Hoodie Nam
(1, '/images/Áo nam/Áo Hoodie Nam Tay Dài TS25FH22C-LS.jpg', 'Áo Hoodie Nam Tay Dài', 1, TRUE),
-- Áo Khoác Nam 1
(2, '/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH04C-PA.jpg', 'Áo Khoác Nam Cá Tính', 1, TRUE),
-- Áo Len Nam
(3, '/images/Áo nam/Áo Len Nam Tay Dài Form Vừa SW25FH04M-LB.jpg', 'Áo Len Nam Tay Dài', 1, TRUE),
-- Áo Blouse Nữ
(4, '/images/Áo nữ/Áo Blouse Nữ Thời Trang FWBL25SS01C.jpg', 'Áo Blouse Nữ Thời Trang', 1, TRUE),
-- Áo Polo Nữ
(5, '/images/Áo nữ/Áo Polo Nữ Cổ Lưới FWKS25SS11G.jpg', 'Áo Polo Nữ Cổ Lưới', 1, TRUE),
-- Áo Sơ Mi Nữ
(6, '/images/Áo nữ/Áo Sơ Mi Nữ Tay Ngắn FWSH25SS03C.jpg', 'Áo Sơ Mi Nữ Tay Ngắn', 1, TRUE),
-- Áo Thun Nữ
(7, '/images/Áo nữ/Áo Thun Nữ Basic Form Rộng FWTS25SS04C.jpg', 'Áo Thun Nữ Basic', 1, TRUE);

-- Insert product variants with different sizes and colors
-- Áo Hoodie Nam variants
INSERT INTO product_variants (product_id, sku, name, price, stock_quantity, is_active, sort_order) VALUES
(1, 'JH-MH-001-S-BLACK', 'Áo Hoodie Nam - S - Đen', 800000, 10, TRUE, 1),
(1, 'JH-MH-001-M-BLACK', 'Áo Hoodie Nam - M - Đen', 800000, 15, TRUE, 2),
(1, 'JH-MH-001-L-BLACK', 'Áo Hoodie Nam - L - Đen', 800000, 12, TRUE, 3),
(1, 'JH-MH-001-S-GRAY', 'Áo Hoodie Nam - S - Xám', 800000, 8, TRUE, 4),
(1, 'JH-MH-001-M-GRAY', 'Áo Hoodie Nam - M - Xám', 800000, 10, TRUE, 5);

-- Áo Khoác Nam variants
INSERT INTO product_variants (product_id, sku, name, price, stock_quantity, is_active, sort_order) VALUES
(2, 'JH-MJ-001-M-NAVY', 'Áo Khoác Nam - M - Navy', 1500000, 8, TRUE, 1),
(2, 'JH-MJ-001-L-NAVY', 'Áo Khoác Nam - L - Navy', 1500000, 10, TRUE, 2),
(2, 'JH-MJ-001-M-BLACK', 'Áo Khoác Nam - M - Đen', 1500000, 7, TRUE, 3);

-- Insert variant attributes
-- Hoodie variants attributes
INSERT INTO product_variant_attributes (variant_id, attribute_id, attribute_value_id) VALUES
-- Size S, Color Black
(1, 1, 2), (1, 2, 1),
-- Size M, Color Black  
(2, 1, 3), (2, 2, 1),
-- Size L, Color Black
(3, 1, 4), (3, 2, 1),
-- Size S, Color Gray
(4, 1, 2), (4, 2, 6),
-- Size M, Color Gray
(5, 1, 3), (5, 2, 6);

-- Jacket variants attributes
INSERT INTO product_variant_attributes (variant_id, attribute_id, attribute_value_id) VALUES
-- Size M, Color Navy
(6, 1, 3), (6, 2, 5),
-- Size L, Color Navy
(7, 1, 4), (7, 2, 5),
-- Size M, Color Black
(8, 1, 3), (8, 2, 1);

-- Insert blog categories
INSERT INTO blog_categories (name, slug, description, is_active, sort_order) VALUES
('Tin tức', 'tin-tuc', 'Tin tức về thương hiệu và sản phẩm', TRUE, 1),
('Xu hướng thời trang', 'xu-huong-thoi-trang', 'Các xu hướng thời trang mới nhất', TRUE, 2),
('Hướng dẫn phối đồ', 'huong-dan-phoi-do', 'Hướng dẫn cách phối đồ đẹp', TRUE, 3),
('Sự kiện', 'su-kien', 'Các sự kiện của thương hiệu', TRUE, 4);

-- Insert sample blog posts
INSERT INTO blog_posts (title, slug, content, excerpt, featured_image, author_id, category_id, tags, published_at, is_published, is_featured) VALUES
('Grand Opening John Henry Cầu Giấy', 'grand-opening-john-henry-cau-giay', 'Chính thức có mặt tại 355 Cầu Giấy, P. Cầu Giấy, TP. Hà Nội, JOHN HENRY mang đến một không gian mua sắm hoàn toàn mới...', 'Chính thức có mặt tại 355 Cầu Giấy, P. Cầu Giấy, TP. Hà Nội, JOHN HENRY mang đến một không gian mua sắm...', '/images/Banner/banner-man-main.jpg', 1, 4, ARRAY['grand-opening', 'cua-hang-moi'], NOW() - INTERVAL '2 days', TRUE, TRUE),
('New Drop Jeans Collection', 'new-drop-jeans-collection', 'Muốn để vibe nữ văn thoái mái cả ngày? Quần jeans ống rộng darkwash mới của Freelancer giúp nàng g...', 'Muốn để vibe nữ văn thoái mái cả ngày? Quần jeans ống rộng darkwash mới của Freelancer...', '/images/Banner/banner-women-1.jpg', 1, 1, ARRAY['jeans', 'new-collection'], NOW() - INTERVAL '3 days', TRUE, FALSE),
('Cool Style Easy Wear', 'cool-style-easy-wear', 'Chỉ cần một chiếc áo polo basic, một chiếc quần short năng động, thêm đôi giày là đã có ng...', 'Chỉ cần một chiếc áo polo basic, một chiếc quần short năng động, thêm đôi giày...', '/images/Banner/banner-man-0.jpg', 1, 3, ARRAY['style-guide', 'casual'], NOW() - INTERVAL '5 days', TRUE, FALSE);

-- Insert sample coupons
INSERT INTO coupons (code, name, description, discount_type, discount_value, minimum_amount, usage_limit, start_date, end_date, is_active) VALUES
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

-- Insert email templates
INSERT INTO email_templates (name, subject, body, variables, is_active) VALUES
('order_confirmation', 'Xác nhận đơn hàng #{order_number}', 'Chào {customer_name},\n\nCảm ơn bạn đã đặt hàng tại John Henry. Đơn hàng #{order_number} của bạn đã được xác nhận.\n\nThông tin đơn hàng:\n{order_details}\n\nTổng tiền: {total_amount}\n\nCảm ơn bạn!', '{"customer_name": "Tên khách hàng", "order_number": "Số đơn hàng", "order_details": "Chi tiết đơn hàng", "total_amount": "Tổng tiền"}', TRUE),
('welcome_email', 'Chào mừng bạn đến với John Henry', 'Chào {customer_name},\n\nChào mừng bạn đến với gia đình John Henry! Cảm ơn bạn đã đăng ký tài khoản.\n\nHãy khám phá bộ sưu tập thời trang cao cấp của chúng tôi.\n\nTrân trọng,\nTeam John Henry', '{"customer_name": "Tên khách hàng"}', TRUE);

-- Create views for reporting
CREATE VIEW product_sales_summary AS
SELECT 
    p.id,
    p.name,
    p.sku,
    p.price,
    COALESCE(SUM(oi.quantity), 0) as total_sold,
    COALESCE(SUM(oi.total_price), 0) as total_revenue,
    p.stock_quantity,
    c.name as category_name,
    b.name as brand_name
FROM products p
LEFT JOIN order_items oi ON p.id = oi.product_id
LEFT JOIN orders o ON oi.order_id = o.id AND o.order_status NOT IN ('Cancelled', 'Returned')
LEFT JOIN categories c ON p.category_id = c.id
LEFT JOIN brands b ON p.brand_id = b.id
GROUP BY p.id, p.name, p.sku, p.price, p.stock_quantity, c.name, b.name;

CREATE VIEW order_summary AS
SELECT 
    DATE(created_at) as order_date,
    COUNT(*) as total_orders,
    SUM(total_amount) as total_revenue,
    AVG(total_amount) as avg_order_value,
    COUNT(CASE WHEN order_status = 'Delivered' THEN 1 END) as completed_orders
FROM orders
GROUP BY DATE(created_at)
ORDER BY order_date DESC;
