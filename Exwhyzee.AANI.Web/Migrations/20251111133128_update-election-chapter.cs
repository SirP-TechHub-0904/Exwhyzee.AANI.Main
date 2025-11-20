using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class updateelectionchapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterAccreditedVoters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChapterId = table.Column<long>(type: "bigint", nullable: true),
                    Voted = table.Column<bool>(type: "bit", nullable: false),
                    DateVoted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoteTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TokenSentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterAccreditedVoters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterAccreditedVoters_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChapterAccreditedVoters_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateTable(
                name: "ChapterElections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChapterId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterElections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterElections_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ElectionCandidates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectionId = table.Column<long>(type: "bigint", nullable: false),
                    CandidateParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Manifesto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BallotOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionCandidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_AspNetUsers_CandidateParticipantId",
                        column: x => x.CandidateParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ElectionCandidates_ChapterElections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "ChapterElections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectionId = table.Column<long>(type: "bigint", nullable: false),
                    ChapterAccreditedVoterId = table.Column<long>(type: "bigint", nullable: false),
                    CastAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_ChapterAccreditedVoters_ChapterAccreditedVoterId",
                        column: x => x.ChapterAccreditedVoterId,
                        principalTable: "ChapterAccreditedVoters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Votes_ChapterElections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "ChapterElections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VoteAudits",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteAudits_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "VoteChoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteId = table.Column<long>(type: "bigint", nullable: false),
                    CandidateId = table.Column<long>(type: "bigint", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteChoices_ElectionCandidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "ElectionCandidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_VoteChoices_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterAccreditedVoters_ChapterId",
                table: "ChapterAccreditedVoters",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterAccreditedVoters_ParticipantId",
                table: "ChapterAccreditedVoters",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterContesters_ContesterId",
                table: "ChapterContesters",
                column: "ContesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterElections_ChapterId",
                table: "ChapterElections",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_CandidateParticipantId",
                table: "ElectionCandidates",
                column: "CandidateParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCandidates_ElectionId",
                table: "ElectionCandidates",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteAudits_VoteId",
                table: "VoteAudits",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteChoices_CandidateId",
                table: "VoteChoices",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteChoices_VoteId",
                table: "VoteChoices",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ChapterAccreditedVoterId",
                table: "Votes",
                column: "ChapterAccreditedVoterId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ElectionId",
                table: "Votes",
                column: "ElectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterContesters");

            migrationBuilder.DropTable(
                name: "VoteAudits");

            migrationBuilder.DropTable(
                name: "VoteChoices");

            migrationBuilder.DropTable(
                name: "ElectionCandidates");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "ChapterAccreditedVoters");

            migrationBuilder.DropTable(
                name: "ChapterElections");
        }
    }
}
