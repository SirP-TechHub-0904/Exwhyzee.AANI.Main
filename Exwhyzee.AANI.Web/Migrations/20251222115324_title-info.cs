using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class titleinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NameTitles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameTitles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NameTitles",
                column: "Title",
                values: new object[]
                {
                "ACG",
                "AIG",
                "Air Comdr.",
                "Alhaji",
                "Ambassador",
                "AVM",
                "Barr.",
                "Brig Gen.",
                "Brig.",
                "Captain",
                "Capt Navy.",
                "Chief",
                "Col.",
                "Commodore",
                "Comrade",
                "CP",
                "Dep. Compol",
                "DIG",
                "Dr",
                "Dr Mrs",
                "Engr.",
                "Gp Capt.",
                "Hajiya",
                "Hon.",
                "Hon. Justice",
                "HRH",
                "Major",
                "Major General",
                "Mallam",
                "Miss",
                "Mr",
                "Mrs",
                "Pharm.",
                "Prince",
                "Princess",
                "Prof.",
                "Rear Admiral"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NameTitles");
        }
    }

}
