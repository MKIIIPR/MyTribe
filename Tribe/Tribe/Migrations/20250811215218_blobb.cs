using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations
{
    /// <inheritdoc />
    public partial class blobb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "UserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "UserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "RoleClaims");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "UserClaims",
                newName: "IX_UserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaims",
                table: "UserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaims",
                table: "RoleClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TribeProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProfileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCreator = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageTemplateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowerCount = table.Column<int>(type: "int", nullable: true),
                    TotalRaffles = table.Column<int>(type: "int", nullable: true),
                    TotalTokensDistributed = table.Column<int>(type: "int", nullable: true),
                    PatreonUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitchUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TikTokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptingCollaborations = table.Column<bool>(type: "bit", nullable: true),
                    CollaborationInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedCreator = table.Column<bool>(type: "bit", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TribeProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TribeProfiles_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AffiliatePartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PartneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PartnerLogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AffiliatePartner_TribeProfiles_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CreatorTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TokenSymbol = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TokenImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalSupply = table.Column<int>(type: "int", nullable: false),
                    CirculatingSupply = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatorTokens_TribeProfiles_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileFollows",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FollowedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileFollows", x => x.Id);
                    table.CheckConstraint("CK_ProfileFollow_NotSelf", "[FollowerId] <> [CreatorProfileId]");
                    table.ForeignKey(
                        name: "FK_ProfileFollows_TribeProfiles_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileFollows_TribeProfiles_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Raffles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxEntries = table.Column<int>(type: "int", nullable: false),
                    CurrentEntries = table.Column<int>(type: "int", nullable: false),
                    RaffleType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RaffleConfig = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrizeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrizeValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrizeCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DrawnAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raffles", x => x.Id);
                    table.CheckConstraint("CK_Raffle_ValidDateRange", "[EndDate] > [StartDate]");
                    table.ForeignKey(
                        name: "FK_Raffles_TribeProfiles_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTokenHoldings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorTokenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTokenHoldings", x => x.Id);
                    table.CheckConstraint("CK_TokenHolding_PositiveAmount", "[Amount] >= 0");
                    table.ForeignKey(
                        name: "FK_ProfileTokenHoldings_CreatorTokens_CreatorTokenId",
                        column: x => x.CreatorTokenId,
                        principalTable: "CreatorTokens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProfileTokenHoldings_TribeProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RaffleEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RaffleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TokensSpent = table.Column<int>(type: "int", nullable: false),
                    UsedTokenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QualifiedForNextStage = table.Column<bool>(type: "bit", nullable: false),
                    EntryCount = table.Column<int>(type: "int", nullable: false),
                    EntryNumbers = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleEntries", x => x.Id);
                    table.CheckConstraint("CK_RaffleEntry_PositiveCount", "[EntryCount] > 0");
                    table.CheckConstraint("CK_RaffleEntry_PositiveTokens", "[TokensSpent] >= 0");
                    table.ForeignKey(
                        name: "FK_RaffleEntries_CreatorTokens_UsedTokenId",
                        column: x => x.UsedTokenId,
                        principalTable: "CreatorTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaffleEntries_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RaffleEntries_TribeProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RaffleTokenRequirements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RaffleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorTokenId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequiredAmount = table.Column<int>(type: "int", nullable: false),
                    RequirementType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsOptional = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleTokenRequirements", x => x.Id);
                    table.CheckConstraint("CK_RaffleTokenReq_PositiveAmount", "[RequiredAmount] >= 0");
                    table.ForeignKey(
                        name: "FK_RaffleTokenRequirements_CreatorTokens_CreatorTokenId",
                        column: x => x.CreatorTokenId,
                        principalTable: "CreatorTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RaffleTokenRequirements_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RaffleWinners",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RaffleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    WinningNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrizeDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrizeValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrizeClaimed = table.Column<bool>(type: "bit", nullable: false),
                    PrizeClaimedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DrawnAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaffleWinners", x => x.Id);
                    table.CheckConstraint("CK_RaffleWinner_PositivePosition", "[Position] > 0");
                    table.ForeignKey(
                        name: "FK_RaffleWinners_RaffleEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "RaffleEntries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RaffleWinners_Raffles_RaffleId",
                        column: x => x.RaffleId,
                        principalTable: "Raffles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RaffleWinners_TribeProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliatePartner_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorTokens_CreatorProfileId_TokenName",
                table: "CreatorTokens",
                columns: new[] { "CreatorProfileId", "TokenName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFollows_CreatorProfileId",
                table: "ProfileFollows",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFollows_FollowerId_CreatorProfileId",
                table: "ProfileFollows",
                columns: new[] { "FollowerId", "CreatorProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTokenHoldings_CreatorTokenId",
                table: "ProfileTokenHoldings",
                column: "CreatorTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTokenHoldings_ProfileId_CreatorTokenId",
                table: "ProfileTokenHoldings",
                columns: new[] { "ProfileId", "CreatorTokenId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaffleEntries_ProfileId",
                table: "RaffleEntries",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleEntries_RaffleId",
                table: "RaffleEntries",
                column: "RaffleId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleEntries_UsedTokenId",
                table: "RaffleEntries",
                column: "UsedTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Raffles_CreatorProfileId",
                table: "Raffles",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleTokenRequirements_CreatorTokenId",
                table: "RaffleTokenRequirements",
                column: "CreatorTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleTokenRequirements_RaffleId_CreatorTokenId",
                table: "RaffleTokenRequirements",
                columns: new[] { "RaffleId", "CreatorTokenId" },
                unique: true,
                filter: "[CreatorTokenId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleWinners_EntryId",
                table: "RaffleWinners",
                column: "EntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaffleWinners_ProfileId",
                table: "RaffleWinners",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_RaffleWinners_RaffleId",
                table: "RaffleWinners",
                column: "RaffleId");

            migrationBuilder.CreateIndex(
                name: "IX_TribeProfiles_ApplicationUserId",
                table: "TribeProfiles",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TribeProfiles_CreatorName",
                table: "TribeProfiles",
                column: "CreatorName",
                unique: true,
                filter: "[CreatorName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TribeProfiles_DisplayName",
                table: "TribeProfiles",
                column: "DisplayName");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.DropTable(
                name: "AffiliatePartner");

            migrationBuilder.DropTable(
                name: "ProfileFollows");

            migrationBuilder.DropTable(
                name: "ProfileTokenHoldings");

            migrationBuilder.DropTable(
                name: "RaffleTokenRequirements");

            migrationBuilder.DropTable(
                name: "RaffleWinners");

            migrationBuilder.DropTable(
                name: "RaffleEntries");

            migrationBuilder.DropTable(
                name: "CreatorTokens");

            migrationBuilder.DropTable(
                name: "Raffles");

            migrationBuilder.DropTable(
                name: "TribeProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaims",
                table: "UserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaims",
                table: "RoleClaims");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
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
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
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
        }
    }
}
