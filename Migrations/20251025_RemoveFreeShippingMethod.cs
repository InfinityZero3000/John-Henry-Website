using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFreeShippingMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa phương thức "Miễn phí giao hàng" vì đã tự động áp dụng miễn phí cho đơn hàng >= 500k
            migrationBuilder.Sql("DELETE FROM \"ShippingMethods\" WHERE \"Code\" = 'FREE'");
            
            // Cập nhật SortOrder của các phương thức còn lại
            migrationBuilder.Sql(@"
                UPDATE ""ShippingMethods"" 
                SET ""SortOrder"" = 4, ""UpdatedAt"" = NOW() 
                WHERE ""Code"" = 'ECONOMY'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Khôi phục lại phương thức "Miễn phí giao hàng"
            migrationBuilder.InsertData(
                table: "ShippingMethods",
                columns: new[] { "Name", "Code", "Description", "Cost", "EstimatedDays", "IsActive", "MinOrderAmount", "MaxWeight", "AvailableRegions", "SortOrder", "CreatedAt", "UpdatedAt" },
                values: new object[] { 
                    "Miễn phí giao hàng", 
                    "FREE", 
                    "Miễn phí vận chuyển cho đơn hàng trên 500k", 
                    0m, 
                    5, 
                    true, 
                    500000m, 
                    null, 
                    null, 
                    4, 
                    DateTime.UtcNow, 
                    DateTime.UtcNow 
                });
                
            // Cập nhật lại SortOrder của ECONOMY về 5
            migrationBuilder.Sql(@"
                UPDATE ""ShippingMethods"" 
                SET ""SortOrder"" = 5, ""UpdatedAt"" = NOW() 
                WHERE ""Code"" = 'ECONOMY'
            ");
        }
    }
}
