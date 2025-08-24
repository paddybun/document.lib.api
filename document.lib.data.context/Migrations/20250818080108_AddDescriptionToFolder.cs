using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.data.context.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Folders");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionGroup",
                table: "Folders",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_Name",
                table: "Folders",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_Name",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "DescriptionGroup",
                table: "Folders");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Folders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
