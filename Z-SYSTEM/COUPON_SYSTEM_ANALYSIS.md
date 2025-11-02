# Phân Tích Hệ Thống Mã Giảm Giá

## 🔍 Vấn Đề Phát Hiện

Hệ thống hiện tại có **2 bảng mã giảm giá khác nhau** và chúng **KHÔNG tích hợp** với nhau:

### 1. Bảng `Coupons` (Models/DomainModels.cs - dòng 228)
- **Quản lý bởi**: `CouponController.cs` và trang `/Admin/Coupons`
- **Chức năng**:
  - Admin có thể tạo/sửa/xóa mã giảm giá
  - Có các API: `/Coupon/Apply`, `/Coupon/Calculate`, `/Coupon/Available`
  - Có đầy đủ validation (ngày hết hạn, số lần sử dụng, đơn tối thiểu)
- **⚠️ VẤN ĐỀ**: Không được sử dụng trong quá trình checkout!

### 2. Bảng `Promotions` (Models/DomainModels.cs - dòng 610)
- **Được sử dụng bởi**: `CheckoutController.cs` - hàm `CalculateDiscountAsync()` (dòng 684)
- **Chức năng**:
  - Được áp dụng khi khách hàng checkout
  - Tính toán giảm giá thực tế cho đơn hàng
- **⚠️ VẤN ĐỀ**: Không có giao diện quản lý cho Admin!

## 📊 So Sánh Cấu Trúc

| Trường | Coupon | Promotion | Ghi chú |
|--------|--------|-----------|---------|
| Code | ✅ | ✅ | Mã giảm giá |
| Name | ✅ | ✅ | Tên |
| Type | ✅ | ✅ | percentage / fixed_amount |
| Value | ✅ | ✅ | Giá trị giảm |
| MinOrderAmount | ✅ | ✅ | Đơn tối thiểu |
| MaxDiscountAmount | ❌ (NotMapped) | ✅ | Giảm tối đa |
| UsageLimit | ✅ | ✅ | Số lần dùng |
| UsageCount | ✅ | ✅ | Đã dùng |
| StartDate | ✅ | ✅ | Ngày bắt đầu |
| EndDate | ✅ | ✅ | Ngày kết thúc |
| IsActive | ✅ | ✅ | Trạng thái |
| Description | ✅ | ✅ | Mô tả |
| **CouponUsages** | ✅ | ❌ | Lịch sử sử dụng chi tiết |
| ApplicableProductIds | ❌ | ✅ | SP áp dụng |
| ApplicableCategoryIds | ❌ | ✅ | Danh mục áp dụng |

## 🔥 Tác Động

### ❌ Hiện Tại (Không Hoạt Động)
```
Admin tạo mã "SUMMER2024" → Lưu vào bảng Coupons
                                    ↓
Khách hàng nhập "SUMMER2024" → CheckoutController tìm trong Promotions
                                    ↓
                        ❌ KHÔNG TÌM THẤY → Mã không hợp lệ!
```

### ✅ Nếu Sửa (Sẽ Hoạt Động)
```
Admin tạo mã "SUMMER2024" → Lưu vào bảng Coupons
                                    ↓
Khách hàng nhập "SUMMER2024" → CheckoutController tìm trong Coupons
                                    ↓
                        ✅ TÌM THẤY → Áp dụng giảm giá thành công!
```

## 💡 Giải Pháp Đề Xuất

### Option 1: Sửa CheckoutController (Khuyến Nghị) ⭐
**Ưu điểm**: 
- Nhanh chóng, ít thay đổi
- Tận dụng UI admin đã có
- Không cần migration database

**Thay đổi cần thiết**:
```csharp
// File: Controllers/CheckoutController.cs - dòng 689
// TỪ:
var promotion = await _context.Promotions
    .FirstOrDefaultAsync(p => p.Code == couponCode && ...);

// ĐỔI THÀNH:
var coupon = await _context.Coupons
    .FirstOrDefaultAsync(c => c.Code == couponCode && 
                            c.IsActive && 
                            (c.StartDate == null || c.StartDate <= DateTime.UtcNow) &&
                            (c.EndDate == null || c.EndDate >= DateTime.UtcNow) &&
                            (c.UsageLimit == null || c.UsageCount < c.UsageLimit) &&
                            (c.MinOrderAmount == null || subtotal >= c.MinOrderAmount));

if (coupon == null)
    return 0;

var discount = coupon.Type switch
{
    "percentage" => subtotal * (coupon.Value / 100),
    "fixed_amount" => coupon.Value,
    _ => 0
};

return discount;
```

### Option 2: Migration Dữ Liệu
**Ưu điểm**: Thống nhất database
**Nhược điểm**: Phức tạp, có thể mất dữ liệu cũ

### Option 3: Kết Hợp Cả 2 Bảng
**Ưu điểm**: Linh hoạt
**Nhược điểm**: Phức tạp, khó maintain

## 🛠️ Các File Cần Sửa

### 1. CheckoutController.cs (Bắt buộc)
- **Dòng 684-710**: Hàm `CalculateDiscountAsync()`
- **Dòng 306**: Gọi `CalculateDiscountAsync()`
- **Dòng 326**: Lưu `CouponCode` vào Order

### 2. Views/Checkout/Index.cshtml (Kiểm tra)
- **Dòng 392-405**: UI nhập mã giảm giá
- **Dòng 717**: Hàm JavaScript `applyCoupon()`

### 3. CouponController.cs (Tùy chọn - Thêm API)
- Có thể thêm endpoint `/Coupon/IncrementUsage` để tăng `UsageCount` sau khi thanh toán

## 📝 Các Bước Thực Hiện

### Bước 1: Sửa CheckoutController
```bash
# Mở file
Controllers/CheckoutController.cs

# Tìm hàm CalculateDiscountAsync (dòng 684)
# Thay đổi từ _context.Promotions sang _context.Coupons
```

### Bước 2: Test Chức Năng
1. Tạo mã giảm giá mới trong Admin → Coupons
2. Thêm sản phẩm vào giỏ hàng
3. Vào trang Checkout
4. Nhập mã giảm giá vừa tạo
5. Kiểm tra xem có áp dụng thành công không

### Bước 3: Cập Nhật UsageCount
Sau khi đơn hàng thành công, cần tăng `UsageCount`:
```csharp
// Trong CheckoutController - sau khi tạo Order
if (!string.IsNullOrEmpty(order.CouponCode))
{
    var coupon = await _context.Coupons
        .FirstOrDefaultAsync(c => c.Code == order.CouponCode);
    if (coupon != null)
    {
        coupon.UsageCount++;
        await _context.SaveChangesAsync();
    }
}
```

## 🎯 Kết Luận

**Trạng thái hiện tại**: ❌ Mã giảm giá KHÔNG hoạt động
**Nguyên nhân**: Sử dụng 2 bảng khác nhau không đồng bộ
**Giải pháp**: Sửa CheckoutController để sử dụng bảng Coupons
**Thời gian ước tính**: 30 phút - 1 giờ

---

**Tạo bởi**: GitHub Copilot  
**Ngày**: 30/10/2025  
**Tình trạng**: Cần sửa ngay
