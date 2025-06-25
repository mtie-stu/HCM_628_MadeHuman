using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class addParttime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartTime_Part_Time_Company_CompanyId",
                table: "PartTime");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_PartTime_PartTimeId",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_PartTime_PartTimeId1",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersTasks_PartTime_PartTimeId",
                table: "UsersTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartTime",
                table: "PartTime");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Part_Time_Company",
                table: "Part_Time_Company");

            migrationBuilder.RenameTable(
                name: "PartTime",
                newName: "PartTimes");

            migrationBuilder.RenameTable(
                name: "Part_Time_Company",
                newName: "PartTimeCompanies");

            migrationBuilder.RenameIndex(
                name: "IX_PartTime_CompanyId",
                table: "PartTimes",
                newName: "IX_PartTimes_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartTimes",
                table: "PartTimes",
                column: "PartTimeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartTimeCompanies",
                table: "PartTimeCompanies",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_PartTimeCompanies_CompanyId",
                table: "PartTimeAssignment",
                column: "CompanyId",
                principalTable: "PartTimeCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_PartTimes_PartTimeId",
                table: "PartTimeAssignment",
                column: "PartTimeId",
                principalTable: "PartTimes",
                principalColumn: "PartTimeId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_PartTimes_PartTimeId1",
                table: "PartTimeAssignment",
                column: "PartTimeId1",
                principalTable: "PartTimes",
                principalColumn: "PartTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimes_PartTimeCompanies_CompanyId",
                table: "PartTimes",
                column: "CompanyId",
                principalTable: "PartTimeCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTasks_PartTimes_PartTimeId",
                table: "UsersTasks",
                column: "PartTimeId",
                principalTable: "PartTimes",
                principalColumn: "PartTimeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_PartTimeCompanies_CompanyId",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_PartTimes_PartTimeId",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimeAssignment_PartTimes_PartTimeId1",
                table: "PartTimeAssignment");

            migrationBuilder.DropForeignKey(
                name: "FK_PartTimes_PartTimeCompanies_CompanyId",
                table: "PartTimes");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersTasks_PartTimes_PartTimeId",
                table: "UsersTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartTimes",
                table: "PartTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartTimeCompanies",
                table: "PartTimeCompanies");

            migrationBuilder.RenameTable(
                name: "PartTimes",
                newName: "PartTime");

            migrationBuilder.RenameTable(
                name: "PartTimeCompanies",
                newName: "Part_Time_Company");

            migrationBuilder.RenameIndex(
                name: "IX_PartTimes_CompanyId",
                table: "PartTime",
                newName: "IX_PartTime_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartTime",
                table: "PartTime",
                column: "PartTimeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Part_Time_Company",
                table: "Part_Time_Company",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTime_Part_Time_Company_CompanyId",
                table: "PartTime",
                column: "CompanyId",
                principalTable: "Part_Time_Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_PartTime_PartTimeId",
                table: "PartTimeAssignment",
                column: "PartTimeId",
                principalTable: "PartTime",
                principalColumn: "PartTimeId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_PartTime_PartTimeId1",
                table: "PartTimeAssignment",
                column: "PartTimeId1",
                principalTable: "PartTime",
                principalColumn: "PartTimeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartTimeAssignment_Part_Time_Company_CompanyId",
                table: "PartTimeAssignment",
                column: "CompanyId",
                principalTable: "Part_Time_Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTasks_PartTime_PartTimeId",
                table: "UsersTasks",
                column: "PartTimeId",
                principalTable: "PartTime",
                principalColumn: "PartTimeId");
        }
    }
}
