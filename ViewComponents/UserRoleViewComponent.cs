using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JohnHenryFashionWeb.Models;
using System;

namespace JohnHenryFashionWeb.ViewComponents
{
    public class UserRoleViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var isAdmin = false;
            var isSeller = false;

            if (User?.Identity?.IsAuthenticated == true && HttpContext?.User != null)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    if (currentUser != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(currentUser);
                        if (userRoles != null)
                        {
                            isAdmin = userRoles.Contains("Admin");
                            isSeller = userRoles.Contains("Seller");
                        }
                    }
                }
                catch (Exception)
                {
                    // Fallback: use User.IsInRole if async fails
                    try
                    {
                        var principal = HttpContext.User as System.Security.Claims.ClaimsPrincipal;
                        if (principal != null)
                        {
                            isAdmin = principal.IsInRole("Admin");
                            isSeller = principal.IsInRole("Seller");
                        }
                    }
                    catch
                    {
                        // Silently fail - user will not see dashboard button
                    }
                }
            }

            return View(new UserRoleViewModel { IsAdmin = isAdmin, IsSeller = isSeller });
        }
    }

    public class UserRoleViewModel
    {
        public bool IsAdmin { get; set; }
        public bool IsSeller { get; set; }
    }
}

