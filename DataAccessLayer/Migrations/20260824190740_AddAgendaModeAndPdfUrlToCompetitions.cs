using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddAgendaModeAndPdfUrlToCompetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgendaMode",
                schema: "Identity",
                table: "Competitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AgendaPdfUrl",
                schema: "Identity",
                table: "Competitions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgendaMode",
                schema: "Identity",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "AgendaPdfUrl",
                schema: "Identity",
                table: "Competitions");
        }
    }
}
