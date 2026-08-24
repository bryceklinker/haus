using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceNetworkAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "NetworkAddress",
                table: "Devices",
                type: "INTEGER",
                nullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "NetworkAddress", table: "Devices");
        }
    }
}
