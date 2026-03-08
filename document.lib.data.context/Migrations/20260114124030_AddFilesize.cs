using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.data.context.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FilesizeInBytes",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilesizeInBytes",
                table: "Documents");
        }
    }
}
