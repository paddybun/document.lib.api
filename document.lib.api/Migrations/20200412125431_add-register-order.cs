using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace document.lib.api.Migrations
{
    public partial class addregisterorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Registers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 4, 12, 14, 54, 31, 634, DateTimeKind.Unspecified).AddTicks(4105), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 4, 1, 11, 15, 36, 460, DateTimeKind.Unspecified).AddTicks(3149), new TimeSpan(0, 2, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Registers");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 4, 1, 11, 15, 36, 460, DateTimeKind.Unspecified).AddTicks(3149), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 4, 12, 14, 54, 31, 634, DateTimeKind.Unspecified).AddTicks(4105), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
