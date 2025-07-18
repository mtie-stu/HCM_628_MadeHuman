using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class removePickTaskIdv12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails");

            migrationBuilder.DropIndex(
                name: "IX_PickTaskDetails_PickTasksId",
                table: "PickTaskDetails");

            migrationBuilder.DropColumn(
                name: "PickTasksId",
                table: "PickTaskDetails");

            migrationBuilder.RenameColumn(
                name: "PicksTasksId",
                table: "PickTaskDetails",
                newName: "PickTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_PickTaskId",
                table: "PickTaskDetails",
                column: "PickTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTaskId",
                table: "PickTaskDetails",
                column: "PickTaskId",
                principalTable: "PickTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTaskId",
                table: "PickTaskDetails");

            migrationBuilder.DropIndex(
                name: "IX_PickTaskDetails_PickTaskId",
                table: "PickTaskDetails");

            migrationBuilder.RenameColumn(
                name: "PickTaskId",
                table: "PickTaskDetails",
                newName: "PicksTasksId");

            migrationBuilder.AddColumn<Guid>(
                name: "PickTasksId",
                table: "PickTaskDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickTaskDetails_PickTasksId",
                table: "PickTaskDetails",
                column: "PickTasksId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickTaskDetails_PickTasks_PickTasksId",
                table: "PickTaskDetails",
                column: "PickTasksId",
                principalTable: "PickTasks",
                principalColumn: "Id");
        }
    }
}
