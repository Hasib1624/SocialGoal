using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SocialGoal.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilePicUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activated = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommentUsers",
                columns: table => new
                {
                    CommentUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentUsers", x => x.CommentUserId);
                });

            migrationBuilder.CreateTable(
                name: "FollowRequest",
                columns: table => new
                {
                    FollowRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowRequest", x => x.FollowRequestId);
                });

            migrationBuilder.CreateTable(
                name: "GoalStatus",
                columns: table => new
                {
                    GoalStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalStatusType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalStatus", x => x.GoalStatusId);
                });

            migrationBuilder.CreateTable(
                name: "GroupCommentUsers",
                columns: table => new
                {
                    GroupCommentUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupCommentId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupCommentUsers", x => x.GroupCommentUserId);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "GroupUpdateUsers",
                columns: table => new
                {
                    GroupUpdateUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupUpdateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUpdateUsers", x => x.GroupUpdateUserId);
                });

            migrationBuilder.CreateTable(
                name: "GroupUsers",
                columns: table => new
                {
                    GroupUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Admin = table.Column<bool>(type: "bit", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUsers", x => x.GroupUserId);
                });

            migrationBuilder.CreateTable(
                name: "Metrics",
                columns: table => new
                {
                    MetricId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrics", x => x.MetricId);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationToken",
                columns: table => new
                {
                    RegistrationTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationToken", x => x.RegistrationTokenId);
                });

            migrationBuilder.CreateTable(
                name: "SecurityTokens",
                columns: table => new
                {
                    SecurityTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityTokens", x => x.SecurityTokenId);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    UserProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEdited = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<double>(type: "float", nullable: true),
                    ContactNo = table.Column<double>(type: "float", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.UserProfileId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowUser",
                columns: table => new
                {
                    FollowUserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FromUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Accepted = table.Column<bool>(type: "bit", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUser", x => x.FollowUserId);
                    table.ForeignKey(
                        name: "FK_FollowUser_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FollowUser_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Focuses",
                columns: table => new
                {
                    FocusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FocusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Focuses", x => x.FocusId);
                    table.ForeignKey(
                        name: "FK_Focuses_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupInvitation",
                columns: table => new
                {
                    GroupInvitationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvitation", x => x.GroupInvitationId);
                    table.ForeignKey(
                        name: "FK_GroupInvitation_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupRequests",
                columns: table => new
                {
                    GroupRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Accepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRequests", x => x.GroupRequestId);
                    table.ForeignKey(
                        name: "FK_GroupRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupRequests_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    GoalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalName = table.Column<string>(type: "nvarchar(55)", maxLength: 55, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Target = table.Column<double>(type: "float", nullable: true),
                    GoalType = table.Column<bool>(type: "bit", nullable: false),
                    MetricId = table.Column<int>(type: "int", nullable: true),
                    GoalStatusId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.GoalId);
                    table.ForeignKey(
                        name: "FK_Goals_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Goals_GoalStatus_GoalStatusId",
                        column: x => x.GoalStatusId,
                        principalTable: "GoalStatus",
                        principalColumn: "GoalStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "MetricId");
                });

            migrationBuilder.CreateTable(
                name: "GroupGoal",
                columns: table => new
                {
                    GroupGoalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Target = table.Column<double>(type: "float", nullable: true),
                    MetricId = table.Column<int>(type: "int", nullable: true),
                    FocusId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GoalStatusId = table.Column<int>(type: "int", nullable: false),
                    GroupUserId = table.Column<int>(type: "int", nullable: false),
                    AssignedGroupUserId = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupGoal", x => x.GroupGoalId);
                    table.ForeignKey(
                        name: "FK_GroupGoal_Focuses_FocusId",
                        column: x => x.FocusId,
                        principalTable: "Focuses",
                        principalColumn: "FocusId");
                    table.ForeignKey(
                        name: "FK_GroupGoal_GoalStatus_GoalStatusId",
                        column: x => x.GoalStatusId,
                        principalTable: "GoalStatus",
                        principalColumn: "GoalStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupGoal_GroupUsers_GroupUserId",
                        column: x => x.GroupUserId,
                        principalTable: "GroupUsers",
                        principalColumn: "GroupUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupGoal_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupGoal_Metrics_MetricId",
                        column: x => x.MetricId,
                        principalTable: "Metrics",
                        principalColumn: "MetricId");
                });

            migrationBuilder.CreateTable(
                name: "Support",
                columns: table => new
                {
                    SupportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Support", x => x.SupportId);
                    table.ForeignKey(
                        name: "FK_Support_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "GoalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportInvitation",
                columns: table => new
                {
                    SupportInvitationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accepted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportInvitation", x => x.SupportInvitationId);
                    table.ForeignKey(
                        name: "FK_SupportInvitation_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "GoalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Updates",
                columns: table => new
                {
                    UpdateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Updatemsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<double>(type: "float", nullable: true),
                    GoalId = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Updates", x => x.UpdateId);
                    table.ForeignKey(
                        name: "FK_Updates_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "GoalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUpdate",
                columns: table => new
                {
                    GroupUpdateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Updatemsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<double>(type: "float", nullable: true),
                    GroupGoalId = table.Column<int>(type: "int", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUpdate", x => x.GroupUpdateId);
                    table.ForeignKey(
                        name: "FK_GroupUpdate_GroupGoal_GroupGoalId",
                        column: x => x.GroupGoalId,
                        principalTable: "GroupGoal",
                        principalColumn: "GroupGoalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdateId = table.Column<int>(type: "int", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Updates_UpdateId",
                        column: x => x.UpdateId,
                        principalTable: "Updates",
                        principalColumn: "UpdateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UpdateSupports",
                columns: table => new
                {
                    UpdateSupportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateSupportedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateSupports", x => x.UpdateSupportId);
                    table.ForeignKey(
                        name: "FK_UpdateSupports_Updates_UpdateId",
                        column: x => x.UpdateId,
                        principalTable: "Updates",
                        principalColumn: "UpdateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupComment",
                columns: table => new
                {
                    GroupCommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupUpdateId = table.Column<int>(type: "int", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupComment", x => x.GroupCommentId);
                    table.ForeignKey(
                        name: "FK_GroupComment_GroupUpdate_GroupUpdateId",
                        column: x => x.GroupUpdateId,
                        principalTable: "GroupUpdate",
                        principalColumn: "GroupUpdateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUpdateSupports",
                columns: table => new
                {
                    GroupUpdateSupportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupUpdateId = table.Column<int>(type: "int", nullable: false),
                    GroupUserId = table.Column<int>(type: "int", nullable: false),
                    UpdateSupportedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUpdateSupports", x => x.GroupUpdateSupportId);
                    table.ForeignKey(
                        name: "FK_GroupUpdateSupports_GroupUpdate_GroupUpdateId",
                        column: x => x.GroupUpdateId,
                        principalTable: "GroupUpdate",
                        principalColumn: "GroupUpdateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "GoalStatus",
                columns: new[] { "GoalStatusId", "GoalStatusType" },
                values: new object[,]
                {
                    { 1, "In Progress" },
                    { 2, "On Hold" },
                    { 3, "Completed" }
                });

            migrationBuilder.InsertData(
                table: "Metrics",
                columns: new[] { "MetricId", "Type" },
                values: new object[,]
                {
                    { 1, "%" },
                    { 2, "$" },
                    { 3, "$ M" },
                    { 4, "Rs" },
                    { 5, "Hours" },
                    { 6, "Km" },
                    { 7, "Kg" },
                    { 8, "Years" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

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
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UpdateId",
                table: "Comments",
                column: "UpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Focuses_GroupId",
                table: "Focuses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUser_FromUserId",
                table: "FollowUser",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowUser_ToUserId",
                table: "FollowUser",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalStatusId",
                table: "Goals",
                column: "GoalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_MetricId",
                table: "Goals",
                column: "MetricId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_UserId",
                table: "Goals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupComment_GroupUpdateId",
                table: "GroupComment",
                column: "GroupUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupGoal_FocusId",
                table: "GroupGoal",
                column: "FocusId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupGoal_GoalStatusId",
                table: "GroupGoal",
                column: "GoalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupGoal_GroupId",
                table: "GroupGoal",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupGoal_GroupUserId",
                table: "GroupGoal",
                column: "GroupUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupGoal_MetricId",
                table: "GroupGoal",
                column: "MetricId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvitation_GroupId",
                table: "GroupInvitation",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRequests_GroupId",
                table: "GroupRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRequests_UserId",
                table: "GroupRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUpdate_GroupGoalId",
                table: "GroupUpdate",
                column: "GroupGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUpdateSupports_GroupUpdateId",
                table: "GroupUpdateSupports",
                column: "GroupUpdateId");

            migrationBuilder.CreateIndex(
                name: "IX_Support_GoalId",
                table: "Support",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportInvitation_GoalId",
                table: "SupportInvitation",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Updates_GoalId",
                table: "Updates",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateSupports_UpdateId",
                table: "UpdateSupports",
                column: "UpdateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CommentUsers");

            migrationBuilder.DropTable(
                name: "FollowRequest");

            migrationBuilder.DropTable(
                name: "FollowUser");

            migrationBuilder.DropTable(
                name: "GroupComment");

            migrationBuilder.DropTable(
                name: "GroupCommentUsers");

            migrationBuilder.DropTable(
                name: "GroupInvitation");

            migrationBuilder.DropTable(
                name: "GroupRequests");

            migrationBuilder.DropTable(
                name: "GroupUpdateSupports");

            migrationBuilder.DropTable(
                name: "GroupUpdateUsers");

            migrationBuilder.DropTable(
                name: "RegistrationToken");

            migrationBuilder.DropTable(
                name: "SecurityTokens");

            migrationBuilder.DropTable(
                name: "Support");

            migrationBuilder.DropTable(
                name: "SupportInvitation");

            migrationBuilder.DropTable(
                name: "UpdateSupports");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "GroupUpdate");

            migrationBuilder.DropTable(
                name: "Updates");

            migrationBuilder.DropTable(
                name: "GroupGoal");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "Focuses");

            migrationBuilder.DropTable(
                name: "GroupUsers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "GoalStatus");

            migrationBuilder.DropTable(
                name: "Metrics");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
