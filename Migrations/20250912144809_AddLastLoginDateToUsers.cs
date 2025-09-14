using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JohnHenryFashionWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddLastLoginDateToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AnalyticsData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true),
                    Campaign = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalyticsData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalyticsData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckoutSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CouponCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShippingAddress = table.Column<string>(type: "text", nullable: true),
                    BillingAddress = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckoutSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ConversionEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    ConversionType = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true),
                    Campaign = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductIds = table.Column<string>(type: "text", nullable: true),
                    ConvertedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdditionalData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversionEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConversionEvents_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ChangedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_AspNetUsers_ChangedBy",
                        column: x => x.ChangedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistories_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PaymentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OrderId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "VND"),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrderId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Orders_OrderId1",
                        column: x => x.OrderId1,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresRedirect = table.Column<bool>(type: "boolean", nullable: false),
                    MinAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SupportedCurrencies = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaxDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    UsageLimit = table.Column<int>(type: "integer", nullable: true),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ApplicableProductIds = table.Column<string>(type: "text", nullable: true),
                    ApplicableCategoryIds = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefundRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OrderId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AdminNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RefundTransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestedBy = table.Column<string>(type: "text", nullable: false),
                    ProcessedBy = table.Column<string>(type: "text", nullable: true),
                    OrderId1 = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundRequests_AspNetUsers_RequestedBy",
                        column: x => x.RequestedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundRequests_Orders_OrderId1",
                        column: x => x.OrderId1,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<string>(type: "text", nullable: false),
                    Frequency = table.Column<string>(type: "text", nullable: false),
                    Parameters = table.Column<string>(type: "text", nullable: true),
                    Configuration = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastRunDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextRunDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportTemplates_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalOrders = table.Column<int>(type: "integer", nullable: false),
                    TotalProducts = table.Column<int>(type: "integer", nullable: false),
                    AverageOrderValue = table.Column<decimal>(type: "numeric", nullable: false),
                    ReportData = table.Column<string>(type: "text", nullable: true),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeneratedBy = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    GeneratedByUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesReports_AspNetUsers_GeneratedByUserId",
                        column: x => x.GeneratedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShippingMethods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EstimatedDays = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaxWeight = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    AvailableRegions = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeviceType = table.Column<string>(type: "text", nullable: true),
                    Browser = table.Column<string>(type: "text", nullable: true),
                    OperatingSystem = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckoutSessionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckoutSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ProductImage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckoutSessionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckoutSessionItems_CheckoutSessions_CheckoutSessionId",
                        column: x => x.CheckoutSessionId,
                        principalTable: "CheckoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckoutSessionItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    Page = table.Column<string>(type: "text", nullable: false),
                    Referrer = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeOnPage = table.Column<int>(type: "integer", nullable: true),
                    ExitPage = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true),
                    Campaign = table.Column<string>(type: "text", nullable: true),
                    SessionId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PageViews_UserSessions_SessionId1",
                        column: x => x.SessionId1,
                        principalTable: "UserSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnalyticsData_UserId",
                table: "AnalyticsData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessionItems_CheckoutSessionId",
                table: "CheckoutSessionItems",
                column: "CheckoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessionItems_ProductId",
                table: "CheckoutSessionItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessions_ExpiresAt",
                table: "CheckoutSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessions_Status",
                table: "CheckoutSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CheckoutSessions_UserId",
                table: "CheckoutSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_OrderId",
                table: "ConversionEvents",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversionEvents_UserId",
                table: "ConversionEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_ChangedBy",
                table: "OrderStatusHistories",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_CreatedAt",
                table: "OrderStatusHistories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_OrderId",
                table: "OrderStatusHistories",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_Status",
                table: "OrderStatusHistories",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_SessionId1",
                table: "PageViews",
                column: "SessionId1");

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_UserId",
                table: "PageViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_CreatedAt",
                table: "PaymentAttempts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_OrderId",
                table: "PaymentAttempts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_OrderId1",
                table: "PaymentAttempts",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_PaymentId",
                table: "PaymentAttempts",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_Status",
                table: "PaymentAttempts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_UserId",
                table: "PaymentAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_Code",
                table: "PaymentMethods",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_IsActive",
                table: "PaymentMethods",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_SortOrder",
                table: "PaymentMethods",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_Code",
                table: "Promotions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_EndDate",
                table: "Promotions",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_IsActive",
                table: "Promotions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_StartDate",
                table: "Promotions",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_OrderId",
                table: "RefundRequests",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_OrderId1",
                table: "RefundRequests",
                column: "OrderId1");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_PaymentId",
                table: "RefundRequests",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_RequestedBy",
                table: "RefundRequests",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_RefundRequests_Status",
                table: "RefundRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_CreatedByUserId",
                table: "ReportTemplates",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReports_GeneratedByUserId",
                table: "SalesReports",
                column: "GeneratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_Code",
                table: "ShippingMethods",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_IsActive",
                table: "ShippingMethods",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingMethods_SortOrder",
                table: "ShippingMethods",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalyticsData");

            migrationBuilder.DropTable(
                name: "CheckoutSessionItems");

            migrationBuilder.DropTable(
                name: "ConversionEvents");

            migrationBuilder.DropTable(
                name: "OrderStatusHistories");

            migrationBuilder.DropTable(
                name: "PageViews");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "RefundRequests");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropTable(
                name: "SalesReports");

            migrationBuilder.DropTable(
                name: "ShippingMethods");

            migrationBuilder.DropTable(
                name: "CheckoutSessions");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "AspNetUsers");
        }
    }
}
