using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class budgetinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "EventBudget",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AssignTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBudget_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBudget_EventId",
                table: "EventBudget",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBudget");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "Events",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
