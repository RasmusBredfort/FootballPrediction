using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballPrediction.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FootballDataMatchId",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MatchDay",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeasonStartYear",
                table: "Matches",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Matches",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FootballDataMatchId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "MatchDay",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SeasonStartYear",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Matches");
        }
    }
}
