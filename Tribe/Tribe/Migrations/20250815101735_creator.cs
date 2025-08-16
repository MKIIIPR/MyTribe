using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations
{
    /// <inheritdoc />
    public partial class creator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorPlanId",
                table: "TribeProfiles");

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
                name: "CreatorSubscription",
                columns: table => new
                {
                    Guid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TribeProfileId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorPlanPricingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorSubscription", x => x.Guid);
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
                        name: "FK_CreatorSubscription_TribeProfiles_TribeProfileId",
                        column: x => x.TribeProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlanPricing_CreatorPlanGuid",
                table: "CreatorPlanPricing",
                column: "CreatorPlanGuid");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_CreatorPlanId",
                table: "CreatorSubscription",
                column: "CreatorPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_CreatorPlanPricingId",
                table: "CreatorSubscription",
                column: "CreatorPlanPricingId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorSubscription_TribeProfileId",
                table: "CreatorSubscription",
                column: "TribeProfileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreatorSubscription");

            migrationBuilder.DropTable(
                name: "CreatorPlanPricing");

            migrationBuilder.DropTable(
                name: "CreatorPlan");

            migrationBuilder.AddColumn<string>(
                name: "CreatorPlanId",
                table: "TribeProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
