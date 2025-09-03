using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class SetNavigationForOutboundTaskId_PickTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks");

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks",
                column: "OutboundTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks");

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks",
                column: "OutboundTaskId",
                unique: true);
        }
    }
}
