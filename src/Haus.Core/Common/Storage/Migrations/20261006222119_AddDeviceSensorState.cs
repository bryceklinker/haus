using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceSensorState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(name: "BatteryLevel", table: "Devices", type: "INTEGER", nullable: true);

            migrationBuilder.AddColumn<long>(name: "Illuminance", table: "Devices", type: "INTEGER", nullable: true);

            migrationBuilder.AddColumn<long>(name: "Lux", table: "Devices", type: "INTEGER", nullable: true);

            migrationBuilder.AddColumn<double>(name: "Temperature", table: "Devices", type: "REAL", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BatteryLevel", table: "Devices");

            migrationBuilder.DropColumn(name: "Illuminance", table: "Devices");

            migrationBuilder.DropColumn(name: "Lux", table: "Devices");

            migrationBuilder.DropColumn(name: "Temperature", table: "Devices");
        }
    }
}
