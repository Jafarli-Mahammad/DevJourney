using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCabinetEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfessionalRole",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Course",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExperienceLevel",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MainRoleId",
                schema: "Student",
                table: "StudentProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioUrl",
                schema: "Student",
                table: "StudentProfiles",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                schema: "Student",
                table: "StudentProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MainRoles",
                schema: "Student",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("PK_MainRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Professions",
                schema: "Student",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
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
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

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
                name: "IX_StudentProfiles_MainRoleId",
                schema: "Student",
                table: "StudentProfiles",
                column: "MainRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_ProfessionId",
                schema: "Student",
                table: "StudentProfiles",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfiles_UniversityId",
                schema: "Student",
                table: "StudentProfiles",
                column: "UniversityId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_MainRoles_MainRoleId",
                schema: "Student",
                table: "StudentProfiles",
                column: "MainRoleId",
                principalSchema: "Student",
                principalTable: "MainRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_Professions_ProfessionId",
                schema: "Student",
                table: "StudentProfiles",
                column: "ProfessionId",
                principalSchema: "Student",
                principalTable: "Professions",
                principalColumn: "Id");

            migrationBuilder.Sql("UPDATE [Student].[StudentProfiles] SET [UniversityId] = NULL WHERE [UniversityId] IS NOT NULL AND [UniversityId] NOT IN (SELECT [Id] FROM [University].[UniversityProfiles]);");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_UniversityProfiles_UniversityId",
                schema: "Student",
                table: "StudentProfiles",
                column: "UniversityId",
                principalSchema: "University",
                principalTable: "UniversityProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_MainRoles_MainRoleId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_Professions_ProfessionId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_UniversityProfiles_UniversityId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropTable(
                name: "MainRoles",
                schema: "Student");

            migrationBuilder.DropTable(
                name: "Professions",
                schema: "Student");

            migrationBuilder.DropTable(
                name: "StudentLanguages",
                schema: "Student");

            migrationBuilder.DropTable(
                name: "StudentSkills",
                schema: "Student");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_MainRoleId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_ProfessionId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropIndex(
                name: "IX_StudentProfiles_UniversityId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "Course",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "MainRoleId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "PortfolioUrl",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                schema: "Student",
                table: "StudentProfiles");

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalRole",
                schema: "Student",
                table: "StudentProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
