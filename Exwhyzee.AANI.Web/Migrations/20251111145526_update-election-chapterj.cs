using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class updateelectionchapterj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterContesters");

            migrationBuilder.AddColumn<long>(
                name: "PositionId",
                table: "ElectionCandidates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChapterElectionId",
                table: "ChapterAccreditedVoters",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ElectionPositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectionId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    MaxChoices = table.Column<int>(type: "int", nullable: false),
                    BallotOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionPositions_ChapterElections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "ChapterElections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_PositionId",
                table: "ElectionCandidates",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterAccreditedVoters_ChapterElectionId",
                table: "ChapterAccreditedVoters",
                column: "ChapterElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionPositions_ElectionId",
                table: "ElectionPositions",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChapterAccreditedVoters_ChapterElections_ChapterElectionId",
                table: "ChapterAccreditedVoters",
                column: "ChapterElectionId",
                principalTable: "ChapterElections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionCandidates_ElectionPositions_PositionId",
                table: "ElectionCandidates",
                column: "PositionId",
                principalTable: "ElectionPositions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChapterAccreditedVoters_ChapterElections_ChapterElectionId",
                table: "ChapterAccreditedVoters");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionCandidates_ElectionPositions_PositionId",
                table: "ElectionCandidates");

            migrationBuilder.DropTable(
                name: "ElectionPositions");

            migrationBuilder.DropIndex(
                name: "IX_ElectionCandidates_PositionId",
                table: "ElectionCandidates");

            migrationBuilder.DropIndex(
                name: "IX_ChapterAccreditedVoters_ChapterElectionId",
                table: "ChapterAccreditedVoters");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "ElectionCandidates");

            migrationBuilder.DropColumn(
                name: "ChapterElectionId",
                table: "ChapterAccreditedVoters");

            migrationBuilder.CreateTable(
                name: "ChapterContesters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContesterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterContesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterContesters_AspNetUsers_ContesterId",
                        column: x => x.ContesterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContesters_ContesterId",
                table: "ChapterContesters",
                column: "ContesterId");
        }
    }
}
