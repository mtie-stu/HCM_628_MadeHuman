using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class updb1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "PartTimeAssignment",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment",
                column: "CompanyId",
                principalTable: "Part_Time_Company",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "PartTimeAssignment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment",
                column: "CompanyId",
                principalTable: "Part_Time_Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
