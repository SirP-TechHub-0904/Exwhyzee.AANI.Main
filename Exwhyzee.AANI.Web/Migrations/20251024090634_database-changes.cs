using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class databasechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperationYearId",
                table: "Funds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OfficeId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoginHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginHistories_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funds_OperationYearId",
                table: "Funds",
                column: "OperationYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginHistories_ParticipantId",
                table: "LoginHistories",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Offices_OfficeId",
                table: "AspNetUsers",
                column: "OfficeId",
                principalTable: "Offices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Funds_OperationYears_OperationYearId",
                table: "Funds",
                column: "OperationYearId",
                principalTable: "OperationYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Offices_OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Funds_OperationYears_OperationYearId",
                table: "Funds");

            migrationBuilder.DropTable(
                name: "LoginHistories");

            migrationBuilder.DropIndex(
                name: "IX_Funds_OperationYearId",
                table: "Funds");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OfficeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OperationYearId",
                table: "Funds");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "AspNetUsers");
        }
    }
}
