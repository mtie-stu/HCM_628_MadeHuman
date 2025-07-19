using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class Set11ForPickTaskAndOutboundTaskv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OutboundTaskId",
                table: "PickTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks",
                column: "OutboundTaskId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PickTasks_OutboundTasks_OutboundTaskId",
                table: "PickTasks",
                column: "OutboundTaskId",
                principalTable: "OutboundTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PickTasks_OutboundTasks_OutboundTaskId",
                table: "PickTasks");

            migrationBuilder.DropIndex(
                name: "IX_PickTasks_OutboundTaskId",
                table: "PickTasks");

            migrationBuilder.DropColumn(
                name: "OutboundTaskId",
                table: "PickTasks");
        }
    }
}
