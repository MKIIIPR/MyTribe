using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tribe.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTicketOnlineFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnlineEvent",
                table: "EventTicketProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresIdentification",
                table: "EventTicketProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StreamUrl",
                table: "EventTicketProducts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnlineEvent",
                table: "EventTicketProducts");

            migrationBuilder.DropColumn(
                name: "RequiresIdentification",
                table: "EventTicketProducts");

            migrationBuilder.DropColumn(
                name: "StreamUrl",
                table: "EventTicketProducts");
        }
    }
}
