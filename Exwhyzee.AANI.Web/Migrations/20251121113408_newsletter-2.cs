using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class newsletter2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParameterHints",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "MessageTemplates");

            migrationBuilder.DropColumn(
                name: "TemplateCode",
                table: "MessageTemplates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParameterHints",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateCode",
                table: "MessageTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
