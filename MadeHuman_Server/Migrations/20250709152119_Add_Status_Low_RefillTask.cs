using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class Add_Status_Low_RefillTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusRefillTasks",
                table: "RefillTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusLowStockAlerts",
                table: "LowStockAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusRefillTasks",
                table: "RefillTasks");

            migrationBuilder.DropColumn(
                name: "StatusLowStockAlerts",
                table: "LowStockAlerts");
        }
    }
}
