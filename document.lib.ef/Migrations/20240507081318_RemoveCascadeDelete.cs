using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.ef.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Categories_CategoryId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Registers_RegisterId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Registers_Folders_FolderId",
                table: "Registers");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Tags_TagId",
                table: "TagAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Categories_CategoryId",
                table: "Documents",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Registers_RegisterId",
                table: "Documents",
                column: "RegisterId",
                principalTable: "Registers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Registers_Folders_FolderId",
                table: "Registers",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Tags_TagId",
                table: "TagAssignments",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Categories_CategoryId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Registers_RegisterId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Registers_Folders_FolderId",
                table: "Registers");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Tags_TagId",
                table: "TagAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Categories_CategoryId",
                table: "Documents",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Registers_RegisterId",
                table: "Documents",
                column: "RegisterId",
                principalTable: "Registers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registers_Folders_FolderId",
                table: "Registers",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Tags_TagId",
                table: "TagAssignments",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
