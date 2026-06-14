using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class AktualizacjaInstruktora : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumerLicencji",
                table: "Instruktorzy",
                newName: "Telefon");

            migrationBuilder.AddColumn<string>(
                name: "NumerLegitymacji",
                table: "Instruktorzy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumerLegitymacji",
                table: "Instruktorzy");

            migrationBuilder.RenameColumn(
                name: "Telefon",
                table: "Instruktorzy",
                newName: "NumerLicencji");
        }
    }
}
