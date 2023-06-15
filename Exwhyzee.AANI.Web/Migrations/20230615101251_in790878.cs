using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class in790878 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplateContents_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "MessageTemplateContents");

            migrationBuilder.AlterColumn<long>(
                name: "MessageTemplateCategoryId",
                table: "MessageTemplateContents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplateContents_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "MessageTemplateContents",
                column: "MessageTemplateCategoryId",
                principalTable: "MessageTemplateCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageTemplateContents_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "MessageTemplateContents");

            migrationBuilder.AlterColumn<long>(
                name: "MessageTemplateCategoryId",
                table: "MessageTemplateContents",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageTemplateContents_MessageTemplateCategories_MessageTemplateCategoryId",
                table: "MessageTemplateContents",
                column: "MessageTemplateCategoryId",
                principalTable: "MessageTemplateCategories",
                principalColumn: "Id");
        }
    }
}
