using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboundTaskItemIdToPickTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRefilled",
                table: "RefillTaskDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OutboundTaskItemId",
                table: "PickTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_OutboundTaskItemId",
                table: "PickTasks",
                column: "OutboundTaskItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_PickTasks_OutboundTaskItems_OutboundTaskItemId",
                table: "PickTasks",
                column: "OutboundTaskItemId",
                principalTable: "OutboundTaskItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTasks_OutboundTaskItems_OutboundTaskItemId",
                table: "PickTasks");

            migrationBuilder.DropIndex(
                name: "IX_PickTasks_OutboundTaskItemId",
                table: "PickTasks");

            migrationBuilder.DropColumn(
                name: "IsRefilled",
                table: "RefillTaskDetails");

            migrationBuilder.DropColumn(
                name: "OutboundTaskItemId",
                table: "PickTasks");
        }
    }
}
