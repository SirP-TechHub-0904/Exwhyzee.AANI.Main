using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    /// <inheritdoc />
    public partial class defaultdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "PageCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    MenuSortOrder = table.Column<int>(type: "int", nullable: false),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Publish = table.Column<bool>(type: "bit", nullable: false),
                    SecurityPage = table.Column<bool>(type: "bit", nullable: false),
                    PageCategoryId = table.Column<long>(type: "bigint", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInMainTop = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMenuDropDown = table.Column<bool>(type: "bit", nullable: false),
                    ShowInMainMenu = table.Column<bool>(type: "bit", nullable: false),
                    ShowInFooter = table.Column<bool>(type: "bit", nullable: false),
                    EnableDirectUrl = table.Column<bool>(type: "bit", nullable: false),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false),
                    MainPageId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebPages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebPages_PageCategories_PageCategoryId",
                        column: x => x.PageCategoryId,
                        principalTable: "PageCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YoutubeUrlLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qoute = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShowInHome = table.Column<bool>(type: "bit", nullable: false),
                    DisableButton = table.Column<bool>(type: "bit", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    FixedAfterFooter = table.Column<bool>(type: "bit", nullable: false),
                    HomePageSortOrder = table.Column<int>(type: "int", nullable: false),
                    HomeSortFrom = table.Column<int>(type: "int", nullable: false),
                    PageSortOrder = table.Column<int>(type: "int", nullable: false),
                    BlogSectionPosition = table.Column<int>(type: "int", nullable: false),
                    ShowSectionInBlog = table.Column<bool>(type: "bit", nullable: false),
                    WebPageId = table.Column<long>(type: "bigint", nullable: true),
                    TemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddToRibon = table.Column<bool>(type: "bit", nullable: false),
                    RibonCustomDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonSortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSections_WebPages_WebPageId",
                        column: x => x.WebPageId,
                        principalTable: "WebPages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PageSectionLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiniTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoreDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ButtonLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    PageSectionId = table.Column<long>(type: "bigint", nullable: true),
                    AddToRibon = table.Column<bool>(type: "bit", nullable: false),
                    RibonCustomDisplayTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RibonSortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageSectionLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageSectionLists_PageSections_PageSectionId",
                        column: x => x.PageSectionId,
                        principalTable: "PageSections",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageSectionLists_PageSectionId",
                table: "PageSectionLists",
                column: "PageSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageSections_WebPageId",
                table: "PageSections",
                column: "WebPageId");

            migrationBuilder.CreateIndex(
                name: "IX_WebPages_PageCategoryId",
                table: "WebPages",
                column: "PageCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageSectionLists");

            migrationBuilder.DropTable(
                name: "PageSections");

            migrationBuilder.DropTable(
                name: "WebPages");

            migrationBuilder.DropTable(
                name: "PageCategories");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);
        }
    }
}
