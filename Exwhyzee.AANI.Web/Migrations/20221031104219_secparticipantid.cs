using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class secparticipantid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFamiliesOnSECs_AspNetUsers_ParticipantId1",
                table: "ParticipantFamiliesOnSECs");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantFamiliesOnSECs_ParticipantId1",
                table: "ParticipantFamiliesOnSECs");

            migrationBuilder.DropColumn(
                name: "ParticipantId1",
                table: "ParticipantFamiliesOnSECs");

            migrationBuilder.AlterColumn<string>(
                name: "ParticipantId",
                table: "ParticipantFamiliesOnSECs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantFamiliesOnSECs_ParticipantId",
                table: "ParticipantFamiliesOnSECs",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFamiliesOnSECs_AspNetUsers_ParticipantId",
                table: "ParticipantFamiliesOnSECs",
                column: "ParticipantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParticipantFamiliesOnSECs_AspNetUsers_ParticipantId",
                table: "ParticipantFamiliesOnSECs");

            migrationBuilder.DropIndex(
                name: "IX_ParticipantFamiliesOnSECs_ParticipantId",
                table: "ParticipantFamiliesOnSECs");

            migrationBuilder.AlterColumn<long>(
                name: "ParticipantId",
                table: "ParticipantFamiliesOnSECs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ParticipantId1",
                table: "ParticipantFamiliesOnSECs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantFamiliesOnSECs_ParticipantId1",
                table: "ParticipantFamiliesOnSECs",
                column: "ParticipantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ParticipantFamiliesOnSECs_AspNetUsers_ParticipantId1",
                table: "ParticipantFamiliesOnSECs",
                column: "ParticipantId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
