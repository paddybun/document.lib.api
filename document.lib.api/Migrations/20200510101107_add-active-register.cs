using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace document.lib.api.Migrations
{
    public partial class addactiveregister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Registers");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Registers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 5, 10, 12, 11, 7, 869, DateTimeKind.Unspecified).AddTicks(5165), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 4, 12, 14, 54, 31, 634, DateTimeKind.Unspecified).AddTicks(4105), new TimeSpan(0, 2, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Registers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Registers",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 4, 12, 14, 54, 31, 634, DateTimeKind.Unspecified).AddTicks(4105), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 5, 10, 12, 11, 7, 869, DateTimeKind.Unspecified).AddTicks(5165), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
