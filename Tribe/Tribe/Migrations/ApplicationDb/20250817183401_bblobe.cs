using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tribe.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class bblobe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillingAddress",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingAddress", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "CreatorPlan",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenMenge = table.Column<int>(type: "int", nullable: false),
                    FeaturesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanUploadDigitalContent = table.Column<bool>(type: "bit", nullable: false),
                    HaveShopItems = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateEvents = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateRaffles = table.Column<bool>(type: "bit", nullable: false),
                    CanUseWindowsApp = table.Column<bool>(type: "bit", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlan", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "CreatorPlans",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenMenge = table.Column<int>(type: "int", nullable: false),
                    FeaturesJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanUploadDigitalContent = table.Column<bool>(type: "bit", nullable: false),
                    HaveShopItems = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateEvents = table.Column<bool>(type: "bit", nullable: false),
                    CanCreateRaffles = table.Column<bool>(type: "bit", nullable: false),
                    CanUseWindowsApp = table.Column<bool>(type: "bit", nullable: false),
                    Aktiv = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlans", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "PaymentInfo",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardHolderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CvvCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcceptTerms = table.Column<bool>(type: "bit", nullable: false),
                    AcceptRecurring = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInfo", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TribeUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProfileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsCreator = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_TribeUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOrRegion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LegalForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VATId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVATPayer = table.Column<bool>(type: "bit", nullable: false),
                    StripeAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopifyToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalPayerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsAccepted = table.Column<bool>(type: "bit", nullable: false),
                    TermsAcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrivacyPolicyAccepted = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyPolicyAcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingAddressGuid = table.Column<string>(type: "nvarchar(450)", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_AspNetUsers_BillingAddress_BillingAddressGuid",
                        column: x => x.BillingAddressGuid,
                        principalTable: "BillingAddress",
                        principalColumn: "Guid");
                });

            migrationBuilder.CreateTable(
                name: "CreatorPlanPricing",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueGbPound = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "CreatorPlanPricings",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueUSD = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueEuro = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueGbPound = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlanPricings", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CreatorPlanPricings_CreatorPlans_CreatorPlanGuid",
                        column: x => x.CreatorPlanGuid,
                        principalTable: "CreatorPlans",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
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
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
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
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AffiliatePartner_TribeUsers_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorPlacement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlacementName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlacementLogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebHookUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorProfileId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorPlacement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatorPlacement_TribeUsers_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeUsers",
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
                        name: "FK_CreatorTokens_TribeUsers_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeUsers",
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
                        name: "FK_ProfileFollows_TribeUsers_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileFollows_TribeUsers_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "TribeUsers",
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
                        name: "FK_Raffles_TribeUsers_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
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
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorSubscription",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TribeProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanPricingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BillingAddressGuid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentInfoGuid = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorSubscription", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_CreatorSubscription_BillingAddress_BillingAddressGuid",
                        column: x => x.BillingAddressGuid,
                        principalTable: "BillingAddress",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorSubscription_CreatorPlanPricing_CreatorPlanPricingId",
                        column: x => x.CreatorPlanPricingId,
                        principalTable: "CreatorPlanPricing",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreatorSubscription_CreatorPlan_CreatorPlanId",
                        column: x => x.CreatorPlanId,
                        principalTable: "CreatorPlan",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreatorSubscription_PaymentInfo_PaymentInfoGuid",
                        column: x => x.PaymentInfoGuid,
                        principalTable: "PaymentInfo",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorSubscription_TribeUsers_TribeProfileId",
                        column: x => x.TribeProfileId,
                        principalTable: "TribeUsers",
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
                        name: "FK_ProfileTokenHoldings_TribeUsers_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeUsers",
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
                        name: "FK_RaffleEntries_TribeUsers_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeUsers",
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
                        name: "FK_RaffleWinners_TribeUsers_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "TribeUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "CreatorPlans",
                columns: new[] { "Guid", "Aktiv", "CanCreateEvents", "CanCreateRaffles", "CanUploadDigitalContent", "CanUseWindowsApp", "FeaturesJson", "HaveShopItems", "Name", "TokenMenge" },
                values: new object[,]
                {
                    { "basic-plan-guid-001", true, true, true, true, true, "{\"maxProjects\":1,\"support\":\"email\"}", true, "Basic Plan", 10000 },
                    { "pro-plan-guid-002", true, true, true, true, true, "{\"maxProjects\":10,\"support\":\"priority\"}", true, "Pro Plan", 50000 }
                });

            migrationBuilder.InsertData(
                table: "CreatorPlanPricings",
                columns: new[] { "Guid", "CreatorPlanGuid", "Duration", "ValueEuro", "ValueGbPound", "ValueUSD" },
                values: new object[,]
                {
                    { "basic-plan-guid-001-annual", "basic-plan-guid-001", "Annual", 89.99m, 79.99m, 99.99m },
                    { "basic-plan-guid-001-monthly", "basic-plan-guid-001", "Monthly", 8.99m, 7.99m, 9.99m },
                    { "pro-plan-guid-002-annual", "pro-plan-guid-002", "Annual", 269.99m, 239.99m, 299.99m },
                    { "pro-plan-guid-002-monthly", "pro-plan-guid-002", "Monthly", 26.99m, 23.99m, 29.99m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AffiliatePartner_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BillingAddressGuid",
                table: "AspNetUsers",
                column: "BillingAddressGuid");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlacement_CreatorProfileId",
                table: "CreatorPlacement",
                column: "CreatorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlanPricing_CreatorPlanGuid",
                table: "CreatorPlanPricing",
                column: "CreatorPlanGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlanPricings_CreatorPlanGuid",
                table: "CreatorPlanPricings",
                column: "CreatorPlanGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_BillingAddressGuid",
                table: "CreatorSubscription",
                column: "BillingAddressGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_CreatorPlanId",
                table: "CreatorSubscription",
                column: "CreatorPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_CreatorPlanPricingId",
                table: "CreatorSubscription",
                column: "CreatorPlanPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_PaymentInfoGuid",
                table: "CreatorSubscription",
                column: "PaymentInfoGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_TribeProfileId",
                table: "CreatorSubscription",
                column: "TribeProfileId",
                unique: true);

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
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TribeUsers_ApplicationUserId",
                table: "TribeUsers",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TribeUsers_CreatorName",
                table: "TribeUsers",
                column: "CreatorName",
                unique: true,
                filter: "[CreatorName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TribeUsers_DisplayName",
                table: "TribeUsers",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AffiliatePartner");

            migrationBuilder.DropTable(
                name: "CreatorPlacement");

            migrationBuilder.DropTable(
                name: "CreatorPlanPricings");

            migrationBuilder.DropTable(
                name: "CreatorSubscription");

            migrationBuilder.DropTable(
                name: "ProfileFollows");

            migrationBuilder.DropTable(
                name: "ProfileTokenHoldings");

            migrationBuilder.DropTable(
                name: "RaffleTokenRequirements");

            migrationBuilder.DropTable(
                name: "RaffleWinners");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "CreatorPlans");

            migrationBuilder.DropTable(
                name: "CreatorPlanPricing");

            migrationBuilder.DropTable(
                name: "PaymentInfo");

            migrationBuilder.DropTable(
                name: "RaffleEntries");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CreatorPlan");

            migrationBuilder.DropTable(
                name: "CreatorTokens");

            migrationBuilder.DropTable(
                name: "Raffles");

            migrationBuilder.DropTable(
                name: "BillingAddress");

            migrationBuilder.DropTable(
                name: "TribeUsers");
        }
    }
}
