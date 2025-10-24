using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <summary>
    /// Migration to fix product SKU issues
    /// - Generate SKU for products with null/empty SKU
    /// - Log products with issues
    /// </summary>
    public partial class FixProductSKUIssues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Update products with NULL or empty SKU
            migrationBuilder.Sql(@"
                UPDATE ""Products""
                SET ""SKU"" = CONCAT('SKU-', ""Id""::text),
                    ""UpdatedAt"" = CURRENT_TIMESTAMP
                WHERE ""SKU"" IS NULL OR ""SKU"" = '';
            ");

            // Step 2: Make SKU column required (not nullable)
            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            // Step 3: Create unique index on SKU to prevent duplicates
            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            // Step 4: Log fixed products to console
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    fixed_count INTEGER;
                BEGIN
                    SELECT COUNT(*) INTO fixed_count
                    FROM ""Products""
                    WHERE ""SKU"" LIKE 'SKU-%';
                    
                    RAISE NOTICE 'Fixed % products with missing SKU', fixed_count;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove unique index
            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                table: "Products");

            // Make SKU nullable again
            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
