using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class operationalYearexecutiveupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OperationYearId",
                table: "Executives",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "ExecutivePositions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Executives_OperationYearId",
                table: "Executives",
                column: "OperationYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Executives_OperationYears_OperationYearId",
                table: "Executives",
                column: "OperationYearId",
                principalTable: "OperationYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Executives_OperationYears_OperationYearId",
                table: "Executives");

            migrationBuilder.DropIndex(
                name: "IX_Executives_OperationYearId",
                table: "Executives");

            migrationBuilder.DropColumn(
                name: "OperationYearId",
                table: "Executives");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "ExecutivePositions");
        }
    }
}
