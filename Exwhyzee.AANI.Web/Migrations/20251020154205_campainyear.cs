using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class campainyear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CampainYearId",
                table: "Campains",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampainYears",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampainYears", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Campains_CampainYearId",
                table: "Campains",
                column: "CampainYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campains_CampainYears_CampainYearId",
                table: "Campains",
                column: "CampainYearId",
                principalTable: "CampainYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campains_CampainYears_CampainYearId",
                table: "Campains");

            migrationBuilder.DropTable(
                name: "CampainYears");

            migrationBuilder.DropIndex(
                name: "IX_Campains_CampainYearId",
                table: "Campains");

            migrationBuilder.DropColumn(
                name: "CampainYearId",
                table: "Campains");
        }
    }
}
