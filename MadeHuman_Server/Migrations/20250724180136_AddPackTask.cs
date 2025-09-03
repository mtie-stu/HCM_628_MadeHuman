using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPackTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MadeAt = table.Column<string>(type: "text", nullable: false),
                    StatusPackTask = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboundTaskItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackTask_OutboundTaskItems_OutboundTaskItemId",
                        column: x => x.OutboundTaskItemId,
                        principalTable: "OutboundTaskItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackTask_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackTask_OutboundTaskItemId",
                table: "PackTask",
                column: "OutboundTaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackTask_UsersTasksId",
                table: "PackTask",
                column: "UsersTasksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackTask");
        }
    }
}
