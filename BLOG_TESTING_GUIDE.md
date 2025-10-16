# 🧪 BLOG TESTING GUIDE - Hướng dẫn Test Blog

## 📋 **TEST SCENARIOS**

### **Scenario 1: Test Blog trên Trang chủ (Có bài viết)**

**Điều kiện:** Database có ít nhất 3 bài blog đã publish

**Steps:**
1. Truy cập `http://localhost:5101/`
2. Scroll xuống section "TIN TỨC THỜI TRANG"

**Expected Results:**
- ✅ Hiển thị 3 bài blog mới nhất
- ✅ Mỗi card có:
  - Featured image hoặc default image
  - Date badge (ngày/tháng)
  - Category badge
  - Title
  - Excerpt (max 120 chars)
  - "Đọc thêm" button
- ✅ Click vào bài viết → Redirect đến `/blog/{id}`
- ✅ Responsive: Mobile (1 column), Tablet (2 columns), Desktop (3 columns)

---

### **Scenario 2: Test Blog trên Trang chủ (Chưa có bài viết)**

**Điều kiện:** Database không có blog published hoặc tất cả là draft

**Steps:**
1. Truy cập `http://localhost:5101/`
2. Scroll xuống section "TIN TỨC THỜI TRANG"

**Expected Results:**
- ✅ Hiển thị 3 blog placeholder mặc định
- ✅ Placeholder có:
  - Default images (banner-women-main.jpg, banner-man-main.jpg, banner-women-1.jpg)
  - Hardcoded titles
  - Hardcoded excerpts
  - Link "Đọc thêm" → `/Blog`
- ✅ Không có lỗi console
- ✅ UI không bị vỡ

---

### **Scenario 3: Tạo Blog Post từ Admin**

**Điều kiện:** User đã login với role Admin

**Steps:**
1. Truy cập `http://localhost:5101/admin/blog`
2. Click "Viết bài mới"
3. Điền form:
   ```
   Title: "Test Blog Post 1"
   Slug: "test-blog-post-1" (auto-generated)
   Excerpt: "This is a test excerpt for blog post 1. Lorem ipsum dolor sit amet..."
   Content: "<p>Full content with <strong>HTML</strong> formatting.</p>"
   Category: Select any
   Featured Image: Upload image
   Status: "published"
   Featured: Check/uncheck
   Meta Title: "Test Blog - SEO Title"
   Meta Description: "Test meta description"
   Tags: "test, blog, fashion"
   ```
4. Click "Xuất bản ngay"

**Expected Results:**
- ✅ Redirect về `/admin/blog`
- ✅ Success message hiển thị
- ✅ Blog post xuất hiện trong danh sách
- ✅ Status = "Đã xuất bản" (badge màu xanh)
- ✅ Featured image hiển thị trong table

---

### **Scenario 4: Edit Blog Post**

**Điều kiện:** Có blog post đã tạo

**Steps:**
1. Truy cập `/admin/blog`
2. Click icon "Sửa" trên một blog post
3. Thay đổi Title → "Test Blog Post 1 - Updated"
4. Thay đổi Content
5. Click "Cập nhật"

**Expected Results:**
- ✅ Redirect về `/admin/blog`
- ✅ Success message hiển thị
- ✅ Title mới hiển thị trong list
- ✅ UpdatedAt timestamp cập nhật

---

### **Scenario 5: Xóa Blog Post**

**Điều kiện:** Có blog post đã tạo

**Steps:**
1. Truy cập `/admin/blog`
2. Click icon "Xóa" trên một blog post
3. Confirm delete

**Expected Results:**
- ✅ Blog post biến mất khỏi list
- ✅ Success message hiển thị
- ✅ Không còn trên database (hoặc status = deleted nếu soft delete)

---

### **Scenario 6: Publish Draft Post**

**Điều kiện:** Có blog post status = "draft"

**Steps:**
1. Truy cập `/admin/blog`
2. Filter "Bản nháp"
3. Click icon "Xuất bản" (send icon) trên draft post
4. Confirm

**Expected Results:**
- ✅ Status chuyển từ "Bản nháp" → "Đã xuất bản"
- ✅ PublishedAt timestamp được set
- ✅ Post xuất hiện trên trang chủ và `/blog`

---

### **Scenario 7: View Blog Details**

**Điều kiện:** Có blog post published

**Steps:**
1. Từ trang chủ hoặc `/blog`, click vào một blog post
2. Kiểm tra trang chi tiết `/blog/{id}`

**Expected Results:**
- ✅ Title hiển thị
- ✅ Featured image hiển thị
- ✅ Category badge
- ✅ Date, author, view count
- ✅ Tags display
- ✅ Full content HTML render đúng
- ✅ Social share buttons hoạt động
- ✅ Related posts hiển thị (nếu có)
- ✅ Breadcrumb navigation đúng
- ✅ View count tăng lên sau mỗi lần view

