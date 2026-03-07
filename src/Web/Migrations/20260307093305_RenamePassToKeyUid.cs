using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControl.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenamePassToKeyUid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pass",
                table: "Keys");

            migrationBuilder.AddColumn<string>(
                name: "KeyUid",
                table: "Keys",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyUid",
                table: "Keys");

            migrationBuilder.AddColumn<string>(
                name: "Pass",
                table: "Keys",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
