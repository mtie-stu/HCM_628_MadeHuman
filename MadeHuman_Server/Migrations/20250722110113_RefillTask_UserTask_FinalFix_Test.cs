using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class RefillTask_UserTask_FinalFix_Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.DropColumn(
                name: "RefillTasksId",
                table: "RefillTaskDetails");

      

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_RefillTaskId",
                table: "RefillTaskDetails",
                column: "RefillTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefillTaskDetails_RefillTasks_RefillTaskId",
                table: "RefillTaskDetails",
                column: "RefillTaskId",
                principalTable: "RefillTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefillTaskDetails_RefillTasks_RefillTaskId",
                table: "RefillTaskDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RefillTasks_UsersTasks_UserTaskId",
                table: "RefillTasks");

            migrationBuilder.DropIndex(
                name: "IX_RefillTasks_UserTaskId",
                table: "RefillTasks");

            migrationBuilder.DropIndex(
                name: "IX_RefillTaskDetails_RefillTaskId",
                table: "RefillTaskDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "RefillTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RefillTasksId",
                table: "RefillTaskDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RefillTasks_UserId",
                table: "RefillTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefillTaskDetails_RefillTasksId",
                table: "RefillTaskDetails",
                column: "RefillTasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefillTaskDetails_RefillTasks_RefillTasksId",
                table: "RefillTaskDetails",
                column: "RefillTasksId",
                principalTable: "RefillTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefillTasks_UsersTasks_UserId",
                table: "RefillTasks",
                column: "UserId",
                principalTable: "UsersTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
