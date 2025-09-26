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

-- Insert all products based on images folder structure

-- ========================================
-- ÁO NAM (JOHN HENRY) - CATEGORY IDs: 10-14
-- ========================================
INSERT INTO products (sku, name, slug, description, short_description, category_id, brand_id, price, sale_price, stock_quantity, featured_image_url, gallery_images, size, color, material, is_active, is_featured, status, seo_metadata) VALUES

-- Áo Hoodie Nam (Category 12 - Áo Khoác Nam)
('JH-HD-001', 'Áo Hoodie Nam Tay Dài Premium', 'ao-hoodie-nam-tay-dai-premium', 'Áo hoodie nam tay dài chất liệu cotton cao cấp, thiết kế hiện đại, phù hợp cho mọi hoạt động hàng ngày.', 'Áo hoodie nam tay dài chất liệu premium', 12, 1, 800000, 720000, 50, '/images/Áo nam/Áo Hoodie Nam Tay Dài TS25FH22C-LS.jpg', ARRAY['/images/Áo nam/Áo Hoodie Nam Tay Dài TS25FH22C-LS.jpg'], 'S,M,L,XL', 'Đen,Xám,Navy', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Hoodie Nam Premium", "description": "Áo hoodie nam chất liệu cotton cao cấp"}'),

-- Áo Khoác Nam (Category 12 - Áo Khoác Nam)
('JH-JK-001', 'Áo Khoác Nam Cá Tính', 'ao-khoac-nam-ca-tinh', 'Áo khoác nam thiết kế cá tính, chất liệu cao cấp, phù hợp cho nhiều dịp khác nhau.', 'Áo khoác nam cá tính cao cấp', 12, 1, 1500000, 1350000, 30, '/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH04C-PA.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH04C-PA.jpg'], 'M,L,XL', 'Đen,Navy,Xám', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Áo Khoác Nam Cá Tính", "description": "Áo khoác nam thiết kế cá tính"}'),
('JH-JK-002', 'Áo Khoác Nam Cá Tính JK25FH09P', 'ao-khoac-nam-ca-tinh-jk25fh09p', 'Áo khoác nam thiết kế cá tính với chất liệu cao cấp, phù hợp cho thời tiết mát mẻ.', 'Áo khoác nam cá tính thời trang', 12, 1, 1450000, 1305000, 25, '/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH09P-KA.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Cá Tính JK25FH09P-KA.jpg'], 'M,L,XL', 'Xanh,Đen,Xám', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Áo Khoác Nam Cá Tính", "description": "Áo khoác nam thiết kế thời trang"}'),
('JH-JK-003', 'Áo Khoác Nam Thời Trang JK25FH02P', 'ao-khoac-nam-thoi-trang-jk25fh02p', 'Áo khoác nam thiết kế thời trang, phong cách hiện đại và năng động.', 'Áo khoác nam thời trang hiện đại', 12, 1, 1380000, 1242000, 35, '/images/Áo nam/Áo Khoác Nam Thời Trang JK25FH02P-CT.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Thời Trang JK25FH02P-CT.jpg'], 'S,M,L,XL', 'Nâu,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Khoác Nam Thời Trang", "description": "Áo khoác nam phong cách hiện đại"}'),
('JH-JK-004', 'Áo Khoác Nam Thời Trang JK25FH10T', 'ao-khoac-nam-thoi-trang-jk25fh10t', 'Áo khoác nam thiết kế thời trang với form dáng chuẩn, phù hợp mọi dịp.', 'Áo khoác nam thời trang cao cấp', 12, 1, 1420000, NULL, 28, '/images/Áo nam/Áo Khoác Nam Thời Trang JK25FH10T-KA.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Thời Trang JK25FH10T-KA.jpg'], 'M,L,XL', 'Xanh,Đen,Xám', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Áo Khoác Nam Thời Trang", "description": "Áo khoác nam cao cấp"}'),
('JH-JK-005', 'Áo Khoác Nam Thời Trang JK25SS01T', 'ao-khoac-nam-thoi-trang-jk25ss01t', 'Áo khoác nam mùa hè, chất liệu thoáng mát, thiết kế trẻ trung.', 'Áo khoác nam mùa hè thoáng mát', 12, 1, 1200000, 1080000, 40, '/images/Áo nam/Áo Khoác Nam Thời Trang JK25SS01T-PA.jpg', ARRAY['/images/Áo nam/Áo Khoác Nam Thời Trang JK25SS01T-PA.jpg'], 'S,M,L,XL', 'Xanh,Be,Đen', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Áo Khoác Nam Mùa Hè", "description": "Áo khoác nam thoáng mát"}'),

-- Áo Len Nam (Category 11 - Áo Len Nam)
('JH-SW-001', 'Áo Len Nam Tay Dài Form Vừa', 'ao-len-nam-tay-dai-form-vua', 'Áo len nam tay dài form vừa, chất liệu wool cao cấp, giữ ấm tốt.', 'Áo len nam tay dài form vừa', 11, 1, 890000, NULL, 40, '/images/Áo nam/Áo Len Nam Tay Dài Form Vừa SW25FH04M-LB.jpg', ARRAY['/images/Áo nam/Áo Len Nam Tay Dài Form Vừa SW25FH04M-LB.jpg'], 'S,M,L,XL', 'Be,Xám,Navy', '100% Wool', TRUE, FALSE, 'active', '{"title": "Áo Len Nam Form Vừa", "description": "Áo len nam chất liệu wool cao cấp"}'),
('JH-SW-002', 'Áo Len Nam Tay Ngắn Form Vừa', 'ao-len-nam-tay-ngan-form-vua', 'Áo len nam tay ngắn form vừa, phù hợp cho thời tiết mát mẻ.', 'Áo len nam tay ngắn thời trang', 11, 1, 750000, 675000, 35, '/images/Áo nam/Áo Len Nam Tay Ngắn Form Vừa SW25FH03P-SA.jpg', ARRAY['/images/Áo nam/Áo Len Nam Tay Ngắn Form Vừa SW25FH03P-SA.jpg'], 'S,M,L,XL', 'Hồng,Đen,Xám', 'Wool blend', TRUE, FALSE, 'active', '{"title": "Áo Len Nam Tay Ngắn", "description": "Áo len nam form vừa"}'),

-- Áo Polo Nam (Category 10 - Áo Polo Nam)
('JH-KS-001', 'Áo Polo Nam Form Ôm Tay Ngắn', 'ao-polo-nam-form-om-tay-ngan', 'Áo polo nam form ôm tay ngắn, thiết kế hiện đại và năng động.', 'Áo polo nam form ôm cao cấp', 10, 1, 650000, 585000, 45, '/images/Áo nam/Áo Polo Nam Form Ôm Tay Ngắn KS25FH49C-SLWK.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Form Ôm Tay Ngắn KS25FH49C-SLWK.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Đen', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Polo Nam Form Ôm", "description": "Áo polo nam thiết kế hiện đại"}'),
('JH-KS-002', 'Áo Polo Nam Form Vừa KS25FH44C', 'ao-polo-nam-form-vua-ks25fh44c', 'Áo polo nam form vừa, phong cách lịch lãm và thoải mái.', 'Áo polo nam form vừa lịch lãm', 10, 1, 620000, NULL, 50, '/images/Áo nam/Áo Polo Nam Form Vừa KS25FH44C-SCCA.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Form Vừa KS25FH44C-SCCA.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nam Form Vừa", "description": "Áo polo nam lịch lãm"}'),
('JH-KS-003', 'Áo Polo Nam Tay Ngắn KS25FH43C', 'ao-polo-nam-tay-ngan-ks25fh43c', 'Áo polo nam tay ngắn form vừa, chất liệu cotton cao cấp.', 'Áo polo nam tay ngắn cotton', 10, 1, 580000, 522000, 55, '/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH43C-SCCA.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH43C-SCCA.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nam Tay Ngắn", "description": "Áo polo nam cotton cao cấp"}'),
('JH-KS-004', 'Áo Polo Nam Tay Ngắn KS25FH45C', 'ao-polo-nam-tay-ngan-ks25fh45c', 'Áo polo nam tay ngắn form vừa, thiết kế tối giản và thanh lịch.', 'Áo polo nam tối giản thanh lịch', 10, 1, 600000, NULL, 48, '/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH45C-SCCA.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH45C-SCCA.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nam Tối Giản", "description": "Áo polo nam thanh lịch"}'),
('JH-KS-005', 'Áo Polo Nam Tay Ngắn KS25FH46T', 'ao-polo-nam-tay-ngan-ks25fh46t', 'Áo polo nam tay ngắn form vừa, phong cách trẻ trung và hiện đại.', 'Áo polo nam trẻ trung hiện đại', 10, 1, 620000, 558000, 42, '/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH46T-SCCA.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH46T-SCCA.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nam Trẻ Trung", "description": "Áo polo nam hiện đại"}'),
('JH-KS-006', 'Áo Polo Nam Tay Ngắn KS25FH57C', 'ao-polo-nam-tay-ngan-ks25fh57c', 'Áo polo nam tay ngắn form vừa, chất liệu cotton thoáng mát.', 'Áo polo nam cotton thoáng mát', 10, 1, 640000, NULL, 38, '/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH57C-SC .jpg', ARRAY['/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25FH57C-SC .jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nam Thoáng Mát", "description": "Áo polo nam cotton thoáng mát"}'),
('JH-KS-007', 'Áo Polo Nam Tay Ngắn KS25SS08C', 'ao-polo-nam-tay-ngan-ks25ss08c', 'Áo polo nam mùa hè, thiết kế trẻ trung và thoáng mát.', 'Áo polo nam mùa hè thoáng mát', 10, 1, 590000, 531000, 60, '/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25SS08C-SCHE.jpg', ARRAY['/images/Áo nam/Áo Polo Nam Tay Ngắn Form Vừa KS25SS08C-SCHE.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Polo Nam Mùa Hè", "description": "Áo polo nam thoáng mát"}'),

-- Áo Sơ Mi Nam (Category 9 - Áo Sơ Mi Nam)
('JH-WS-001', 'Áo Sơ Mi Denim Thời Trang', 'ao-so-mi-denim-thoi-trang', 'Áo sơ mi denim nam phong cách thời trang, chất liệu bền đẹp.', 'Áo sơ mi denim thời trang', 9, 1, 780000, 702000, 32, '/images/Áo nam/Áo Sơ Mi Denim Thời Trang WS24SS16P-LCRG.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Denim Thời Trang WS24SS16P-LCRG.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, TRUE, 'active', '{"title": "Áo Sơ Mi Denim", "description": "Áo sơ mi denim thời trang"}'),
('JH-WS-002', 'Áo Sơ Mi Nam Form Ôm Tay Dài', 'ao-so-mi-nam-form-om-tay-dai', 'Áo sơ mi nam form ôm tay dài, thiết kế thanh lịch cho công sở.', 'Áo sơ mi nam form ôm thanh lịch', 9, 1, 720000, NULL, 45, '/images/Áo nam/Áo Sơ Mi Nam Form Ôm Tay Dài WS25FH98T-LD.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Form Ôm Tay Dài WS25FH98T-LD.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Đen', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Form Ôm", "description": "Áo sơ mi nam thanh lịch"}'),
('JH-WS-003', 'Áo Sơ Mi Nam Form Ôm Tay Ngắn WS25FH65P', 'ao-so-mi-nam-form-om-tay-ngan-ws25fh65p', 'Áo sơ mi nam form ôm tay ngắn, phù hợp cho thời tiết ấm áp.', 'Áo sơ mi nam form ôm tay ngắn', 9, 1, 680000, 612000, 40, '/images/Áo nam/Áo Sơ Mi Nam Form Ôm Tay Ngắn WS25FH65P-SDBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Form Ôm Tay Ngắn WS25FH65P-SDBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Tay Ngắn", "description": "Áo sơ mi nam form ôm"}'),
('JH-WS-004', 'Áo Sơ Mi Nam Tay Dài Form Ôm WS25FH63P', 'ao-so-mi-nam-tay-dai-form-om-ws25fh63p', 'Áo sơ mi nam tay dài form ôm, thiết kế lịch lãm cho công sở.', 'Áo sơ mi nam tay dài lịch lãm', 9, 1, 750000, NULL, 35, '/images/Áo nam/Áo Sơ Mi Nam Tay Dài Form Ôm WS25FH63P-LC.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Dài Form Ôm WS25FH63P-LC.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Tay Dài", "description": "Áo sơ mi nam lịch lãm"}'),
('JH-WS-005', 'Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH68C', 'ao-so-mi-nam-tay-ngan-form-om-ws25fh68c', 'Áo sơ mi nam tay ngắn form ôm, phong cách hiện đại và trẻ trung.', 'Áo sơ mi nam hiện đại trẻ trung', 9, 1, 690000, 621000, 38, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH68C-SDBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH68C-SDBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Hiện Đại", "description": "Áo sơ mi nam trẻ trung"}'),
('JH-WS-006', 'Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH70T', 'ao-so-mi-nam-tay-ngan-form-om-ws25fh70t', 'Áo sơ mi nam tay ngắn form ôm, thiết kế sang trọng và thanh lịch.', 'Áo sơ mi nam sang trọng thanh lịch', 9, 1, 710000, NULL, 42, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH70T-SDBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25FH70T-SDBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Sang Trọng", "description": "Áo sơ mi nam thanh lịch"}'),
('JH-WS-007', 'Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25SS33P', 'ao-so-mi-nam-tay-ngan-form-om-ws25ss33p', 'Áo sơ mi nam mùa hè, form ôm thoáng mát và thoải mái.', 'Áo sơ mi nam mùa hè thoáng mát', 9, 1, 650000, 585000, 55, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25SS33P-SDBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Ôm WS25SS33P-SDBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Áo Sơ Mi Nam Mùa Hè", "description": "Áo sơ mi nam thoáng mát"}'),
('JH-WS-008', 'Áo Sơ Mi Nam Tay Ngắn Form Rộng', 'ao-so-mi-nam-tay-ngan-form-rong', 'Áo sơ mi nam tay ngắn form rộng, phong cách thoải mái và năng động.', 'Áo sơ mi nam form rộng thoải mái', 9, 1, 670000, NULL, 48, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Rộng WS25FH78P-CL.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Rộng WS25FH78P-CL.jpg'], 'S,M,L,XL,XXL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Form Rộng", "description": "Áo sơ mi nam thoải mái"}'),
('JH-WS-009', 'Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH64C', 'ao-so-mi-nam-tay-ngan-form-vua-ws25fh64c', 'Áo sơ mi nam tay ngắn form vừa, thiết kế cân đối và lịch sự.', 'Áo sơ mi nam form vừa lịch sự', 9, 1, 690000, 621000, 44, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH64C-CFBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH64C-CFBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Form Vừa", "description": "Áo sơ mi nam lịch sự"}'),
('JH-WS-010', 'Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH66C', 'ao-so-mi-nam-tay-ngan-form-vua-ws25fh66c', 'Áo sơ mi nam tay ngắn form vừa, phong cách thanh lịch và chuyên nghiệp.', 'Áo sơ mi nam thanh lịch chuyên nghiệp', 9, 1, 700000, NULL, 40, '/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH66C-CFBB.jpg', ARRAY['/images/Áo nam/Áo Sơ Mi Nam Tay Ngắn Form Vừa WS25FH66C-CFBB.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Nam Chuyên Nghiệp", "description": "Áo sơ mi nam thanh lịch"}'),

-- Áo Thun Nam (Category 8 - Áo Thun Nam)
('JH-TS-001', 'Áo Thun Nam Form Vừa Tay Ngắn TS25SS09C', 'ao-thun-nam-form-vua-tay-ngan-ts25ss09c', 'Áo thun nam form vừa tay ngắn, chất liệu cotton thoáng mát.', 'Áo thun nam cotton thoáng mát', 8, 1, 420000, 378000, 65, '/images/Áo nam/Áo Thun Nam Form Vừa Tay Ngắn TS25SS09C-RGUS.jpg', ARRAY['/images/Áo nam/Áo Thun Nam Form Vừa Tay Ngắn TS25SS09C-RGUS.jpg'], 'S,M,L,XL,XXL', 'Đen,Trắng,Xám,Navy', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Thun Nam Cotton", "description": "Áo thun nam thoáng mát"}'),
('JH-TS-002', 'Áo Thun Nam Tay Ngắn Form Vừa TS25FH19T', 'ao-thun-nam-tay-ngan-form-vua-ts25fh19t', 'Áo thun nam tay ngắn form vừa, thiết kế basic và thoải mái.', 'Áo thun nam basic thoải mái', 8, 1, 380000, NULL, 75, '/images/Áo nam/Áo Thun Nam Tay Ngắn Form Vừa TS25FH19T-RGUS.jpg', ARRAY['/images/Áo nam/Áo Thun Nam Tay Ngắn Form Vừa TS25FH19T-RGUS.jpg'], 'S,M,L,XL,XXL', 'Đen,Trắng,Xám,Navy', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Thun Nam Basic", "description": "Áo thun nam thoải mái"}'),
('JH-TS-003', 'Áo Thun Nam Tay Ngắn Form Vừa TS25SS12C', 'ao-thun-nam-tay-ngan-form-vua-ts25ss12c', 'Áo thun nam mùa hè, form vừa thoáng mát và dễ phối đồ.', 'Áo thun nam mùa hè dễ phối', 8, 1, 400000, 360000, 80, '/images/Áo nam/Áo Thun Nam Tay Ngắn Form Vừa TS25SS12C-RGUS.jpg', ARRAY['/images/Áo nam/Áo Thun Nam Tay Ngắn Form Vừa TS25SS12C-RGUS.jpg'], 'S,M,L,XL,XXL', 'Đen,Trắng,Xám,Navy', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Thun Nam Mùa Hè", "description": "Áo thun nam dễ phối"}'),

-- ========================================
-- ÁO NỮ (FREELANCER) - CATEGORY IDs: 18-23
-- ========================================

-- Áo Blouse Nữ (Category 18 - Áo Blouse Nữ)
('FL-BL-001', 'Áo Blouse Cổ Trụ Freelancer', 'ao-blouse-co-tru-freelancer', 'Áo blouse nữ cổ trụ thiết kế thanh lịch, phù hợp cho công sở và dạo phố.', 'Áo blouse nữ cổ trụ thanh lịch', 18, 2, 780000, 702000, 35, '/images/Áo nữ/Áo Blouse Cổ Trụ Freelancer FWBL25SS07C.jpg', ARRAY['/images/Áo nữ/Áo Blouse Cổ Trụ Freelancer FWBL25SS07C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Silk blend', TRUE, TRUE, 'active', '{"title": "Áo Blouse Cổ Trụ", "description": "Áo blouse nữ thanh lịch"}'),
('FL-BL-002', 'Áo Blouse Freelancer FWBL25SS03C', 'ao-blouse-freelancer-fwbl25ss03c', 'Áo blouse nữ thiết kế hiện đại, chất liệu mềm mại và thoáng mát.', 'Áo blouse nữ hiện đại mềm mại', 18, 2, 720000, NULL, 40, '/images/Áo nữ/Áo Blouse Freelancer FWBL25SS03C.jpg', ARRAY['/images/Áo nữ/Áo Blouse Freelancer FWBL25SS03C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Silk blend', TRUE, FALSE, 'active', '{"title": "Áo Blouse Hiện Đại", "description": "Áo blouse nữ mềm mại"}'),
('FL-BL-003', 'Áo Blouse Nữ Thời Trang', 'ao-blouse-nu-thoi-trang', 'Áo blouse nữ thiết kế thời trang, chất liệu mềm mại, phù hợp cho công sở.', 'Áo blouse nữ thời trang công sở', 18, 2, 750000, 675000, 45, '/images/Áo nữ/Áo Blouse Nữ Thời Trang FWBL25SS01C.jpg', ARRAY['/images/Áo nữ/Áo Blouse Nữ Thời Trang FWBL25SS01C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Silk blend', TRUE, TRUE, 'active', '{"title": "Áo Blouse Nữ Thời Trang", "description": "Áo blouse nữ cho công sở"}'),
('FL-BL-004', 'Áo kiểu Nữ thời trang', 'ao-kieu-nu-thoi-trang', 'Áo kiểu nữ thiết kế thời trang, phong cách trẻ trung và năng động.', 'Áo kiểu nữ trẻ trung năng động', 18, 2, 680000, 612000, 38, '/images/Áo nữ/Áo kiểu Nữ thời trang FWBL25SS09C.jpg', ARRAY['/images/Áo nữ/Áo kiểu Nữ thời trang FWBL25SS09C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Áo Kiểu Nữ Thời Trang", "description": "Áo kiểu nữ trẻ trung"}'),
('FL-BL-005', 'Áo Nữ Thời Trang FWBL25SS02C', 'ao-nu-thoi-trang-fwbl25ss02c', 'Áo nữ thiết kế thời trang với chất liệu cao cấp và form dáng đẹp.', 'Áo nữ thời trang cao cấp', 18, 2, 760000, NULL, 42, '/images/Áo nữ/Áo Nữ Thời Trang FWBL25SS02C.jpg', ARRAY['/images/Áo nữ/Áo Nữ Thời Trang FWBL25SS02C.jpg'], 'S,M,L,XL', 'Trắng,Hồng,Be', 'Silk blend', TRUE, FALSE, 'active', '{"title": "Áo Nữ Thời Trang", "description": "Áo nữ cao cấp"}'),

-- Áo Polo Nữ (Category 21 - Áo Polo Nữ)
('FL-PL-001', 'Áo Polo Cổ V', 'ao-polo-co-v', 'Áo polo nữ cổ V thiết kế thanh lịch, phù hợp cho nhiều dịp khác nhau.', 'Áo polo nữ cổ V thanh lịch', 21, 2, 620000, 558000, 45, '/images/Áo nữ/Áo Polo Cổ V – FWKS25SS02G.jpg', ARRAY['/images/Áo nữ/Áo Polo Cổ V – FWKS25SS02G.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Hồng', '100% Cotton', TRUE, TRUE, 'active', '{"title": "Áo Polo Cổ V", "description": "Áo polo nữ thanh lịch"}'),
('FL-PL-002', 'Áo Polo Nữ Cổ Lưới', 'ao-polo-nu-co-luoi', 'Áo polo nữ cổ lưới thiết kế độc đáo, thoáng mát và thoải mái.', 'Áo polo nữ cổ lưới thoáng mát', 21, 2, 680000, NULL, 35, '/images/Áo nữ/Áo Polo Nữ Cổ Lưới FWKS25SS11G.jpg', ARRAY['/images/Áo nữ/Áo Polo Nữ Cổ Lưới FWKS25SS11G.jpg'], 'S,M,L', 'Trắng,Xanh,Hồng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Polo Nữ Cổ Lưới", "description": "Áo polo nữ thoáng mát"}'),

-- Áo Sơ Mi Nữ (Category 20 - Áo Sơ Mi Nữ)
('FL-SM-001', 'Áo Sơ Mi Cổ Trụ Tay Ngắn', 'ao-so-mi-co-tru-tay-ngan', 'Áo sơ mi nữ cổ trụ tay ngắn, thiết kế lịch sự cho công sở.', 'Áo sơ mi nữ cổ trụ lịch sự', 20, 2, 720000, 648000, 40, '/images/Áo nữ/Áo Sơ Mi Cổ Trụ Tay Ngắn FWWS24FH03C.jpg', ARRAY['/images/Áo nữ/Áo Sơ Mi Cổ Trụ Tay Ngắn FWWS24FH03C.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Cổ Trụ", "description": "Áo sơ mi nữ lịch sự"}'),
('FL-SM-002', 'Áo Sơ Mi Freelancer FWWS25SS06G', 'ao-so-mi-freelancer-fwws25ss06g', 'Áo sơ mi nữ Freelancer thiết kế hiện đại và trẻ trung.', 'Áo sơ mi nữ hiện đại trẻ trung', 20, 2, 740000, NULL, 35, '/images/Áo nữ/Áo Sơ Mi Freelancer – FWWS25SS06G.jpg', ARRAY['/images/Áo nữ/Áo Sơ Mi Freelancer – FWWS25SS06G.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Freelancer", "description": "Áo sơ mi nữ hiện đại"}'),
('FL-SM-003', 'Áo Sơ Mi Freelancer FWWS25SS04C', 'ao-so-mi-freelancer-fwws25ss04c', 'Áo sơ mi nữ Freelancer phong cách thanh lịch và chuyên nghiệp.', 'Áo sơ mi nữ thanh lịch chuyên nghiệp', 20, 2, 760000, 684000, 38, '/images/Áo nữ/Áo Sơ Mi Freelancer FWWS25SS04C.jpg', ARRAY['/images/Áo nữ/Áo Sơ Mi Freelancer FWWS25SS04C.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Áo Sơ Mi Chuyên Nghiệp", "description": "Áo sơ mi nữ thanh lịch"}'),
('FL-SM-004', 'Áo Sơ Mi Nữ Tay Ngắn', 'ao-so-mi-nu-tay-ngan', 'Áo sơ mi nữ tay ngắn phong cách hiện đại, phù hợp cho nhiều dịp.', 'Áo sơ mi nữ tay ngắn hiện đại', 20, 2, 890000, 801000, 25, '/images/Áo nữ/Áo Sơ Mi Nữ Tay Ngắn FWSH25SS03C.jpg', ARRAY['/images/Áo nữ/Áo Sơ Mi Nữ Tay Ngắn FWSH25SS03C.jpg'], 'S,M,L,XL', 'Trắng,Xanh,Be', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Áo Sơ Mi Nữ Tay Ngắn", "description": "Áo sơ mi nữ hiện đại"}'),

-- Áo T-Shirt Nữ (Category 19 - Áo T-Shirt Nữ)
('FL-TS-001', 'Áo Thun Nữ Basic Form Rộng', 'ao-thun-nu-basic-form-rong', 'Áo thun nữ basic form rộng, thoải mái và dễ phối đồ.', 'Áo thun nữ basic thoải mái', 19, 2, 450000, NULL, 60, '/images/Áo nữ/Áo Thun Nữ Basic Form Rộng FWTS25SS04C.jpg', ARRAY['/images/Áo nữ/Áo Thun Nữ Basic Form Rộng FWTS25SS04C.jpg'], 'S,M,L,XL,XXL', 'Trắng,Đen,Xám,Hồng', '100% Cotton', TRUE, FALSE, 'active', '{"title": "Áo Thun Nữ Basic", "description": "Áo thun nữ basic form rộng"}'),

-- ========================================
-- CHÂN VÁY NỮ (FREELANCER) - Category 26 - Chân váy
-- ========================================
('FL-SK-001', 'Chân Váy A Xếp Ly Màu Be', 'chan-vay-a-xep-ly-mau-be', 'Chân váy A xếp ly màu be, thiết kế thanh lịch và nữ tính.', 'Chân váy A xếp ly thanh lịch', 26, 2, 650000, 585000, 40, '/images/Chân váy nữ/Chân Váy A Xếp Ly Màu Be FWSK24FH15C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy A Xếp Ly Màu Be FWSK24FH15C.jpg'], 'S,M,L,XL', 'Be,Đen,Navy', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Chân Váy A Xếp Ly", "description": "Chân váy A thanh lịch"}'),
('FL-SK-002', 'Chân Váy Chữ A Thanh Lịch', 'chan-vay-chu-a-thanh-lich', 'Chân váy chữ A thiết kế thanh lịch, phù hợp cho công sở và dạo phố.', 'Chân váy chữ A thanh lịch', 26, 2, 580000, NULL, 45, '/images/Chân váy nữ/Chân Váy Chữ A Thanh Lịch FWSK24SS01C-O.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Chữ A Thanh Lịch FWSK24SS01C-O.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Chữ A", "description": "Chân váy thanh lịch"}'),
('FL-SK-003', 'Chân Váy Dài FWSK25SS09C', 'chan-vay-dai-fwsk25ss09c', 'Chân váy dài thiết kế hiện đại, tạo vẻ đẹp quyến rũ và sang trọng.', 'Chân váy dài hiện đại sang trọng', 26, 2, 720000, 648000, 35, '/images/Chân váy nữ/Chân Váy Dài FWSK25SS09C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Dài FWSK25SS09C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Chân Váy Dài", "description": "Chân váy dài sang trọng"}'),
('FL-SK-004', 'Chân Váy FWSK25FH04G', 'chan-vay-fwsk25fh04g', 'Chân váy thiết kế thời trang, phong cách trẻ trung và năng động.', 'Chân váy thời trang trẻ trung', 26, 2, 590000, 531000, 42, '/images/Chân váy nữ/Chân Váy FWSK25FH04G.jpg', ARRAY['/images/Chân váy nữ/Chân Váy FWSK25FH04G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Thời Trang", "description": "Chân váy trẻ trung"}'),
('FL-SK-005', 'Chân Váy Freelancer FWSK25SS02G', 'chan-vay-freelancer-fwsk25ss02g', 'Chân váy Freelancer thiết kế hiện đại, chất liệu cao cấp.', 'Chân váy Freelancer hiện đại', 26, 2, 620000, NULL, 38, '/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS02G.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS02G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Freelancer", "description": "Chân váy hiện đại"}'),
('FL-SK-006', 'Chân Váy Freelancer FWSK25SS04G', 'chan-vay-freelancer-fwsk25ss04g', 'Chân váy Freelancer phong cách thanh lịch và chuyên nghiệp.', 'Chân váy thanh lịch chuyên nghiệp', 26, 2, 640000, 576000, 40, '/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS04G.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS04G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Chuyên Nghiệp", "description": "Chân váy thanh lịch"}'),
('FL-SK-007', 'Chân Váy Freelancer FWSK25SS07C', 'chan-vay-freelancer-fwsk25ss07c', 'Chân váy Freelancer thiết kế sang trọng, phù hợp cho nhiều dịp.', 'Chân váy sang trọng đa dụng', 26, 2, 680000, NULL, 36, '/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS07C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Freelancer – FWSK25SS07C.jpg'], 'S,M,L,XL', 'Trắng,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Sang Trọng", "description": "Chân váy đa dụng"}'),
('FL-SK-008', 'Chân Váy Jean Xẻ Lai Sành Điệu', 'chan-vay-jean-xe-lai-sanh-dieu', 'Chân váy jean xẻ lai thiết kế sành điệu, phong cách cá tính.', 'Chân váy jean sành điệu cá tính', 26, 2, 750000, 675000, 30, '/images/Chân váy nữ/Chân Váy Jean Xẻ Lai Sành Điệu FWSK24SS07G-J.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Jean Xẻ Lai Sành Điệu FWSK24SS07G-J.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, TRUE, 'active', '{"title": "Chân Váy Jean Xẻ Lai", "description": "Chân váy jean sành điệu"}'),
('FL-SK-009', 'Chân Váy Jeans Cá Tính FWSK23FH06G', 'chan-vay-jeans-ca-tinh-fwsk23fh06g', 'Chân váy jeans cá tính, thiết kế trẻ trung và năng động.', 'Chân váy jeans cá tính trẻ trung', 26, 2, 690000, 621000, 32, '/images/Chân váy nữ/Chân Váy Jeans Cá Tính FWSK23FH06G.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Jeans Cá Tính FWSK23FH06G.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Chân Váy Jeans Cá Tính", "description": "Chân váy jeans trẻ trung"}'),
('FL-SK-010', 'Chân Váy Jeans Cá Tính FWSK24SS05G', 'chan-vay-jeans-ca-tinh-fwsk24ss05g', 'Chân váy jeans cá tính mùa hè, thoải mái và thời trang.', 'Chân váy jeans mùa hè thời trang', 26, 2, 710000, NULL, 28, '/images/Chân váy nữ/Chân Váy Jeans Cá Tính FWSK24SS05G-J.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Jeans Cá Tính FWSK24SS05G-J.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Chân Váy Jeans Mùa Hè", "description": "Chân váy jeans thời trang"}'),
('FL-SK-011', 'Chân Váy Jeans Sành Điệu', 'chan-vay-jeans-sanh-dieu', 'Chân váy jeans sành điệu, phong cách hiện đại và cá tính.', 'Chân váy jeans hiện đại cá tính', 26, 2, 720000, 648000, 35, '/images/Chân váy nữ/Chân Váy Jeans Sành Điệu FWSK24FH02G-J.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Jeans Sành Điệu FWSK24FH02G-J.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Chân Váy Jeans Sành Điệu", "description": "Chân váy jeans hiện đại"}'),
('FL-SK-012', 'Chân Váy Lưng Thun', 'chan-vay-lung-thun', 'Chân váy lưng thun thiết kế thoải mái, dễ mặc và dễ phối đồ.', 'Chân váy lưng thun thoải mái', 26, 2, 580000, 522000, 50, '/images/Chân váy nữ/Chân Váy Lưng Thun FWSK24FH03G-J.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Lưng Thun FWSK24FH03G-J.jpg'], 'S,M,L,XL,XXL', 'Xanh,Đen,Navy', 'Denim blend', TRUE, TRUE, 'active', '{"title": "Chân Váy Lưng Thun", "description": "Chân váy thoải mái"}'),
('FL-SK-013', 'Chân Váy Ngắn Bất Đối Xứng', 'chan-vay-ngan-bat-doi-xung', 'Chân váy ngắn bất đối xứng thiết kế độc đáo, phong cách trẻ trung.', 'Chân váy ngắn độc đáo trẻ trung', 26, 2, 650000, NULL, 38, '/images/Chân váy nữ/Chân Váy Ngắn Bất Đối Xứng FWSK25SS12C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Ngắn Bất Đối Xứng FWSK25SS12C.jpg'], 'S,M,L,XL', 'Trắng,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Bất Đối Xứng", "description": "Chân váy độc đáo"}'),
('FL-SK-014', 'Chân Váy Nữ FWSK24FH05C', 'chan-vay-nu-fwsk24fh05c', 'Chân váy nữ thiết kế hiện đại, phù hợp cho nhiều dịp khác nhau.', 'Chân váy nữ hiện đại đa dụng', 26, 2, 620000, 558000, 42, '/images/Chân váy nữ/Chân Váy Nữ FWSK24FH05C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Nữ FWSK24FH05C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Nữ Hiện Đại", "description": "Chân váy đa dụng"}'),
('FL-SK-015', 'Chân Váy Nữ Thanh Lịch', 'chan-vay-nu-thanh-lich', 'Chân váy nữ thanh lịch, thiết kế sang trọng cho công sở.', 'Chân váy thanh lịch công sở', 26, 2, 680000, NULL, 40, '/images/Chân váy nữ/Chân Váy Nữ Thanh Lịch FWSK24FH01C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Nữ Thanh Lịch FWSK24FH01C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Thanh Lịch", "description": "Chân váy công sở"}'),
('FL-SK-016', 'Chân Váy Ôm Xẻ Lai', 'chan-vay-om-xe-lai', 'Chân váy ôm xẻ lai thiết kế quyến rũ, tôn dáng hiệu quả.', 'Chân váy ôm quyến rũ tôn dáng', 26, 2, 750000, 675000, 30, '/images/Chân váy nữ/Chân Váy Ôm Xẻ Lai FWSK24SS02C-O.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Ôm Xẻ Lai FWSK24SS02C-O.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Chân Váy Ôm Xẻ Lai", "description": "Chân váy quyến rũ"}'),
('FL-SK-017', 'Chân Váy Thanh Lịch FWSK24FH04C', 'chan-vay-thanh-lich-fwsk24fh04c', 'Chân váy thanh lịch thiết kế tinh tế, phù hợp cho môi trường công sở.', 'Chân váy thanh lịch tinh tế', 26, 2, 640000, 576000, 45, '/images/Chân váy nữ/Chân Váy Thanh Lịch FWSK24FH04C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Thanh Lịch FWSK24FH04C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Tinh Tế", "description": "Chân váy thanh lịch"}'),
('FL-SK-018', 'Chân Váy Thiết Kế', 'chan-vay-thiet-ke', 'Chân váy thiết kế độc đáo, phong cách thời trang hiện đại.', 'Chân váy thiết kế thời trang', 26, 2, 720000, NULL, 35, '/images/Chân váy nữ/Chân Váy Thiết Kế FWSK24FH13C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Thiết Kế FWSK24FH13C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Chân Váy Thiết Kế", "description": "Chân váy thời trang"}'),
('FL-SK-019', 'Chân Váy Vạt Chéo Thanh Lịch', 'chan-vay-vat-cheo-thanh-lich', 'Chân váy vạt chéo thanh lịch, thiết kế sang trọng và quyến rũ.', 'Chân váy vạt chéo sang trọng', 26, 2, 780000, 702000, 28, '/images/Chân váy nữ/Chân Váy Vạt Chéo Thanh Lịch FWSK23FH02C.jpg', ARRAY['/images/Chân váy nữ/Chân Váy Vạt Chéo Thanh Lịch FWSK23FH02C.jpg'], 'S,M,L,XL', 'Đen,Navy,Be', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Chân Váy Vạt Chéo", "description": "Chân váy sang trọng"}'),
('FL-SK-020', 'Mini Skirt Basic Màu Beige', 'mini-skirt-basic-mau-beige', 'Mini skirt basic màu beige, thiết kế đơn giản và dễ phối đồ.', 'Mini skirt basic dễ phối đồ', 26, 2, 520000, 468000, 55, '/images/Chân váy nữ/Mini Skirt Basic Màu Beige FWSK25FH12C.jpg', ARRAY['/images/Chân váy nữ/Mini Skirt Basic Màu Beige FWSK25FH12C.jpg'], 'S,M,L,XL', 'Be,Đen,Navy', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Mini Skirt Basic", "description": "Mini skirt dễ phối"}'),

-- ========================================
-- ĐẦM NỮ (FREELANCER) - Category 9 - Đầm Nữ  
-- ========================================
('FL-DR-001', 'Đầm Cách Điệu Thời Trang', 'dam-cach-dieu-thoi-trang', 'Đầm cách điệu thời trang, thiết kế hiện đại và nữ tính.', 'Đầm cách điệu hiện đại nữ tính', 9, 2, 980000, 882000, 25, '/images/Đầm nữ/Đầm Cách Điệu Thời Trang FWDR24SS30G.jpg', ARRAY['/images/Đầm nữ/Đầm Cách Điệu Thời Trang FWDR24SS30G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Đầm Cách Điệu Thời Trang", "description": "Đầm hiện đại nữ tính"}'),
('FL-DR-002', 'Đầm Cổ Vest Thanh Lịch', 'dam-co-vest-thanh-lich', 'Đầm cổ vest thanh lịch, phong cách chuyên nghiệp và sang trọng.', 'Đầm cổ vest chuyên nghiệp', 9, 2, 1150000, NULL, 20, '/images/Đầm nữ/Đầm Cổ Vest Thanh Lịch FWDR24SS21C.jpg', ARRAY['/images/Đầm nữ/Đầm Cổ Vest Thanh Lịch FWDR24SS21C.jpg'], 'S,M,L,XL', 'Trắng,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Cổ Vest", "description": "Đầm chuyên nghiệp"}'),
('FL-DR-003', 'Đầm Dáng Suông Cổ Polo', 'dam-dang-suong-co-polo', 'Đầm dáng suông cổ polo, thiết kế thoải mái và trẻ trung.', 'Đầm suông thoải mái trẻ trung', 9, 2, 850000, 765000, 35, '/images/Đầm nữ/Đầm Dáng Suông Cổ Polo FWDR25SS26G.jpg', ARRAY['/images/Đầm nữ/Đầm Dáng Suông Cổ Polo FWDR25SS26G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Dáng Suông Polo", "description": "Đầm thoải mái"}'),
('FL-DR-004', 'Đầm Denim Sát Nách Cổ Sơ Mi', 'dam-denim-sat-nach-co-so-mi', 'Đầm denim sát nách cổ sơ mi, phong cách cá tính và hiện đại.', 'Đầm denim cá tính hiện đại', 9, 2, 920000, 828000, 30, '/images/Đầm nữ/Đầm Denim Sát Nách Cổ Sơ Mi FWDR25SS32G.jpg', ARRAY['/images/Đầm nữ/Đầm Denim Sát Nách Cổ Sơ Mi FWDR25SS32G.jpg'], 'S,M,L,XL', 'Xanh,Đen', '100% Denim', TRUE, TRUE, 'active', '{"title": "Đầm Denim Sát Nách", "description": "Đầm denim cá tính"}'),
('FL-DR-005', 'Đầm Nữ Cổ Vest Thanh Lịch', 'dam-nu-co-vest-thanh-lich', 'Đầm nữ cổ vest thanh lịch, thiết kế sang trọng cho công sở.', 'Đầm vest sang trọng công sở', 9, 2, 1200000, NULL, 22, '/images/Đầm nữ/Đầm Nữ Cổ Vest Thanh Lịch FWDR24FH03C.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Cổ Vest Thanh Lịch FWDR24FH03C.jpg'], 'S,M,L,XL', 'Trắng,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Vest Thanh Lịch", "description": "Đầm sang trọng"}'),
('FL-DR-006', 'Đầm Nữ Dáng Dài Gài Nút', 'dam-nu-dang-dai-gai-nut', 'Đầm nữ dáng dài gài nút, thiết kế thanh lịch và quyến rũ.', 'Đầm dáng dài thanh lịch', 9, 2, 1080000, 972000, 25, '/images/Đầm nữ/Đầm Nữ Dáng Dài Gài Nút FWDR25SS014G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Dáng Dài Gài Nút FWDR25SS014G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Đầm Dáng Dài Gài Nút", "description": "Đầm thanh lịch"}'),
('FL-DR-007', 'Đầm Nữ Dáng Xòe Cổ Trụ Thanh Lịch', 'dam-nu-dang-xoe-co-tru-thanh-lich', 'Đầm nữ dáng xòe cổ trụ thanh lịch, tôn dáng và nữ tính.', 'Đầm xòe tôn dáng nữ tính', 9, 2, 950000, 855000, 28, '/images/Đầm nữ/Đầm Nữ Dáng Xòe Cổ Trụ Thanh Lịch FWDR25SS010G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Dáng Xòe Cổ Trụ Thanh Lịch FWDR25SS010G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Xòe Cổ Trụ", "description": "Đầm tôn dáng"}'),
('FL-DR-008', 'Đầm Nữ Dáng Xòe Cổ V Phối Dây', 'dam-nu-dang-xoe-co-v-phoi-day', 'Đầm nữ dáng xòe cổ V phối dây, thiết kế quyến rũ và hiện đại.', 'Đầm xòe cổ V quyến rũ', 9, 2, 880000, NULL, 32, '/images/Đầm nữ/Đầm Nữ Dáng Xòe Cổ V Phối Dây FWDR25SS04G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Dáng Xòe Cổ V Phối Dây FWDR25SS04G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Đầm Xòe Cổ V", "description": "Đầm quyến rũ"}'),
('FL-DR-009', 'Đầm Nữ Họa Tiết', 'dam-nu-hoa-tiet', 'Đầm nữ họa tiết, thiết kế độc đáo và bắt mắt.', 'Đầm họa tiết độc đáo bắt mắt', 9, 2, 820000, 738000, 38, '/images/Đầm nữ/Đầm Nữ Họa Tiết FWDR25SS25G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Họa Tiết FWDR25SS25G.jpg'], 'S,M,L,XL', 'Đa màu', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Đầm Nữ Họa Tiết", "description": "Đầm độc đáo"}'),
('FL-DR-010', 'Đầm Nữ Họa Tiết Hoa Nhí', 'dam-nu-hoa-tiet-hoa-nhi', 'Đầm nữ họa tiết hoa nhí, phong cách ngọt ngào và nữ tính.', 'Đầm hoa nhí ngọt ngào nữ tính', 9, 2, 780000, 702000, 40, '/images/Đầm nữ/Đầm Nữ Họa Tiết Hoa Nhí FWDR24FH04G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Họa Tiết Hoa Nhí FWDR24FH04G.jpg'], 'S,M,L,XL', 'Hồng,Xanh,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Hoa Nhí", "description": "Đầm ngọt ngào"}'),
('FL-DR-011', 'Đầm Nữ Thanh Lịch FWDR24FH10C', 'dam-nu-thanh-lich-fwdr24fh10c', 'Đầm nữ thanh lịch, thiết kế tinh tế cho công sở.', 'Đầm thanh lịch tinh tế', 9, 2, 1050000, NULL, 26, '/images/Đầm nữ/Đầm Nữ Thanh Lịch FWDR24FH10C.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Thanh Lịch FWDR24FH10C.jpg'], 'S,M,L,XL', 'Trắng,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Đầm Thanh Lịch", "description": "Đầm tinh tế"}'),
('FL-DR-012', 'Đầm Nữ Thời Trang FWDR24FH23G', 'dam-nu-thoi-trang-fwdr24fh23g', 'Đầm nữ thời trang, thiết kế hiện đại và trẻ trung.', 'Đầm thời trang hiện đại', 9, 2, 890000, 801000, 35, '/images/Đầm nữ/Đầm Nữ Thời Trang FWDR24FH23G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Thời Trang FWDR24FH23G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Đầm Thời Trang", "description": "Đầm hiện đại"}'),
('FL-DR-013', 'Đầm Nữ Thời Trang FWDR25SS08G', 'dam-nu-thoi-trang-fwdr25ss08g', 'Đầm nữ thời trang mùa hè, thoáng mát và phong cách.', 'Đầm thời trang mùa hè', 9, 2, 850000, 765000, 42, '/images/Đầm nữ/Đầm Nữ Thời Trang FWDR25SS08G.jpg', ARRAY['/images/Đầm nữ/Đầm Nữ Thời Trang FWDR25SS08G.jpg'], 'S,M,L,XL', 'Xanh,Đen,Navy', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Đầm Mùa Hè", "description": "Đầm thoáng mát"}'),

-- ========================================
-- QUẦN NAM (JOHN HENRY) - CATEGORY IDs: 13-16
-- ========================================

-- Quần Jeans Nam (Category 13 - Quần Jean Nam)
('JH-JN-001', 'Quần Jeans Nam Form Ôm', 'quan-jeans-nam-form-om', 'Quần jeans nam form ôm, thiết kế hiện đại và tôn dáng.', 'Quần jeans nam form ôm tôn dáng', 13, 1, 890000, 801000, 40, '/images/Quần nam/Quần Jeans Nam Form Ôm JN25FH39C-SL.jpg', ARRAY['/images/Quần nam/Quần Jeans Nam Form Ôm JN25FH39C-SL.jpg'], '29,30,31,32,33,34', 'Xanh,Đen', '100% Denim', TRUE, TRUE, 'active', '{"title": "Quần Jeans Form Ôm", "description": "Quần jeans tôn dáng"}'),
('JH-JN-002', 'Quần Jeans Nam Form Ôm JN25FH47T', 'quan-jeans-nam-form-om-jn25fh47t', 'Quần jeans nam form ôm thiết kế thanh lịch, phù hợp cho nhiều dịp.', 'Quần jeans form ôm thanh lịch', 13, 1, 920000, NULL, 35, '/images/Quần nam/Quần Jeans Nam Form Ôm JN25FH47T-SL.jpg', ARRAY['/images/Quần nam/Quần Jeans Nam Form Ôm JN25FH47T-SL.jpg'], '29,30,31,32,33,34', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Quần Jeans Thanh Lịch", "description": "Quần jeans form ôm"}'),
('JH-JN-003', 'Quần Jeans Nam Form Rộng', 'quan-jeans-nam-form-rong', 'Quần jeans nam form rộng, phong cách thoải mái và năng động.', 'Quần jeans form rộng thoải mái', 13, 1, 850000, 765000, 45, '/images/Quần nam/Quần Jeans Nam Form Rộng JN25FH40T-CL.jpg', ARRAY['/images/Quần nam/Quần Jeans Nam Form Rộng JN25FH40T-CL.jpg'], '29,30,31,32,33,34,36', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Quần Jeans Form Rộng", "description": "Quần jeans thoải mái"}'),
('JH-JN-004', 'Quần Jeans Nam Form Vừa JN25FH38C', 'quan-jeans-nam-form-vua-jn25fh38c', 'Quần jeans nam form vừa, thiết kế cân đối và lịch lãm.', 'Quần jeans form vừa cân đối', 13, 1, 880000, NULL, 50, '/images/Quần nam/Quần Jeans Nam Form Vừa JN25FH38C-RG.jpg', ARRAY['/images/Quần nam/Quần Jeans Nam Form Vừa JN25FH38C-RG.jpg'], '29,30,31,32,33,34', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Quần Jeans Form Vừa", "description": "Quần jeans cân đối"}'),
('JH-JN-005', 'Quần Jeans Nam Form Vừa JN25FH41P', 'quan-jeans-nam-form-vua-jn25fh41p', 'Quần jeans nam form vừa, phong cách hiện đại và trẻ trung.', 'Quần jeans hiện đại trẻ trung', 13, 1, 900000, 810000, 42, '/images/Quần nam/Quần Jeans Nam Form Vừa JN25FH41P-RG.jpg', ARRAY['/images/Quần nam/Quần Jeans Nam Form Vừa JN25FH41P-RG.jpg'], '29,30,31,32,33,34', 'Xanh,Đen', '100% Denim', TRUE, FALSE, 'active', '{"title": "Quần Jeans Hiện Đại", "description": "Quần jeans trẻ trung"}'),

-- Quần Khaki Nam (Category 14 - Quần Tây Nam)
('JH-KP-001', 'Quần Khaki Nam Form Ôm', 'quan-khaki-nam-form-om', 'Quần khaki nam form ôm, thiết kế lịch lãm cho công sở.', 'Quần khaki form ôm lịch lãm', 14, 1, 750000, 675000, 45, '/images/Quần nam/Quần Khaki Nam Form Ôm KP25FH16T-NMSL.jpg', ARRAY['/images/Quần nam/Quần Khaki Nam Form Ôm KP25FH16T-NMSL.jpg'], '29,30,31,32,33,34', 'Be,Đen,Navy', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Quần Khaki Form Ôm", "description": "Quần khaki lịch lãm"}'),
('JH-KP-002', 'Quần Khaki Nam Form Ôm KP25SS12C', 'quan-khaki-nam-form-om-kp25ss12c', 'Quần khaki nam form ôm mùa hè, thoáng mát và thoải mái.', 'Quần khaki mùa hè thoáng mát', 14, 1, 720000, NULL, 50, '/images/Quần nam/Quần Khaki Nam Form Ôm KP25SS12C-JNSL.jpg', ARRAY['/images/Quần nam/Quần Khaki Nam Form Ôm KP25SS12C-JNSL.jpg'], '29,30,31,32,33,34', 'Be,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Quần Khaki Mùa Hè", "description": "Quần khaki thoáng mát"}'),
('JH-KP-003', 'Quần Khaki Nam Form Vừa KP25FH18C', 'quan-khaki-nam-form-vua-kp25fh18c', 'Quần khaki nam form vừa, thiết kế thanh lịch và chuyên nghiệp.', 'Quần khaki thanh lịch chuyên nghiệp', 14, 1, 780000, 702000, 40, '/images/Quần nam/Quần Khaki Nam Form Vừa KP25FH18C-NMSC.jpg', ARRAY['/images/Quần nam/Quần Khaki Nam Form Vừa KP25FH18C-NMSC.jpg'], '29,30,31,32,33,34', 'Be,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Quần Khaki Chuyên Nghiệp", "description": "Quần khaki thanh lịch"}'),

-- Quần Short Nam (Category 15 - Quần Short Nam)
('JH-SP-001', 'Quần Short Nam Form Trên Gối', 'quan-short-nam-form-tren-goi', 'Quần short nam form trên gối, thiết kế năng động và thoải mái.', 'Quần short năng động thoải mái', 15, 1, 520000, 468000, 60, '/images/Quần nam/Quần Short Nam Form Trên Gối SP25FH23P-AK.jpg', ARRAY['/images/Quần nam/Quần Short Nam Form Trên Gối SP25FH23P-AK.jpg'], '29,30,31,32,33,34', 'Be,Đen,Navy', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Quần Short Form Trên Gối", "description": "Quần short năng động"}'),
('JH-SP-002', 'Quần Short Nam Năng Động', 'quan-short-nam-nang-dong', 'Quần short nam năng động, phù hợp cho thể thao và dạo phố.', 'Quần short thể thao dạo phố', 15, 1, 480000, NULL, 65, '/images/Quần nam/Quần Short Nam Năng Động SP25FH32P-AK.jpg', ARRAY['/images/Quần nam/Quần Short Nam Năng Động SP25FH32P-AK.jpg'], '29,30,31,32,33,34', 'Be,Đen,Navy', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Quần Short Năng Động", "description": "Quần short thể thao"}'),

-- ========================================
-- UNIFORM (ĐỒNG PHỤC) - Category 3 - Phụ kiện  
-- ========================================
('UF-BU-001', 'Business Uniform', 'business-uniform', 'Đồng phục doanh nghiệp, thiết kế chuyên nghiệp và lịch sự.', 'Đồng phục doanh nghiệp chuyên nghiệp', 3, 1, 1500000, 1350000, 20, '/images/Uniform/Business Uniform.jpg', ARRAY['/images/Uniform/Business Uniform.jpg'], 'S,M,L,XL,XXL', 'Đen,Navy,Xám', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Business Uniform", "description": "Đồng phục doanh nghiệp"}'),
('UF-BS-001', 'Beauty Salon Uniform', 'beauty-salon-uniform', 'Đồng phục salon làm đẹp, thiết kế tiện dụng và thời trang.', 'Đồng phục salon tiện dụng', 3, 1, 850000, 765000, 30, '/images/Uniform/beauty-salon-uniform.jpg', ARRAY['/images/Uniform/beauty-salon-uniform.jpg'], 'S,M,L,XL,XXL', 'Trắng,Hồng,Xanh', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Beauty Salon Uniform", "description": "Đồng phục salon"}'),
('UF-BL-001', 'Blazer Uniform', 'blazer-uniform', 'Đồng phục blazer, thiết kế sang trọng và chuyên nghiệp.', 'Đồng phục blazer sang trọng', 3, 1, 1200000, NULL, 25, '/images/Uniform/blazer-uniform.jpg', ARRAY['/images/Uniform/blazer-uniform.jpg'], 'S,M,L,XL,XXL', 'Đen,Navy,Xám', 'Wool blend', TRUE, FALSE, 'active', '{"title": "Blazer Uniform", "description": "Đồng phục blazer"}'),
('UF-EV-001', 'Events Uniform', 'events-uniform', 'Đồng phục sự kiện, thiết kế nổi bật và thu hút.', 'Đồng phục sự kiện nổi bật', 3, 1, 950000, 855000, 35, '/images/Uniform/events-uniform.jpg', ARRAY['/images/Uniform/events-uniform.jpg'], 'S,M,L,XL,XXL', 'Đỏ,Xanh,Đen', 'Polyester blend', TRUE, TRUE, 'active', '{"title": "Events Uniform", "description": "Đồng phục sự kiện"}'),
('UF-GR-001', 'Group Uniform', 'group-uniform', 'Đồng phục nhóm, thiết kế đồng bộ và chuyên nghiệp.', 'Đồng phục nhóm đồng bộ', 3, 1, 750000, 675000, 50, '/images/Uniform/group-uniform.jpg', ARRAY['/images/Uniform/group-uniform.jpg'], 'S,M,L,XL,XXL', 'Đen,Navy,Xám', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Group Uniform", "description": "Đồng phục nhóm"}'),
('UF-JK-001', 'Jacket Uniform', 'jacket-uniform', 'Đồng phục áo khoác, thiết kế ấm áp và thời trang.', 'Đồng phục áo khoác ấm áp', 3, 1, 1100000, NULL, 28, '/images/Uniform/jacket-uniform.jpg', ARRAY['/images/Uniform/jacket-uniform.jpg'], 'S,M,L,XL,XXL', 'Đen,Navy,Xám', 'Polyester blend', TRUE, FALSE, 'active', '{"title": "Jacket Uniform", "description": "Đồng phục áo khoác"}'),
('UF-SF-001', 'Safety Uniform', 'safety-uniform', 'Đồng phục an toàn, thiết kế bảo vệ và tiện dụng.', 'Đồng phục an toàn bảo vệ', 3, 1, 650000, 585000, 40, '/images/Uniform/safety-uniform.jpg', ARRAY['/images/Uniform/safety-uniform.jpg'], 'S,M,L,XL,XXL', 'Cam,Vàng,Xanh', 'Cotton blend', TRUE, TRUE, 'active', '{"title": "Safety Uniform", "description": "Đồng phục an toàn"}'),
('UF-SR-001', 'Showroom Uniform', 'showroom-uniform', 'Đồng phục showroom, thiết kế thanh lịch và chuyên nghiệp.', 'Đồng phục showroom thanh lịch', 3, 1, 880000, NULL, 32, '/images/Uniform/showroom-uniform.jpg', ARRAY['/images/Uniform/showroom-uniform.jpg'], 'S,M,L,XL,XXL', 'Đen,Navy,Trắng', 'Cotton blend', TRUE, FALSE, 'active', '{"title": "Showroom Uniform", "description": "Đồng phục showroom"}');



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
