using Microsoft.EntityFrameworkCore.Migrations;

namespace Haus.Portal.Web.Common.Storage.Migrations.HausPortalDb
{
    public partial class RemoveAuthorityFromAuthSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authority",
                table: "AuthSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "AuthSettings",
                type: "text",
                nullable: true);
        }
    }
}
