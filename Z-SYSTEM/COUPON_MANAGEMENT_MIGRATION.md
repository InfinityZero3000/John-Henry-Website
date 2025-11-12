# Coupon Management Migration

## Summary
Consolidated coupon management to use **ONLY** Admin Controller (`/admin/coupons`) with modal-based UI.

## Changes Made (Nov 5, 2025)

### ✅ Removed Files:
- `/Views/Coupon/Create.cshtml` - Deleted
- `/Views/Coupon/Edit.cshtml` - Deleted
- `/Views/Coupon/Manage.cshtml` - Deleted
- `/Views/Coupon/Details.cshtml` - Deleted
- `/Views/Coupon/Delete.cshtml` - Deleted
- `/Views/Coupon/` directory - Deleted (empty)

### ✅ Modified Controllers:

**CouponController.cs:**
- `Create()` GET → Redirects to `/admin/coupons`
- `Create(Coupon)` POST → Redirects to `/admin/coupons`
- `Edit(Guid)` GET → Redirects to `/admin/coupons`
- `Edit(Guid, Coupon)` POST → Redirects to `/admin/coupons`
- `Manage()` → Redirects to `/admin/coupons`
- Original code preserved in comments for reference

### ✅ Active Coupon Management:

**AdminController.cs:**
- `Coupons()` - Main page: `/admin/coupons`
- `CreateCoupon()` GET - Route: `/admin/coupons/create`
- `CreateCoupon(Coupon)` POST - Route: `/admin/coupons/create`
- `SaveCoupon(Coupon)` - AJAX endpoint: `/admin/coupons/save`
- `GetCoupon(Guid)` - AJAX endpoint: `/admin/coupons/get/{id}`
- `ToggleCouponStatus(Guid)` - AJAX endpoint: `/admin/coupons/toggle/{id}`

**Admin View:**
- `/Views/Admin/Coupons.cshtml` - Main page with Bootstrap modal

## Usage

### Create/Edit Coupon:
1. Navigate to: `http://localhost:5101/admin/coupons`
2. Click "Tạo mã giảm giá" button
3. Fill form in modal
4. Submit via AJAX

### Features:
- ✅ Modal-based UI (no page reload)
- ✅ AJAX save with JSON
- ✅ Real-time validation
- ✅ Search & filter
- ✅ Inline toggle active/inactive
- ✅ Pagination
- ✅ Statistics cards

## API Endpoints

### GET
- `/admin/coupons` - List all coupons
- `/admin/coupons/get/{id}` - Get coupon by ID (JSON)

### POST
- `/admin/coupons/save` - Create/Update coupon (JSON body)
- `/admin/coupons/toggle/{id}` - Toggle active status

## Data Model

```csharp
public class Coupon
{
    public Guid Id { get; set; }
    public string Code { get; set; }          // Required, unique
    public string Name { get; set; }          // Required
    public string? Description { get; set; }
    public string Type { get; set; }          // "percentage" or "fixed_amount"
    public decimal Value { get; set; }        // >0, ≤100 for percentage
    public decimal? MinOrderAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Validation Rules

1. **Code**: Required, unique, auto-uppercase
2. **Name**: Required
3. **Type**: Must be "percentage" or "fixed_amount"
4. **Value**: 
   - Must be > 0
   - If percentage: ≤ 100
5. **Dates**: EndDate must be after StartDate (if both provided)

## Benefits of Consolidation

1. **Single Source of Truth**: All coupon CRUD in one place
2. **Consistent UI**: Modal-based, no navigation needed
3. **Better UX**: No page reloads, faster interactions
4. **Easier Maintenance**: Fix bugs in one location only
5. **Cleaner Codebase**: Removed duplicate views and actions

## Migration Notes

- Old URLs (`/Coupon/Create`, `/Coupon/Edit`, `/Coupon/Manage`) now redirect to `/admin/coupons`
- Original controller logic preserved in comments for reference
- No database changes required
- No breaking changes to Apply/Validate coupon functionality (used by checkout)

## Testing Checklist

- [ ] Create new coupon via modal
- [ ] Edit existing coupon
- [ ] Toggle coupon active/inactive
- [ ] Search coupons by code/name
- [ ] Pagination works
- [ ] Validation displays correctly
- [ ] Duplicate code prevention
- [ ] Date range validation
- [ ] Statistics cards update
- [ ] Apply coupon at checkout still works

## Related Files

- `Controllers/AdminController.cs` - Lines 1409-1580 (Coupon actions)
- `Controllers/CouponController.cs` - Apply/Validate actions (unchanged)
- `Views/Admin/Coupons.cshtml` - Main UI
- `Models/DomainModels.cs` - Coupon model (lines 228-260)
