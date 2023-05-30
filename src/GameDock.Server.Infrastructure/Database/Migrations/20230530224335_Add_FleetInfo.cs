using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDock.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_FleetInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FleetInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BuildId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Runtime = table.Column<string>(type: "TEXT", nullable: false),
                    Ports = table.Column<string>(type: "TEXT", nullable: false),
                    LaunchParameters = table.Column<string>(type: "TEXT", nullable: true),
                    Variables = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    ImageId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FleetInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FleetInfos_BuildInfos_BuildId",
                        column: x => x.BuildId,
                        principalTable: "BuildInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20ceda78-8778-488a-9ae1-321a1ce43bfb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd890037-7e9c-4f9a-a866-ae4823912f91", "AQAAAAIAAYagAAAAEEA4kq0GcyI3sliB6lNA95ZfJZY/0+4BqjhSkhqYVgQ6LcY9wj2fHkjH2KEntENHDA==", "9cde62ed-61f8-46ce-b3c9-9c960dc5b063" });

            migrationBuilder.CreateIndex(
                name: "IX_FleetInfos_BuildId",
                table: "FleetInfos",
                column: "BuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FleetInfos");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20ceda78-8778-488a-9ae1-321a1ce43bfb",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "055ecc95-3dc4-4b6c-bd2a-b14064e2ef70", "AQAAAAIAAYagAAAAEC/i79f3/vS6uoQxe8k6UgADnJ6/4SwdVhuAI3KNWAyDgiTdQbZIj7wGAL7/EKG4Jw==", "a9b00ba1-84bc-4c4a-80f8-4d5ff33c15a5" });
        }
    }
}
