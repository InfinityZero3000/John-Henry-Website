# Hệ thống Địa chỉ Việt Nam - Vietnamese Address System

## Tổng quan

Hệ thống quản lý địa chỉ đầy đủ theo đơn vị hành chính Việt Nam với 3 cấp:
- **Tỉnh/Thành phố** (63 tỉnh thành)
- **Quận/Huyện**
- **Phường/Xã**

## Cấu trúc Database

### Tables đã tạo:

1. **Provinces** (Tỉnh/Thành phố)
   - Id (int, Primary Key)
   - Code (string) - Mã tỉnh
   - Name (string) - Tên ngắn (VD: "Hà Nội")
   - FullName (string) - Tên đầy đủ (VD: "Thành phố Hà Nội")
   - CodeName (string) - Slug (VD: "ha_noi")

2. **Districts** (Quận/Huyện)
   - Id (int, Primary Key)
   - Code (string)
   - Name (string)
   - FullName (string)
   - CodeName (string)
   - ProvinceId (int, Foreign Key → Provinces)

3. **Wards** (Phường/Xã)
   - Id (int, Primary Key)
   - Code (string)
   - Name (string)
   - FullName (string)
   - CodeName (string)
   - DistrictId (int, Foreign Key → Districts)

### Dữ liệu đã import:

✅ 63 tỉnh/thành phố
✅ 60 quận/huyện (cho Hà Nội, TP.HCM, Đà Nẵng)
✅ 212 phường/xã (cho các quận trung tâm)

## API Endpoints

### 1. Lấy danh sách tỉnh/thành phố
```http
GET /api/AddressApi/provinces
```

**Response:**
```json
[
  {
    "id": 1,
    "code": "01",
    "name": "Hà Nội",
    "fullName": "Thành phố Hà Nội"
  },
  {
    "id": 2,
    "code": "79",
    "name": "Hồ Chí Minh",
    "fullName": "Thành phố Hồ Chí Minh"
  }
]
```

### 2. Lấy danh sách quận/huyện theo tỉnh
```http
GET /api/AddressApi/districts/{provinceId}
```

**Example:** `/api/AddressApi/districts/1` (Lấy quận/huyện của Hà Nội)

**Response:**
```json
[
  {
    "id": 1,
    "code": "001",
    "name": "Ba Đình",
    "fullName": "Quận Ba Đình"
  },
  {
    "id": 2,
    "code": "002",
    "name": "Hoàn Kiếm",
    "fullName": "Quận Hoàn Kiếm"
  }
]
```

### 3. Lấy danh sách phường/xã theo quận/huyện
```http
GET /api/AddressApi/wards/{districtId}
```

**Example:** `/api/AddressApi/wards/1` (Lấy phường của Quận Ba Đình)

**Response:**
```json
[
  {
    "id": 1,
    "code": "00001",
    "name": "Phúc Xá",
    "fullName": "Phường Phúc Xá"
  },
  {
    "id": 2,
    "code": "00004",
    "name": "Trúc Bạch",
    "fullName": "Phường Trúc Bạch"
  }
]
```

## Cách sử dụng trong Checkout

### Form HTML với Dropdown Cascade:

```html
<!-- Tỉnh/Thành phố -->
<select id="newProvince" class="form-select">
    <option value="">-- Chọn tỉnh/thành --</option>
</select>

<!-- Quận/Huyện (disabled cho đến khi chọn tỉnh) -->
<select id="newDistrict" class="form-select" disabled>
    <option value="">-- Chọn quận/huyện --</option>
</select>

<!-- Phường/Xã (disabled cho đến khi chọn quận) -->
<select id="newWard" class="form-select" disabled>
    <option value="">-- Chọn phường/xã --</option>
</select>
```

### JavaScript Implementation:

```javascript
// 1. Load provinces khi page load
$(document).ready(function() {
    fetch('/api/AddressApi/provinces')
        .then(response => response.json())
        .then(data => {
            const select = $('#newProvince');
            data.forEach(province => {
                select.append(`<option value="${province.id}" data-name="${province.name}">${province.name}</option>`);
            });
        });
});

// 2. Khi chọn tỉnh → load quận/huyện
$('#newProvince').change(function() {
    const provinceId = $(this).val();
    $('#newDistrict').prop('disabled', false);
    
    fetch(`/api/AddressApi/districts/${provinceId}`)
        .then(response => response.json())
        .then(data => {
            $('#newDistrict').empty().append('<option value="">-- Chọn quận/huyện --</option>');
            data.forEach(district => {
                $('#newDistrict').append(`<option value="${district.id}" data-name="${district.name}">${district.name}</option>`);
            });
        });
});

// 3. Khi chọn quận → load phường/xã
$('#newDistrict').change(function() {
    const districtId = $(this).val();
    $('#newWard').prop('disabled', false);
    
    fetch(`/api/AddressApi/wards/${districtId}`)
        .then(response => response.json())
        .then(data => {
            $('#newWard').empty().append('<option value="">-- Chọn phường/xã --</option>');
            data.forEach(ward => {
                $('#newWard').append(`<option value="${ward.id}" data-name="${ward.name}">${ward.name}</option>`);
            });
        });
});
```

