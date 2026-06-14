using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class AktualizacjaPojazdow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Pojazdy",
                newName: "ModelPojazdu");

            migrationBuilder.AddColumn<string>(
                name: "Kategoria",
                table: "Pojazdy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kategoria",
                table: "Pojazdy");

            migrationBuilder.RenameColumn(
                name: "ModelPojazdu",
                table: "Pojazdy",
                newName: "Model");
        }
    }
}
