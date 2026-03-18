using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControl.Web.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSessionTokenWithTokenVersio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionToken",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "TokenVersion",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenVersion",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "SessionToken",
                table: "Accounts",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);
        }
    }
}
