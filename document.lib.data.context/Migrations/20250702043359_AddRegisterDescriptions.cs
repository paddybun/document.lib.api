using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.data.context.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisterDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescriptionId",
                table: "Registers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegisterDescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterDescriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registers_DescriptionId",
                table: "Registers",
                column: "DescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registers_RegisterDescriptions_DescriptionId",
                table: "Registers",
                column: "DescriptionId",
                principalTable: "RegisterDescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registers_RegisterDescriptions_DescriptionId",
                table: "Registers");

            migrationBuilder.DropTable(
                name: "RegisterDescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Registers_DescriptionId",
                table: "Registers");

            migrationBuilder.DropColumn(
                name: "DescriptionId",
                table: "Registers");
        }
    }
}
