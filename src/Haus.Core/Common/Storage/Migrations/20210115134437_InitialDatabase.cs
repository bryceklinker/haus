using Microsoft.EntityFrameworkCore.Migrations;

namespace Haus.Core.Common.Storage.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discovery",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    State = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discovery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Lighting_State = table.Column<string>(type: "TEXT", nullable: true),
                    Lighting_Level_Value = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Level_Min = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Level_Max = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Value = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Min = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Max = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Color_Red = table.Column<byte>(type: "INTEGER", nullable: true),
                    Lighting_Color_Green = table.Column<byte>(type: "INTEGER", nullable: true),
                    Lighting_Color_Blue = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceType = table.Column<string>(type: "TEXT", nullable: false),
                    LightType = table.Column<int>(type: "INTEGER", nullable: false),
                    RoomId = table.Column<long>(type: "INTEGER", nullable: true),
                    Lighting_State = table.Column<string>(type: "TEXT", nullable: true),
                    Lighting_Level_Value = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Level_Min = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Level_Max = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Value = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Min = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Temperature_Max = table.Column<double>(type: "REAL", nullable: true),
                    Lighting_Color_Red = table.Column<byte>(type: "INTEGER", nullable: true),
                    Lighting_Color_Green = table.Column<byte>(type: "INTEGER", nullable: true),
                    Lighting_Color_Blue = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMetadata",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<long>(type: "INTEGER", nullable: true),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceMetadata_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMetadata_DeviceId",
                table: "DeviceMetadata",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_RoomId",
                table: "Devices",
                column: "RoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceMetadata");

            migrationBuilder.DropTable(
                name: "Discovery");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
