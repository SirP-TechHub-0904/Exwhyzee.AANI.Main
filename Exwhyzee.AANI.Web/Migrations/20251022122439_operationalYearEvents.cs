using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class operationalYearEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperationYearId",
                table: "Events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_OperationYearId",
                table: "Events",
                column: "OperationYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_OperationYears_OperationYearId",
                table: "Events",
                column: "OperationYearId",
                principalTable: "OperationYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_OperationYears_OperationYearId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_OperationYearId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OperationYearId",
                table: "Events");
        }
    }
}
