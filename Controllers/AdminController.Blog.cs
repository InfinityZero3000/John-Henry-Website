using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.ViewModels;

namespace JohnHenryFashionWeb.Controllers
{
    public partial class AdminController
    {
        #region Blog Management

        [HttpGet("blog")]
        public async Task<IActionResult> Blog(int page = 1, string? status = null, string? category = null, string? search = null)
        {
            const int pageSize = 20;
            
            var query = _context.BlogPosts
                .Include(b => b.Category)
                .Include(b => b.Author)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            // Filter by category
            if (!string.IsNullOrEmpty(category) && Guid.TryParse(category, out var categoryId))
            {
                query = query.Where(b => b.CategoryId == categoryId);
            }

            // Search functionality
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) || 
                                       b.Content.Contains(search) ||
                                       (b.Excerpt != null && b.Excerpt.Contains(search)));
            }

            var totalPosts = await query.CountAsync();
            var posts = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categories = await _context.BlogCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var viewModel = new AdminBlogViewModel
            {
                Posts = posts,
                Categories = categories,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalPosts / pageSize),
                TotalPosts = totalPosts,
                StatusFilter = status,
                CategoryFilter = category,
                SearchTerm = search
            };

            return View(viewModel);
        }

        [HttpGet("blog/create")]
        public async Task<IActionResult> CreateBlogPost()
        {
            var categories = await _context.BlogCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(new BlogPost());
        }

        [HttpPost("blog/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogPost(BlogPost model, IFormFile? featuredImage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = _userManager.GetUserId(User);
                    
                    model.Id = Guid.NewGuid();
                    model.AuthorId = userId!;
                    model.CreatedAt = DateTime.UtcNow;
                    model.UpdatedAt = DateTime.UtcNow;
                    
                    // Generate slug if not provided
                    if (string.IsNullOrEmpty(model.Slug))
                    {
                        model.Slug = GenerateSlug(model.Title);
                    }

                    // Handle featured image upload
                    if (featuredImage != null && featuredImage.Length > 0)
                    {
                        var fileName = await SaveBlogImage(featuredImage);
                        model.FeaturedImageUrl = $"/images/blog/{fileName}";
                    }

                    // Set publish date if publishing
                    if (model.Status == "published" && !model.PublishedAt.HasValue)
                    {
                        model.PublishedAt = DateTime.UtcNow;
                    }

                    _context.BlogPosts.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Bài viết đã được tạo thành công!";
                    return RedirectToAction("Blog");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
            }

            var categories = await _context.BlogCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(model);
        }

        [HttpGet("blog/edit/{id}")]
        public async Task<IActionResult> EditBlogPost(Guid id)
        {
            var post = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var categories = await _context.BlogCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(post);
        }

        [HttpPost("blog/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlogPost(Guid id, BlogPost model, IFormFile? featuredImage)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingPost = await _context.BlogPosts.FindAsync(id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    // Update fields
                    existingPost.Title = model.Title;
                    existingPost.Slug = string.IsNullOrEmpty(model.Slug) ? GenerateSlug(model.Title) : model.Slug;
                    existingPost.Excerpt = model.Excerpt;
                    existingPost.Content = model.Content;
                    existingPost.CategoryId = model.CategoryId;
                    existingPost.Tags = model.Tags;
                    existingPost.MetaTitle = model.MetaTitle;
                    existingPost.MetaDescription = model.MetaDescription;
                    existingPost.IsFeatured = model.IsFeatured;
                    existingPost.Status = model.Status;
                    existingPost.UpdatedAt = DateTime.UtcNow;

                    // Handle featured image upload
                    if (featuredImage != null && featuredImage.Length > 0)
                    {
                        var fileName = await SaveBlogImage(featuredImage);
                        existingPost.FeaturedImageUrl = $"/images/blog/{fileName}";
                    }

                    // Set publish date if publishing for first time
                    if (model.Status == "published" && !existingPost.PublishedAt.HasValue)
                    {
                        existingPost.PublishedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Bài viết đã được cập nhật thành công!";
                    return RedirectToAction("Blog");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
            }

            var categories = await _context.BlogCategories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;
            return View(model);
        }

        [HttpPost("blog/delete/{id}")]
        public async Task<IActionResult> DeleteBlogPost(Guid id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết" });
            }

            try
            {
                _context.BlogPosts.Remove(post);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa bài viết thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost("blog/toggle-status/{id}")]
        public async Task<IActionResult> ToggleBlogStatus(Guid id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài viết" });
            }

            try
            {
                post.Status = post.Status == "published" ? "draft" : "published";
                
                if (post.Status == "published" && !post.PublishedAt.HasValue)
                {
                    post.PublishedAt = DateTime.UtcNow;
                }

                post.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    message = $"Trạng thái bài viết đã được thay đổi thành '{(post.Status == "published" ? "Đã xuất bản" : "Bản nháp")}'!",
                    newStatus = post.Status
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        #region Blog Categories

        [HttpGet("blog/categories")]
        public async Task<IActionResult> BlogCategories()
        {
            var categories = await _context.BlogCategories
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();

            return View(categories);
        }

        [HttpGet("blog/categories/create")]
        public IActionResult CreateBlogCategory()
        {
            return View(new BlogCategory());
        }

        [HttpPost("blog/categories/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogCategory(BlogCategory model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.Id = Guid.NewGuid();
                    model.CreatedAt = DateTime.UtcNow;
                    model.UpdatedAt = DateTime.UtcNow;
                    
                    if (string.IsNullOrEmpty(model.Slug))
                    {
                        model.Slug = GenerateSlug(model.Name);
                    }

                    _context.BlogCategories.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Danh mục blog đã được tạo thành công!";
                    return RedirectToAction("BlogCategories");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
            }

            return View(model);
        }

        [HttpGet("blog/categories/edit/{id}")]
        public async Task<IActionResult> EditBlogCategory(Guid id)
        {
            var category = await _context.BlogCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost("blog/categories/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlogCategory(Guid id, BlogCategory model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingCategory = await _context.BlogCategories.FindAsync(id);
                    if (existingCategory == null)
                    {
                        return NotFound();
                    }

                    existingCategory.Name = model.Name;
                    existingCategory.Slug = string.IsNullOrEmpty(model.Slug) ? GenerateSlug(model.Name) : model.Slug;
                    existingCategory.Description = model.Description;
                    existingCategory.IsActive = model.IsActive;
                    existingCategory.SortOrder = model.SortOrder;
                    existingCategory.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Danh mục blog đã được cập nhật thành công!";
                    return RedirectToAction("BlogCategories");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
            }

            return View(model);
        }

        [HttpPost("blog/categories/delete/{id}")]
        public async Task<IActionResult> DeleteBlogCategory(Guid id)
        {
            var category = await _context.BlogCategories
                .Include(c => c.BlogPosts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return Json(new { success = false, message = "Không tìm thấy danh mục" });
            }

            if (category.BlogPosts?.Any() == true)
            {
                return Json(new { success = false, message = "Không thể xóa danh mục đang có bài viết" });
            }

            try
            {
                _context.BlogCategories.Remove(category);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        #endregion

        #region Helper Methods

        private async Task<string> SaveBlogImage(IFormFile image)
        {
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "blog");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return fileName;
        }

        #endregion

        #endregion
    }
}
