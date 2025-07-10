using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class addFix_ProductBatch_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_InboundTaskId",
                table: "ProductBatches");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_InboundTaskId",
                table: "ProductBatches",
                column: "InboundTaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_InboundTaskId",
                table: "ProductBatches");

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_InboundTaskId",
                table: "ProductBatches",
                column: "InboundTaskId",
                unique: true);
        }
    }
}
