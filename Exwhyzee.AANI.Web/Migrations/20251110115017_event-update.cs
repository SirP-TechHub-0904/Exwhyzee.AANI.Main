using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class eventupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageKey",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogDescription",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogSubtitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BlogTitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventDescription",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventSubtitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveDescription",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveSubtitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveTitle",
                table: "ContactSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageKey",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "BlogDescription",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "BlogSubtitle",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "BlogTitle",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "EventDescription",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "EventSubtitle",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "EventTitle",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "ExecutiveDescription",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "ExecutiveSubtitle",
                table: "ContactSettings");

            migrationBuilder.DropColumn(
                name: "ExecutiveTitle",
                table: "ContactSettings");
        }
    }
}
