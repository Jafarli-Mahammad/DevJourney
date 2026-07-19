using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPostTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseTypes",
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
                    table.PrimaryKey("PK_CourseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventTypes",
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
                    table.PrimaryKey("PK_EventTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NetworkingEventPosts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,9)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,9)", nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxAttendees = table.Column<int>(type: "int", nullable: false),
                    TicketContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkingEventPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NetworkingEventPosts_Posts_Id",
                        column: x => x.Id,
                        principalSchema: "Posts",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "B2BCoursePromoPosts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CourseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetMajor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LocationOrLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaxAttendees = table.Column<int>(type: "int", nullable: false),
                    DurationInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HasDiscount = table.Column<bool>(type: "bit", nullable: false),
                    DiscountNote = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HasCertificate = table.Column<bool>(type: "bit", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InstructorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstructorLinkedIn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    InstructorReviewsLink = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2BCoursePromoPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B2BCoursePromoPosts_CourseTypes_CourseTypeId",
                        column: x => x.CourseTypeId,
                        principalSchema: "Identity",
                        principalTable: "CourseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_B2BCoursePromoPosts_Posts_Id",
                        column: x => x.Id,
                        principalSchema: "Posts",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateEventPosts",
                schema: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EventTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialOccasion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,9)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,9)", nullable: true),
                    TargetAudience = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaxAttendees = table.Column<int>(type: "int", nullable: false),
                    ConfidentialityNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApplicationMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EventLanguage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateEventPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateEventPosts_EventTypes_EventTypeId",
                        column: x => x.EventTypeId,
                        principalSchema: "Identity",
                        principalTable: "EventTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorporateEventPosts_Posts_Id",
                        column: x => x.Id,
                        principalSchema: "Posts",
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NetworkingEventStops",
                schema: "Posts",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkingEventStops", x => new { x.PostId, x.Order });
                    table.ForeignKey(
                        name: "FK_NetworkingEventStops_NetworkingEventPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Posts",
                        principalTable: "NetworkingEventPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateEventAgendaItems",
                schema: "Posts",
                columns: table => new
                {
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activity = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateEventAgendaItems", x => new { x.PostId, x.Order });
                    table.ForeignKey(
                        name: "FK_CorporateEventAgendaItems_CorporateEventPosts_PostId",
                        column: x => x.PostId,
                        principalSchema: "Posts",
                        principalTable: "CorporateEventPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt",
                schema: "Posts",
                table: "Posts",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_B2BCoursePromoPosts_CourseTypeId",
                schema: "Posts",
                table: "B2BCoursePromoPosts",
                column: "CourseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateEventPosts_EventTypeId",
                schema: "Posts",
                table: "CorporateEventPosts",
                column: "EventTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B2BCoursePromoPosts",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "CorporateEventAgendaItems",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "NetworkingEventStops",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "CourseTypes",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "CorporateEventPosts",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "NetworkingEventPosts",
                schema: "Posts");

            migrationBuilder.DropTable(
                name: "EventTypes",
                schema: "Identity");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedAt",
                schema: "Posts",
                table: "Posts");
        }
    }
}
