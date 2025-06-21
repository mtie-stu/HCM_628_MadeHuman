using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class updb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductSKU",
                table: "ProductBatches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSKU",
                table: "ProductBatches");
        }
    }
}
