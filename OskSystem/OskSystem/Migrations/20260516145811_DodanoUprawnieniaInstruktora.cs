using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class DodanoUprawnieniaInstruktora : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uprawnienia",
                table: "Instruktorzy",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uprawnienia",
                table: "Instruktorzy");
        }
    }
}
