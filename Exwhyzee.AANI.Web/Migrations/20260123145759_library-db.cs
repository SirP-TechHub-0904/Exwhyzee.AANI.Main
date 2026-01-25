using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class librarydb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "File",
                table: "Papers",
                newName: "FileUrl");

            migrationBuilder.AddColumn<string>(
                name: "FileKey",
                table: "Papers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileKey",
                table: "Papers");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "Papers",
                newName: "File");
        }
    }
}
