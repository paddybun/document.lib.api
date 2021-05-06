using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace document.lib.api.Migrations
{
    public partial class adddocumentdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2019, 6, 1, 8, 26, 32, 980, DateTimeKind.Unspecified).AddTicks(1954), new TimeSpan(0, 2, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "LibDocuments");
        }
    }
}
