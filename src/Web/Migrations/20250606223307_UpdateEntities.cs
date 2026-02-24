using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControl.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_Owners_OwnerId1",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Owners_OwnerId1",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Owners");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Owners",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Owners",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Owners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId1",
                table: "Owners",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Owners_OwnerId1",
                table: "Owners",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_Owners_OwnerId1",
                table: "Owners",
                column: "OwnerId1",
                principalTable: "Owners",
                principalColumn: "Id");
        }
    }
}
