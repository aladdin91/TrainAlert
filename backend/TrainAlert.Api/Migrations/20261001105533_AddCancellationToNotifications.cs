using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainAlert.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CurrentCancelled",
                table: "Notifications",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentDisruptionReason",
                table: "Notifications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PreviousCancelled",
                table: "Notifications",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousDisruptionReason",
                table: "Notifications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCancelled",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CurrentDisruptionReason",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PreviousCancelled",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "PreviousDisruptionReason",
                table: "Notifications");
        }
    }
}
