using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.ef.Migrations
{
    /// <inheritdoc />
    public partial class AddTagAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Documents_EfDocumentId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EfDocumentId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "EfDocumentId",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "TagAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    EfTagId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagAssignments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TagAssignments_Tags_EfTagId",
                        column: x => x.EfTagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_DocumentId",
                table: "TagAssignments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_EfTagId",
                table: "TagAssignments",
                column: "EfTagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagAssignments");

            migrationBuilder.AddColumn<int>(
                name: "EfDocumentId",
                table: "Tags",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EfDocumentId",
                table: "Tags",
                column: "EfDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Documents_EfDocumentId",
                table: "Tags",
                column: "EfDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }
    }
}
