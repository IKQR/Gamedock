using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDock.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_SessionInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessionInfoEntity",
                columns: table => new
                {
                    ContainerId = table.Column<string>(type: "TEXT", nullable: false),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    Ports = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionInfoEntity", x => x.ContainerId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20ceda78-8778-488a-9ae1-321a1ce43bfb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ee1e7131-85aa-479a-97a6-9013e2ba112c", "AQAAAAIAAYagAAAAEDX3Wcik8xyfWbGMRr9NTkU+rhHB+nS53SDGRwZ8enD3sy6C1tn8WUzE3LSkkaPB9g==", "51db06df-a72c-499f-a809-3e0712655e5a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionInfoEntity");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20ceda78-8778-488a-9ae1-321a1ce43bfb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd890037-7e9c-4f9a-a866-ae4823912f91", "AQAAAAIAAYagAAAAEEA4kq0GcyI3sliB6lNA95ZfJZY/0+4BqjhSkhqYVgQ6LcY9wj2fHkjH2KEntENHDA==", "9cde62ed-61f8-46ce-b3c9-9c960dc5b063" });
        }
    }
}
