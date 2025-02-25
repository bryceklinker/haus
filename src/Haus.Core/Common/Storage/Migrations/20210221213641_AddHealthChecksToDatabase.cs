using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Haus.Core.Common.Storage.Migrations
{
    public partial class AddHealthChecksToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthCheckEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ExceptionMessage = table.Column<string>(type: "TEXT", nullable: true),
                    DurationOfCheckInMilliseconds = table.Column<double>(type: "REAL", nullable: false),
                    LastUpdatedTimestamp = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthCheckEntity", x => x.Id);
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "HealthCheckEntity");
        }
    }
}
