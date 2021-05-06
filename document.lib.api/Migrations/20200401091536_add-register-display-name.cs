using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace document.lib.api.Migrations
{
    public partial class addregisterdisplayname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Registers",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 4, 1, 11, 15, 36, 460, DateTimeKind.Unspecified).AddTicks(3149), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2019, 6, 9, 13, 21, 8, 732, DateTimeKind.Unspecified).AddTicks(5137), new TimeSpan(0, 2, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Registers");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2019, 6, 9, 13, 21, 8, 732, DateTimeKind.Unspecified).AddTicks(5137), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 4, 1, 11, 15, 36, 460, DateTimeKind.Unspecified).AddTicks(3149), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
