# KẾ HOẠCH NÂNG CẤP HARDCODE - JOHN HENRY WEBSITE

**Ngày tạo:** 21 tháng 10, 2025  
**Trạng thái:** ✅ 75% HOÀN THÀNH (3/4 tasks done)

**Progress:**
- ✅ Task 3: Account/Orders.cshtml Avatar (HOÀN THÀNH - 21 Oct 2025)
- ✅ Task 1: Admin/Permissions.cshtml (HOÀN THÀNH - 22 Oct 2025)
  - ✅ Bước 1.1: Tạo API Controller
  - ✅ Bước 1.2: Update JavaScript  
  - ✅ Bước 1.3: Thêm Edit Roles Modal
  - ✅ Bước 1.4: Testing (APIs verified)
- ✅ Task 2: Admin/Logs.cshtml (HOÀN THÀNH - 22 Oct 2025)
  - ✅ Bước 2.1: Tạo LogService
  - ✅ Bước 2.2: Tạo LogsApiController với 5 endpoints
  - ✅ Bước 2.3: Update JavaScript để gọi APIs
  - ✅ Bước 2.4: Register service trong Program.cs
- ⏸️ Task 4: Cart/Index.cshtml (Chưa bắt đầu - Optional)

---

## 📋 MỤC LỤC NHIỆM VỤ

