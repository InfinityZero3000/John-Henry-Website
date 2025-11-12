using Microsoft.EntityFrameworkCore;
using JohnHenryFashionWeb.Data;

namespace JohnHenryFashionWeb.Scripts
{
    /// <summary>
    /// Script to fix Freelancer product image paths
    /// Run this once to update all FW* product image URLs to match actual file locations
    /// </summary>
    public class FixFreelancerImages
    {
        public static async Task RunAsync(ApplicationDbContext context, ILogger logger)
        {
            logger.LogInformation("Starting Freelancer image path fix...");
            
            var fixedCount = 0;
            
            // Get all Freelancer products (SKU starts with FW)
            var freelancerProducts = await context.Products
                .Where(p => p.SKU.StartsWith("FW"))
                .ToListAsync();
            
            logger.LogInformation($"Found {freelancerProducts.Count} Freelancer products");
            
            foreach (var product in freelancerProducts)
            {
                var oldPath = product.FeaturedImageUrl;
                string? newPath = null;
                
                // Determine correct folder based on SKU prefix
                if (product.SKU.StartsWith("FWSK"))
                {
                    // Chân Váy (Skirts)
                    newPath = $"/images/chan-vay-nu/{product.SKU}.jpg";
                }
                else if (product.SKU.StartsWith("FWDP") || 
                         product.SKU.StartsWith("FWSP") || 
                         product.SKU.StartsWith("FWQP"))
                {
                    // Quần (Pants)
                    newPath = $"/images/quan-nu/{product.SKU}.jpg";
                }
                else if (product.SKU.StartsWith("FWDR"))
                {
                    // Váy/Dresses
                    newPath = $"/images/dam-nu/{product.SKU}.jpg";
                }
                else if (product.SKU.StartsWith("FWJN") || 
                         product.SKU.StartsWith("FWBZ") ||
                         product.SKU.StartsWith("FWTS") || 
                         product.SKU.StartsWith("FWWS"))
                {
                    // Áo Nữ (Women's tops, blazers, shirts)
                    newPath = $"/images/ao-nu/{product.SKU}.jpg";
                }
                
                if (newPath != null && product.FeaturedImageUrl != newPath)
                {
                    product.FeaturedImageUrl = newPath;
                    product.UpdatedAt = DateTime.UtcNow;
                    fixedCount++;
                    
                    logger.LogInformation($"Fixed {product.SKU}: {oldPath} → {newPath}");
                }
            }
            
            if (fixedCount > 0)
            {
                await context.SaveChangesAsync();
                logger.LogInformation($"Successfully updated {fixedCount} Freelancer product image paths");
            }
            else
            {
                logger.LogInformation("No Freelancer products needed path updates");
            }
        }
    }
}
