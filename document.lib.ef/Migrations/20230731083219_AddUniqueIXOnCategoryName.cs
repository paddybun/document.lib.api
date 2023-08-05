using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.ef.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIXOnCategoryName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Registers_CurrentRegisterId",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Documents_DocLibDocumentId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Folders_CurrentRegisterId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "CurrentRegisterId",
                table: "Folders");

            migrationBuilder.RenameColumn(
                name: "DocLibDocumentId",
                table: "Tags",
                newName: "EfDocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_DocLibDocumentId",
                table: "Tags",
                newName: "IX_Tags_EfDocumentId");

            migrationBuilder.AddColumn<int>(
                name: "DocumentCount",
                table: "Registers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFull",
                table: "Folders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateOfDocument",
                table: "Documents",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Documents_EfDocumentId",
                table: "Tags",
                column: "EfDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Documents_EfDocumentId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DocumentCount",
                table: "Registers");

            migrationBuilder.DropColumn(
                name: "IsFull",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "EfDocumentId",
                table: "Tags",
                newName: "DocLibDocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_EfDocumentId",
                table: "Tags",
                newName: "IX_Tags_DocLibDocumentId");

            migrationBuilder.AddColumn<int>(
                name: "CurrentRegisterId",
                table: "Folders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateOfDocument",
                table: "Documents",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CurrentRegisterId",
                table: "Folders",
                column: "CurrentRegisterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Registers_CurrentRegisterId",
                table: "Folders",
                column: "CurrentRegisterId",
                principalTable: "Registers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Documents_DocLibDocumentId",
                table: "Tags",
                column: "DocLibDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }
    }
}
