using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Readit.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddReadingStatusToUserBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserBooks");
        }
    }
}
