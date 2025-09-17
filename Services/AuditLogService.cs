using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace JohnHenryFashionWeb.Services
{
    public interface IAuditLogService
    {
        Task LogUserActionAsync(string userId, string action, string details, string? ipAddress = null);
        Task LogAdminActionAsync(string adminId, string action, string targetUserId, string details, string? ipAddress = null);
        Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50);
        Task<List<AuditLog>> GetSystemAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 100);
        Task LogSecurityEventAsync(string eventType, string description, string? userId = null, string? ipAddress = null);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly ApplicationDbContext _context;

        public AuditLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogUserActionAsync(string userId, string action, string details, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                UserAgent = GetUserAgent()
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogAdminActionAsync(string adminId, string action, string targetUserId, string details, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = adminId,
                Action = $"ADMIN_{action}",
                TargetUserId = targetUserId,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                UserAgent = GetUserAgent()
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50)
        {
            return await _context.AuditLogs
                .Where(log => log.UserId == userId || log.TargetUserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetSystemAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 100)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(log => log.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(log => log.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(log => log.Timestamp)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task LogSecurityEventAsync(string eventType, string description, string? userId = null, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = $"SECURITY_{eventType}",
                Details = description,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow,
                UserAgent = GetUserAgent()
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        private string GetUserAgent()
        {
            // This would need to be injected from HttpContext in a real implementation
            return "System";
        }
    }
}