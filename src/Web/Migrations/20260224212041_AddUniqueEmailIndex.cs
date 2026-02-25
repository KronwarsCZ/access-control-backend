using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccessControl.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HardwareId",
                table: "Devices",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProvisionedDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CertificateSerialNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CertificateThumbprint = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CertificateIssuedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CertificateExpiresUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RenewalCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastSeenUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LinkedDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProvisionedDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProvisionedDevices_Devices_LinkedDeviceId",
                        column: x => x.LinkedDeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_HardwareId",
                table: "Devices",
                column: "HardwareId",
                unique: true,
                filter: "[HardwareId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionedDevices_CertificateThumbprint",
                table: "ProvisionedDevices",
                column: "CertificateThumbprint");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionedDevices_DeviceId",
                table: "ProvisionedDevices",
                column: "DeviceId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProvisionedDevices_LinkedDeviceId",
                table: "ProvisionedDevices",
                column: "LinkedDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProvisionedDevices");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropIndex(
                name: "IX_Devices_HardwareId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_Email",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "HardwareId",
                table: "Devices");
        }
    }
}
