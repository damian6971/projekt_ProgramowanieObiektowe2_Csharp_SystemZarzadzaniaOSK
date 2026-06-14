using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class DodanoKategorieKursanta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kategoria",
                table: "Kursanci",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kategoria",
                table: "Kursanci");
        }
    }
}
