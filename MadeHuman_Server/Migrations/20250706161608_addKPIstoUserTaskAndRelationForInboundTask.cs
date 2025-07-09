using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class addKPIstoUserTaskAndRelationForInboundTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HourlyKPIs",
                table: "UsersTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalKPI",
                table: "UsersTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserTaskId",
                table: "InboundTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_InboundTasks_UserTaskId",
                table: "InboundTasks",
                column: "UserTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_InboundTasks_UsersTasks_UserTaskId",
                table: "InboundTasks",
                column: "UserTaskId",
                principalTable: "UsersTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InboundTasks_UsersTasks_UserTaskId",
                table: "InboundTasks");

            migrationBuilder.DropIndex(
                name: "IX_InboundTasks_UserTaskId",
                table: "InboundTasks");

            migrationBuilder.DropColumn(
                name: "HourlyKPIs",
                table: "UsersTasks");

            migrationBuilder.DropColumn(
                name: "TotalKPI",
                table: "UsersTasks");

            migrationBuilder.DropColumn(
                name: "UserTaskId",
                table: "InboundTasks");
        }
    }
}
