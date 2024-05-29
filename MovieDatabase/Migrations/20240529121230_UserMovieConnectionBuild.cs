using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieDatabase.Migrations
{
    /// <inheritdoc />
    public partial class UserMovieConnectionBuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieUser_Movie_watchlistid",
                table: "MovieUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser");

            migrationBuilder.DropIndex(
                name: "IX_MovieUser_watchlistid",
                table: "MovieUser");

            migrationBuilder.RenameColumn(
                name: "watchlistid",
                table: "MovieUser",
                newName: "moviesid");

            migrationBuilder.AddColumn<int>(
                name: "related_account_id",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser",
                columns: new[] { "moviesid", "usersid" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieUser_usersid",
                table: "MovieUser",
                column: "usersid");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieUser_Movie_moviesid",
                table: "MovieUser",
                column: "moviesid",
                principalTable: "Movie",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddColumn<int>(
                name: "context_id",
                table: "MovieUser",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieUser_Movie_moviesid",
                table: "MovieUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser");

            migrationBuilder.DropIndex(
                name: "IX_MovieUser_usersid",
                table: "MovieUser");

            migrationBuilder.DropColumn(
                name: "related_account_id",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "moviesid",
                table: "MovieUser",
                newName: "watchlistid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovieUser",
                table: "MovieUser",
                columns: new[] { "usersid", "watchlistid" });

            migrationBuilder.CreateIndex(
                name: "IX_MovieUser_watchlistid",
                table: "MovieUser",
                column: "watchlistid");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieUser_Movie_watchlistid",
                table: "MovieUser",
                column: "watchlistid",
                principalTable: "Movie",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
