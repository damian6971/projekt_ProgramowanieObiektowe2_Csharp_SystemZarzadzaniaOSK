using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Instruktorzy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumerLicencji = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instruktorzy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kursanci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nazwisko = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pesel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kursanci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pojazdy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumerRejestracyjny = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pojazdy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jazdy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataRozpoczecia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CzasTrwaniaGodziny = table.Column<int>(type: "int", nullable: false),
                    KursantId = table.Column<int>(type: "int", nullable: false),
                    InstruktorId = table.Column<int>(type: "int", nullable: false),
                    PojazdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jazdy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jazdy_Instruktorzy_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "Instruktorzy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jazdy_Kursanci_KursantId",
                        column: x => x.KursantId,
                        principalTable: "Kursanci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jazdy_Pojazdy_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazdy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jazdy_InstruktorId",
                table: "Jazdy",
                column: "InstruktorId");

            migrationBuilder.CreateIndex(
                name: "IX_Jazdy_KursantId",
                table: "Jazdy",
                column: "KursantId");

            migrationBuilder.CreateIndex(
                name: "IX_Jazdy_PojazdId",
                table: "Jazdy",
                column: "PojazdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jazdy");

            migrationBuilder.DropTable(
                name: "Instruktorzy");

            migrationBuilder.DropTable(
                name: "Kursanci");

            migrationBuilder.DropTable(
                name: "Pojazdy");
        }
    }
}
