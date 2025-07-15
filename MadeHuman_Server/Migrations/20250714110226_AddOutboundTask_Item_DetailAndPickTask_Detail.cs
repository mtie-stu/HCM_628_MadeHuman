using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboundTask_Item_DetailAndPickTask_Detail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "PickTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickTasks_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OutBoundTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                        column: x => x.OutBoundTaskId,
                        principalTable: "OutboundTasks",
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
                name: "PickTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductSKUId = table.Column<Guid>(type: "uuid", nullable: false),
                    PickTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    PickTasksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                        column: x => x.PickTasksId,
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

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_OutBoundTaskId",
                table: "Baskets",
                column: "OutBoundTaskId",
                unique: true);

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
                name: "IX_PickTaskDetails_PickTasksId",
                table: "PickTaskDetails",
                column: "PickTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_ProductSKUId",
                table: "PickTaskDetails",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_WarehouseLocationId",
                table: "PickTaskDetails",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_UsersTasksId",
                table: "PickTasks",
                column: "UsersTasksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "OutboundTaskItemDetails");

            migrationBuilder.DropTable(
                name: "PickTaskDetails");

            migrationBuilder.DropTable(
                name: "OutboundTaskItems");

            migrationBuilder.DropTable(
                name: "PickTasks");

            migrationBuilder.DropTable(
                name: "OutboundTasks");
        }
    }
}
