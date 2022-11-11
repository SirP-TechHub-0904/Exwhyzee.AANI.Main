using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class eventcomment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventComments_AspNetUsers_ParticipantId",
                table: "EventComments");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "EventComments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "EventComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "EventComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_EventComments_AspNetUsers_ParticipantId",
                table: "EventComments",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventComments_AspNetUsers_ParticipantId",
                table: "EventComments");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "EventComments");

            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "EventComments");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "EventComments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventComments_AspNetUsers_ParticipantId",
                table: "EventComments",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
