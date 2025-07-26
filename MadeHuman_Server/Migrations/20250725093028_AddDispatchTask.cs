using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatchTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispatchTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StatusDispatchTasks = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboundTaskItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchTasks_OutboundTaskItems_OutboundTaskItemId",
                        column: x => x.OutboundTaskItemId,
                        principalTable: "OutboundTaskItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispatchTasks_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTasks_OutboundTaskItemId",
                table: "DispatchTasks",
                column: "OutboundTaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchTasks_UsersTasksId",
                table: "DispatchTasks",
                column: "UsersTasksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchTasks");
        }
    }
}
