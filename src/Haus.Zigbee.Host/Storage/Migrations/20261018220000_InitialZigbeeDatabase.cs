using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Zigbee.Host.Storage.Migrations
{
    /// <inheritdoc />
    public partial class InitialZigbeeDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnownDevices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false),
                    NetworkAddress = table.Column<ushort>(type: "INTEGER", nullable: false),
                    EndpointsJson = table.Column<string>(type: "TEXT", nullable: false),
                    IsSleepy = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownDevices", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_KnownDevices_ExternalId",
                table: "KnownDevices",
                column: "ExternalId",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "KnownDevices");
        }
    }
}
