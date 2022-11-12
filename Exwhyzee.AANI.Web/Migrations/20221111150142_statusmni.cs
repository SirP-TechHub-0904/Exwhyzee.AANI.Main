using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class statusmni : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MniStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MniStatus",
                table: "AspNetUsers");
        }
    }
}
