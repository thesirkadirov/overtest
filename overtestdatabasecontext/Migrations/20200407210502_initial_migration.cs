using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sirkadirov.Overtest.Libraries.Shared.Database.Migrations
{
    public partial class initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    SyntaxHighlightingOptions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingLanguages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingTaskCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingTaskCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Source = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhotos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastTestingDataModification = table.Column<DateTime>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false, defaultValue: true),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Difficulty = table.Column<byte>(nullable: false, defaultValue: (byte)100),
                    CategoryId = table.Column<Guid>(nullable: false),
                    TestingDataId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammingTasks_ProgrammingTaskCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProgrammingTaskCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgrammingTaskTestingDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DataPackageFile = table.Column<byte[]>(nullable: false),
                    DataPackageHash = table.Column<string>(nullable: false),
                    ProgrammingTaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingTaskTestingDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgrammingTaskTestingDatas_ProgrammingTasks_ProgrammingTask~",
                        column: x => x.ProgrammingTaskId,
                        principalTable: "ProgrammingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Competitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    VisibleTo = table.Column<int>(nullable: false, defaultValue: 3),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Starts = table.Column<DateTime>(nullable: false),
                    Ends = table.Column<DateTime>(nullable: false),
                    PinCodeEnabled = table.Column<bool>(nullable: false, defaultValue: false),
                    PinCodeClearText = table.Column<string>(nullable: false),
                    UserExitEnabled = table.Column<bool>(nullable: false, defaultValue: false),
                    UserExitAction = table.Column<int>(nullable: false, defaultValue: 0),
                    EnableWaitingPage = table.Column<bool>(nullable: false, defaultValue: false),
                    WaitingPageActivationTime = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CuratorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionProgrammingTasks",
                columns: table => new
                {
                    CompetitionId = table.Column<Guid>(nullable: false),
                    ProgrammingTaskId = table.Column<Guid>(nullable: false),
                    JudgementType = table.Column<int>(nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionProgrammingTasks", x => new { x.CompetitionId, x.ProgrammingTaskId });
                    table.ForeignKey(
                        name: "FK_CompetitionProgrammingTasks_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionProgrammingTasks_ProgrammingTasks_ProgrammingTask~",
                        column: x => x.ProgrammingTaskId,
                        principalTable: "ProgrammingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetitionUsers",
                columns: table => new
                {
                    CompetitionId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionUsers", x => new { x.CompetitionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CompetitionUsers_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestingApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    ProcessingTime = table.Column<TimeSpan>(nullable: false, defaultValue: new TimeSpan(0, 0, 0, 0, 0)),
                    TestingType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false, defaultValue: 0),
                    SourceCode_SourceCode = table.Column<byte[]>(nullable: true),
                    SourceCode_ProgrammingLanguageId = table.Column<Guid>(nullable: true),
                    TestingResults_RawTestingResults = table.Column<string>(nullable: true),
                    TestingResults_CompletionPercentage = table.Column<byte>(nullable: true, defaultValue: (byte)0),
                    TestingResults_SolutionAdjudgement = table.Column<int>(nullable: true, defaultValue: 0),
                    AuthorId = table.Column<Guid>(nullable: false),
                    CompetitionId = table.Column<Guid>(nullable: true),
                    ProgrammingTaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestingApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestingApplications_ProgrammingLanguages_SourceCode_Programm~",
                        column: x => x.SourceCode_ProgrammingLanguageId,
                        principalTable: "ProgrammingLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestingApplications_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestingApplications_ProgrammingTasks_ProgrammingTaskId",
                        column: x => x.ProgrammingTaskId,
                        principalTable: "ProgrammingTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    CuratorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    InstitutionName = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsBanned = table.Column<bool>(nullable: false, defaultValue: false),
                    Registered = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    LastSeen = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified)),
                    CuratorId = table.Column<Guid>(nullable: true),
                    UserGroupId = table.Column<Guid>(nullable: true),
                    UserPhotoId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_AspNetUsers_CuratorId",
                        column: x => x.CuratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_UserPhotos_UserPhotoId",
                        column: x => x.UserPhotoId,
                        principalTable: "UserPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CuratorId",
                table: "AspNetUsers",
                column: "CuratorId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserGroupId",
                table: "AspNetUsers",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserPhotoId",
                table: "AspNetUsers",
                column: "UserPhotoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionProgrammingTasks_ProgrammingTaskId",
                table: "CompetitionProgrammingTasks",
                column: "ProgrammingTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Competitions_CuratorId",
                table: "Competitions",
                column: "CuratorId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionUsers_UserId",
                table: "CompetitionUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammingTasks_CategoryId",
                table: "ProgrammingTasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgrammingTaskTestingDatas_ProgrammingTaskId",
                table: "ProgrammingTaskTestingDatas",
                column: "ProgrammingTaskId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestingApplications_SourceCode_ProgrammingLanguageId",
                table: "TestingApplications",
                column: "SourceCode_ProgrammingLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingApplications_AuthorId",
                table: "TestingApplications",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingApplications_CompetitionId",
                table: "TestingApplications",
                column: "CompetitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TestingApplications_ProgrammingTaskId",
                table: "TestingApplications",
                column: "ProgrammingTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_CuratorId_DisplayName",
                table: "UserGroups",
                columns: new[] { "CuratorId", "DisplayName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Competitions_AspNetUsers_CuratorId",
                table: "Competitions",
                column: "CuratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionUsers_AspNetUsers_UserId",
                table: "CompetitionUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestingApplications_AspNetUsers_AuthorId",
                table: "TestingApplications",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_AspNetUsers_CuratorId",
                table: "UserGroups",
                column: "CuratorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_AspNetUsers_CuratorId",
                table: "UserGroups");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CompetitionProgrammingTasks");

            migrationBuilder.DropTable(
                name: "CompetitionUsers");

            migrationBuilder.DropTable(
                name: "ProgrammingTaskTestingDatas");

            migrationBuilder.DropTable(
                name: "TestingApplications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguages");

            migrationBuilder.DropTable(
                name: "Competitions");

            migrationBuilder.DropTable(
                name: "ProgrammingTasks");

            migrationBuilder.DropTable(
                name: "ProgrammingTaskCategories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "UserPhotos");
        }
    }
}
