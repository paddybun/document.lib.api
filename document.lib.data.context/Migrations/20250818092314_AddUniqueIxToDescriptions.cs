using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace document.lib.data.context.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIxToDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RegisterDescriptions_Group_Order",
                table: "RegisterDescriptions",
                columns: new[] { "Group", "Order" },
                unique: true,
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegisterDescriptions_Group_Order",
                table: "RegisterDescriptions");
        }
    }
}
