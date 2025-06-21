using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class updb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSKU",
                table: "ProductBatches");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductSKUId",
                table: "ProductBatches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductSKUId",
                table: "InboundReceiptItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ProductBatches_ProductSKUId",
                table: "ProductBatches",
                column: "ProductSKUId");

            migrationBuilder.CreateIndex(
                name: "IX_InboundReceiptItems_ProductSKUId",
                table: "InboundReceiptItems",
                column: "ProductSKUId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundReceiptItems_ProductSKUs_ProductSKUId",
                table: "InboundReceiptItems",
                column: "ProductSKUId",
                principalTable: "ProductSKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductBatches_ProductSKUs_ProductSKUId",
                table: "ProductBatches",
                column: "ProductSKUId",
                principalTable: "ProductSKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboundReceiptItems_ProductSKUs_ProductSKUId",
                table: "InboundReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductBatches_ProductSKUs_ProductSKUId",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_ProductBatches_ProductSKUId",
                table: "ProductBatches");

            migrationBuilder.DropIndex(
                name: "IX_InboundReceiptItems_ProductSKUId",
                table: "InboundReceiptItems");

            migrationBuilder.DropColumn(
                name: "ProductSKUId",
                table: "ProductBatches");

            migrationBuilder.DropColumn(
                name: "ProductSKUId",
                table: "InboundReceiptItems");

            migrationBuilder.AddColumn<string>(
                name: "ProductSKU",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
