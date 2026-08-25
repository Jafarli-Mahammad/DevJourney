using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionAndProfilePerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_JuryId",
                schema: "Identity",
                table: "Evaluations",
                column: "JuryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTeamMembers_StudentProfileId",
                schema: "Identity",
                table: "CompetitionTeamMembers",
                column: "StudentProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionParticipants_CaptainId",
                schema: "Identity",
                table: "CompetitionParticipants",
                column: "CaptainId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionParticipants_CompetitionId",
                schema: "Identity",
                table: "CompetitionParticipants",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionParticipants_CompetitionId_Status",
                schema: "Identity",
                table: "CompetitionParticipants",
                columns: new[] { "CompetitionId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionParticipants_IndividualStudentId",
                schema: "Identity",
                table: "CompetitionParticipants",
                column: "IndividualStudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evaluations_JuryId",
                schema: "Identity",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionTeamMembers_StudentProfileId",
                schema: "Identity",
                table: "CompetitionTeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionParticipants_CaptainId",
                schema: "Identity",
                table: "CompetitionParticipants");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionParticipants_CompetitionId",
                schema: "Identity",
                table: "CompetitionParticipants");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionParticipants_CompetitionId_Status",
                schema: "Identity",
                table: "CompetitionParticipants");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionParticipants_IndividualStudentId",
                schema: "Identity",
                table: "CompetitionParticipants");
        }
    }
}
