using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class in7908 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MessageTemplateCategoryId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MessageTemplateCategoryId",
                table: "AspNetUsers",
                column: "MessageTemplateCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "AspNetUsers",
                column: "MessageTemplateCategoryId",
                principalTable: "MessageTemplateCategories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MessageTemplateCategoryId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MessageTemplateCategoryId",
                table: "AspNetUsers");
        }
    }
}
