using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaWebsiteToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột Website nếu chưa tồn tại
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Stores' 
                        AND column_name = 'Website'
                    ) THEN
                        ALTER TABLE ""Stores"" 
                        ADD COLUMN ""Website"" character varying(255) NULL;
                    END IF;
                END $$;
            ");

            // Thêm cột SocialMedia nếu chưa tồn tại
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Stores' 
                        AND column_name = 'SocialMedia'
                    ) THEN
                        ALTER TABLE ""Stores"" 
                        ADD COLUMN ""SocialMedia"" character varying(100) NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "SocialMedia",
                table: "Stores");
        }
    }
}
