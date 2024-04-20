using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodigoDelSurApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_DB_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FavoriteGenre",
                table: "UserPreferences",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "EnableNotifications",
                table: "UserPreferences",
                newName: "Genre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Language",
                table: "UserPreferences",
                newName: "FavoriteGenre");

            migrationBuilder.RenameColumn(
                name: "Genre",
                table: "UserPreferences",
                newName: "EnableNotifications");
        }
    }
}
