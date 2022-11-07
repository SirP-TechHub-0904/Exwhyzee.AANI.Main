using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class executive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Executives",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Executives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Executives_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PastExecutiveYear",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastExecutiveYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PastExecutiveMembers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PastExecutiveYearId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PastExecutiveMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PastExecutiveMembers_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PastExecutiveMembers_PastExecutiveYear_PastExecutiveYearId",
                        column: x => x.PastExecutiveYearId,
                        principalTable: "PastExecutiveYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Executives_ParticipantId",
                table: "Executives",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PastExecutiveMembers_ParticipantId",
                table: "PastExecutiveMembers",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_PastExecutiveMembers_PastExecutiveYearId",
                table: "PastExecutiveMembers",
                column: "PastExecutiveYearId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Executives");

            migrationBuilder.DropTable(
                name: "PastExecutiveMembers");

            migrationBuilder.DropTable(
                name: "PastExecutiveYear");
        }
    }
}