## Lưu địa chỉ vào Database

### Backend (PaymentController.cs):

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> SaveAddress(
    string firstName, 
    string lastName, 
    string phone, 
    string address1, 
    string? address2,
    string city,        // Tên tỉnh/thành
    string state,       // Quận/huyện + Phường/xã
    string postalCode,
    bool isDefault = false)
{
    var newAddress = new Address
    {
        UserId = userId,
        FirstName = firstName,
        LastName = lastName,
        Phone = phone,
        Address1 = address1,      // Số nhà, tên đường
        Address2 = address2,      // Ghi chú thêm
        City = city,              // "Hà Nội"
        State = state,            // "Ba Đình, Phúc Xá"
        Country = "Vietnam",
        IsDefault = isDefault
    };
    
    _context.Addresses.Add(newAddress);
    await _context.SaveChangesAsync();
    
    return Json(new { success = true, addressId = newAddress.Id });
}
```

### Frontend submission:

```javascript
const provinceName = $('#newProvince option:selected').data('name');
const districtName = $('#newDistrict option:selected').data('name');
const wardName = $('#newWard option:selected').data('name');

fetch('/Payment/SaveAddress', {
    method: 'POST',
    body: new URLSearchParams({
        'firstName': firstName,
        'lastName': lastName,
        'phone': phone,
        'address1': address1,
        'address2': address2,
        'city': provinceName,                              // "Hà Nội"
        'state': districtName + ', ' + wardName,          // "Ba Đình, Phúc Xá"
        'isDefault': isDefault
    })
});
```

## Validation Rules

✅ **Bắt buộc:**
- Họ, Tên
- Số điện thoại
- Tỉnh/Thành phố
- Quận/Huyện
- Phường/Xã
- Địa chỉ (số nhà, tên đường)

⚠️ **Tùy chọn:**
- Ghi chú thêm (Address2)
- Mã bưu điện
- Đặt làm mặc định

## Migration Commands

### Tạo migration:
```bash
dotnet ef migrations add AddVietnameseAdministrativeDivisions --context ApplicationDbContext
```

### Apply migration:
```bash
dotnet ef database update --context ApplicationDbContext
```

### Import dữ liệu:
```bash
psql -U johnhenry_user -d johnhenry_db -f database/import_vietnam_addresses.sql
```

## Mở rộng trong tương lai

### Thêm dữ liệu đầy đủ:

1. **Thêm quận/huyện cho các tỉnh còn lại:**
   - Hiện tại chỉ có Hà Nội (30), TP.HCM (22), Đà Nẵng (8)
   - Cần thêm ~690 quận/huyện cho 60 tỉnh còn lại

2. **Thêm phường/xã đầy đủ:**
   - Hiện tại: 212 phường/xã (các quận trung tâm)
   - Tổng số: ~11,000 phường/xã trên toàn quốc

3. **Data source:**
   - Tổng cục Thống kê Việt Nam
   - API: https://provinces.open-api.vn/api/
   - GitHub: https://github.com/madnh/hanhchinhvn

### Tối ưu hóa:

1. **Caching:**
```csharp
// Cache provinces (hiếm khi thay đổi)
var provinces = await _cacheService.GetOrSetAsync(
    "vietnam_provinces",
    () => _context.Provinces.OrderBy(p => p.Name).ToListAsync(),
    TimeSpan.FromDays(30)
);
```

2. **Indexing:**
```sql
CREATE INDEX idx_districts_province ON "Districts"("ProvinceId");
CREATE INDEX idx_wards_district ON "Wards"("DistrictId");
```

3. **Lazy loading districts/wards:**
   - Chỉ load khi user chọn cấp cha
   - Giảm tải server ban đầu

## Testing

### Test API endpoints:
```bash
# Get provinces
curl http://localhost:5101/api/AddressApi/provinces

# Get districts for Hanoi (id=1)
curl http://localhost:5101/api/AddressApi/districts/1

# Get wards for Ba Dinh (id=1)
curl http://localhost:5101/api/AddressApi/wards/1
```

### Test trong browser:
1. Vào http://localhost:5101/Checkout
2. Click "Thêm địa chỉ mới"
3. Chọn Tỉnh → Quận → Phường cascade
4. Nhập thông tin và lưu
5. Kiểm tra địa chỉ hiển thị đúng trong danh sách

## Files đã thay đổi

✅ **Models/DomainModels.cs** - Thêm Province, District, Ward models
✅ **Data/ApplicationDbContext.cs** - Thêm DbSets
✅ **Controllers/Api/AddressApiController.cs** - API lấy địa chỉ
✅ **Views/Payment/Checkout.cshtml** - Form với dropdown cascade
✅ **database/import_vietnam_addresses.sql** - Data import script

## Kết luận

Hệ thống địa chỉ Việt Nam đã hoàn chỉnh với:
- ✅ 3-tier hierarchy (Province → District → Ward)
- ✅ RESTful API endpoints
- ✅ Cascade dropdown selection
- ✅ Auto-save to database
- ✅ Full validation
- ✅ 63 provinces with major cities' districts/wards

**Truy cập:** http://localhost:5101/Checkout → "Thêm địa chỉ mới"
