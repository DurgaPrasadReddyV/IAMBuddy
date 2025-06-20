using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAMBuddy.RequestIntakeService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BusinessJustification = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApproverEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkflowId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_RequestedDate",
                table: "AccountRequests",
                column: "RequestedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_RequestorEmail",
                table: "AccountRequests",
                column: "RequestorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_Status",
                table: "AccountRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_Username_Server_Database",
                table: "AccountRequests",
                columns: new[] { "Username", "ServerName", "DatabaseName" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountRequests_WorkflowId",
                table: "AccountRequests",
                column: "WorkflowId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountRequests");
        }
    }
}
