using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;

namespace JohnHenryFashionWeb.Controllers
{
    [Authorize(Policy = AdminPolicies.RequireAdminRole)]
    [Route("admin/support")]
    public class SupportManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SupportManagementController> _logger;

        public SupportManagementController(
            ApplicationDbContext context,
            ILogger<SupportManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Trang tổng quan hỗ trợ khách hàng
        /// </summary>
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            ViewData["CurrentSection"] = "Support";
            ViewData["Title"] = "Quản lý hỗ trợ khách hàng";

            // Thống kê
            var totalTickets = await _context.SupportTickets.CountAsync();
            var openTickets = await _context.SupportTickets.CountAsync(t => t.Status == "open");
            var inProgressTickets = await _context.SupportTickets.CountAsync(t => t.Status == "in_progress");
            var resolvedTickets = await _context.SupportTickets.CountAsync(t => t.Status == "resolved");

            // Tickets gần đây
            var recentTickets = await _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.AssignedAdmin)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();

            ViewBag.TotalTickets = totalTickets;
            ViewBag.OpenTickets = openTickets;
            ViewBag.InProgressTickets = inProgressTickets;
            ViewBag.ResolvedTickets = resolvedTickets;
            ViewBag.RecentTickets = recentTickets;

            return View("~/Views/Admin/Support.cshtml");
        }

        #region Support Tickets

        // GET: admin/support/tickets
        [HttpGet("tickets")]
        public async Task<IActionResult> Tickets(string? status = null, string? priority = null, string? category = null, string? search = null, int page = 1)
        {
            var query = _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.AssignedAdmin)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(priority))
            {
                query = query.Where(t => t.Priority.ToLower() == priority.ToLower());
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => 
                    (t.Subject != null && t.Subject.Contains(search)) || 
                    (t.TicketNumber != null && t.TicketNumber.Contains(search)) ||
                    (t.User != null && t.User.Email != null && t.User.Email.Contains(search)));
            }

            // Statistics
            var stats = new
            {
                TotalTickets = await _context.SupportTickets.CountAsync(),
                OpenTickets = await _context.SupportTickets.CountAsync(t => t.Status == "Open"),
                InProgressTickets = await _context.SupportTickets.CountAsync(t => t.Status == "InProgress"),
                ResolvedTickets = await _context.SupportTickets.CountAsync(t => t.Status == "Resolved"),
                ClosedTickets = await _context.SupportTickets.CountAsync(t => t.Status == "Closed"),
                HighPriorityCount = await _context.SupportTickets.CountAsync(t => t.Priority == "High" && t.Status != "Closed"),
                UnassignedCount = await _context.SupportTickets.CountAsync(t => t.AssignedTo == null && t.Status != "Closed")
            };

            // Pagination
            var pageSize = 20;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Statistics = stats;
            ViewBag.StatusFilter = status;
            ViewBag.PriorityFilter = priority;
            ViewBag.CategoryFilter = category;
            ViewBag.SearchTerm = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(tickets);
        }

        // GET: admin/support/ticket/{id}
        [HttpGet("ticket/{id}")]
        public async Task<IActionResult> TicketDetails(Guid id)
        {
            var ticket = await _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.AssignedAdmin)
                .Include(t => t.Replies)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            // Get available admins for assignment
            var adminRoleId = await _context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var admins = await _context.UserRoles
                .Where(ur => ur.RoleId == adminRoleId)
                .Join(_context.Users,
                    ur => ur.UserId,
                    u => u.Id,
                    (ur, u) => new { u.Id, u.Email })
                .OrderBy(u => u.Email)
                .ToListAsync();

            ViewBag.Admins = admins;

            return View(ticket);
        }

        // POST: admin/support/ticket/{id}/assign
        [HttpPost("ticket/{id}/assign")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(Guid id, string adminId)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.AssignedTo = adminId;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (ticket.Status == "Open")
            {
                ticket.Status = "InProgress";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Ticket {ticket.TicketNumber} assigned to admin {adminId} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = "Ticket đã được gán thành công!";
            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        // POST: admin/support/ticket/{id}/reply
        [HttpPost("ticket/{id}/reply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReplyTicket(Guid id, string message, bool isInternal = false)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập nội dung trả lời!";
                return RedirectToAction(nameof(TicketDetails), new { id });
            }

            var reply = new TicketReply
            {
                TicketId = id,
                UserId = User.Identity!.Name!,
                Message = message,
                IsInternal = isInternal,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketReplies.Add(reply);

            // Update ticket status
            if (ticket.Status == "Open")
            {
                ticket.Status = "InProgress";
            }
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Reply added to ticket {ticket.TicketNumber} by {User.Identity.Name}");

            TempData["SuccessMessage"] = "Trả lời đã được gửi thành công!";
            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        // POST: admin/support/ticket/{id}/status
        [HttpPost("ticket/{id}/status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicketStatus(Guid id, string status)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Open", "InProgress", "Resolved", "Closed" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest("Invalid status");
            }

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (status == "Resolved")
            {
                ticket.ResolvedAt = DateTime.UtcNow;
            }
            else if (status == "Closed")
            {
                ticket.ClosedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Ticket {ticket.TicketNumber} status changed to {status} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = $"Trạng thái ticket đã được cập nhật thành {status}!";
            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        // POST: admin/support/ticket/{id}/priority
        [HttpPost("ticket/{id}/priority")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTicketPriority(Guid id, string priority)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var validPriorities = new[] { "Low", "Medium", "High", "Urgent" };
            if (!validPriorities.Contains(priority))
            {
                return BadRequest("Invalid priority");
            }

            ticket.Priority = priority;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Ticket {ticket.TicketNumber} priority changed to {priority} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = $"Độ ưu tiên đã được cập nhật thành {priority}!";
            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        #endregion

        #region Disputes

        // GET: admin/support/disputes
        [HttpGet("disputes")]
        public async Task<IActionResult> Disputes(string? status = null, string? search = null, int page = 1)
        {
            var query = _context.Disputes
                .Include(d => d.Order)
                .Include(d => d.Customer)
                .Include(d => d.Seller)
                .Include(d => d.Resolver)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(d => d.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => 
                    (d.DisputeNumber != null && d.DisputeNumber.Contains(search)) ||
                    (d.Order != null && d.Order.OrderNumber != null && d.Order.OrderNumber.Contains(search)));
            }

            // Statistics
            var stats = new
            {
                TotalDisputes = await _context.Disputes.CountAsync(),
                PendingCount = await _context.Disputes.CountAsync(d => d.Status == "Pending"),
                UnderReviewCount = await _context.Disputes.CountAsync(d => d.Status == "UnderReview"),
                ResolvedCount = await _context.Disputes.CountAsync(d => d.Status == "Resolved"),
                CustomerFavorCount = await _context.Disputes.CountAsync(d => d.ResolutionType == "CustomerFavor"),
                SellerFavorCount = await _context.Disputes.CountAsync(d => d.ResolutionType == "SellerFavor")
            };

            // Pagination
            var pageSize = 15;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));

            var disputes = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Statistics = stats;
            ViewBag.StatusFilter = status;
            ViewBag.SearchTerm = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(disputes);
        }

        // GET: admin/support/dispute/{id}
        [HttpGet("dispute/{id}")]
        public async Task<IActionResult> DisputeDetails(Guid id)
        {
            var dispute = await _context.Disputes
                .Include(d => d.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .Include(d => d.Customer)
                .Include(d => d.Seller)
                .Include(d => d.Resolver)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dispute == null)
            {
                return NotFound();
            }

            return View(dispute);
        }

        // POST: admin/support/dispute/{id}/resolve
        [HttpPost("dispute/{id}/resolve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResolveDispute(Guid id, string outcome, string resolution, decimal? refundAmount = null)
        {
            var dispute = await _context.Disputes.FindAsync(id);
            if (dispute == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(resolution))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập giải pháp!";
                return RedirectToAction(nameof(DisputeDetails), new { id });
            }

            var validOutcomes = new[] { "CustomerFavor", "SellerFavor", "Compromise", "NoFault" };
            if (!validOutcomes.Contains(outcome))
            {
                return BadRequest("Invalid outcome");
            }

            dispute.Status = "Resolved";
            dispute.ResolutionType = outcome;
            dispute.Resolution = resolution;
            dispute.ResolvedAt = DateTime.UtcNow;
            dispute.ResolvedBy = User.Identity!.Name;
            dispute.UpdatedAt = DateTime.UtcNow;

            if (refundAmount.HasValue && refundAmount.Value > 0)
            {
                dispute.RefundAmount = refundAmount.Value;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Dispute {dispute.DisputeNumber} resolved with outcome {outcome} by {User.Identity.Name}");

            TempData["SuccessMessage"] = "Tranh chấp đã được giải quyết thành công!";
            return RedirectToAction(nameof(DisputeDetails), new { id });
        }

        // POST: admin/support/dispute/{id}/respond-seller
        [HttpPost("dispute/{id}/respond-seller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSellerResponse(Guid id, string response)
        {
            var dispute = await _context.Disputes.FindAsync(id);
            if (dispute == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(response))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập phản hồi của seller!";
                return RedirectToAction(nameof(DisputeDetails), new { id });
            }

            dispute.SellerResponse = response;
            dispute.SellerRespondedAt = DateTime.UtcNow;
            dispute.UpdatedAt = DateTime.UtcNow;

            if (dispute.Status == "Pending")
            {
                dispute.Status = "UnderReview";
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Phản hồi của seller đã được lưu!";
            return RedirectToAction(nameof(DisputeDetails), new { id });
        }

        #endregion

        #region FAQs

        // GET: admin/support/faqs
        [HttpGet("faqs")]
        public async Task<IActionResult> FAQs(string? category = null, string? search = null)
        {
            var query = _context.FAQs.AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(f => f.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(f => 
                    (f.Question != null && f.Question.Contains(search)) || 
                    (f.Answer != null && f.Answer.Contains(search)));
            }

            var faqs = await query
                .OrderBy(f => f.SortOrder)
                .ThenByDescending(f => f.CreatedAt)
                .ToListAsync();

            // Get categories
            var categories = await _context.FAQs
                .Select(f => f.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.CategoryFilter = category;
            ViewBag.SearchTerm = search;

            return View(faqs);
        }

        // GET: admin/support/faq/create
        [HttpGet("faq/create")]
        public IActionResult CreateFAQ()
        {
            return View();
        }

        // POST: admin/support/faq/create
        [HttpPost("faq/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFAQ(FAQ faq)
        {
            if (string.IsNullOrWhiteSpace(faq.Question) || string.IsNullOrWhiteSpace(faq.Answer))
            {
                ModelState.AddModelError("", "Câu hỏi và câu trả lời không được để trống!");
                return View(faq);
            }

            faq.CreatedAt = DateTime.UtcNow;
            faq.UpdatedAt = DateTime.UtcNow;

            _context.FAQs.Add(faq);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"FAQ created: {faq.Question} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = "FAQ đã được tạo thành công!";
            return RedirectToAction(nameof(FAQs));
        }

        // GET: admin/support/faq/{id}/edit
        [HttpGet("faq/{id}/edit")]
        public async Task<IActionResult> EditFAQ(Guid id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            return View(faq);
        }

        // POST: admin/support/faq/{id}/edit
        [HttpPost("faq/{id}/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFAQ(Guid id, FAQ faq)
        {
            if (id != faq.Id)
            {
                return BadRequest();
            }

            var existingFAQ = await _context.FAQs.FindAsync(id);
            if (existingFAQ == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(faq.Question) || string.IsNullOrWhiteSpace(faq.Answer))
            {
                ModelState.AddModelError("", "Câu hỏi và câu trả lời không được để trống!");
                return View(faq);
            }

            existingFAQ.Category = faq.Category;
            existingFAQ.Question = faq.Question;
            existingFAQ.Answer = faq.Answer;
            existingFAQ.SortOrder = faq.SortOrder;
            existingFAQ.IsActive = faq.IsActive;
            existingFAQ.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"FAQ updated: {existingFAQ.Question} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = "FAQ đã được cập nhật thành công!";
            return RedirectToAction(nameof(FAQs));
        }

        // POST: admin/support/faq/{id}/delete
        [HttpPost("faq/{id}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFAQ(Guid id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            _context.FAQs.Remove(faq);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"FAQ deleted: {faq.Question} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = "FAQ đã được xóa thành công!";
            return RedirectToAction(nameof(FAQs));
        }

        // POST: admin/support/faq/{id}/toggle
        [HttpPost("faq/{id}/toggle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFAQ(Guid id)
        {
            var faq = await _context.FAQs.FindAsync(id);
            if (faq == null)
            {
                return NotFound();
            }

            faq.IsActive = !faq.IsActive;
            faq.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"FAQ {faq.Question} active status changed to {faq.IsActive} by {User.Identity!.Name}");

            TempData["SuccessMessage"] = $"FAQ đã được {(faq.IsActive ? "kích hoạt" : "ẩn")}!";
            return RedirectToAction(nameof(FAQs));
        }

        #endregion
    }
}
