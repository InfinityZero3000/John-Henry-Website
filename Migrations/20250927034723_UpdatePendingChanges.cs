using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_AspNetUsers_CreatorId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_AspNetUsers_SellerResponseUserId",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_SellerResponseUserId",
                table: "ProductReviews");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_CreatorId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "HelpfulCount",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "IsHelpful",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "IsVerifiedPurchase",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "SellerResponse",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "SellerResponseBy",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "SellerResponseDate",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "SellerResponseUserId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ProductCategories",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "ProductIds",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "TargetAudience",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "UsageLimitPerUser",
                table: "Coupons");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HelpfulCount",
                table: "ProductReviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHelpful",
                table: "ProductReviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedPurchase",
                table: "ProductReviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SellerResponse",
                table: "ProductReviews",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerResponseBy",
                table: "ProductReviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SellerResponseDate",
                table: "ProductReviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SellerResponseUserId",
                table: "ProductReviews",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Coupons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Coupons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Coupons",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCategories",
                table: "Coupons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductIds",
                table: "Coupons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetAudience",
                table: "Coupons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageLimitPerUser",
                table: "Coupons",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_SellerResponseUserId",
                table: "ProductReviews",
                column: "SellerResponseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_CreatorId",
                table: "Coupons",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_AspNetUsers_CreatorId",
                table: "Coupons",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_AspNetUsers_SellerResponseUserId",
                table: "ProductReviews",
                column: "SellerResponseUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