---

### **Scenario 8: Search Blog**

**Điều kiện:** Có nhiều blog posts

**Steps:**
1. Truy cập `/blog`
2. Nhập keyword vào search box: "thời trang"
3. Click "Search" hoặc Enter

**Expected Results:**
- ✅ Chỉ hiển thị posts có "thời trang" trong title/content/excerpt
- ✅ Search term highlight (nếu có)
- ✅ Alert hiển thị: "Kết quả tìm kiếm cho: thời trang"
- ✅ Button "Xóa bộ lọc" hiển thị

---

### **Scenario 9: Filter by Category**

**Điều kiện:** Có blog posts thuộc nhiều categories

**Steps:**
1. Truy cập `/blog`
2. Click vào một category button (vd: "Xu hướng")

**Expected Results:**
- ✅ Chỉ hiển thị posts thuộc category đó
- ✅ Category button active (màu primary)
- ✅ Breadcrumb update với category name

---

### **Scenario 10: Pagination**

**Điều kiện:** Có > 9 blog posts

**Steps:**
1. Truy cập `/blog`
2. Scroll xuống pagination
3. Click "Trang 2"

**Expected Results:**
- ✅ Load posts 10-18
- ✅ URL update: `/blog?page=2`
- ✅ Page 2 button active
- ✅ "Trước" button enable
- ✅ Smooth scroll to top (optional)

---

### **Scenario 11: Featured Posts**

**Điều kiện:** Có posts với IsFeatured = true

**Steps:**
1. Trong admin, tạo post với "Featured" checked
2. Publish post
3. Truy cập `/blog`

**Expected Results:**
- ✅ Featured posts xuất hiện ở sidebar (nếu có)
- ✅ Featured badge hiển thị (optional)
- ✅ Featured posts ưu tiên trong homepage

---

### **Scenario 12: SEO Meta Tags**

**Điều kiện:** Blog post có Meta Title, Meta Description

**Steps:**
1. Truy cập blog details: `/blog/{id}`
2. View page source (Ctrl+U hoặc Cmd+Option+U)

**Expected Results:**
- ✅ `<title>` = MetaTitle hoặc Title
- ✅ `<meta name="description">` = MetaDescription hoặc Excerpt
- ✅ `<meta property="og:title">` = MetaTitle
- ✅ `<meta property="og:description">` = MetaDescription
- ✅ `<meta property="og:image">` = FeaturedImageUrl
- ✅ `<meta name="keywords">` = Tags

---

### **Scenario 13: Empty States**

**13.1 Admin Blog List Empty**
- Xóa tất cả blog posts
- Truy cập `/admin/blog`
- Expected: Empty state message với "Viết bài mới" button

**13.2 Public Blog List Empty**
- Set tất cả posts thành draft
- Truy cập `/blog`
- Expected: "Không có bài viết nào" message

**13.3 Category Empty**
- Truy cập category không có posts
- Expected: "Không có bài viết trong danh mục này"

---

### **Scenario 14: Image Upload**

**Steps:**
1. Tạo blog post mới
2. Upload featured image (JPG, 1MB)
3. Preview image trong form
4. Save post
5. View post details

**Expected Results:**
- ✅ Image upload success
- ✅ Preview hiển thị trong form
- ✅ Image lưu vào `/wwwroot/uploads/blog/`
- ✅ FeaturedImageUrl save vào database
- ✅ Image hiển thị trong homepage và details

---

### **Scenario 15: Slug Generation**

**Steps:**
1. Tạo blog post mới
2. Nhập Title: "Xu Hướng Thời Trang 2025 - Top 10 Mẫu Hot"
3. Để trống Slug
4. Save

**Expected Results:**
- ✅ Slug tự động generate: "xu-huong-thoi-trang-2025-top-10-mau-hot"
- ✅ URL thân thiện: `/blog/xu-huong-thoi-trang-2025-top-10-mau-hot`
- ✅ Không có ký tự đặc biệt, dấu, viết hoa

---

### **Scenario 16: TinyMCE Editor**

**Steps:**
1. Trong Create/Edit blog post
2. Test TinyMCE editor:
   - Bold, italic, underline
   - Headings (H1-H6)
   - Lists (ordered, unordered)
   - Links
   - Images
   - Tables
   - Code blocks

**Expected Results:**
- ✅ All formatting hoạt động
- ✅ HTML output đúng
- ✅ Preview chính xác trong details page

---

### **Scenario 17: Draft Auto-load (Admin)**

