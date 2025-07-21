using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MadeHuman_Server.Migrations
{
    /// <inheritdoc />
    public partial class addtableCheckTask_Details : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MadeAt = table.Column<string>(type: "text", nullable: false),
                    StatusCheckTask = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsersTasksId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutboundTaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckTasks_OutboundTasks_OutboundTaskId",
                        column: x => x.OutboundTaskId,
                        principalTable: "OutboundTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckTasks_UsersTasks_UsersTasksId",
                        column: x => x.UsersTasksId,
                        principalTable: "UsersTasks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CheckTaskDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StatusCheckDetailTask = table.Column<int>(type: "integer", nullable: false),
                    FinishAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CheckTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundTaskItemId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckTaskDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckTaskDetails_CheckTasks_CheckTaskId",
                        column: x => x.CheckTaskId,
                        principalTable: "CheckTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckTaskDetails_OutboundTaskItems_OutboundTaskItemId",
                        column: x => x.OutboundTaskItemId,
                        principalTable: "OutboundTaskItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckTaskDetails_CheckTaskId",
                table: "CheckTaskDetails",
                column: "CheckTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckTaskDetails_OutboundTaskItemId",
                table: "CheckTaskDetails",
                column: "OutboundTaskItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckTasks_OutboundTaskId",
                table: "CheckTasks",
                column: "OutboundTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckTasks_UsersTasksId",
                table: "CheckTasks",
                column: "UsersTasksId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckTaskDetails");

            migrationBuilder.DropTable(
                name: "CheckTasks");
        }
    }
}
