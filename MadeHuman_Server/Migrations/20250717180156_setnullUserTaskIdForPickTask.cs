using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class setnullUserTaskIdForPickTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                table: "Baskets");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserTaskId",
                table: "PickTasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "StockQuantity",
                table: "Inventory",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuantityBooked",
                table: "Inventory",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OutBoundTaskId",
                table: "Baskets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                table: "Baskets",
                column: "OutBoundTaskId",
                principalTable: "OutboundTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                table: "Baskets");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserTaskId",
                table: "PickTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StockQuantity",
                table: "Inventory",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "QuantityBooked",
                table: "Inventory",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "OutBoundTaskId",
                table: "Baskets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_OutboundTasks_OutBoundTaskId",
                table: "Baskets",
                column: "OutBoundTaskId",
                principalTable: "OutboundTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
