using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readit.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "UserBooks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "UserBooks");
        }
    }
}
