using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class Fix_OutboundTaskItemDetails_Collection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundTaskItemDetails_OutboundTaskItemId",
                table: "OutboundTaskItemDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItemDetails_OutboundTaskItemId",
                table: "OutboundTaskItemDetails",
                column: "OutboundTaskItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OutboundTaskItemDetails_OutboundTaskItemId",
                table: "OutboundTaskItemDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OutboundTaskItemDetails_OutboundTaskItemId",
                table: "OutboundTaskItemDetails",
                column: "OutboundTaskItemId",
                unique: true);
        }
    }
}