**Steps:**
1. Tạo blog post với Status = "draft"
2. Save
3. Truy cập `/admin/blog`
4. Filter "Bản nháp"

**Expected Results:**
- ✅ Draft post hiển thị
- ✅ Status badge màu vàng: "Bản nháp"
- ✅ Không xuất hiện trên homepage/public blog

---

### **Scenario 18: Archive Post**

**Steps:**
1. Edit blog post
2. Đổi Status = "archived"
3. Save

**Expected Results:**
- ✅ Post biến mất khỏi homepage
- ✅ Không xuất hiện trong `/blog`
- ✅ Vẫn có trong admin với status "Lưu trữ"
- ✅ Có thể restore về published

---

### **Scenario 19: Related Posts**

**Điều kiện:** Có nhiều posts cùng category

**Steps:**
1. Truy cập blog details
2. Scroll xuống "Bài viết liên quan"

**Expected Results:**
- ✅ Hiển thị 3 posts cùng category
- ✅ Exclude post hiện tại
- ✅ Cards có thumbnail, title, date
- ✅ Click vào related post → View details

---

### **Scenario 20: Mobile Responsive**

**Device sizes to test:**
- iPhone SE (375px)
- iPhone 12 Pro (390px)
- iPad (768px)
- iPad Pro (1024px)

**Expected Results:**
- ✅ Homepage blog cards: 1 column (mobile), 2 (tablet), 3 (desktop)
- ✅ Blog list: Responsive grid
- ✅ Blog details: Readable trên mobile
- ✅ Images scale properly
- ✅ Navigation menu collapse
- ✅ Touch-friendly buttons

---

## 🐛 **COMMON BUGS TO CHECK**

### **Bug 1: Null Reference**
- [ ] BlogPost không có Category → Check null
- [ ] BlogPost không có Author → Check null
- [ ] FeaturedImageUrl null → Use default image
- [ ] Excerpt null → Auto-generate từ content

### **Bug 2: Date Format**
- [ ] PublishedAt null → Use CreatedAt
- [ ] Date format consistency (dd/MM/yyyy)
- [ ] Time zone issues (UTC vs Local)

### **Bug 3: HTML Injection**
- [ ] Content có script tags → Sanitize
- [ ] Title có HTML → Escape
- [ ] Excerpt có HTML → Strip tags

### **Bug 4: Image Upload**
- [ ] File quá lớn → Validate max size
- [ ] File type không hợp lệ → Validate extensions
- [ ] Path không tồn tại → Create directory
- [ ] Permission denied → Check folder permissions

### **Bug 5: Pagination**
- [ ] Page < 1 → Redirect page 1
- [ ] Page > totalPages → Redirect last page
- [ ] Pagination links broken with filters

### **Bug 6: Search**
- [ ] Special characters trong search → Escape
- [ ] Empty search → Show all
- [ ] Case sensitivity → Make case-insensitive

---

## ✅ **ACCEPTANCE CRITERIA**

### **Must Have:**
- [x] Admin có thể CRUD blog posts
- [x] Homepage hiển thị 3 blog mới nhất
- [x] Public blog list với pagination
- [x] Blog details page hoạt động
- [x] Search functionality
- [x] Category filtering
- [x] Responsive design
- [x] SEO meta tags

### **Should Have:**
- [x] Featured posts
- [x] View count tracking
- [x] Related posts
- [x] Social share buttons
- [x] Image upload
- [x] Rich text editor
- [x] Slug auto-generation

### **Nice to Have:**
- [ ] Comments system
- [ ] Like/reaction
- [ ] Draft auto-save
- [ ] Revision history
- [ ] Analytics dashboard
- [ ] Email notifications

---

## 📊 **PERFORMANCE METRICS**

### **Load Times (Target):**
- Homepage: < 2s
- Blog list: < 1.5s
- Blog details: < 1s
- Admin blog list: < 2s

### **Database Queries:**
- Homepage blog: 1 query (with Include)
- Blog list: 2 queries (posts + categories)
- Blog details: 3 queries (post + related + view count update)

### **Image Optimization:**
- Featured images: < 500KB
- Thumbnails: < 100KB
- Format: WebP (preferred) or JPG

---

## 🎉 **SIGN-OFF CHECKLIST**

- [ ] All test scenarios passed
- [ ] No critical bugs
- [ ] Performance acceptable
- [ ] Responsive on all devices
- [ ] SEO meta tags correct
- [ ] Images loading properly
- [ ] No console errors
- [ ] Database queries optimized
- [ ] Security checks passed
- [ ] User documentation complete

---

**Testing completed by:** _________________  
**Date:** _________________  
**Sign-off:** ✅ APPROVED / ❌ REJECTED  
**Notes:** _________________________________
