using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class chapterexecutiveupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChapterExecutives",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParticipantId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChapterId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChapterExecutives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChapterExecutives_AspNetUsers_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChapterExecutives_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChapterExecutives_ChapterId",
                table: "ChapterExecutives",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChapterExecutives_ParticipantId",
                table: "ChapterExecutives",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChapterExecutives");
        }
    }
}
