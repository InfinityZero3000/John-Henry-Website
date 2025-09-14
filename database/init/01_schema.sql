-- ========================================
-- JOHN HENRY E-COMMERCE DATABASE SCHEMA
-- PostgreSQL Version
-- ========================================

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ========================================
-- USERS & AUTHENTICATION TABLES
-- ========================================

-- Users table (extended for ASP.NET Core Identity compatibility)
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    user_name VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    date_of_birth DATE,
    gender VARCHAR(20),
    avatar VARCHAR(500),
    role VARCHAR(50) DEFAULT 'Customer' CHECK (role IN ('Customer', 'Seller', 'Admin')),
    is_active BOOLEAN DEFAULT TRUE,
    
    -- Seller specific fields
    company_name VARCHAR(255),
    business_license VARCHAR(100),
    tax_code VARCHAR(50),
    address TEXT,
    phone VARCHAR(20),
    is_approved BOOLEAN DEFAULT FALSE,
    approved_at TIMESTAMP WITH TIME ZONE,
    approved_by VARCHAR(450),
    notes TEXT,
    
    -- Timestamps
    last_login_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- User addresses (simplified for Vietnam market)
CREATE TABLE addresses (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    type VARCHAR(50) DEFAULT 'shipping' CHECK (type IN ('shipping', 'billing', 'both')),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    company VARCHAR(255),
    address_line_1 VARCHAR(255) NOT NULL,
    address_line_2 VARCHAR(255),
    city VARCHAR(100) NOT NULL,
    state VARCHAR(100) NOT NULL, -- Province/State
    postal_code VARCHAR(20),
    country VARCHAR(100) DEFAULT 'Vietnam',
    phone VARCHAR(20),
    is_default BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- PRODUCT CATALOG TABLES
-- ========================================

-- Categories (simplified SEO fields)
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    description TEXT,
    parent_category_id INTEGER REFERENCES categories(id),
    image_url VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    sort_order INTEGER DEFAULT 0,
    seo_metadata JSONB, -- Combined SEO fields
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Products (simplified and aligned with models)
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    sku VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    description TEXT,
    short_description VARCHAR(500),
    category_id INTEGER NOT NULL REFERENCES categories(id),
    brand_id INTEGER REFERENCES brands(id),
    price DECIMAL(12,2) NOT NULL,
    sale_price DECIMAL(12,2), -- Compare price for sales
    stock_quantity INTEGER DEFAULT 0,
    manage_stock BOOLEAN DEFAULT TRUE,
    in_stock BOOLEAN DEFAULT TRUE,
    featured_image_url VARCHAR(500),
    gallery_images TEXT[], -- Array of image URLs
    size VARCHAR(100),
    color VARCHAR(100),
    material VARCHAR(255),
    weight DECIMAL(8,2), -- in grams
    dimensions VARCHAR(100),
    is_featured BOOLEAN DEFAULT FALSE,
    is_active BOOLEAN DEFAULT TRUE,
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'out_of_stock')),
    view_count INTEGER DEFAULT 0,
    rating DECIMAL(3,2), -- Average rating
    review_count INTEGER DEFAULT 0,
    seo_metadata JSONB, -- Combined SEO fields
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Product images
CREATE TABLE product_images (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    image_url VARCHAR(500) NOT NULL,
    alt_text VARCHAR(255),
    is_primary BOOLEAN DEFAULT FALSE,
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Remove complex product variants system for simplicity
-- The system uses simple size/color fields in products table instead

-- ========================================
-- SHOPPING CART & WISHLIST TABLES
-- ========================================

-- Shopping carts (updated to match models)
CREATE TABLE shopping_carts (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL, -- ASP.NET Identity UserId
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    quantity INTEGER NOT NULL DEFAULT 1 CHECK (quantity > 0),
    size VARCHAR(100),
    color VARCHAR(100),
    price DECIMAL(12,2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Wishlists (updated to match models)
CREATE TABLE wishlists (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL, -- ASP.NET Identity UserId
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    UNIQUE(user_id, product_id)
);

-- ========================================
-- ORDER MANAGEMENT TABLES
-- ========================================

-- Orders (updated to match models)
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    order_number VARCHAR(50) UNIQUE NOT NULL,
    user_id VARCHAR(450), -- ASP.NET Identity UserId
    email VARCHAR(255) NOT NULL,
    phone_number VARCHAR(20),
    
    -- Shipping address (simplified)
    shipping_address TEXT NOT NULL,
    
    -- Billing address (simplified)
    billing_address TEXT NOT NULL,
    
    -- Pricing
    total_amount DECIMAL(12,2) NOT NULL,
    shipping_fee DECIMAL(12,2) DEFAULT 0,
    tax DECIMAL(12,2) DEFAULT 0,
    discount_amount DECIMAL(12,2) DEFAULT 0,
    coupon_code VARCHAR(50),
    
    -- Status and payment
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'processing', 'shipped', 'delivered', 'cancelled', 'returned')),
    payment_method VARCHAR(50) CHECK (payment_method IN ('cod', 'bank_transfer', 'credit_card', 'momo', 'zalopay', 'vnpay')),
    payment_status VARCHAR(50) DEFAULT 'pending' CHECK (payment_status IN ('pending', 'paid', 'failed', 'refunded', 'partially_refunded')),
    payment_reference VARCHAR(255), -- Payment gateway reference
    
    -- Shipping
    tracking_number VARCHAR(255),
    shipped_at TIMESTAMP WITH TIME ZONE,
    delivered_at TIMESTAMP WITH TIME ZONE,
    
    -- Additional info
    notes TEXT,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Order items (updated to match models)
CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id INTEGER NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(12,2) NOT NULL,
    total_price DECIMAL(12,2) NOT NULL,
    product_name VARCHAR(255),
    product_sku VARCHAR(50),
    product_image VARCHAR(500),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Order status history
CREATE TABLE order_status_history (
    id SERIAL PRIMARY KEY,
    order_id INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    status VARCHAR(50) NOT NULL,
    notes TEXT,
    changed_by VARCHAR(450), -- ASP.NET Identity UserId
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- REVIEWS & RATINGS TABLES
-- ========================================

-- Product reviews (updated to match models)
CREATE TABLE product_reviews (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    user_id VARCHAR(450), -- ASP.NET Identity UserId
    order_id INTEGER REFERENCES orders(id),
    rating INTEGER NOT NULL CHECK (rating >= 1 AND rating <= 5),
    title VARCHAR(255),
    comment TEXT,
    is_approved BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Remove review helpful votes table as it's not used in current system

-- ========================================
-- DISCOUNTS & COUPONS TABLES
-- ========================================

-- Coupons (updated to match models)
CREATE TABLE coupons (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    type VARCHAR(20) NOT NULL CHECK (type IN ('percentage', 'fixed_amount')),
    value DECIMAL(12,2) NOT NULL,
    min_order_amount DECIMAL(12,2),
    usage_limit INTEGER,
    usage_count INTEGER DEFAULT 0,
    start_date TIMESTAMP WITH TIME ZONE,
    end_date TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Coupon usage tracking
CREATE TABLE coupon_usages (
    id SERIAL PRIMARY KEY,
    coupon_id INTEGER NOT NULL REFERENCES coupons(id),
    user_id VARCHAR(450), -- ASP.NET Identity UserId
    order_id INTEGER NOT NULL REFERENCES orders(id),
    discount_amount DECIMAL(12,2) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- BLOG TABLES
-- ========================================

-- Blog categories
CREATE TABLE blog_categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    description TEXT,
    image_url VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Blog posts (updated to match models)
CREATE TABLE blog_posts (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    title VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    excerpt VARCHAR(500),
    content TEXT NOT NULL,
    featured_image_url VARCHAR(500),
    author_id VARCHAR(450) NOT NULL, -- ASP.NET Identity UserId
    category_id INTEGER REFERENCES blog_categories(id),
    tags TEXT[], -- Array of tags
    status VARCHAR(50) DEFAULT 'draft' CHECK (status IN ('draft', 'published', 'archived')),
    is_featured BOOLEAN DEFAULT FALSE,
    view_count INTEGER DEFAULT 0,
    meta_title VARCHAR(255),
    meta_description VARCHAR(500),
    published_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- BRANDS TABLE
-- ========================================

-- Thêm bảng brands
CREATE TABLE brands (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    slug VARCHAR(255) UNIQUE NOT NULL,
    image_url VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Thêm cột brand_id vào bảng products (đã có trong bảng products mới)

-- ========================================
-- MISSING ESSENTIAL TABLES
-- ========================================

-- Contact messages
CREATE TABLE contact_messages (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    phone VARCHAR(20),
    subject VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    is_read BOOLEAN DEFAULT FALSE,
    is_replied BOOLEAN DEFAULT FALSE,
    admin_notes TEXT,
    user_id VARCHAR(450), -- Optional: if logged in user
    replied_at TIMESTAMP WITH TIME ZONE,
    replied_by VARCHAR(450),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Notifications
CREATE TABLE notifications (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL,
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    type VARCHAR(50) NOT NULL, -- order, product, system, welcome, admin_order
    action_url VARCHAR(500),
    is_read BOOLEAN DEFAULT FALSE,
    read_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Security logs
CREATE TABLE security_logs (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL,
    event_type VARCHAR(100) NOT NULL, -- LoginSuccess, LoginFailed, PasswordChange, etc.
    description TEXT NOT NULL,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Password history
CREATE TABLE password_histories (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Active sessions
CREATE TABLE active_sessions (
    id SERIAL PRIMARY KEY,
    session_id VARCHAR(255) NOT NULL,
    user_id VARCHAR(450) NOT NULL,
    ip_address INET,
    user_agent TEXT,
    device_type VARCHAR(100),
    location VARCHAR(255),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    last_activity TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Two factor tokens
CREATE TABLE two_factor_tokens (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(450) NOT NULL,
    token VARCHAR(255) NOT NULL,
    purpose VARCHAR(100) NOT NULL, -- Login, PasswordReset, etc.
    is_used BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Payments
CREATE TABLE payments (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    order_id INTEGER NOT NULL REFERENCES orders(id),
    payment_method VARCHAR(50) NOT NULL,
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN ('pending', 'completed', 'failed', 'refunded')),
    amount DECIMAL(12,2) NOT NULL,
    transaction_id VARCHAR(255),
    gateway_response TEXT,
    processed_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Payment attempts
CREATE TABLE payment_attempts (
    id SERIAL PRIMARY KEY,
    payment_id VARCHAR(255) NOT NULL,
    order_id VARCHAR(255) NOT NULL,
    user_id VARCHAR(450) NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    currency VARCHAR(10) DEFAULT 'VND',
    payment_method VARCHAR(50) NOT NULL,
    status VARCHAR(50) NOT NULL,
    transaction_id VARCHAR(255),
    error_message TEXT,
    ip_address INET,
    user_agent TEXT,
    completed_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Payment methods
CREATE TABLE payment_methods (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    icon_url VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    requires_redirect BOOLEAN DEFAULT FALSE,
    min_amount DECIMAL(12,2),
    max_amount DECIMAL(12,2),
    supported_currencies VARCHAR(255),
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Checkout sessions
CREATE TABLE checkout_sessions (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    user_id VARCHAR(450) NOT NULL,
    email VARCHAR(255),
    status VARCHAR(50) DEFAULT 'active' CHECK (status IN ('active', 'completed', 'expired', 'cancelled')),
    total_amount DECIMAL(12,2) NOT NULL,
    shipping_fee DECIMAL(12,2) DEFAULT 0,
    tax DECIMAL(12,2) DEFAULT 0,
    discount_amount DECIMAL(12,2) DEFAULT 0,
    coupon_code VARCHAR(50),
    shipping_method VARCHAR(100),
    payment_method VARCHAR(50),
    shipping_address TEXT,
    billing_address TEXT,
    notes TEXT,
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Checkout session items
CREATE TABLE checkout_session_items (
    id SERIAL PRIMARY KEY,
    checkout_session_id INTEGER NOT NULL REFERENCES checkout_sessions(id) ON DELETE CASCADE,
    product_id INTEGER NOT NULL REFERENCES products(id),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(12,2) NOT NULL,
    total_price DECIMAL(12,2) NOT NULL,
    size VARCHAR(100),
    color VARCHAR(100),
    product_name VARCHAR(255),
    product_image VARCHAR(500)
);

-- Refund requests
CREATE TABLE refund_requests (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    payment_id VARCHAR(255) NOT NULL,
    order_id VARCHAR(255) NOT NULL,
    amount DECIMAL(12,2) NOT NULL,
    reason TEXT NOT NULL,
    status VARCHAR(50) DEFAULT 'pending' CHECK (status IN ('pending', 'approved', 'rejected', 'completed')),
    admin_notes TEXT,
    refund_transaction_id VARCHAR(255),
    requested_by VARCHAR(450) NOT NULL,
    processed_by VARCHAR(450),
    processed_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Shipping methods
CREATE TABLE shipping_methods (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    cost DECIMAL(12,2) NOT NULL,
    estimated_days INTEGER NOT NULL,
    is_active BOOLEAN DEFAULT TRUE,
    min_order_amount DECIMAL(12,2),
    max_weight DECIMAL(8,2),
    available_regions TEXT,
    sort_order INTEGER DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Promotions
CREATE TABLE promotions (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    code VARCHAR(50) UNIQUE NOT NULL,
    type VARCHAR(50) DEFAULT 'percentage' CHECK (type IN ('percentage', 'fixed_amount', 'free_shipping')),
    value DECIMAL(12,2) NOT NULL,
    min_order_amount DECIMAL(12,2),
    max_discount_amount DECIMAL(12,2),
    usage_limit INTEGER,
    usage_count INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    start_date TIMESTAMP WITH TIME ZONE,
    end_date TIMESTAMP WITH TIME ZONE,
    description TEXT,
    applicable_product_ids TEXT, -- JSON array
    applicable_category_ids TEXT, -- JSON array
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- ANALYTICS & REPORTING TABLES
-- ========================================

-- User sessions for analytics
CREATE TABLE user_sessions (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    session_id VARCHAR(255) NOT NULL,
    user_id VARCHAR(450),
    start_time TIMESTAMP WITH TIME ZONE NOT NULL,
    end_time TIMESTAMP WITH TIME ZONE,
    duration INTEGER, -- in minutes
    user_agent TEXT,
    ip_address INET,
    is_active BOOLEAN DEFAULT TRUE,
    device_type VARCHAR(100),
    browser VARCHAR(100),
    operating_system VARCHAR(100),
    country VARCHAR(100),
    city VARCHAR(100),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Page views
CREATE TABLE page_views (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    user_id VARCHAR(450),
    session_id VARCHAR(255) NOT NULL,
    page VARCHAR(500) NOT NULL,
    referrer VARCHAR(500),
    user_agent TEXT,
    ip_address INET,
    time_on_page INTEGER, -- in seconds
    exit_page VARCHAR(500),
    source VARCHAR(255),
    medium VARCHAR(255),
    campaign VARCHAR(255),
    viewed_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Conversion events
CREATE TABLE conversion_events (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    user_id VARCHAR(450),
    session_id VARCHAR(255) NOT NULL,
    conversion_type VARCHAR(100) NOT NULL, -- purchase, signup, newsletter, etc.
    value DECIMAL(12,2) DEFAULT 0,
    payment_method VARCHAR(50),
    source VARCHAR(255),
    medium VARCHAR(255),
    campaign VARCHAR(255),
    order_id INTEGER REFERENCES orders(id),
    product_ids TEXT, -- JSON array
    additional_data JSONB,
    converted_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Analytics data
CREATE TABLE analytics_data (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    entity_id VARCHAR(255),
    user_id VARCHAR(450),
    session_id VARCHAR(255) NOT NULL,
    source VARCHAR(255),
    medium VARCHAR(255),
    campaign VARCHAR(255),
    data JSONB,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Sales reports
CREATE TABLE sales_reports (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    report_type VARCHAR(100) NOT NULL,
    start_date TIMESTAMP WITH TIME ZONE NOT NULL,
    end_date TIMESTAMP WITH TIME ZONE NOT NULL,
    total_revenue DECIMAL(12,2) NOT NULL,
    total_orders INTEGER NOT NULL,
    total_products INTEGER NOT NULL,
    average_order_value DECIMAL(12,2) NOT NULL,
    report_data JSONB,
    status VARCHAR(50) DEFAULT 'generating' CHECK (status IN ('generating', 'completed', 'failed')),
    generated_by VARCHAR(450),
    generated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Report templates
CREATE TABLE report_templates (
    id SERIAL PRIMARY KEY,
    uuid UUID DEFAULT uuid_generate_v4() UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    report_type VARCHAR(100) NOT NULL,
    format VARCHAR(50) DEFAULT 'excel' CHECK (format IN ('excel', 'pdf', 'csv', 'json')),
    frequency VARCHAR(50) DEFAULT 'monthly' CHECK (frequency IN ('daily', 'weekly', 'monthly', 'quarterly', 'yearly')),
    parameters JSONB,
    configuration JSONB,
    is_active BOOLEAN DEFAULT TRUE,
    created_by VARCHAR(450),
    last_run_date TIMESTAMP WITH TIME ZONE,
    next_run_date TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- SYSTEM TABLES
-- ========================================

-- Settings table for system configuration
CREATE TABLE settings (
    id SERIAL PRIMARY KEY,
    key VARCHAR(255) UNIQUE NOT NULL,
    value TEXT,
    description TEXT,
    type VARCHAR(50) DEFAULT 'string', -- string, number, boolean, json
    is_public BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Email templates
CREATE TABLE email_templates (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    subject VARCHAR(255) NOT NULL,
    body TEXT NOT NULL,
    type VARCHAR(100) NOT NULL, -- welcome, order_confirmation, order_status_update, etc.
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- ========================================
-- INDEXES FOR PERFORMANCE (UPDATED)
-- ========================================

-- User indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
CREATE INDEX idx_users_is_active ON users(is_active);
CREATE INDEX idx_users_is_approved ON users(is_approved);

-- Product indexes
CREATE INDEX idx_products_category_id ON products(category_id);
CREATE INDEX idx_products_brand_id ON products(brand_id);
CREATE INDEX idx_products_sku ON products(sku);
CREATE INDEX idx_products_slug ON products(slug);
CREATE INDEX idx_products_is_active ON products(is_active);
CREATE INDEX idx_products_is_featured ON products(is_featured);
CREATE INDEX idx_products_status ON products(status);
CREATE INDEX idx_products_price ON products(price);
CREATE INDEX idx_products_created_at ON products(created_at);

-- Category indexes
CREATE INDEX idx_categories_parent_id ON categories(parent_category_id);
CREATE INDEX idx_categories_slug ON categories(slug);
CREATE INDEX idx_categories_is_active ON categories(is_active);

-- Order indexes
CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_order_number ON orders(order_number);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_payment_status ON orders(payment_status);
CREATE INDEX idx_orders_created_at ON orders(created_at);
CREATE INDEX idx_orders_email ON orders(email);

-- Shopping cart indexes
CREATE INDEX idx_shopping_carts_user_id ON shopping_carts(user_id);
CREATE INDEX idx_shopping_carts_product_id ON shopping_carts(product_id);

-- Review indexes
CREATE INDEX idx_product_reviews_product_id ON product_reviews(product_id);
CREATE INDEX idx_product_reviews_user_id ON product_reviews(user_id);
CREATE INDEX idx_product_reviews_is_approved ON product_reviews(is_approved);

-- Blog indexes
CREATE INDEX idx_blog_posts_slug ON blog_posts(slug);
CREATE INDEX idx_blog_posts_author_id ON blog_posts(author_id);
CREATE INDEX idx_blog_posts_category_id ON blog_posts(category_id);
CREATE INDEX idx_blog_posts_status ON blog_posts(status);
CREATE INDEX idx_blog_posts_published_at ON blog_posts(published_at);

-- Notification indexes
CREATE INDEX idx_notifications_user_id ON notifications(user_id);
CREATE INDEX idx_notifications_is_read ON notifications(is_read);
CREATE INDEX idx_notifications_type ON notifications(type);

-- Security log indexes
CREATE INDEX idx_security_logs_user_id ON security_logs(user_id);
CREATE INDEX idx_security_logs_event_type ON security_logs(event_type);
CREATE INDEX idx_security_logs_created_at ON security_logs(created_at);

-- Session indexes
CREATE INDEX idx_active_sessions_user_id ON active_sessions(user_id);
CREATE INDEX idx_active_sessions_session_id ON active_sessions(session_id);
CREATE INDEX idx_active_sessions_is_active ON active_sessions(is_active);

-- Analytics indexes
CREATE INDEX idx_user_sessions_user_id ON user_sessions(user_id);
CREATE INDEX idx_user_sessions_session_id ON user_sessions(session_id);
CREATE INDEX idx_page_views_user_id ON page_views(user_id);
CREATE INDEX idx_page_views_session_id ON page_views(session_id);
CREATE INDEX idx_conversion_events_user_id ON conversion_events(user_id);
CREATE INDEX idx_conversion_events_type ON conversion_events(conversion_type);

-- ========================================
-- TRIGGERS FOR AUTO UPDATE
-- ========================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Apply update triggers to relevant tables
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_addresses_updated_at BEFORE UPDATE ON addresses FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON categories FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_products_updated_at BEFORE UPDATE ON products FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_shopping_carts_updated_at BEFORE UPDATE ON shopping_carts FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_orders_updated_at BEFORE UPDATE ON orders FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_product_reviews_updated_at BEFORE UPDATE ON product_reviews FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_coupons_updated_at BEFORE UPDATE ON coupons FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_blog_posts_updated_at BEFORE UPDATE ON blog_posts FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_brands_updated_at BEFORE UPDATE ON brands FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_payments_updated_at BEFORE UPDATE ON payments FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_checkout_sessions_updated_at BEFORE UPDATE ON checkout_sessions FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_shipping_methods_updated_at BEFORE UPDATE ON shipping_methods FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_promotions_updated_at BEFORE UPDATE ON promotions FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_payment_methods_updated_at BEFORE UPDATE ON payment_methods FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_settings_updated_at BEFORE UPDATE ON settings FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_email_templates_updated_at BEFORE UPDATE ON email_templates FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER update_report_templates_updated_at BEFORE UPDATE ON report_templates FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
