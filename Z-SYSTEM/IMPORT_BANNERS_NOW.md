# 🚀 IMPORT BANNERS NGAY - Hướng Dẫn Nhanh

## Bước 1: Mở pgAdmin hoặc DBeaver

### Nếu dùng pgAdmin:
1. Mở pgAdmin 4
2. Connect tới PostgreSQL server (localhost:5432)
3. Chọn database: `johnhenry_db`
4. Click chuột phải vào database → **Query Tool**

### Nếu dùng DBeaver:
1. Mở DBeaver
2. Connect tới PostgreSQL
3. Chọn database `johnhenry_db`
4. Click vào biểu tượng **SQL Editor** (hoặc nhấn F3)

---

## Bước 2: Copy & Paste Script

Mở file: **`Scripts/SeedBanners.sql`**

Copy **TOÀN BỘ** nội dung file và paste vào Query Tool/SQL Editor.

---

## Bước 3: Chạy Script

- **pgAdmin**: Nhấn nút ▶️ Execute/Refresh (hoặc F5)
- **DBeaver**: Nhấn nút ▶️ Execute SQL Statement (hoặc Ctrl+Enter)

Đợi vài giây cho script chạy xong.

---

## Bước 4: Verify Kết Quả

Chạy query này để kiểm tra:

```sql
-- Kiểm tra tổng số banners
SELECT COUNT(*) FROM "MarketingBanners";
-- Expected: 24

-- Xem chi tiết theo position
SELECT "Position", "TargetPage", COUNT(*) as "Count"
FROM "MarketingBanners"
GROUP BY "Position", "TargetPage"
ORDER BY "Position", "TargetPage";
```

**Kết quả mong đợi:**

| Position | TargetPage | Count |
|----------|------------|-------|
| category_banner | AoSoMiNam | 1 |
| category_banner | AoSoMiNu | 1 |
| category_banner | ChanVayNu | 1 |
| category_banner | DamNu | 1 |
| category_banner | PhuKienNam | 1 |
| category_banner | PhuKienNu | 1 |
| category_banner | QuanShortNu | 1 |
| category_banner | QuanTayNam | 1 |
| collection_hero | BestSeller | 2 |
| collection_hero | Freelancer | 4 |
| collection_hero | JohnHenry | 4 |
| home_main | NULL | 3 |
| home_side | NULL | 2 |
| page_hero | Blog | 1 |

**Tổng: 24 banners** ✅

---

## Bước 5: Test Trên Website

### Khởi động lại app (nếu cần):
```bash
# Trong terminal đang chạy dotnet run, nhấn Ctrl+C
# Sau đó chạy lại:
dotnet run
```

### Truy cập các trang:

1. **Trang chủ**: http://localhost:5101/
   - ✅ Xem 3 banners carousel
   - ✅ Xem 2 small banners

2. **John Henry**: http://localhost:5101/Home/JohnHenry
   - ✅ Xem 4 banners carousel

3. **Freelancer**: http://localhost:5101/Home/Freelancer
   - ✅ Xem 4 banners carousel

4. **Admin Panel**: http://localhost:5101/admin/banners
   - ✅ Quản lý banners

---

## ⚠️ Troubleshooting

### Lỗi: "gen_random_uuid() does not exist"
Chạy lệnh này trước:
```sql
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
```

### Lỗi: "MarketingBanners table does not exist"
Chạy migration:
```bash
dotnet ef database update
```

### Banners không hiển thị trên website?
1. Check console log có lỗi không
2. Hard refresh browser: Ctrl+Shift+R (hoặc Cmd+Shift+R)
3. Verify lại database có 24 banners

---

## ✅ Checklist

- [ ] Mở pgAdmin/DBeaver
- [ ] Connect tới database `johnhenry_db`
- [ ] Copy & Paste script từ `Scripts/SeedBanners.sql`
- [ ] Execute script (F5)
- [ ] Verify: SELECT COUNT(*) → 24 banners
- [ ] Restart app: `dotnet run`
- [ ] Test trang chủ
- [ ] Test John Henry page
- [ ] Test Freelancer page
- [ ] Test admin panel

---

## 🎉 Done!

Sau khi import xong, tất cả banners sẽ load động từ database!

Bạn có thể quản lý chúng tại: **http://localhost:5101/admin/banners**

---

## 📁 Files Liên Quan

- **SQL Script**: `Scripts/SeedBanners.sql`
- **Full Guide**: `Scripts/BANNER_IMPORT_GUIDE.md`
- **Summary**: `BANNER_IMPORT_SUMMARY.md`
- **Quick Start**: `QUICK_START_BANNERS.md`
