using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class IdeaField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMemberSearchPosts_IdeaField_IdeaFieldId",
                schema: "Posts",
                table: "TeamMemberSearchPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamSearchPostIdeaFields_IdeaField_IdeaFieldId",
                schema: "Posts",
                table: "TeamSearchPostIdeaFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdeaField",
                schema: "Identity",
                table: "IdeaField");

            migrationBuilder.RenameTable(
                name: "IdeaField",
                schema: "Identity",
                newName: "LookupIdeaFields",
                newSchema: "Identity");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Identity",
                table: "Role",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LookupIdeaFields",
                schema: "Identity",
                table: "LookupIdeaFields",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMemberSearchPosts_LookupIdeaFields_IdeaFieldId",
                schema: "Posts",
                table: "TeamMemberSearchPosts",
                column: "IdeaFieldId",
                principalSchema: "Identity",
                principalTable: "LookupIdeaFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamSearchPostIdeaFields_LookupIdeaFields_IdeaFieldId",
                schema: "Posts",
                table: "TeamSearchPostIdeaFields",
                column: "IdeaFieldId",
                principalSchema: "Identity",
                principalTable: "LookupIdeaFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMemberSearchPosts_LookupIdeaFields_IdeaFieldId",
                schema: "Posts",
                table: "TeamMemberSearchPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamSearchPostIdeaFields_LookupIdeaFields_IdeaFieldId",
                schema: "Posts",
                table: "TeamSearchPostIdeaFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LookupIdeaFields",
                schema: "Identity",
                table: "LookupIdeaFields");

            migrationBuilder.RenameTable(
                name: "LookupIdeaFields",
                schema: "Identity",
                newName: "IdeaField",
                newSchema: "Identity");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Identity",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdeaField",
                schema: "Identity",
                table: "IdeaField",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMemberSearchPosts_IdeaField_IdeaFieldId",
                schema: "Posts",
                table: "TeamMemberSearchPosts",
                column: "IdeaFieldId",
                principalSchema: "Identity",
                principalTable: "IdeaField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamSearchPostIdeaFields_IdeaField_IdeaFieldId",
                schema: "Posts",
                table: "TeamSearchPostIdeaFields",
                column: "IdeaFieldId",
                principalSchema: "Identity",
                principalTable: "IdeaField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
