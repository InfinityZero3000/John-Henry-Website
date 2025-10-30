using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class SeedShippingMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed shipping methods
            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Name", "Code", "Description", "Cost", "EstimatedDays", "IsActive", "MinOrderAmount", "MaxWeight", "AvailableRegions", "SortOrder", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 
                        "Giao hàng tiêu chuẩn", 
                        "STANDARD", 
                        "Giao hàng trong 3-5 ngày làm việc", 
                        30000m, 
                        4, 
                        true, 
                        null, 
                        null, 
                        null, 
                        1, 
                        DateTime.UtcNow, 
                        DateTime.UtcNow 
                    },
                    { 
                        "Giao hàng nhanh", 
                        "EXPRESS", 
                        "Giao hàng trong 1-2 ngày làm việc", 
                        50000m, 
                        1, 
                        true, 
                        null, 
                        null, 
                        null, 
                        2, 
                        DateTime.UtcNow, 
                        DateTime.UtcNow 
                    },
                    { 
                        "Giao hàng hỏa tốc", 
                        "SUPER_EXPRESS", 
                        "Giao hàng trong vòng 24 giờ (nội thành)", 
                        100000m, 
                        1, 
                        true, 
                        500000m, 
                        null, 
                        "Ho Chi Minh,Ha Noi", 
                        3, 
                        DateTime.UtcNow, 
                        DateTime.UtcNow 
                    },
                    { 
                        "Giao hàng tiết kiệm", 
                        "ECONOMY", 
                        "Giao hàng trong 5-7 ngày làm việc (phí thấp)", 
                        20000m, 
                        6, 
                        true, 
                        null, 
                        null, 
                        null, 
                        4, 
                        DateTime.UtcNow, 
                        DateTime.UtcNow 
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded shipping methods
            migrationBuilder.Sql("DELETE FROM \"ShippingMethods\" WHERE \"Code\" IN ('STANDARD', 'EXPRESS', 'SUPER_EXPRESS', 'ECONOMY')");
        }
    }
}
