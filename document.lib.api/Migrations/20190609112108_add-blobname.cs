using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace document.lib.api.Migrations
{
    public partial class addblobname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2019, 6, 9, 13, 21, 8, 732, DateTimeKind.Unspecified).AddTicks(5137), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2019, 6, 1, 8, 26, 32, 980, DateTimeKind.Unspecified).AddTicks(1954), new TimeSpan(0, 2, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Blobname",
                table: "LibDocuments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Blobname",
                table: "LibDocuments");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Date",
                table: "LibDocuments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2019, 6, 1, 8, 26, 32, 980, DateTimeKind.Unspecified).AddTicks(1954), new TimeSpan(0, 2, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2019, 6, 9, 13, 21, 8, 732, DateTimeKind.Unspecified).AddTicks(5137), new TimeSpan(0, 2, 0, 0, 0)));
        }
    }
}
