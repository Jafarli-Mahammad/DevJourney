using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class PostEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Posts");

            migrationBuilder.CreateTable(
                name: "IdeaField",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_IdeaField", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_ApplicationUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Identity",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberSearchPosts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdeaFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MembersNeeded = table.Column<int>(type: "int", nullable: false),
                    WorkMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SocialLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LookingForAge = table.Column<int>(type: "int", nullable: true),
                    LookingForLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LookingForLanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdditionalNote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberSearchPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPosts_IdeaField_IdeaFieldId",
                        column: x => x.IdeaFieldId,
                        principalSchema: "Identity",
                        principalTable: "IdeaField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPosts_Languages_LookingForLanguageId",
                        column: x => x.LookingForLanguageId,
                        principalSchema: "Student",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPosts_Posts_Id",
                        column: x => x.Id,
                        principalSchema: "Posts",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamSearchPosts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamSearchPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamSearchPosts_Posts_Id",
                        column: x => x.Id,
                        principalSchema: "Posts",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberSearchPostRoles",
                schema: "Posts",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberSearchPostRoles", x => new { x.PostId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPostRoles_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPostRoles_TeamMemberSearchPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Posts",
                        principalTable: "TeamMemberSearchPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberSearchPostSkills",
                schema: "Posts",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberSearchPostSkills", x => new { x.PostId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPostSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Student",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamMemberSearchPostSkills_TeamMemberSearchPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Posts",
                        principalTable: "TeamMemberSearchPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamSearchPostIdeaFields",
                schema: "Posts",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdeaFieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamSearchPostIdeaFields", x => new { x.PostId, x.IdeaFieldId });
                    table.ForeignKey(
                        name: "FK_TeamSearchPostIdeaFields_IdeaField_IdeaFieldId",
                        column: x => x.IdeaFieldId,
                        principalSchema: "Identity",
                        principalTable: "IdeaField",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamSearchPostIdeaFields_TeamSearchPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Posts",
                        principalTable: "TeamSearchPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                schema: "Posts",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberSearchPostRoles_RoleId",
                schema: "Posts",
                table: "TeamMemberSearchPostRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberSearchPosts_IdeaFieldId",
                schema: "Posts",
                table: "TeamMemberSearchPosts",
                column: "IdeaFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberSearchPosts_LookingForLanguageId",
                schema: "Posts",
                table: "TeamMemberSearchPosts",
                column: "LookingForLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberSearchPostSkills_SkillId",
                schema: "Posts",
                table: "TeamMemberSearchPostSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamSearchPostIdeaFields_IdeaFieldId",
                schema: "Posts",
                table: "TeamSearchPostIdeaFields",
                column: "IdeaFieldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMemberSearchPostRoles",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "TeamMemberSearchPostSkills",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "TeamSearchPostIdeaFields",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "TeamMemberSearchPosts",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "TeamSearchPosts",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "IdeaField",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "Posts");
        }
    }
}
