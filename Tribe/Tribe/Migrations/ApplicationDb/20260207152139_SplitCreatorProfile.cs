using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class SplitCreatorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliatePartner_TribeUsers_CreatorProfileId",
                table: "AffiliatePartner");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorPlacement_TribeUsers_CreatorProfileId",
                table: "CreatorPlacement");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlanPricing_CreatorPlanPricingId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlan_CreatorPlanId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorTokens_TribeUsers_CreatorProfileId",
                table: "CreatorTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileFollows_TribeUsers_CreatorProfileId",
                table: "ProfileFollows");

            migrationBuilder.DropTable(
                name: "CreatorPlanPricing");

            migrationBuilder.DropTable(
                name: "CreatorPlan");

            migrationBuilder.DropIndex(
                name: "IX_TribeUsers_CreatorName",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "AcceptingCollaborations",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "CollaborationInfo",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "DiscordUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "FollowerCount",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "ImageTemplateUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "PatreonUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "TikTokUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "TotalRaffles",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "TotalTokensDistributed",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "TwitchUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "TwitterUrl",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "VerifiedCreator",
                table: "TribeUsers");

            migrationBuilder.DropColumn(
                name: "YouTubeUrl",
                table: "TribeUsers");

            migrationBuilder.CreateTable(
                name: "CreatorProfiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageTemplateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FollowerCount = table.Column<int>(type: "int", nullable: false),
                    TotalRaffles = table.Column<int>(type: "int", nullable: false),
                    TotalTokensDistributed = table.Column<int>(type: "int", nullable: false),
                    PatreonUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitchUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TikTokUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiscordUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptingCollaborations = table.Column<bool>(type: "bit", nullable: false),
                    CollaborationInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedCreator = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatorProfiles_TribeUsers_Id",
                        column: x => x.Id,
                        principalTable: "TribeUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatorProfiles_CreatorName",
                table: "CreatorProfiles",
                column: "CreatorName",
                unique: true,
                filter: "[CreatorName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliatePartner_CreatorProfiles_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId",
                principalTable: "CreatorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorPlacement_CreatorProfiles_CreatorProfileId",
                table: "CreatorPlacement",
                column: "CreatorProfileId",
                principalTable: "CreatorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_CreatorPlanPricings_CreatorPlanPricingId",
                table: "CreatorSubscription",
                column: "CreatorPlanPricingId",
                principalTable: "CreatorPlanPricings",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_CreatorPlans_CreatorPlanId",
                table: "CreatorSubscription",
                column: "CreatorPlanId",
                principalTable: "CreatorPlans",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorTokens_CreatorProfiles_CreatorProfileId",
                table: "CreatorTokens",
                column: "CreatorProfileId",
                principalTable: "CreatorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileFollows_CreatorProfiles_CreatorProfileId",
                table: "ProfileFollows",
                column: "CreatorProfileId",
                principalTable: "CreatorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliatePartner_CreatorProfiles_CreatorProfileId",
                table: "AffiliatePartner");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorPlacement_CreatorProfiles_CreatorProfileId",
                table: "CreatorPlacement");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlanPricings_CreatorPlanPricingId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlans_CreatorPlanId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorTokens_CreatorProfiles_CreatorProfileId",
                table: "CreatorTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileFollows_CreatorProfiles_CreatorProfileId",
                table: "ProfileFollows");

            migrationBuilder.DropTable(
                name: "CreatorProfiles");

            migrationBuilder.AddColumn<bool>(
                name: "AcceptingCollaborations",
                table: "TribeUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CollaborationInfo",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "TribeUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscordUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "TribeUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FollowerCount",
                table: "TribeUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageTemplateUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatreonUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TikTokUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRaffles",
                table: "TribeUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalTokensDistributed",
                table: "TribeUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitchUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwitterUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "TribeUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VerifiedCreator",
                table: "TribeUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeUrl",
                table: "TribeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreatorPlan",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateEvents = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateRaffles = table.Column<bool>(type: "bit", nullable: false),
                    CanUploadDigitalContent = table.Column<bool>(type: "bit", nullable: false),
                    CanUseWindowsApp = table.Column<bool>(type: "bit", nullable: false),
                    FeaturesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HaveShopItems = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenMenge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlan", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "CreatorPlanPricing",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueGbPound = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlanPricing", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CreatorPlanPricing_CreatorPlan_CreatorPlanGuid",
                        column: x => x.CreatorPlanGuid,
                        principalTable: "CreatorPlan",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TribeUsers_CreatorName",
                table: "TribeUsers",
                column: "CreatorName",
                unique: true,
                filter: "[CreatorName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlanPricing_CreatorPlanGuid",
                table: "CreatorPlanPricing",
                column: "CreatorPlanGuid");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliatePartner_TribeUsers_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorPlacement_TribeUsers_CreatorProfileId",
                table: "CreatorPlacement",
                column: "CreatorProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_CreatorPlanPricing_CreatorPlanPricingId",
                table: "CreatorSubscription",
                column: "CreatorPlanPricingId",
                principalTable: "CreatorPlanPricing",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_CreatorPlan_CreatorPlanId",
                table: "CreatorSubscription",
                column: "CreatorPlanId",
                principalTable: "CreatorPlan",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorTokens_TribeUsers_CreatorProfileId",
                table: "CreatorTokens",
                column: "CreatorProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileFollows_TribeUsers_CreatorProfileId",
                table: "ProfileFollows",
                column: "CreatorProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
