# ✅ FIX: Biểu đồ doanh thu trong Dashboard Admin

## 🔍 Vấn đề
Mặc dù đã có dữ liệu mẫu trong database (`SalesReports` table), biểu đồ doanh thu trong dashboard admin vẫn không hiển thị hoặc hiển thị dữ liệu cứng (hard-coded).

## 🎯 Nguyên nhân
1. **View Dashboard.cshtml** sử dụng dữ liệu tĩnh thay vì dữ liệu từ Model
2. **ReportingService** chỉ truy vấn từ `Orders` table, không sử dụng `SalesReports` table
3. Không có dữ liệu Orders trong database, chỉ có SalesReports sample data

## ✅ Giải pháp đã áp dụng

### 1. Cập nhật View (Dashboard.cshtml)
**File:** `/Views/Admin/Dashboard.cshtml`

**Thay đổi:**
- Sử dụng dữ liệu từ `Model.SalesChartData` và `Model.RevenueTimeSeriesData`
- Xóa dữ liệu hard-coded `[1200000, 1900000, 3000000, ...]`
- Thêm logic fallback khi không có dữ liệu
- Tích hợp API để load dữ liệu theo period (7 ngày, 30 ngày, 3 tháng)

**Code mới:**
```javascript
// Get data from Model
const chartData = @Html.Raw(Json.Serialize(Model.SalesChartData ?? new List<ChartData>()));
const revenueData = @Html.Raw(Json.Serialize(Model.RevenueTimeSeriesData ?? new List<TimeSeriesData>()));

// Prepare labels and data
let labels = [];
let dataValues = [];

if (revenueData && revenueData.length > 0) {
    labels = revenueData.map(d => {
        const date = new Date(d.timestamp);
        return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' });
    });
    dataValues = revenueData.map(d => d.value);
}
```

### 2. Cập nhật ReportingService
**File:** `/Services/ReportingService.cs`

**Thay đổi:**
- Ưu tiên sử dụng dữ liệu từ `SalesReports` table trước
- Fallback về `Orders` table nếu không có SalesReports
- Thêm logging để dễ debug

**Code mới:**
```csharp
public async Task<List<ChartData>> GetSalesChartDataAsync(string period = "daily", int days = 30)
{
    var endDate = DateTime.UtcNow.Date;
    var startDate = endDate.AddDays(-days);

    // Try to get data from SalesReports first (for sample data)
    var salesReports = await _context.SalesReports
        .Where(r => r.ReportType == "daily" && 
                   r.StartDate >= startDate && 
                   r.EndDate <= endDate)
        .OrderBy(r => r.StartDate)
        .ToListAsync();

    if (salesReports.Any())
    {
        return salesReports
            .Select(r => new ChartData
            {
                Label = r.StartDate.ToString("MM/dd"),
                Value = r.TotalRevenue,
                AdditionalData = new Dictionary<string, object>
                {
                    ["Orders"] = r.TotalOrders,
                    ["Products"] = r.TotalProducts
                }
            })
            .ToList();
    }

    // Fallback to Orders table...
}
```

### 3. Cập nhật TimeSeriesData Model
**File:** `/Models/AnalyticsModels.cs`

**Thay đổi:**
- Thêm property `Timestamp` (alias cho `Date`)
- Thêm `AdditionalData` dictionary

**Code mới:**
```csharp
public class TimeSeriesData
{
    public DateTime Date { get; set; }
    public DateTime Timestamp => Date; // Alias for compatibility
    public decimal Value { get; set; }
    public string? Label { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
```

## 📊 Import dữ liệu mẫu

### Tự động (khuyến nghị)
```bash
cd database
./import_dashboard_data.sh
```

### Thủ công
```bash
# Kết nối database và chạy script
psql -h YOUR_HOST -p YOUR_PORT -d YOUR_DB -U YOUR_USER -f insert_sample_dashboard_data_v2.sql
```

## 🧪 Kiểm tra

### 1. Xem logs
Khi truy cập `/admin/dashboard`, check logs xem có dòng này không:
```
Using X SalesReports records for chart data
```

### 2. Kiểm tra database
```sql
-- Kiểm tra số lượng SalesReports
SELECT COUNT(*) FROM "SalesReports" WHERE "ReportType" = 'daily';

-- Xem dữ liệu mẫu
SELECT "StartDate", "TotalRevenue", "TotalOrders" 
FROM "SalesReports" 
WHERE "ReportType" = 'daily' 
ORDER BY "StartDate" DESC 
LIMIT 10;
```

### 3. Test trong browser
1. Truy cập: `https://your-site.com/admin/dashboard`
2. Kiểm tra biểu đồ "Doanh thu theo thời gian"
3. Click các nút "7 ngày", "30 ngày", "3 tháng"
4. Mở Console (F12) để xem logs

## 📝 Lưu ý

### Dữ liệu mẫu
File `insert_sample_dashboard_data_v2.sql` tạo:
- **48 SalesReports**: 30 daily + 12 weekly + 6 monthly
- **100 AnalyticsData events**
- **50 UserSessions**
- **200+ PageViews**
- **30 SupportTickets**
- **2 FlashSales**
- **2 EmailCampaigns**

### Production
Trong môi trường production:
1. Dữ liệu thực sẽ được tạo từ Orders
2. Có thể tạo scheduled job để tạo SalesReports tự động
3. Cache dữ liệu dashboard để tăng performance

### Performance
- Dữ liệu chart được cache 15 phút
- Sử dụng `IAnalyticsService` và `ICacheService`
- Dashboard tự refresh mỗi 30 giây

## 🔗 Files đã sửa

1. `/Views/Admin/Dashboard.cshtml` - View hiển thị dashboard
2. `/Services/ReportingService.cs` - Service lấy dữ liệu
3. `/Models/AnalyticsModels.cs` - Model TimeSeriesData
4. `/database/import_dashboard_data.sh` - Script import dữ liệu

## 🚀 Commit message
```
fix(dashboard): Hiển thị biểu đồ doanh thu từ SalesReports data

- Cập nhật Dashboard view để sử dụng dữ liệu thực từ Model
- Sửa ReportingService ưu tiên lấy data từ SalesReports table
- Thêm fallback về Orders table khi không có SalesReports
- Thêm Timestamp property cho TimeSeriesData model
- Thêm script import_dashboard_data.sh để import sample data
- Fix type casting issues trong ReportingService

Closes #[issue-number]
```

## 📞 Support
Nếu vẫn gặp vấn đề, check:
1. Database connection string
2. Permissions của user database
3. Logs trong `/logs` folder
4. Browser console errors

---
*Generated by GitHub Copilot*  
*Date: 2025-11-12*
