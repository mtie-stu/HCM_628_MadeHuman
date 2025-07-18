using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class removePickTaskIdv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "PickTasksId",
                table: "PickTaskDetails",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails",
                column: "PickTasksId",
                principalTable: "PickTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "PickTasksId",
                table: "PickTaskDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails",
                column: "PickTasksId",
                principalTable: "PickTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
