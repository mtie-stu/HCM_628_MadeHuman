using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class setnullUserTaskIdForPickTaskv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTasks_UsersTasks_UsersTasksId",
                table: "PickTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsersTasksId",
                table: "PickTasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_PickTasks_UsersTasks_UsersTasksId",
                table: "PickTasks",
                column: "UsersTasksId",
                principalTable: "UsersTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTasks_UsersTasks_UsersTasksId",
                table: "PickTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "UsersTasksId",
                table: "PickTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PickTasks_UsersTasks_UsersTasksId",
                table: "PickTasks",
                column: "UsersTasksId",
                principalTable: "UsersTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
