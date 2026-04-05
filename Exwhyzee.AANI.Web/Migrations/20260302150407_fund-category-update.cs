using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class fundcategoryupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "FundCategories",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChapterId",
                table: "FundCategories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ChapterShare",
                table: "FundCategories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlexible",
                table: "FundCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNationalLevel",
                table: "FundCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "NationalShare",
                table: "FundCategories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SplitType",
                table: "FundCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MonnifyAccountName",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonnifyAccountNumber",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonnifyBankName",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonnifySubAccountCode",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FundCategories_ChapterId",
                table: "FundCategories",
                column: "ChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundCategories_Chapters_ChapterId",
                table: "FundCategories",
                column: "ChapterId",
                principalTable: "Chapters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundCategories_Chapters_ChapterId",
                table: "FundCategories");

            migrationBuilder.DropIndex(
                name: "IX_FundCategories_ChapterId",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "ChapterId",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "ChapterShare",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "IsFlexible",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "IsNationalLevel",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "NationalShare",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "SplitType",
                table: "FundCategories");

            migrationBuilder.DropColumn(
                name: "MonnifyAccountName",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "MonnifyAccountNumber",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "MonnifyBankName",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "MonnifySubAccountCode",
                table: "Chapters");
        }
    }
}
