# 🔍 KIỂM TRA BIỂU ĐỒ DOANH THU - BROWSER DEBUG

## ✅ Cách kiểm tra trong Browser

### 1. Mở Dashboard
1. Chạy ứng dụng: `dotnet run --project JohnHenryFashionWeb.csproj`
2. Truy cập: `http://localhost:5101/admin/dashboard`
3. Đăng nhập với tài khoản Admin

### 2. Mở Developer Tools
- **Windows/Linux**: `F12` hoặc `Ctrl + Shift + I`
- **Mac**: `Cmd + Option + I`

### 3. Kiểm tra Console
Trong tab **Console**, bạn sẽ thấy các logs debug:

#### ✅ Logs thành công:
```javascript
📊 Dashboard Chart Data Debug:
Chart Data: [{label: "10/13", value: 850000, additionalData: {...}}, ...]
Revenue Data: [{date: "2025-10-13T00:00:00", value: 850000, label: "10/13", ...}, ...]
Chart Data Length: 28
Revenue Data Length: 28
✅ Using revenue time series data
Labels: ["10/13", "10/14", "10/15", ...]
Values: [850000, 920000, 1100000, ...]
```

#### ❌ Logs lỗi:
```javascript
Canvas element #salesChart not found!  // Canvas không tồn tại
Chart Data: []                          // Không có dữ liệu từ server
⚠️ No data available, using empty default  // Không có dữ liệu
```

### 4. Kiểm tra Network
Trong tab **Network**:
1. Reload trang (`Ctrl/Cmd + R`)
2. Tìm request `/admin/dashboard`
3. Click vào request đó
4. Xem tab **Response** để xem HTML đã render
5. Tìm đoạn code: `const chartData = [...]` và `const revenueData = [...]`

### 5. Kiểm tra Elements
Trong tab **Elements**:
1. Tìm `<canvas id="salesChart">`
2. Kiểm tra xem canvas có tồn tại không
3. Kiểm tra xem có message "Chưa có dữ liệu doanh thu" không

## 🐛 Các lỗi thường gặp

### Lỗi 1: Canvas không tìm thấy
**Triệu chứng:**
```javascript
Canvas element #salesChart not found!
```

**Nguyên nhân:**
- Canvas được tạo trong phần code bị comment
- Lỗi syntax HTML

**Giải pháp:**
```html
<!-- Đảm bảo canvas tồn tại -->
<div class="chart-container">
    <canvas id="salesChart"></canvas>
</div>
```

### Lỗi 2: Chart.js không load
**Triệu chứng:**
```javascript
Uncaught ReferenceError: Chart is not defined
```

**Nguyên nhân:**
- Thiếu thư viện Chart.js

**Giải pháp:**
Thêm vào `_AdminLayout.cshtml` hoặc `Dashboard.cshtml`:
```html
<script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
```

### Lỗi 3: Dữ liệu empty
**Triệu chứng:**
```javascript
Chart Data: []
Revenue Data: []
```

**Nguyên nhân:**
- Chưa import dữ liệu mẫu
- Service không truy vấn đúng

**Giải pháp:**
```bash
cd database
./import_dashboard_data.sh
```

### Lỗi 4: Property không match
**Triệu chứng:**
```javascript
Cannot read property 'timestamp' of undefined
Cannot read property 'label' of undefined
```

**Nguyên nhân:**
- JSON serialize dùng PascalCase (Label, Value)
- JavaScript tìm camelCase (label, value)

**Giải pháp:**
Đã fix trong code - sử dụng `JsonNamingPolicy.CamelCase`

### Lỗi 5: Lucide icons không hiển thị
**Triệu chứng:**
- Icons không hiển thị
- Console warning về lucide

**Giải pháp:**
Thêm vào cuối script:
```javascript
// Initialize Lucide icons
if (typeof lucide !== 'undefined') {
    lucide.createIcons();
}
```

## 📊 Kiểm tra dữ liệu trong Database

### Query trực tiếp database:
```sql
-- Kiểm tra có SalesReports không
SELECT COUNT(*) as total_reports 
FROM "SalesReports" 
WHERE "ReportType" = 'daily';

-- Xem 10 records gần nhất
SELECT 
    "StartDate",
    "TotalRevenue",
    "TotalOrders",
    "TotalProducts"
FROM "SalesReports"
WHERE "ReportType" = 'daily'
ORDER BY "StartDate" DESC
LIMIT 10;

-- Kiểm tra range date
SELECT 
    MIN("StartDate") as oldest_date,
    MAX("StartDate") as newest_date,
    COUNT(*) as total_records
FROM "SalesReports"
WHERE "ReportType" = 'daily';
```

## 🎯 Checklist Troubleshooting

- [ ] Server logs hiển thị "Using X SalesReports records"
- [ ] Browser console hiển thị Chart Data và Revenue Data
- [ ] Canvas element tồn tại trong DOM
- [ ] Chart.js library đã load
- [ ] Labels và Values arrays không empty
- [ ] Không có JavaScript errors trong console
- [ ] Lucide icons đã initialize
- [ ] Data được serialize với camelCase naming

## 🚀 Next Steps

Nếu tất cả đã OK nhưng vẫn không thấy biểu đồ:

1. **Clear browser cache**: `Ctrl/Cmd + Shift + Delete`
2. **Hard reload**: `Ctrl/Cmd + Shift + R`
3. **Kiểm tra CSS**: Có thể chart bị ẩn bởi CSS
4. **Kiểm tra z-index**: Chart có thể bị element khác đè lên
5. **Test responsive**: Thu nhỏ/phóng to browser window

## 📱 Contact

Nếu vẫn gặp vấn đề, gửi:
1. Screenshot console logs
2. Screenshot network tab
3. Screenshot elements tab
4. Server logs (từ terminal)

---
*Generated: 2025-11-12*
