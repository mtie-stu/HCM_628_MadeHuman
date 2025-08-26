using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class MadeAt_Null : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackTask_OutboundTaskItems_OutboundTaskItemId",
                table: "PackTask");

            migrationBuilder.AlterColumn<Guid>(
                name: "OutboundTaskItemId",
                table: "PackTask",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MadeAt",
                table: "PackTask",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_PackTask_OutboundTaskItems_OutboundTaskItemId",
                table: "PackTask",
                column: "OutboundTaskItemId",
                principalTable: "OutboundTaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackTask_OutboundTaskItems_OutboundTaskItemId",
                table: "PackTask");

            migrationBuilder.AlterColumn<Guid>(
                name: "OutboundTaskItemId",
                table: "PackTask",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "MadeAt",
                table: "PackTask",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PackTask_OutboundTaskItems_OutboundTaskItemId",
                table: "PackTask",
                column: "OutboundTaskItemId",
                principalTable: "OutboundTaskItems",
                principalColumn: "Id");
        }
    }
}
