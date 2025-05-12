using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Haus.Core.Common.Storage.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForNullableTypeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value",
                table: "Rooms",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Min",
                table: "Rooms",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Max",
                table: "Rooms",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "REAL",
                oldNullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lighting_State",
                table: "Rooms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT"
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Value",
                table: "Rooms",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL"
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Min",
                table: "Rooms",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL"
            );

            migrationBuilder.AlterColumn<double>(
                name: "Lighting_Level_Max",
                table: "Rooms",
                type: "REAL",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "REAL"
            );
        }
    }
}
