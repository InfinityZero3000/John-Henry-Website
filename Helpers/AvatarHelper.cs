using Microsoft.AspNetCore.Hosting;

namespace JohnHenryFashionWeb.Helpers
{
    public static class AvatarHelper
    {
        /// <summary>
        /// Get avatar URL with fallback to default if file doesn't exist
        /// </summary>
        /// <param name="avatarPath">The avatar path from database</param>
        /// <param name="webHostEnvironment">Web host environment for file path checking</param>
        /// <returns>Valid avatar path or default avatar path</returns>
        public static string GetAvatarOrDefault(string? avatarPath, IWebHostEnvironment webHostEnvironment)
        {
            // Default avatar path - using SVG which works better in browsers
            const string defaultAvatar = "/images/default-avatar.svg";
            
            // If avatar path is null or empty, return default
            if (string.IsNullOrEmpty(avatarPath))
            {
                return defaultAvatar;
            }
            
            // Check if the file physically exists
            var physicalPath = Path.Combine(webHostEnvironment.WebRootPath, avatarPath.TrimStart('/'));
            
            if (File.Exists(physicalPath))
            {
                return avatarPath;
            }
            
            // If file doesn't exist, return default avatar
            return defaultAvatar;
        }
        
        /// <summary>
        /// Get user initials for avatar placeholder
        /// </summary>
        /// <param name="firstName">User's first name</param>
        /// <param name="lastName">User's last name</param>
        /// <returns>User initials (max 2 characters)</returns>
        public static string GetUserInitials(string? firstName, string? lastName)
        {
            var initials = "";
            
            if (!string.IsNullOrEmpty(firstName))
            {
                initials += firstName[0];
            }
            
            if (!string.IsNullOrEmpty(lastName))
            {
                initials += lastName[0];
            }
            
            return string.IsNullOrEmpty(initials) ? "U" : initials.ToUpper();
        }

        /// <summary>
        /// Generate a data URI for an SVG avatar with user initials
        /// </summary>
        /// <param name="initials">User initials (1-2 characters)</param>
        /// <param name="bgColor">Background color (hex format)</param>
        /// <returns>Data URI string for use in img src</returns>
        public static string GenerateInitialsAvatarDataUri(string initials, string bgColor = "#667eea")
        {
            var svg = $@"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 200 200"" width=""200"" height=""200"">
    <defs>
        <linearGradient id=""bgGrad"" x1=""0%"" y1=""0%"" x2=""100%"" y2=""100%"">
            <stop offset=""0%"" style=""stop-color:{bgColor};stop-opacity:1"" />
            <stop offset=""100%"" style=""stop-color:{AdjustColorBrightness(bgColor, -20)};stop-opacity:1"" />
        </linearGradient>
    </defs>
    <circle cx=""100"" cy=""100"" r=""100"" fill=""url(#bgGrad)""/>
    <text x=""100"" y=""110"" text-anchor=""middle"" 
          font-family=""Arial, -apple-system, BlinkMacSystemFont, sans-serif"" 
          font-size=""70"" font-weight=""600"" 
          fill=""#ffffff"" opacity=""0.95"">
        {initials.ToUpper()}
    </text>
</svg>";
            
            var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svg));
            return $"data:image/svg+xml;base64,{base64}";
        }

        /// <summary>
        /// Adjust color brightness
        /// </summary>
        private static string AdjustColorBrightness(string color, int percent)
        {
            // Simple brightness adjustment (can be enhanced)
            if (color.StartsWith("#") && color.Length == 7)
            {
                var r = Convert.ToInt32(color.Substring(1, 2), 16);
                var g = Convert.ToInt32(color.Substring(3, 2), 16);
                var b = Convert.ToInt32(color.Substring(5, 2), 16);
                
                r = Math.Clamp(r + (r * percent / 100), 0, 255);
                g = Math.Clamp(g + (g * percent / 100), 0, 255);
                b = Math.Clamp(b + (b * percent / 100), 0, 255);
                
                return $"#{r:X2}{g:X2}{b:X2}";
            }
            return color;
        }

        /// <summary>
        /// Get color based on user name (consistent color for same name)
        /// </summary>
        public static string GetColorFromName(string name)
        {
            var colors = new[]
            {
                "#667eea", // Purple
                "#f093fb", // Pink
                "#4facfe", // Blue
                "#43e97b", // Green
                "#fa709a", // Rose
                "#feca57", // Yellow
                "#ff6348", // Orange
                "#5f27cd", // Deep Purple
                "#00d2d3", // Cyan
                "#ff9ff3"  // Light Pink
            };
            
            var hash = 0;
            foreach (var c in name)
            {
                hash = ((hash << 5) - hash) + c;
                hash = hash & hash; // Convert to 32bit integer
            }
            
            return colors[Math.Abs(hash) % colors.Length];
        }
    }
}
