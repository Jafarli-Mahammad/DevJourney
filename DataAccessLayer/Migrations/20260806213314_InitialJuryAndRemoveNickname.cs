using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialJuryAndRemoveNickname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentLanguages",
                schema: "Student");

            migrationBuilder.DropTable(
                name: "StudentSkills",
                schema: "Student");

            migrationBuilder.DropColumn(
                name: "Achievements",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Age",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Experience",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "PreferredWorkFormat",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.RenameColumn(
                name: "Location",
                schema: "Student",
                table: "StudentProfiles",
                newName: "ProfessionalRole");

            migrationBuilder.AddColumn<Guid>(
                name: "UniversityId",
                schema: "Student",
                table: "StudentProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JuryProfiles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JuryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompetitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_JuryProfiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JuryProfiles",
                schema: "Identity");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.RenameColumn(
                name: "ProfessionalRole",
                schema: "Student",
                table: "StudentProfiles",
                newName: "Location");

            migrationBuilder.AddColumn<string>(
                name: "Achievements",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                schema: "Student",
                table: "StudentProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkFormat",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentLanguages",
                schema: "Student",
                columns: table => new
                {
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProficiencyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLanguages", x => new { x.StudentProfileId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_StudentLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Student",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLanguages_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalSchema: "Student",
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                schema: "Student",
                columns: table => new
                {
                    StudentProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSkills", x => new { x.StudentProfileId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_StudentSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Student",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSkills_StudentProfiles_StudentProfileId",
                        column: x => x.StudentProfileId,
                        principalSchema: "Student",
                        principalTable: "StudentProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLanguages_LanguageId",
                schema: "Student",
                table: "StudentLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillId",
                schema: "Student",
                table: "StudentSkills",
                column: "SkillId");
        }
    }
}
