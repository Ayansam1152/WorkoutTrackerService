using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTrackerServices.Migrations
{
    /// <inheritdoc />
    public partial class foreignkeyinchatHistorywithusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "LlmChatHistories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "IX_LlmChatHistories_UserId",
                table: "LlmChatHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "LlmChatHistory_UserId_fkey",
                table: "LlmChatHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "LlmChatHistory_UserId_fkey",
                table: "LlmChatHistories");

            migrationBuilder.DropIndex(
                name: "IX_LlmChatHistories_UserId",
                table: "LlmChatHistories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "LlmChatHistories",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
