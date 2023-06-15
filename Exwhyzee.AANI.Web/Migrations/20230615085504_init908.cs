using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class init908 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageTemplateCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplateContents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenderStatus = table.Column<int>(type: "int", nullable: false),
                    MessageTemplateCategoryId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTemplateContents_MessageTemplateCategories_MessageTemplateCategoryId",
                        column: x => x.MessageTemplateCategoryId,
                        principalTable: "MessageTemplateCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplateContents_MessageTemplateCategoryId",
                table: "MessageTemplateContents",
                column: "MessageTemplateCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageTemplateContents");

            migrationBuilder.DropTable(
                name: "MessageTemplateCategories");
        }
    }
}