- [Task 1: Nâng cấp Admin/Permissions.cshtml](#task-1-nâng-cấp-adminpermissionscshtml)
- [Task 2: Nâng cấp Admin/Logs.cshtml](#task-2-nâng-cấp-adminlogscshtml)
- [Task 3: Nâng cấp Account/Orders.cshtml Avatar](#task-3-nâng-cấp-accountorderscshtml-avatar)
- [Task 4: Nâng cấp Cart/Index.cshtml Recommendations](#task-4-nâng-cấp-cartindexcshtml-recommendations)

---

## Task 1: Nâng cấp Admin/Permissions.cshtml

### 🎯 Mục tiêu
Thay thế dữ liệu permissions và users hardcode bằng database queries thực tế.

### 📊 Hiện trạng
- File: `Views/Admin/Permissions.cshtml`
- Dòng: 259-330
- Vấn đề: JavaScript hardcode permissions và user data
- Ảnh hưởng: ⚠️ HIGH - Trang quản lý không hiển thị dữ liệu thực

### 🔧 Các bước thực hiện

#### Bước 1.1: Tạo API Controller cho Permissions
**File mới:** `Controllers/Api/PermissionsApiController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Controllers.Api
{
    [ApiController]
    [Route("api/admin/permissions")]
    [Authorize(Roles = "Admin")]
    public class PermissionsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionsApiController(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: api/admin/permissions/roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    normalizedName = r.NormalizedName
                })
                .ToListAsync();

            return Ok(roles);
        }

        // GET: api/admin/permissions/roles/{roleName}
        [HttpGet("roles/{roleName}")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return NotFound(new { error = "Role not found" });

            var permissions = await _context.RolePermissions
                .Where(p => p.RoleId == role.Id)
                .Select(p => new
                {
                    name = p.Permission,
                    module = p.Module,
                    granted = p.IsGranted
                })
                .ToListAsync();

            return Ok(permissions);
        }

        // GET: api/admin/permissions/users/search?term=xxx
        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return BadRequest(new { error = "Search term must be at least 2 characters" });

            var users = await _userManager.Users
                .Where(u => u.Email.Contains(term) || 
                           (u.FirstName + " " + u.LastName).Contains(term))
                .Take(10)
                .ToListAsync();

            var userDtos = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new
                {
                    id = user.Id,
                    email = user.Email,
                    name = $"{user.FirstName} {user.LastName}".Trim(),
                    roles = roles
                });
            }

            return Ok(userDtos);
        }

        // POST: api/admin/permissions/users/{userId}/roles
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> UpdateUserRoles(
            string userId, 
            [FromBody] UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var currentRoles = await _userManager.GetRolesAsync(user);
            
            // Remove old roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return BadRequest(new { error = "Failed to remove old roles" });

            // Add new roles
            var addResult = await _userManager.AddToRolesAsync(user, request.Roles);
            if (!addResult.Succeeded)
                return BadRequest(new { error = "Failed to add new roles" });

            return Ok(new { success = true, message = "User roles updated successfully" });
        }
    }

    public class UpdateUserRolesRequest
    {
        public List<string> Roles { get; set; } = new();
    }
}
```

**✅ Checklist:**
- [ ] Tạo file `Controllers/Api/PermissionsApiController.cs`
- [ ] Thêm using statements cần thiết
- [ ] Test API endpoints với Postman/curl
- [ ] Kiểm tra authorization (chỉ Admin)

---

#### Bước 1.2: Cập nhật View để gọi API

**File:** `Views/Admin/Permissions.cshtml`

**Thay thế function `loadRolePermissions`:**

```javascript
async function loadRolePermissions(roleName) {
    const permissionsList = document.getElementById('permissionsList');
    permissionsList.innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
    
    try {
        const response = await fetch(`/api/admin/permissions/roles/${encodeURIComponent(roleName)}`);
        if (!response.ok) throw new Error('Failed to load permissions');
        
        const permissions = await response.json();
        
        if (permissions.length === 0) {
            permissionsList.innerHTML = `
                <div style="text-align: center; color: var(--admin-text-light); padding: 2rem 0;">
                    <p>Vai trò này chưa có quyền hạn nào được cấu hình</p>
                </div>
            `;
            return;
        }
        
        permissionsList.innerHTML = permissions.map(perm => `
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.75rem;">
                <div>
                    <span>${perm.name}</span>
                    ${perm.module ? `<small style="color: var(--admin-text-light); display: block;">${perm.module}</small>` : ''}
                </div>
                <span class="admin-badge ${perm.granted ? 'admin-badge-success' : 'admin-badge-secondary'}">
                    ${perm.granted ? 'Có quyền' : 'Không có quyền'}
                </span>
            </div>
        `).join('');
        
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
    } catch (error) {
        console.error('Error loading permissions:', error);
        permissionsList.innerHTML = `
            <div class="alert alert-danger">
                Không thể tải danh sách quyền. Vui lòng thử lại.
            </div>
        `;
    }
}
```

**Thay thế function `searchUsers`:**

```javascript
let searchTimeout;
async function searchUsers() {
    const searchTerm = document.getElementById('userSearch').value;
    
    if (searchTerm.length < 2) {
        document.getElementById('userResults').innerHTML = `
            <div style="text-align: center; color: var(--admin-text-light); padding: 2rem 0;">
                <i data-lucide="search" style="width: 48px; height: 48px; margin-bottom: 0.5rem;"></i>
                <p>Nhập ít nhất 2 ký tự để tìm kiếm</p>
            </div>
        `;
        if (typeof lucide !== 'undefined') {
            lucide.createIcons();
        }
        return;
    }

    // Debounce search
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(async () => {
        const userResults = document.getElementById('userResults');
        userResults.innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
        
        try {
            const response = await fetch(`/api/admin/permissions/users/search?term=${encodeURIComponent(searchTerm)}`);
            if (!response.ok) throw new Error('Search failed');
            
            const users = await response.json();
            displayUsers(users);
        } catch (error) {
            console.error('Search error:', error);
            userResults.innerHTML = `
                <div class="alert alert-danger">
                    Không thể tìm kiếm người dùng. Vui lòng thử lại.
                </div>
            `;
        }
    }, 300);
}
```

**Thêm function `editUserRoles` với modal:**

```javascript
function editUserRoles(userId) {
    // Store userId for later use
    window.editingUserId = userId;
    
    // Show modal
    const modal = new bootstrap.Modal(document.getElementById('editRolesModal'));
    modal.show();
}

async function saveUserRoles() {
    const userId = window.editingUserId;
    if (!userId) return;
    
    const selectedRoles = [];
    document.querySelectorAll('.role-checkbox:checked').forEach(cb => {
        selectedRoles.push(cb.value);
    });
    
    try {
        const response = await fetch(`/api/admin/permissions/users/${userId}/roles`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify({ roles: selectedRoles })
        });
        
        if (!response.ok) throw new Error('Failed to update roles');
        
        const result = await response.json();
        alert(result.message || 'Cập nhật quyền thành công!');
        
        bootstrap.Modal.getInstance(document.getElementById('editRolesModal')).hide();
        
        // Refresh search results
        searchUsers();
    } catch (error) {
        console.error('Error updating roles:', error);
        alert('Không thể cập nhật quyền. Vui lòng thử lại.');
    }
}
```

**✅ Checklist:**
- [ ] Thay thế 3 functions: loadRolePermissions, searchUsers, editUserRoles
- [ ] Thêm modal cho edit roles
- [ ] Test search functionality
- [ ] Test permission display
- [ ] Test role assignment

---

#### Bước 1.3: Thêm Modal cho Edit Roles

**Thêm vào cuối file trước `</div>` cuối:**

```html
<!-- Edit User Roles Modal -->
<div class="modal fade" id="editRolesModal" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Chỉnh sửa vai trò</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label">Chọn vai trò:</label>
                    <div class="form-check">
                        <input class="form-check-input role-checkbox" type="checkbox" value="Admin" id="role_admin">
                        <label class="form-check-label" for="role_admin">Admin</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input role-checkbox" type="checkbox" value="Seller" id="role_seller">
                        <label class="form-check-label" for="role_seller">Seller</label>
                    </div>
                    <div class="form-check">
                        <input class="form-check-input role-checkbox" type="checkbox" value="Customer" id="role_customer">
                        <label class="form-check-label" for="role_customer">Customer</label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="admin-btn admin-btn-secondary" data-bs-dismiss="modal">Hủy</button>
                <button type="button" class="admin-btn admin-btn-primary" onclick="saveUserRoles()">Lưu thay đổi</button>
            </div>
        </div>
    </div>
</div>
```

**✅ Checklist:**
- [ ] Thêm modal HTML
- [ ] Test modal hiển thị
- [ ] Test save functionality

---

#### Bước 1.4: Testing

**Test Cases:**

1. **Test Load Permissions:**
   ```bash
   curl -X GET "http://localhost:5101/api/admin/permissions/roles/Admin" \
     -H "Cookie: .AspNetCore.Identity.Application=YOUR_COOKIE"
   ```

2. **Test Search Users:**
   ```bash
   curl -X GET "http://localhost:5101/api/admin/permissions/users/search?term=john" \
     -H "Cookie: .AspNetCore.Identity.Application=YOUR_COOKIE"
   ```

3. **Test Update Roles:**
   ```bash
   curl -X POST "http://localhost:5101/api/admin/permissions/users/USER_ID/roles" \
     -H "Content-Type: application/json" \
     -H "Cookie: .AspNetCore.Identity.Application=YOUR_COOKIE" \
     -d '{"roles":["Admin","Seller"]}'
   ```

**✅ Checklist:**
- [ ] Test với Admin account
- [ ] Test với non-Admin (should fail)
- [ ] Test search với nhiều keywords
- [ ] Test update roles
- [ ] Verify database changes

---

### ⏱️ Ước tính thời gian
- **Backend API:** 2-3 giờ
- **Frontend update:** 1-2 giờ
- **Testing:** 1 giờ
- **Tổng:** ~5 giờ

### ✅ Definition of Done
- [ ] API endpoints hoạt động chính xác
- [ ] View hiển thị dữ liệu từ database
- [ ] Không còn hardcoded data
- [ ] Tất cả test cases pass
- [ ] Code được review và merge

---

## Task 2: Nâng cấp Admin/Logs.cshtml

### 🎯 Mục tiêu
Connect tới log files thực tế và hiển thị log details từ server.

### 📊 Hiện trạng
- File: `Views/Admin/Logs.cshtml`
- Dòng: 489-550
- Vấn đề: Log details hardcoded với sample data
- Ảnh hưởng: ⚠️ HIGH - Không hiển thị logs thực

### 🔧 Các bước thực hiện

#### Bước 2.1: Tạo Log Service
**File mới:** `Services/LogService.cs`

```csharp
using System.Text.RegularExpressions;

namespace JohnHenryFashionWeb.Services
{
    public interface ILogService
    {
        Task<List<LogEntry>> GetLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? level = null);
        Task<LogEntry?> GetLogByIdAsync(string logId);
        Task<List<string>> GetLogFilesAsync();
        Task<string> GetLogFileContentAsync(string date);
    }

    public class LogService : ILogService
    {
        private readonly string _logDirectory;
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        }

        public async Task<List<LogEntry>> GetLogsAsync(
            DateTime? fromDate = null, 
            DateTime? toDate = null, 
            string? level = null)
        {
            var logs = new List<LogEntry>();
            
            if (!Directory.Exists(_logDirectory))
                return logs;

            var files = Directory.GetFiles(_logDirectory, "*.txt")
                .OrderByDescending(f => f);

            foreach (var file in files)
            {
                try
                {
                    var lines = await File.ReadAllLinesAsync(file);
                    foreach (var line in lines)
                    {
                        var logEntry = ParseLogLine(line);
                        if (logEntry != null)
                        {
                            // Filter by date
                            if (fromDate.HasValue && logEntry.Timestamp < fromDate.Value)
                                continue;
                            if (toDate.HasValue && logEntry.Timestamp > toDate.Value)
                                continue;
                            
                            // Filter by level
                            if (!string.IsNullOrEmpty(level) && 
                                !logEntry.Level.Equals(level, StringComparison.OrdinalIgnoreCase))
                                continue;

                            logs.Add(logEntry);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading log file: {File}", file);
                }
            }

            return logs.OrderByDescending(l => l.Timestamp).Take(1000).ToList();
        }

        public async Task<LogEntry?> GetLogByIdAsync(string logId)
        {
            var logs = await GetLogsAsync();
            return logs.FirstOrDefault(l => l.Id == logId);
        }

        public Task<List<string>> GetLogFilesAsync()
        {
            if (!Directory.Exists(_logDirectory))
                return Task.FromResult(new List<string>());

            var files = Directory.GetFiles(_logDirectory, "*.txt")
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Cast<string>()
                .OrderByDescending(f => f)
                .ToList();

            return Task.FromResult(files);
        }

        public async Task<string> GetLogFileContentAsync(string date)
        {
            var fileName = $"john-henry-{date}.txt";
            var filePath = Path.Combine(_logDirectory, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Log file not found");

            return await File.ReadAllTextAsync(filePath);
        }

        private LogEntry? ParseLogLine(string line)
        {
            // Parse Serilog format: [Timestamp] [Level] Message
            var pattern = @"\[(\d{2}:\d{2}:\d{2})\s+(\w+)\]\s+(.+)";
            var match = Regex.Match(line, pattern);

            if (!match.Success)
                return null;

            var timestamp = match.Groups[1].Value;
            var level = match.Groups[2].Value;
            var message = match.Groups[3].Value;

            return new LogEntry
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Today.Add(TimeSpan.Parse(timestamp)),
                Level = level,
                Message = message,
                Source = "Application"
            };
        }
    }

    public class LogEntry
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? UserEmail { get; set; }
        public string? IpAddress { get; set; }
        public string? RequestUrl { get; set; }
    }
}
```

**✅ Checklist:**
- [ ] Tạo file `Services/LogService.cs`
- [ ] Register service trong `Program.cs`
- [ ] Test log parsing

---

#### Bước 2.2: Tạo API cho Logs

**File mới:** `Controllers/Api/LogsApiController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JohnHenryFashionWeb.Services;

namespace JohnHenryFashionWeb.Controllers.Api
{
    [ApiController]
    [Route("api/admin/logs")]
    [Authorize(Roles = "Admin")]
    public class LogsApiController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsApiController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? level)
        {
            var logs = await _logService.GetLogsAsync(fromDate, toDate, level);
            return Ok(logs);
        }

        [HttpGet("{logId}")]
        public async Task<IActionResult> GetLogDetails(string logId)
        {
            var log = await _logService.GetLogByIdAsync(logId);
            if (log == null)
                return NotFound(new { error = "Log not found" });

            return Ok(log);
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetLogFiles()
        {
            var files = await _logService.GetLogFilesAsync();
            return Ok(files);
        }

        [HttpGet("files/{date}")]
        public async Task<IActionResult> GetLogFile(string date)
        {
            try
            {
                var content = await _logService.GetLogFileContentAsync(date);
                return Content(content, "text/plain");
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { error = "Log file not found" });
            }
        }
    }
}
```

**Register service trong `Program.cs`:**

```csharp
// Add sau dòng builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogService, LogService>();
```

**✅ Checklist:**
- [ ] Tạo LogsApiController
- [ ] Register LogService
- [ ] Test API endpoints

---

#### Bước 2.3: Update View

**File:** `Views/Admin/Logs.cshtml`

**Thay thế function `viewLogDetails`:**

```javascript
async function viewLogDetails(logId) {
    const modalContent = document.getElementById('logDetailContent');
    modalContent.innerHTML = '<div class="text-center"><div class="spinner-border"></div></div>';
    
    bootstrap.Modal.getOrCreateInstance(document.getElementById('logDetailModal')).show();
    
    try {
        const response = await fetch(`/api/admin/logs/${logId}`);
        if (!response.ok) throw new Error('Failed to load log details');
        
        const log = await response.json();
        
        modalContent.innerHTML = `
            <div class="row mb-3">
                <div class="col-sm-3"><strong>ID:</strong></div>
                <div class="col-sm-9">${log.id}</div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Timestamp:</strong></div>
                <div class="col-sm-9">${new Date(log.timestamp).toLocaleString('vi-VN')}</div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Level:</strong></div>
                <div class="col-sm-9">
                    <span class="badge ${getBadgeClass(log.level)}">${log.level}</span>
                </div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Source:</strong></div>
                <div class="col-sm-9">${log.source || 'N/A'}</div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Message:</strong></div>
                <div class="col-sm-9">${log.message}</div>
            </div>
            ${log.exception ? `
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Exception:</strong></div>
                <div class="col-sm-9">
                    <pre class="bg-light p-2 rounded">${log.exception}</pre>
                </div>
            </div>
            ` : ''}
            ${log.userEmail ? `
            <div class="row mb-3">
                <div class="col-sm-3"><strong>User:</strong></div>
                <div class="col-sm-9">${log.userEmail}</div>
            </div>
            ` : ''}
            ${log.ipAddress ? `
            <div class="row mb-3">
                <div class="col-sm-3"><strong>IP Address:</strong></div>
                <div class="col-sm-9">${log.ipAddress}</div>
            </div>
            ` : ''}
            ${log.requestUrl ? `
            <div class="row mb-3">
                <div class="col-sm-3"><strong>Request URL:</strong></div>
                <div class="col-sm-9">${log.requestUrl}</div>
            </div>
            ` : ''}
        `;
    } catch (error) {
        console.error('Error loading log details:', error);
        modalContent.innerHTML = `
            <div class="alert alert-danger">
                Không thể tải chi tiết log. Vui lòng thử lại.
            </div>
        `;
    }
}

function getBadgeClass(level) {
    switch(level.toUpperCase()) {
        case 'ERROR': return 'bg-danger';
        case 'WARNING': return 'bg-warning';
        case 'INFO': return 'bg-info';
        default: return 'bg-secondary';
    }
}
```

**✅ Checklist:**
- [ ] Update viewLogDetails function
- [ ] Test log details display
- [ ] Verify all log fields show correctly

---

### ⏱️ Ước tính thời gian
- **LogService:** 2 giờ
- **API Controller:** 1 giờ
- **View update:** 1 giờ
- **Testing:** 1 giờ
- **Tổng:** ~5 giờ

### ✅ Definition of Done
- [ ] Log service reads from actual log files
- [ ] API returns real log data
- [ ] View displays actual logs
- [ ] Log details modal shows complete info
- [ ] All test cases pass

---

## Task 3: Nâng cấp Account/Orders.cshtml Avatar

### 🎯 Mục tiêu
Thay thế placeholder avatar bằng User avatar từ database hoặc default local avatar.

### 📊 Hiện trạng
- File: `Views/Account/Orders.cshtml`
- Dòng: 22
- Vấn đề: Sử dụng `https://via.placeholder.com/80x80`
- Ảnh hưởng: ⚠️ MEDIUM - Phụ thuộc service bên ngoài

### 🔧 Các bước thực hiện

#### Bước 3.1: Thêm Default Avatar

**Tạo hoặc copy avatar mặc định:**

```bash
# Tạo thư mục nếu chưa có
mkdir -p wwwroot/images/avatars

# Copy hoặc tạo file default-avatar.png vào wwwroot/images/avatars/
```

**✅ Checklist:**
- [ ] Tạo/copy `wwwroot/images/avatars/default-avatar.png`
- [ ] Avatar có kích thước 200x200px trở lên
- [ ] File size < 50KB

---

#### Bước 3.2: Update View

**File:** `Views/Account/Orders.cshtml`

**Tìm và thay thế:**

```razor
<!-- TRƯỚC (dòng 20-25) -->
<div class="card-body">
    <div class="text-center mb-3">
        <img src="https://via.placeholder.com/80x80" class="rounded-circle mb-2" alt="Avatar" width="80" height="80">
        <h6 class="mb-1">@(User.Identity?.Name ?? "User")</h6>
        <small class="text-muted">Thành viên</small>
    </div>

<!-- SAU -->
@{
    var user = await UserManager.GetUserAsync(User);
    var avatarUrl = !string.IsNullOrEmpty(user?.Avatar) 
        ? user.Avatar 
        : "/images/avatars/default-avatar.png";
    var userName = !string.IsNullOrEmpty(user?.FirstName) 
        ? $"{user.FirstName} {user.LastName}".Trim()
        : (User.Identity?.Name ?? "User");
}
<div class="card-body">
    <div class="text-center mb-3">
        <img src="@avatarUrl" 
             class="rounded-circle mb-2" 
             alt="Avatar" 
             width="80" 
             height="80"
             onerror="this.src='/images/avatars/default-avatar.png'">
        <h6 class="mb-1">@userName</h6>
        <small class="text-muted">Thành viên</small>
    </div>
```

**✅ Checklist:**
- [ ] Inject UserManager vào view (đã có sẵn)
- [ ] Update avatar code
- [ ] Test với user có avatar
- [ ] Test với user không có avatar
- [ ] Test onerror fallback

---

### ⏱️ Ước tính thời gian
- **Tạo default avatar:** 15 phút
- **Update view:** 15 phút
- **Testing:** 30 phút
- **Tổng:** ~1 giờ

### ✅ Definition of Done
- [ ] Default avatar file tồn tại
- [ ] View sử dụng User.Avatar từ database
- [ ] Fallback to default avatar hoạt động
- [ ] Không còn dependency via.placeholder.com
- [ ] Tested với nhiều user accounts

---

## Task 4: Nâng cấp Cart/Index.cshtml Recommendations

### 🎯 Mục tiêu
Thay thế sản phẩm hardcode bằng recommendation engine thực tế.

### 📊 Hiện trạng
- File: `Views/Cart/Index.cshtml`
- Dòng: 243-268
- Vấn đề: 2 sản phẩm "Có thể bạn quan tâm" được hardcode
- Ảnh hưởng: ⚠️ MEDIUM - Không hiển thị sản phẩm thực

### 🔧 Các bước thực hiện

#### Bước 4.1: Tạo Recommendation Service

**File mới:** `Services/ProductRecommendationService.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Services
{
    public interface IProductRecommendationService
    {
        Task<List<Product>> GetRecommendedProductsAsync(List<Guid> cartProductIds, int count = 4);
    }

    public class ProductRecommendationService : IProductRecommendationService
    {
        private readonly ApplicationDbContext _context;

        public ProductRecommendationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetRecommendedProductsAsync(
            List<Guid> cartProductIds, 
            int count = 4)
        {
            if (!cartProductIds.Any())
            {
                // If cart is empty, return popular products
                return await GetPopularProductsAsync(count);
            }

            // Get categories of products in cart
            var cartCategories = await _context.Products
                .Where(p => cartProductIds.Contains(p.Id))
                .Select(p => p.CategoryId)
                .Distinct()
                .ToListAsync();

            // Find products in same categories, excluding cart items
            var recommendations = await _context.Products
                .Where(p => p.IsActive && 
                           cartCategories.Contains(p.CategoryId) &&
                           !cartProductIds.Contains(p.Id))
                .OrderByDescending(p => p.ViewCount)
                .ThenByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();

            // If not enough recommendations, fill with popular products
            if (recommendations.Count < count)
            {
                var needed = count - recommendations.Count;
                var popularProducts = await GetPopularProductsAsync(needed, 
                    recommendations.Select(p => p.Id).ToList());
                
                recommendations.AddRange(popularProducts);
            }

            return recommendations;
        }

        private async Task<List<Product>> GetPopularProductsAsync(
            int count, 
            List<Guid>? excludeIds = null)
        {
            excludeIds ??= new List<Guid>();

            return await _context.Products
                .Where(p => p.IsActive && !excludeIds.Contains(p.Id))
                .OrderByDescending(p => p.ViewCount)
                .ThenByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();
        }
    }
}
```

**Register trong `Program.cs`:**

```csharp
builder.Services.AddScoped<IProductRecommendationService, ProductRecommendationService>();
```

**✅ Checklist:**
- [ ] Tạo ProductRecommendationService
- [ ] Register service
- [ ] Test recommendation logic

---

#### Bước 4.2: Update CartController

**File:** `Controllers/CartController.cs`

**Thêm vào constructor:**

```csharp
private readonly IProductRecommendationService _recommendationService;

public CartController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IProductRecommendationService recommendationService)
{
    _context = context;
    _userManager = userManager;
    _recommendationService = recommendationService;
}
```

**Update Index action:**

```csharp
public async Task<IActionResult> Index()
{
    List<ShoppingCartItem> cartItems;
    
    if (User.Identity?.IsAuthenticated == true)
    {
        var userId = _userManager.GetUserId(User);
        cartItems = await _context.ShoppingCartItems
            .Include(item => item.Product)
            .Where(item => item.UserId == userId)
            .ToListAsync();
    }
    else
    {
        var sessionId = HttpContext.Session.GetString("SessionId") ?? Guid.NewGuid().ToString();
        HttpContext.Session.SetString("SessionId", sessionId);
        
        cartItems = await _context.ShoppingCartItems
            .Include(item => item.Product)
            .Where(item => item.SessionId == sessionId)
            .ToListAsync();
    }

    // Get recommendations
    var cartProductIds = cartItems.Select(i => i.ProductId).ToList();
    var recommendations = await _recommendationService.GetRecommendedProductsAsync(cartProductIds, 4);
    
    ViewBag.RecommendedProducts = recommendations;
    
    return View(cartItems);
}
```

**✅ Checklist:**
- [ ] Inject IProductRecommendationService
- [ ] Update Index action
- [ ] Pass recommendations to view

---

#### Bước 4.3: Update View

**File:** `Views/Cart/Index.cshtml`

**Tìm và thay thế section "Có thể bạn quan tâm":**

```razor
<!-- TRƯỚC (dòng 240-268) -->
<div class="suggested-products mt-4">
    <h6 class="mb-3">Có thể bạn quan tâm</h6>
    <div class="row g-2">
        <!-- Placeholder for suggested products -->
        <div class="col-6">
            <div class="card h-100">
                <img src="~/images/placeholder-product.jpg" class="card-img-top" alt="Sản phẩm đề xuất">
                <div class="card-body p-2">
                    <small class="card-title">Áo thun basic</small>
                    <div class="text-primary">
                        <small><strong>299.000₫</strong></small>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-6">
            <div class="card h-100">
                <img src="~/images/placeholder-product.jpg" class="card-img-top" alt="Sản phẩm đề xuất">
                <div class="card-body p-2">
                    <small class="card-title">Quần jeans</small>
                    <div class="text-primary">
                        <small><strong>599.000₫</strong></small>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- SAU -->
@if (ViewBag.RecommendedProducts != null && ((List<Product>)ViewBag.RecommendedProducts).Any())
{
    <div class="suggested-products mt-4">
        <h6 class="mb-3">Có thể bạn quan tâm</h6>
        <div class="row g-2">
            @foreach (var product in (List<Product>)ViewBag.RecommendedProducts)
            {
                <div class="col-6">
                    <a href="@Url.Action("ProductDetail", "Products", new { slug = product.Slug })" 
                       class="text-decoration-none">
                        <div class="card h-100 product-card-hover">
                            <img src="@(!string.IsNullOrEmpty(product.FeaturedImageUrl) ? product.FeaturedImageUrl : "/images/placeholder.jpg")" 
                                 class="card-img-top" 
                                 alt="@product.Name"
                                 onerror="this.src='/images/placeholder.jpg'">
                            <div class="card-body p-2">
                                <small class="card-title text-dark d-block" 
                                       style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
                                    @product.Name
                                </small>
                                <div class="text-primary">
                                    @if (product.SalePrice.HasValue && product.SalePrice < product.Price)
                                    {
                                        <small class="text-muted text-decoration-line-through me-1">
                                            @product.Price.ToString("N0")₫
                                        </small>
                                        <small><strong>@product.SalePrice.Value.ToString("N0")₫</strong></small>
                                    }
                                    else
                                    {
                                        <small><strong>@product.Price.ToString("N0")₫</strong></small>
                                    }
                                </div>
                            </div>
                        </div>
                    </a>
                </div>
            }
        </div>
    </div>
}
```

**Thêm CSS cho hover effect:**

```css
<style>
    .product-card-hover {
        transition: transform 0.2s, box-shadow 0.2s;
    }
    
    .product-card-hover:hover {
        transform: translateY(-4px);
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
    }
</style>
```

**✅ Checklist:**
- [ ] Replace hardcoded products
- [ ] Add link to product detail
- [ ] Add hover effect
- [ ] Test with empty cart
- [ ] Test with items in cart
- [ ] Verify recommendations are relevant

---

### ⏱️ Ước tính thời gian
- **Recommendation Service:** 2 giờ
- **Controller update:** 30 phút
- **View update:** 1 giờ
- **Testing:** 1 giờ
- **Tổng:** ~4.5 giờ

### ✅ Definition of Done
- [ ] Recommendation service implemented
- [ ] Controller passes recommendations to view
- [ ] View displays actual products
- [ ] Products link to detail page
- [ ] Recommendations are relevant to cart
- [ ] Works with empty cart (shows popular)
- [ ] All test cases pass

---

## 📊 TỔNG KẾT

### Tổng thời gian ước tính
| Task | Thời gian | Ưu tiên |
|------|-----------|---------|
| Task 1: Permissions | ~5 giờ | 🔥 CAO |
| Task 2: Logs | ~5 giờ | 🔥 CAO |
| Task 3: Avatar | ~1 giờ | 🟡 TRUNG |
| Task 4: Recommendations | ~4.5 giờ | 🟡 TRUNG |
| **TỔNG** | **~15.5 giờ** | |

### Thứ tự thực hiện đề xuất
1. **Task 3** (Avatar) - Nhanh, dễ, ít rủi ro
2. **Task 1** (Permissions) - Quan trọng nhất cho admin
3. **Task 2** (Logs) - Cần cho monitoring
4. **Task 4** (Recommendations) - Cải thiện UX

### Ghi chú
- Tất cả tasks đều độc lập, có thể làm song song
- Mỗi task có test cases riêng
- Code cần review trước khi merge
- Update documentation sau khi hoàn thành

---

**Cập nhật lần cuối:** 21/10/2025
