using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class Fix_RefillTasks_RefillTaskId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    UserTypes = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Combos",
                columns: table => new
                {
                    ComboId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combos", x => x.ComboId);
                });

            migrationBuilder.CreateTable(
                name: "InboundReceipt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundReceipt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboundTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartTimeCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTimeCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WareHouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WareHouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrders",
                columns: table => new
                {
                    ShopOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AppUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrders", x => x.ShopOrderId);
                    table.ForeignKey(
                        name: "FK_ShopOrders_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OutBoundTaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                        column: x => x.OutBoundTaskId,
                        principalTable: "OutboundTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartTimes",
                columns: table => new
                {
                    PartTimeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CCCD = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    StatusPartTimes = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTimes", x => x.PartTimeId);
                    table.ForeignKey(
                        name: "FK_PartTimes_PartTimeCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "PartTimeCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseZones_WareHouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "WareHouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboundTaskItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ShopOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundTaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundTaskItems_OutboundTasks_OutboundTaskId",
                        column: x => x.OutboundTaskId,
                        principalTable: "OutboundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundTaskItems_ShopOrders_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrders",
                        principalColumn: "ShopOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboItems",
                columns: table => new
                {
                    ComboItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ComboId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboItems", x => x.ComboItemId);
                    table.ForeignKey(
                        name: "FK_ComboItems_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_Combo_Imgs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComboId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_Combo_Imgs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_product_Combo_Imgs_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "ComboId");
                    table.ForeignKey(
                        name: "FK_product_Combo_Imgs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "ProductItems",
                columns: table => new
                {
                    ProductItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductItems", x => x.ProductItemId);
                    table.ForeignKey(
                        name: "FK_ProductItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSKUs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SKU = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    ComboId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuantityInStock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSKUs", x => x.Id);
                    table.CheckConstraint("CK_ProductSKU_Owner", "(\"ProductId\" IS NOT NULL AND \"ComboId\" IS NULL) OR (\"ProductId\" IS NULL AND \"ComboId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_ProductSKUs_Combos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "Combos",
                        principalColumn: "ComboId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSKUs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BreakDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    OvertimeDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    PartTimeId = table.Column<Guid>(type: "uuid", nullable: true),
                    TotalKPI = table.Column<int>(type: "integer", nullable: false),
                    HourlyKPIs = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersTasks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersTasks_PartTimes_PartTimeId",
                        column: x => x.PartTimeId,
                        principalTable: "PartTimes",
                        principalColumn: "PartTimeId");
                });

            migrationBuilder.CreateTable(
                name: "InboundReceiptItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    InboundReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundReceiptItems_InboundReceipt_InboundReceiptId",
                        column: x => x.InboundReceiptId,
                        principalTable: "InboundReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundReceiptItems_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ProductSKUsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductItems_ProductItemId",
                        column: x => x.ProductItemId,
                        principalTable: "ProductItems",
                        principalColumn: "ProductItemId");
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductSKUs_ProductSKUsId",
                        column: x => x.ProductSKUsId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ShopOrders_ShopOrderId",
                        column: x => x.ShopOrderId,
                        principalTable: "ShopOrders",
                        principalColumn: "ShopOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutboundTaskItemDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundTaskItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboundTaskItemDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutboundTaskItemDetails_OutboundTaskItems_OutboundTaskItemId",
                        column: x => x.OutboundTaskItemId,
                        principalTable: "OutboundTaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutboundTaskItemDetails_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckInCheckOutLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartTimeId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCheckIn = table.Column<bool>(type: "boolean", nullable: false),
                    IsOvertime = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInCheckOutLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOutLog_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckInCheckOutLog_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MadeAt = table.Column<string>(type: "text", nullable: true),
                    StatusCheckTask = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboundTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckTasks_OutboundTasks_OutboundTaskId",
                        column: x => x.OutboundTaskId,
                        principalTable: "OutboundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckTasks_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InboundTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InboundReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTaskId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboundTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InboundTasks_InboundReceipt_InboundReceiptId",
                        column: x => x.InboundReceiptId,
                        principalTable: "InboundReceipt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InboundTasks_UsersTasks_UserTaskId",
                        column: x => x.UserTaskId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PartTimeAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartTimeId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TaskType = table.Column<int>(type: "integer", nullable: false),
                    ShiftCode = table.Column<string>(type: "text", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckOutTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OvertimeDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    BreakDuration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    PartTimeId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTimeAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartTimeAssignment_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartTimeAssignment_PartTimeCompanies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "PartTimeCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartTimeAssignment_PartTimes_PartTimeId",
                        column: x => x.PartTimeId,
                        principalTable: "PartTimes",
                        principalColumn: "PartTimeId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PartTimeAssignment_PartTimes_PartTimeId1",
                        column: x => x.PartTimeId1,
                        principalTable: "PartTimes",
                        principalColumn: "PartTimeId");
                    table.ForeignKey(
                        name: "FK_PartTimeAssignment_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PickTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboundTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickTasks_OutboundTasks_OutboundTaskId",
                        column: x => x.OutboundTaskId,
                        principalTable: "OutboundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickTasks_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusCheckDetailTask = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundTaskItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckTaskDetails_CheckTasks_CheckTaskId",
                        column: x => x.CheckTaskId,
                        principalTable: "CheckTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckTaskDetails_OutboundTaskItems_OutboundTaskItemId",
                        column: x => x.OutboundTaskItemId,
                        principalTable: "OutboundTaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    QuantityBooked = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventory_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: true),
                    ChangeQuantity = table.Column<int>(type: "integer", nullable: false),
                    ChangeBy = table.Column<string>(type: "text", nullable: false),
                    ActionInventoryLogs = table.Column<int>(type: "integer", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "integer", nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryLogs_Inventory_InventoryId",
                        column: x => x.InventoryId,
                        principalTable: "Inventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LowStockAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "integer", nullable: false),
                    StatusLowStockAlerts = table.Column<int>(type: "integer", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LowStockAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefillTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LowStockId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusRefillTasks = table.Column<int>(type: "integer", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefillTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefillTasks_LowStockAlerts_LowStockId",
                        column: x => x.LowStockId,
                        principalTable: "LowStockAlerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RefillTasks_UsersTasks_UserTaskId",
                        column: x => x.UserTaskId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarehouseLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameLocation = table.Column<string>(type: "text", nullable: false),
                    StatusWareHouse = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: false),
                    LowStockId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseLocations_LowStockAlerts_LowStockId",
                        column: x => x.LowStockId,
                        principalTable: "LowStockAlerts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WarehouseLocations_WarehouseZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "WarehouseZones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefillTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromLocation = table.Column<Guid>(type: "uuid", nullable: false),
                    ToLocation = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    RefillTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefillTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefillTaskDetails_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefillTaskDetails_RefillTasks_RefillTaskId",
                        column: x => x.RefillTaskId,
                        principalTable: "RefillTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPicked = table.Column<bool>(type: "boolean", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityPicked = table.Column<int>(type: "integer", nullable: false),
                    PickTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickTaskDetails_PickTasks_PickTaskId",
                        column: x => x.PickTaskId,
                        principalTable: "PickTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickTaskDetails_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PickTaskDetails_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    StatusProductBatches = table.Column<int>(type: "integer", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false),
                    InboundTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductBatches_InboundTasks_InboundTaskId",
                        column: x => x.InboundTaskId,
                        principalTable: "InboundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBatches_ProductSKUs_ProductSKUId",
                        column: x => x.ProductSKUId,
                        principalTable: "ProductSKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductBatches_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_OutBoundTaskId",
                table: "Baskets",
                column: "OutBoundTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOutLog_UserId",
                table: "CheckInCheckOutLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckInCheckOutLog_UsersTasksId",
                table: "CheckInCheckOutLog",
                column: "UsersTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckTaskDetails_CheckTaskId",
                table: "CheckTaskDetails",
                column: "CheckTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckTaskDetails_OutboundTaskItemId",
                table: "CheckTaskDetails",
                column: "OutboundTaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckTasks_OutboundTaskId",
                table: "CheckTasks",
                column: "OutboundTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckTasks_UsersTasksId",
                table: "CheckTasks",
                column: "UsersTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_ComboId",
                table: "ComboItems",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboItems_ProductId",
                table: "ComboItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundReceiptItems_InboundReceiptId",
                table: "InboundReceiptItems",
                column: "InboundReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundReceiptItems_ProductSKUId",
                table: "InboundReceiptItems",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundTasks_InboundReceiptId",
                table: "InboundTasks",
                column: "InboundReceiptId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboundTasks_UserTaskId",
                table: "InboundTasks",
                column: "UserTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductSKUId",
                table: "Inventory",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_WarehouseLocationId",
                table: "Inventory",
                column: "WarehouseLocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLogs_InventoryId",
                table: "InventoryLogs",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_WarehouseLocationId",
                table: "LowStockAlerts",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductItemId",
                table: "OrderItems",
                column: "ProductItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductSKUsId",
                table: "OrderItems",
                column: "ProductSKUsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ShopOrderId",
                table: "OrderItems",
                column: "ShopOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItemDetails_OutboundTaskItemId",
                table: "OutboundTaskItemDetails",
                column: "OutboundTaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItemDetails_ProductSKUId",
                table: "OutboundTaskItemDetails",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItems_OutboundTaskId",
                table: "OutboundTaskItems",
                column: "OutboundTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItems_ShopOrderId",
                table: "OutboundTaskItems",
                column: "ShopOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartTimeAssignment_CompanyId",
                table: "PartTimeAssignment",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTimeAssignment_PartTimeId",
                table: "PartTimeAssignment",
                column: "PartTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTimeAssignment_PartTimeId1",
                table: "PartTimeAssignment",
                column: "PartTimeId1");

            migrationBuilder.CreateIndex(
                name: "IX_PartTimeAssignment_UserId",
                table: "PartTimeAssignment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTimeAssignment_UsersTasksId",
                table: "PartTimeAssignment",
                column: "UsersTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTimes_CompanyId",
                table: "PartTimes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_PickTaskId",
                table: "PickTaskDetails",
                column: "PickTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_ProductSKUId",
                table: "PickTaskDetails",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_WarehouseLocationId",
                table: "PickTaskDetails",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks",
                column: "OutboundTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_UsersTasksId",
                table: "PickTasks",
                column: "UsersTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_product_Combo_Imgs_ComboId",
                table: "product_Combo_Imgs",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_product_Combo_Imgs_ProductId",
                table: "product_Combo_Imgs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_InboundTaskId",
                table: "ProductBatches",
                column: "InboundTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductSKUId",
                table: "ProductBatches",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_WarehouseLocationId",
                table: "ProductBatches",
                column: "WarehouseLocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_ProductId",
                table: "ProductItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSKUs_ComboId",
                table: "ProductSKUs",
                column: "ComboId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSKUs_ProductId",
                table: "ProductSKUs",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_ProductSKUId",
                table: "RefillTaskDetails",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_RefillTaskId",
                table: "RefillTaskDetails",
                column: "RefillTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillTasks_LowStockId",
                table: "RefillTasks",
                column: "LowStockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefillTasks_UserTaskId",
                table: "RefillTasks",
                column: "UserTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_AppUserId",
                table: "ShopOrders",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTasks_PartTimeId",
                table: "UsersTasks",
                column: "PartTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersTasks_UserId",
                table: "UsersTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_LowStockId",
                table: "WarehouseLocations",
                column: "LowStockId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_ZoneId",
                table: "WarehouseLocations",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseZones_WarehouseId",
                table: "WarehouseZones",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_WarehouseLocations_WarehouseLocationId",
                table: "Inventory",
                column: "WarehouseLocationId",
                principalTable: "WarehouseLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LowStockAlerts_WarehouseLocations_WarehouseLocationId",
                table: "LowStockAlerts",
                column: "WarehouseLocationId",
                principalTable: "WarehouseLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LowStockAlerts_WarehouseLocations_WarehouseLocationId",
                table: "LowStockAlerts");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "CheckInCheckOutLog");

            migrationBuilder.DropTable(
                name: "CheckTaskDetails");

            migrationBuilder.DropTable(
                name: "ComboItems");

            migrationBuilder.DropTable(
                name: "InboundReceiptItems");

            migrationBuilder.DropTable(
                name: "InventoryLogs");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OutboundTaskItemDetails");

            migrationBuilder.DropTable(
                name: "PartTimeAssignment");

            migrationBuilder.DropTable(
                name: "PickTaskDetails");

            migrationBuilder.DropTable(
                name: "product_Combo_Imgs");

            migrationBuilder.DropTable(
                name: "ProductBatches");

            migrationBuilder.DropTable(
                name: "RefillTaskDetails");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CheckTasks");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "ProductItems");

            migrationBuilder.DropTable(
                name: "OutboundTaskItems");

            migrationBuilder.DropTable(
                name: "PickTasks");

            migrationBuilder.DropTable(
                name: "InboundTasks");

            migrationBuilder.DropTable(
                name: "RefillTasks");

            migrationBuilder.DropTable(
                name: "ProductSKUs");

            migrationBuilder.DropTable(
                name: "ShopOrders");

            migrationBuilder.DropTable(
                name: "OutboundTasks");

            migrationBuilder.DropTable(
                name: "InboundReceipt");

            migrationBuilder.DropTable(
                name: "UsersTasks");

            migrationBuilder.DropTable(
                name: "Combos");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PartTimes");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "PartTimeCompanies");

            migrationBuilder.DropTable(
                name: "WarehouseLocations");

            migrationBuilder.DropTable(
                name: "LowStockAlerts");

            migrationBuilder.DropTable(
                name: "WarehouseZones");

            migrationBuilder.DropTable(
                name: "WareHouses");
        }
    }
}
