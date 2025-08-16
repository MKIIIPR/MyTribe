using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations
{
    /// <inheritdoc />
    public partial class blobdddddd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanId",
                table: "TribeProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "CanCreateEvents",
                table: "CreatorPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanCreateRaffles",
                table: "CreatorPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUploadDigitalContent",
                table: "CreatorPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUseWindowsApp",
                table: "CreatorPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HaveShopItems",
                table: "CreatorPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "CreatorPlans",
                keyColumn: "Guid",
                keyValue: "basic-plan-guid-001",
                columns: new[] { "CanCreateEvents", "CanCreateRaffles", "CanUploadDigitalContent", "CanUseWindowsApp", "HaveShopItems" },
                values: new object[] { true, true, true, true, true });

            migrationBuilder.UpdateData(
                table: "CreatorPlans",
                keyColumn: "Guid",
                keyValue: "pro-plan-guid-002",
                columns: new[] { "CanCreateEvents", "CanCreateRaffles", "CanUploadDigitalContent", "CanUseWindowsApp", "HaveShopItems" },
                values: new object[] { true, true, true, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCreateEvents",
                table: "CreatorPlans");

            migrationBuilder.DropColumn(
                name: "CanCreateRaffles",
                table: "CreatorPlans");

            migrationBuilder.DropColumn(
                name: "CanUploadDigitalContent",
                table: "CreatorPlans");

            migrationBuilder.DropColumn(
                name: "CanUseWindowsApp",
                table: "CreatorPlans");

            migrationBuilder.DropColumn(
                name: "HaveShopItems",
                table: "CreatorPlans");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorPlanId",
                table: "TribeProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
