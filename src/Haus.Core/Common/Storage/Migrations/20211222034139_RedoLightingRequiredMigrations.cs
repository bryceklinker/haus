using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    public partial class RedoLightingRequiredMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: "Off");
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value", 
                table: "Rooms", 
                nullable: false,
                defaultValue: 100);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Min_Value", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 0);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Max_Value", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 100);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Value", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 3000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Min", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 2000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Max", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 6000);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Red", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Green", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Blue", 
                table: "Rooms", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
            
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: "Off");
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value", 
                table: "Devices", 
                nullable: false,
                defaultValue: 100);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Min_Value", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 0);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Max_Value", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 100);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Value", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 3000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Min", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 2000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Max", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 6000);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Red", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Green", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Blue", 
                table: "Devices", 
                nullable: false,
                oldNullable: true, 
                defaultValue: 255);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: "Off");
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value", 
                table: "Rooms", 
                nullable: true,
                defaultValue: 100);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Min_Value", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 0);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Max_Value", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 100);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Value", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 3000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Min", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 2000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Max", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 6000);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Red", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Green", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Blue", 
                table: "Rooms", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
            
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: "Off");
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value", 
                table: "Devices", 
                nullable: true,
                defaultValue: 100);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Min_Value", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 0);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Max_Value", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 100);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Value", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 3000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Min", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 2000);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Temperature_Max", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 6000);
            
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Red", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Green", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Color_Blue", 
                table: "Devices", 
                nullable: true,
                oldNullable: true, 
                defaultValue: 255);
        }
    }
}
