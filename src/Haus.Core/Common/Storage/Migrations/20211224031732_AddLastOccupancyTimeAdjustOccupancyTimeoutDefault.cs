using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    public partial class AddLastOccupancyTimeAdjustOccupancyTimeoutDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OccupancyTimeoutInSeconds",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 300,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOccupiedTime",
                table: "Rooms",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOccupiedTime",
                table: "Rooms");

            migrationBuilder.AlterColumn<int>(
                name: "OccupancyTimeoutInSeconds",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 300);
        }
    }
}
