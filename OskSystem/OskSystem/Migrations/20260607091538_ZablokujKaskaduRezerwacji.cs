using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class ZablokujKaskaduRezerwacji : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Instruktorzy_InstruktorId",
                table: "RezerwacjeJazd");

            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Kursanci_KursantId",
                table: "RezerwacjeJazd");

            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Pojazdy_PojazdId",
                table: "RezerwacjeJazd");

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Instruktorzy_InstruktorId",
                table: "RezerwacjeJazd",
                column: "InstruktorId",
                principalTable: "Instruktorzy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Kursanci_KursantId",
                table: "RezerwacjeJazd",
                column: "KursantId",
                principalTable: "Kursanci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Pojazdy_PojazdId",
                table: "RezerwacjeJazd",
                column: "PojazdId",
                principalTable: "Pojazdy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Instruktorzy_InstruktorId",
                table: "RezerwacjeJazd");

            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Kursanci_KursantId",
                table: "RezerwacjeJazd");

            migrationBuilder.DropForeignKey(
                name: "FK_RezerwacjeJazd_Pojazdy_PojazdId",
                table: "RezerwacjeJazd");

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Instruktorzy_InstruktorId",
                table: "RezerwacjeJazd",
                column: "InstruktorId",
                principalTable: "Instruktorzy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Kursanci_KursantId",
                table: "RezerwacjeJazd",
                column: "KursantId",
                principalTable: "Kursanci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RezerwacjeJazd_Pojazdy_PojazdId",
                table: "RezerwacjeJazd",
                column: "PojazdId",
                principalTable: "Pojazdy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
