using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    public partial class AddOccupancyTimeoutToRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OccupancyTimeoutInSeconds",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "OccupancyTimeoutInSeconds", table: "Rooms");
        }
    }
}
