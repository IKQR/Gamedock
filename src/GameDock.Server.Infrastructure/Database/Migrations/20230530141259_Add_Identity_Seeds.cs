using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GameDock.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Add_Identity_Seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "313f6f44-4aa3-4720-beed-23fdf2a31b01", null, "User", "USER" },
                    { "4459f5a8-08f4-4303-b506-534fa2bed5dd", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "20ceda78-8778-488a-9ae1-321a1ce43bfb", 0, "055ecc95-3dc4-4b6c-bd2a-b14064e2ef70", "admin@admin.org", false, false, null, "ADMIN@ADMIN.ORG", "ADMIN", "AQAAAAIAAYagAAAAEC/i79f3/vS6uoQxe8k6UgADnJ6/4SwdVhuAI3KNWAyDgiTdQbZIj7wGAL7/EKG4Jw==", null, false, "a9b00ba1-84bc-4c4a-80f8-4d5ff33c15a5", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4459f5a8-08f4-4303-b506-534fa2bed5dd", "20ceda78-8778-488a-9ae1-321a1ce43bfb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "313f6f44-4aa3-4720-beed-23fdf2a31b01");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4459f5a8-08f4-4303-b506-534fa2bed5dd", "20ceda78-8778-488a-9ae1-321a1ce43bfb" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4459f5a8-08f4-4303-b506-534fa2bed5dd");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "20ceda78-8778-488a-9ae1-321a1ce43bfb");
        }
    }
}
