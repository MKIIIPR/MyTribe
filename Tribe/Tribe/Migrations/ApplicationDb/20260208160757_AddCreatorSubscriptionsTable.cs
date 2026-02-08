using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddCreatorSubscriptionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_BillingAddress_BillingAddressGuid",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlanPricings_CreatorPlanPricingId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_CreatorPlans_CreatorPlanId",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_PaymentInfo_PaymentInfoGuid",
                table: "CreatorSubscription");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscription_TribeUsers_TribeProfileId",
                table: "CreatorSubscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreatorSubscription",
                table: "CreatorSubscription");

            migrationBuilder.RenameTable(
                name: "CreatorSubscription",
                newName: "CreatorSubscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscription_TribeProfileId",
                table: "CreatorSubscriptions",
                newName: "IX_CreatorSubscriptions_TribeProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscription_PaymentInfoGuid",
                table: "CreatorSubscriptions",
                newName: "IX_CreatorSubscriptions_PaymentInfoGuid");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscription_CreatorPlanPricingId",
                table: "CreatorSubscriptions",
                newName: "IX_CreatorSubscriptions_CreatorPlanPricingId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscription_CreatorPlanId",
                table: "CreatorSubscriptions",
                newName: "IX_CreatorSubscriptions_CreatorPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscription_BillingAddressGuid",
                table: "CreatorSubscriptions",
                newName: "IX_CreatorSubscriptions_BillingAddressGuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanPricingId",
                table: "CreatorSubscriptions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanId",
                table: "CreatorSubscriptions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreatorSubscriptions",
                table: "CreatorSubscriptions",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscriptions_BillingAddress_BillingAddressGuid",
                table: "CreatorSubscriptions",
                column: "BillingAddressGuid",
                principalTable: "BillingAddress",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscriptions_CreatorPlanPricings_CreatorPlanPricingId",
                table: "CreatorSubscriptions",
                column: "CreatorPlanPricingId",
                principalTable: "CreatorPlanPricings",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscriptions_CreatorPlans_CreatorPlanId",
                table: "CreatorSubscriptions",
                column: "CreatorPlanId",
                principalTable: "CreatorPlans",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscriptions_PaymentInfo_PaymentInfoGuid",
                table: "CreatorSubscriptions",
                column: "PaymentInfoGuid",
                principalTable: "PaymentInfo",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscriptions_TribeUsers_TribeProfileId",
                table: "CreatorSubscriptions",
                column: "TribeProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscriptions_BillingAddress_BillingAddressGuid",
                table: "CreatorSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscriptions_CreatorPlanPricings_CreatorPlanPricingId",
                table: "CreatorSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscriptions_CreatorPlans_CreatorPlanId",
                table: "CreatorSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscriptions_PaymentInfo_PaymentInfoGuid",
                table: "CreatorSubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_CreatorSubscriptions_TribeUsers_TribeProfileId",
                table: "CreatorSubscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CreatorSubscriptions",
                table: "CreatorSubscriptions");

            migrationBuilder.RenameTable(
                name: "CreatorSubscriptions",
                newName: "CreatorSubscription");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscriptions_TribeProfileId",
                table: "CreatorSubscription",
                newName: "IX_CreatorSubscription_TribeProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscriptions_PaymentInfoGuid",
                table: "CreatorSubscription",
                newName: "IX_CreatorSubscription_PaymentInfoGuid");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscriptions_CreatorPlanPricingId",
                table: "CreatorSubscription",
                newName: "IX_CreatorSubscription_CreatorPlanPricingId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscriptions_CreatorPlanId",
                table: "CreatorSubscription",
                newName: "IX_CreatorSubscription_CreatorPlanId");

            migrationBuilder.RenameIndex(
                name: "IX_CreatorSubscriptions_BillingAddressGuid",
                table: "CreatorSubscription",
                newName: "IX_CreatorSubscription_BillingAddressGuid");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanPricingId",
                table: "CreatorSubscription",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanId",
                table: "CreatorSubscription",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CreatorSubscription",
                table: "CreatorSubscription",
                column: "Guid");

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_BillingAddress_BillingAddressGuid",
                table: "CreatorSubscription",
                column: "BillingAddressGuid",
                principalTable: "BillingAddress",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_CreatorSubscription_PaymentInfo_PaymentInfoGuid",
                table: "CreatorSubscription",
                column: "PaymentInfoGuid",
                principalTable: "PaymentInfo",
                principalColumn: "Guid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CreatorSubscription_TribeUsers_TribeProfileId",
                table: "CreatorSubscription",
                column: "TribeProfileId",
                principalTable: "TribeUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
