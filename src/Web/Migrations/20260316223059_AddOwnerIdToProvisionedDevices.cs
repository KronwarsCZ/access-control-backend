using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControl.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerIdToProvisionedDevices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add column as nullable so existing rows are valid
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "ProvisionedDevices",
                type: "uniqueidentifier",
                nullable: true);

            // Backfill OwnerId from linked Device's owner (devices provisioned with a secret are linked to a Device that has an OwnerId)
            migrationBuilder.Sql(@"
                UPDATE p
                SET p.OwnerId = d.OwnerId
                FROM ProvisionedDevices p
                INNER JOIN Devices d ON d.Id = p.LinkedDeviceId
                WHERE p.LinkedDeviceId IS NOT NULL AND p.OwnerId IS NULL");

            // Any provisioned devices without a linked device: assign first owner so FK can be applied
            migrationBuilder.Sql(@"
                UPDATE ProvisionedDevices
                SET OwnerId = (SELECT TOP 1 Id FROM Owners)
                WHERE OwnerId IS NULL");

            // Now enforce non-null and add FK
            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "ProvisionedDevices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionedDevices_OwnerId",
                table: "ProvisionedDevices",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProvisionedDevices_Owners_OwnerId",
                table: "ProvisionedDevices",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProvisionedDevices_Owners_OwnerId",
                table: "ProvisionedDevices");

            migrationBuilder.DropIndex(
                name: "IX_ProvisionedDevices_OwnerId",
                table: "ProvisionedDevices");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ProvisionedDevices");
        }
    }
}
