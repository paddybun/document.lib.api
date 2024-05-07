using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.ef.Migrations
{
    /// <inheritdoc />
    public partial class EnableCascadeDeleteTagAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_TagAssignments_Documents_DocumentId",
                table: "TagAssignments",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
