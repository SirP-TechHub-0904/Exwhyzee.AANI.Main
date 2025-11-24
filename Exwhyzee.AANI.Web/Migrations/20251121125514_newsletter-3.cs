using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class newsletter3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_ParticipantId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_MessageTemplates_TemplateId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ParticipantId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TemplateId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TemplateCode",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TemplateParametersJson",
                table: "Notifications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParticipantId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateCode",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TemplateId",
                table: "Notifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemplateParametersJson",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ParticipantId",
                table: "Notifications",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TemplateId",
                table: "Notifications",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_ParticipantId",
                table: "Notifications",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_MessageTemplates_TemplateId",
                table: "Notifications",
                column: "TemplateId",
                principalTable: "MessageTemplates",
                principalColumn: "Id");
        }
    }
}
