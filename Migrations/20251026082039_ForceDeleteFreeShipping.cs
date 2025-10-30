using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class ForceDeleteFreeShipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Force delete FREE shipping method (if exists)
            migrationBuilder.Sql(@"
                DELETE FROM ""ShippingMethods"" 
                WHERE ""Code"" = 'FREE';
            ");
            
            // Verify only 4 methods remain: STANDARD, EXPRESS, SUPER_EXPRESS, ECONOMY
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    method_count INTEGER;
                BEGIN
                    SELECT COUNT(*) INTO method_count FROM ""ShippingMethods"";
                    IF method_count <> 4 THEN
                        RAISE NOTICE 'Warning: Expected 4 shipping methods, found %', method_count;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore FREE shipping if needed
            migrationBuilder.Sql(@"
                INSERT INTO ""ShippingMethods"" 
                (""Name"", ""Code"", ""Description"", ""Cost"", ""EstimatedDays"", ""IsActive"", ""MinOrderAmount"", ""SortOrder"", ""CreatedAt"", ""UpdatedAt"")
                VALUES 
                ('Miễn phí giao hàng', 'FREE', 'Miễn phí vận chuyển cho đơn hàng trên 500k', 0, 5, true, 500000, 4, NOW(), NOW())
                ON CONFLICT (""Code"") DO NOTHING;
            ");
        }
    }
}
