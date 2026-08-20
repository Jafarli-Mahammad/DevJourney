using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                schema: "Partner",
                table: "PartnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                schema: "Partner",
                table: "PartnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                schema: "Partner",
                table: "PartnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeName",
                schema: "Partner",
                table: "PartnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeRole",
                schema: "Partner",
                table: "PartnerProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCertificatesPublished",
                schema: "Identity",
                table: "Competitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsJuryActive",
                schema: "Identity",
                table: "Competitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistrationOpen",
                schema: "Identity",
                table: "Competitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsScoreboardLive",
                schema: "Identity",
                table: "Competitions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldAt",
                schema: "Identity",
                table: "CompetitionParticipants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalist",
                schema: "Identity",
                table: "CompetitionParticipants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CheckInLogs",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckInLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyInvitations",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerType = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CaptainId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFinalist = table.Column<bool>(type: "bit", nullable: false),
                    RepoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PitchDeckUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalSchema: "Identity",
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionTeamMembers_TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CompetitionId",
                schema: "Identity",
                table: "Teams",
                column: "CompetitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionTeamMembers_Teams_TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers",
                column: "TeamId",
                principalSchema: "Identity",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionTeamMembers_Teams_TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers");

            migrationBuilder.DropTable(
                name: "CheckInLogs",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "CompanyInvitations",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionTeamMembers_TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                schema: "Partner",
                table: "PartnerProfiles");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                schema: "Partner",
                table: "PartnerProfiles");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                schema: "Partner",
                table: "PartnerProfiles");

            migrationBuilder.DropColumn(
                name: "RepresentativeName",
                schema: "Partner",
                table: "PartnerProfiles");

            migrationBuilder.DropColumn(
                name: "RepresentativeRole",
                schema: "Partner",
                table: "PartnerProfiles");

            migrationBuilder.DropColumn(
                name: "TeamId",
                schema: "Identity",
                table: "CompetitionTeamMembers");

            migrationBuilder.DropColumn(
                name: "IsCertificatesPublished",
                schema: "Identity",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "IsJuryActive",
                schema: "Identity",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "IsRegistrationOpen",
                schema: "Identity",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "IsScoreboardLive",
                schema: "Identity",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "HoldAt",
                schema: "Identity",
                table: "CompetitionParticipants");

            migrationBuilder.DropColumn(
                name: "IsFinalist",
                schema: "Identity",
                table: "CompetitionParticipants");
        }
    }
}
