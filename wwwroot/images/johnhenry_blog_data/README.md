# JOHN HENRY BLOG COLLECTION

> Bộ sưu tập bài viết về thời trang nam từ John Henry Vietnam

![John Henry](https://johnhenry.vn/cdn/shop/files/logo.png)

---

## Thống Kê

| Metric | Value |
|--------|-------|
| **Tổng số bài viết** | 2 |
| 📸 **Tổng số hình ảnh** | 11 |
| **Banner** | 3 |
| **Tổng nội dung** | 4,949 ký tự |
| 📅 **Ngày crawl** | 16/10/2025 |

---

## 📖 Danh Sách Bài Viết

### 1️⃣ [VISCOSE – Chất Liệu Nâng Tầm Trải Nghiệm Đồ Len](post_001_viscose_material.md)

![Preview](banners/banner_e9ada939.jpg)

**📅 08/10/2025** | **🏷️ Chất liệu, Áo Len**

Khám phá chất liệu viscose - "lụa nhân tạo" mang lại sự mềm mại, thoáng khí và bền đẹp cho áo len.

**Highlights:**
- 🌸 Mềm mại & thoáng khí
- 💧 Hút ẩm tốt
- 👔 Giữ form & hạn chế xù lông
- Đa dạng ứng dụng

**Nội dung:** 2,487 ký tự | **Hình ảnh:** 5

[📖 Đọc bài viết →](post_001_viscose_material.md)

---

### 2️⃣ [SHIRTS FOR THE OFFICE - Sơ Mi Ý Nghĩa Ngày Doanh Nhân](post_002_shirts_for_office.md)

![Preview](banners/banner_c7b8ad90.jpg)

**📅 05/10/2025** | **🏷️ New Arrival, Áo sơ mi**

Món quà tinh tế dành tặng Ngày Doanh nhân Việt Nam - Dòng sơ mi công sở lịch lãm, hiện đại.

**Highlights:**
- Đa dạng màu sắc & họa tiết
- 👕 Form slim fit ôm gọn gàng
- 🌟 Chất liệu cotton mềm mại
- 💼 Phù hợp công sở & sự kiện

**Nội dung:** 2,462 ký tự | **Hình ảnh:** 6

[📖 Đọc bài viết →](post_002_shirts_for_office.md)

---

## 📂 Cấu Trúc Thư Mục

```
johnhenry_blog_data/
├── README.md                           # File này
├── post_001_viscose_material.md        # Bài viết 1 (Markdown)
├── post_002_shirts_for_office.md       # Bài viết 2 (Markdown)
├── johnhenry_blog_posts.json           # Dữ liệu JSON
├── johnhenry_blog_posts.csv            # Dữ liệu CSV
├── content/                            # Nội dung text gốc
│   ├── post_001_e9ada939.txt
│   └── post_002_c7b8ad90.txt
├── images/                             # Hình ảnh bài viết
│   ├── img_e9ada939_00.jpg
│   ├── img_e9ada939_01.jpg
│   ├── img_e9ada939_02.jpg
│   ├── img_e9ada939_03.jpg
│   ├── img_e9ada939_04.jpg
│   ├── img_c7b8ad90_01.jpg
│   ├── img_c7b8ad90_02.jpg
│   ├── img_c7b8ad90_03.jpg
│   ├── img_c7b8ad90_04.jpg
│   └── img_c7b8ad90_05.jpg
└── banners/                            # Banner bài viết
    ├── banner_e9ada939.jpg
    └── banner_c7b8ad90.jpg
```

---

## Tính Năng Crawler

### Đã Hoàn Thành

- [x] **Crawl nội dung blog** với 9 CSS selectors
- [x] **Image markers** đánh dấu vị trí ảnh: `[IMAGE_img_xxx_00]`
- [x] **Khử trùng hình ảnh** bằng SHA256 hash
- [x] **Nâng cấp chất lượng ảnh** (loại bỏ `_1024x1024`, `_medium`, etc.)
- [x] **Smart limit** - dừng sớm khi đủ số lượng
- [x] **Interrupt handling** - lưu data khi Ctrl+C
- [x] **Export JSON/CSV** - đầy đủ metadata
- [x] **Content files** - file text riêng cho mỗi bài

### Chất Lượng Data

| Metric | Result |
|--------|--------|
| **Posts with content** | 2/2 (100%) |
| **Posts with images** | 2/2 (100%) |
| **Posts with banners** | 2/2 (100%) |
| **Duplicate images** | 0 (removed) |
| **Avg content length** | 2,474 chars |
| **Total images** | 11 unique |

---

## 🛠️ Cách Sử Dụng

### Đọc File Markdown
```bash
# Mở bằng editor markdown
code post_001_viscose_material.md

# Hoặc xem preview trên GitHub
```

### Đọc Data JSON
```python
import json

with open('johnhenry_blog_posts.json', 'r', encoding='utf-8') as f:
    posts = json.load(f)
    
for post in posts:
    print(f"{post['title']}")
    print(f"Content: {post['content_length']} chars")
    print(f"Images: {post['image_count']}")
```

### Đọc Data CSV
```python
import pandas as pd

df = pd.read_csv('johnhenry_blog_posts.csv')
print(df[['title', 'publish_date', 'content_length', 'image_count']])
```

### Parse Image Markers
```python
import re

with open('content/post_001_e9ada939.txt', 'r', encoding='utf-8') as f:
    content = f.read()

# Tìm tất cả image markers
images = re.findall(r'\[IMAGE_(img_\w+_\d+)\]', content)
print(f"Found {len(images)} image markers: {images}")

# Replace với HTML tags
for img_id in images:
    img_file = f"images/{img_id}.jpg"
    html = f'<img src="{img_file}" alt="Image">'
    content = content.replace(f'[IMAGE_{img_id}]', html)
```

---

## 🌐 Liên Kết

<div align="center">

| Platform | Link |
|----------|------|
| 🌐 **Website** | [johnhenry.vn](https://johnhenry.vn) |
| 🛍️ **Shopee** | [shopeeJOHNHENRY](https://shopee.vn/johnhenry) |
| 🛍️ **Lazada** | [lazadaJOHNHENRY](https://www.lazada.vn/shop/johnhenry) |
| 💬 **Zalo OA** | John Henry Official |
| 🎵 **TikTok** | [@johnhenry.vn](https://www.tiktok.com/@johnhenry.vn) |
| 📞 **Hotline** | 0914 516 446 - 0906 954 368 |

</div>

---

## License & Credits

**Data Source:** [John Henry Vietnam Blog](https://johnhenry.vn/blogs/xu-huong)  
**Crawler:** [Crawl4AI](https://github.com/unclecode/crawl4ai)  
**Crawl Date:** October 16, 2025

---

<div align="center">

### JOHN HENRY
*Thời trang nam chính hãng - Phong cách lịch lãm Việt Nam*

**Made with ❤️ using Crawl4AI**

</div>
