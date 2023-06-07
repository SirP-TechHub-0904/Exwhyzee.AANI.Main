using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class contridata0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChapterId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChapterId",
                table: "AspNetUsers",
                column: "ChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Chapters_ChapterId",
                table: "AspNetUsers",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Chapters_ChapterId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChapterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "AspNetUsers");
        }
    }
}
