using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace JohnHenryFashionWeb.Scripts;

public class SeedShippingMethods
{
    public static async Task Run(ApplicationDbContext context)
    {
        Console.WriteLine("Starting to seed shipping methods...");
        
        // Check if shipping methods already exist
        var existingCount = await context.ShippingMethods.CountAsync();
        if (existingCount > 0)
        {
            Console.WriteLine($"Shipping methods already exist ({existingCount} records). Skipping seed...");
            return;
        }
        
        // Seed shipping methods
        var shippingMethods = new List<ShippingMethod>
        {
            new ShippingMethod
            {
                Name = "Giao hàng tiêu chuẩn",
                Code = "STANDARD",
                Description = "Giao hàng trong 3-5 ngày làm việc",
                Cost = 30000,
                EstimatedDays = 4,
                IsActive = true,
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ShippingMethod
            {
                Name = "Giao hàng nhanh",
                Code = "EXPRESS",
                Description = "Giao hàng trong 1-2 ngày làm việc",
                Cost = 50000,
                EstimatedDays = 1,
                IsActive = true,
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ShippingMethod
            {
                Name = "Giao hàng hỏa tốc",
                Code = "SUPER_EXPRESS",
                Description = "Giao hàng trong vòng 24 giờ (nội thành)",
                Cost = 100000,
                EstimatedDays = 1,
                IsActive = true,
                MinOrderAmount = 500000,
                AvailableRegions = "Ho Chi Minh,Ha Noi",
                SortOrder = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ShippingMethod
            {
                Name = "Giao hàng tiết kiệm",
                Code = "ECONOMY",
                Description = "Giao hàng trong 5-7 ngày làm việc (phí thấp)",
                Cost = 20000,
                EstimatedDays = 6,
                IsActive = true,
                SortOrder = 4,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        
        await context.ShippingMethods.AddRangeAsync(shippingMethods);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"Successfully seeded {shippingMethods.Count} shipping methods!");
    }
}
