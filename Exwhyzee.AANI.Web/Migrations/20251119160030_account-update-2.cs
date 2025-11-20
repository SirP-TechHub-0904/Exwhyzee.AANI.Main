using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class accountupdate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentOffice",
                table: "AspNetUsers",
                newName: "CurrentWorkPlace");

            migrationBuilder.AddColumn<string>(
                name: "CurrentOccupation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentOccupation",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CurrentWorkPlace",
                table: "AspNetUsers",
                newName: "CurrentOffice");
        }
    }
}
