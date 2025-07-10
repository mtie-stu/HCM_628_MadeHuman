using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class Add_Table_Low_RefillTask_Details : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LowStockId",
                table: "WarehouseLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LowStockAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentQuantity = table.Column<int>(type: "integer", nullable: false),
                    WarehouseLocationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LowStockAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LowStockAlerts_WarehouseLocations_WarehouseLocationId",
                        column: x => x.WarehouseLocationId,
                        principalTable: "WarehouseLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefillTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LowStockId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserTaskId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateBy = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
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
                        name: "FK_RefillTasks_UsersTasks_UserId",
                        column: x => x.UserId,
                        principalTable: "UsersTasks",
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
                    RefillTasksId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefillTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefillTaskDetails_RefillTasks_RefillTasksId",
                        column: x => x.RefillTasksId,
                        principalTable: "RefillTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseLocations_LowStockId",
                table: "WarehouseLocations",
                column: "LowStockId");

            migrationBuilder.CreateIndex(
                name: "IX_LowStockAlerts_WarehouseLocationId",
                table: "LowStockAlerts",
                column: "WarehouseLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_RefillTasksId",
                table: "RefillTaskDetails",
                column: "RefillTasksId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillTasks_LowStockId",
                table: "RefillTasks",
                column: "LowStockId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefillTasks_UserId",
                table: "RefillTasks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseLocations_LowStockAlerts_LowStockId",
                table: "WarehouseLocations",
                column: "LowStockId",
                principalTable: "LowStockAlerts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseLocations_LowStockAlerts_LowStockId",
                table: "WarehouseLocations");

            migrationBuilder.DropTable(
                name: "RefillTaskDetails");

            migrationBuilder.DropTable(
                name: "RefillTasks");

            migrationBuilder.DropTable(
                name: "LowStockAlerts");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseLocations_LowStockId",
                table: "WarehouseLocations");

            migrationBuilder.DropColumn(
                name: "LowStockId",
                table: "WarehouseLocations");
        }
    }
}
