﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Exwhyzee.AANI.Web.Migrations
{
    public partial class eventsbudgeto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Funds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Funds");
        }
    }
}
