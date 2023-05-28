using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameDock.Server.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Rework_BuildInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RuntimePath",
                table: "BuildInfos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuntimePath",
                table: "BuildInfos");
        }
    }
}
