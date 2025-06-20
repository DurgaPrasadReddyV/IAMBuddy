using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IAMBuddy.SqlServerManagementService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SqlServerLogins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoginType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sid = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    PasswordExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServerInstance = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlServerLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SqlServerRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RoleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ServerInstance = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlServerRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SqlServerUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ServerInstance = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Sid = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DefaultSchema = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    LoginId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlServerUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SqlServerUsers_SqlServerLogins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "SqlServerLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SqlServerRoleAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    LoginId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    ServerInstance = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignmentReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlServerRoleAssignments", x => x.Id);
                    table.CheckConstraint("CK_RoleAssignment_LoginOrUser", "(LoginId IS NOT NULL AND UserId IS NULL) OR (LoginId IS NULL AND UserId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SqlServerRoleAssignments_SqlServerLogins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "SqlServerLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SqlServerRoleAssignments_SqlServerRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SqlServerRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SqlServerRoleAssignments_SqlServerUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "SqlServerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SqlServerOperations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ServerInstance = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DatabaseName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ResourceName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OperationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    LoginId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    RoleAssignmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SqlServerOperations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SqlServerOperations_SqlServerLogins_LoginId",
                        column: x => x.LoginId,
                        principalTable: "SqlServerLogins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SqlServerOperations_SqlServerRoleAssignments_RoleAssignmentId",
                        column: x => x.RoleAssignmentId,
                        principalTable: "SqlServerRoleAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SqlServerOperations_SqlServerRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SqlServerRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SqlServerOperations_SqlServerUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "SqlServerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "SqlServerRoles",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DatabaseName", "Description", "IsBuiltIn", "IsEnabled", "ModifiedBy", "ModifiedDate", "RoleName", "RoleType", "ServerInstance" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(7240), null, "Members can perform any activity in the server", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(7240), "sysadmin", "Server", "Default" },
                    { 2, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), null, "Members can change server-wide configuration options and shut down the server", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), "serveradmin", "Server", "Default" },
                    { 3, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), null, "Members can manage logins and their properties", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), "securityadmin", "Server", "Default" },
                    { 4, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), null, "Members can end processes that are running in an instance of SQL Server", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), "processadmin", "Server", "Default" },
                    { 5, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), null, "Members can add and remove linked servers", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), "setupadmin", "Server", "Default" },
                    { 6, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), null, "Members can run the BULK INSERT statement", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8400), "bulkadmin", "Server", "Default" },
                    { 7, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), null, "Members can manage disk files", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), "diskadmin", "Server", "Default" },
                    { 8, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), null, "Members can create, alter, drop, and restore any database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), "dbcreator", "Server", "Default" },
                    { 9, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), null, "Every SQL Server login belongs to the public server role", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), "public", "Server", "Default" },
                    { 10, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), "Default", "Members can perform all configuration and maintenance activities on the database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8410), "db_owner", "Database", "Default" },
                    { 11, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "Default", "Members can add or remove access to the database for Windows logins, Windows groups, and SQL Server logins", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "db_accessadmin", "Database", "Default" },
                    { 12, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "Default", "Members can modify role membership and manage permissions", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "db_securityadmin", "Database", "Default" },
                    { 13, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "Default", "Members can run any Data Definition Language (DDL) command in a database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "db_ddladmin", "Database", "Default" },
                    { 14, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "Default", "Members can back up the database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8540), "db_backupoperator", "Database", "Default" },
                    { 15, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "Default", "Members can read all data from all user tables", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "db_datareader", "Database", "Default" },
                    { 16, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "Default", "Members can add, change, or delete data from all user tables", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "db_datawriter", "Database", "Default" },
                    { 17, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "Default", "Members cannot read any data in the user tables within a database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "db_denydatareader", "Database", "Default" },
                    { 18, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "Default", "Members cannot add, modify, or delete any data in the user tables within a database", true, true, "System", new DateTime(2025, 6, 20, 11, 4, 31, 819, DateTimeKind.Utc).AddTicks(8550), "db_denydatawriter", "Database", "Default" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerLogins_LoginName_ServerInstance",
                table: "SqlServerLogins",
                columns: new[] { "LoginName", "ServerInstance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_CreatedDate",
                table: "SqlServerOperations",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_LoginId",
                table: "SqlServerOperations",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_RequestId",
                table: "SqlServerOperations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_RoleAssignmentId",
                table: "SqlServerOperations",
                column: "RoleAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_RoleId",
                table: "SqlServerOperations",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_Status",
                table: "SqlServerOperations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerOperations_UserId",
                table: "SqlServerOperations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerRoleAssignments_LoginId",
                table: "SqlServerRoleAssignments",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerRoleAssignments_RoleId",
                table: "SqlServerRoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerRoleAssignments_UserId",
                table: "SqlServerRoleAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerRoles_RoleName_ServerInstance_DatabaseName",
                table: "SqlServerRoles",
                columns: new[] { "RoleName", "ServerInstance", "DatabaseName" },
                unique: true,
                filter: "[DatabaseName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerUsers_LoginId",
                table: "SqlServerUsers",
                column: "LoginId");

            migrationBuilder.CreateIndex(
                name: "IX_SqlServerUsers_UserName_DatabaseName_ServerInstance",
                table: "SqlServerUsers",
                columns: new[] { "UserName", "DatabaseName", "ServerInstance" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SqlServerOperations");

            migrationBuilder.DropTable(
                name: "SqlServerRoleAssignments");

            migrationBuilder.DropTable(
                name: "SqlServerRoles");

            migrationBuilder.DropTable(
                name: "SqlServerUsers");

            migrationBuilder.DropTable(
                name: "SqlServerLogins");
        }
    }
}
