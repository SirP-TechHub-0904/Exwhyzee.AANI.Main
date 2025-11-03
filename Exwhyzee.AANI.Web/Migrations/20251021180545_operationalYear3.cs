using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class operationalYear3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperationYearId",
                table: "Campains",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campains_OperationYearId",
                table: "Campains",
                column: "OperationYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Campains_OperationYears_OperationYearId",
                table: "Campains",
                column: "OperationYearId",
                principalTable: "OperationYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campains_OperationYears_OperationYearId",
                table: "Campains");

            migrationBuilder.DropIndex(
                name: "IX_Campains_OperationYearId",
                table: "Campains");

            migrationBuilder.DropColumn(
                name: "OperationYearId",
                table: "Campains");
        }
    }
}
