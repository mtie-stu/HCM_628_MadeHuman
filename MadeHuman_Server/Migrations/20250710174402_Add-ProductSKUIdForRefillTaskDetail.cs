using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSKUIdForRefillTaskDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductSKUId",
                table: "RefillTaskDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_ProductSKUId",
                table: "RefillTaskDetails",
                column: "ProductSKUId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefillTaskDetails_ProductSKUs_ProductSKUId",
                table: "RefillTaskDetails",
                column: "ProductSKUId",
                principalTable: "ProductSKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefillTaskDetails_ProductSKUs_ProductSKUId",
                table: "RefillTaskDetails");

            migrationBuilder.DropIndex(
                name: "IX_RefillTaskDetails_ProductSKUId",
                table: "RefillTaskDetails");

            migrationBuilder.DropColumn(
                name: "ProductSKUId",
                table: "RefillTaskDetails");
        }
    }
}
