using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActiveSessionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "ActiveSessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "ActiveSessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "ActiveSessions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "ActiveSessions");
        }
    }
}
