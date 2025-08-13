using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations
{
    /// <inheritdoc />
    public partial class afiliate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliatePartner_TribeProfiles_CreatorProfileId",
                table: "AffiliatePartner");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorProfileId",
                table: "AffiliatePartner",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
                        name: "FK_CreatorPlacement_TribeProfiles_CreatorProfileId",
                        column: x => x.CreatorProfileId,
                        principalTable: "TribeProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatorPlacement_CreatorProfileId",
                table: "CreatorPlacement",
                column: "CreatorProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliatePartner_TribeProfiles_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId",
                principalTable: "TribeProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AffiliatePartner_TribeProfiles_CreatorProfileId",
                table: "AffiliatePartner");

            migrationBuilder.DropTable(
                name: "CreatorPlacement");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorProfileId",
                table: "AffiliatePartner",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_AffiliatePartner_TribeProfiles_CreatorProfileId",
                table: "AffiliatePartner",
                column: "CreatorProfileId",
                principalTable: "TribeProfiles",
                principalColumn: "Id");
        }
    }
}
