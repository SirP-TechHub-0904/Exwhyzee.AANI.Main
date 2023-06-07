using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class init9009 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Executives");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Executives",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Executives",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "ExecutivePositionId",
                table: "Executives",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExecutivePositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutivePositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campains",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutivePositionId = table.Column<long>(type: "bigint", nullable: true),
                    Manifesto = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campains_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Campains_ExecutivePositions_ExecutivePositionId",
                        column: x => x.ExecutivePositionId,
                        principalTable: "ExecutivePositions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CampainPosts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampainId = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampainPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampainPosts_Campains_CampainId",
                        column: x => x.CampainId,
                        principalTable: "Campains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Executives_ExecutivePositionId",
                table: "Executives",
                column: "ExecutivePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampainPosts_CampainId",
                table: "CampainPosts",
                column: "CampainId");

            migrationBuilder.CreateIndex(
                name: "IX_Campains_ExecutivePositionId",
                table: "Campains",
                column: "ExecutivePositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Campains_ParticipantId",
                table: "Campains",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Executives_ExecutivePositions_ExecutivePositionId",
                table: "Executives",
                column: "ExecutivePositionId",
                principalTable: "ExecutivePositions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executives_ExecutivePositions_ExecutivePositionId",
                table: "Executives");

            migrationBuilder.DropTable(
                name: "CampainPosts");

            migrationBuilder.DropTable(
                name: "Campains");

            migrationBuilder.DropTable(
                name: "ExecutivePositions");

            migrationBuilder.DropIndex(
                name: "IX_Executives_ExecutivePositionId",
                table: "Executives");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Executives");

            migrationBuilder.DropColumn(
                name: "ExecutivePositionId",
                table: "Executives");

            migrationBuilder.DropColumn(
                name: "PictureKey",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Executives",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Executives",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
