using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class DodanoRezerwacjeJazd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jazdy_Instruktorzy_InstruktorId",
                table: "Jazdy");

            migrationBuilder.DropForeignKey(
                name: "FK_Jazdy_Kursanci_KursantId",
                table: "Jazdy");

            migrationBuilder.DropForeignKey(
                name: "FK_Jazdy_Pojazdy_PojazdId",
                table: "Jazdy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jazdy",
                table: "Jazdy");

            migrationBuilder.RenameTable(
                name: "Jazdy",
                newName: "Jazda");

            migrationBuilder.RenameIndex(
                name: "IX_Jazdy_PojazdId",
                table: "Jazda",
                newName: "IX_Jazda_PojazdId");

            migrationBuilder.RenameIndex(
                name: "IX_Jazdy_KursantId",
                table: "Jazda",
                newName: "IX_Jazda_KursantId");

            migrationBuilder.RenameIndex(
                name: "IX_Jazdy_InstruktorId",
                table: "Jazda",
                newName: "IX_Jazda_InstruktorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jazda",
                table: "Jazda",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RezerwacjeJazd",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Termin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CzasTrwaniaGodziny = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KursantId = table.Column<int>(type: "int", nullable: false),
                    InstruktorId = table.Column<int>(type: "int", nullable: false),
                    PojazdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RezerwacjeJazd", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RezerwacjeJazd_Instruktorzy_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "Instruktorzy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RezerwacjeJazd_Kursanci_KursantId",
                        column: x => x.KursantId,
                        principalTable: "Kursanci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RezerwacjeJazd_Pojazdy_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazdy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RezerwacjeJazd_InstruktorId",
                table: "RezerwacjeJazd",
                column: "InstruktorId");

            migrationBuilder.CreateIndex(
                name: "IX_RezerwacjeJazd_KursantId",
                table: "RezerwacjeJazd",
                column: "KursantId");

            migrationBuilder.CreateIndex(
                name: "IX_RezerwacjeJazd_PojazdId",
                table: "RezerwacjeJazd",
                column: "PojazdId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jazda_Instruktorzy_InstruktorId",
                table: "Jazda",
                column: "InstruktorId",
                principalTable: "Instruktorzy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jazda_Kursanci_KursantId",
                table: "Jazda",
                column: "KursantId",
                principalTable: "Kursanci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jazda_Pojazdy_PojazdId",
                table: "Jazda",
                column: "PojazdId",
                principalTable: "Pojazdy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jazda_Instruktorzy_InstruktorId",
                table: "Jazda");

            migrationBuilder.DropForeignKey(
                name: "FK_Jazda_Kursanci_KursantId",
                table: "Jazda");

            migrationBuilder.DropForeignKey(
                name: "FK_Jazda_Pojazdy_PojazdId",
                table: "Jazda");

            migrationBuilder.DropTable(
                name: "RezerwacjeJazd");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Jazda",
                table: "Jazda");

            migrationBuilder.RenameTable(
                name: "Jazda",
                newName: "Jazdy");

            migrationBuilder.RenameIndex(
                name: "IX_Jazda_PojazdId",
                table: "Jazdy",
                newName: "IX_Jazdy_PojazdId");

            migrationBuilder.RenameIndex(
                name: "IX_Jazda_KursantId",
                table: "Jazdy",
                newName: "IX_Jazdy_KursantId");

            migrationBuilder.RenameIndex(
                name: "IX_Jazda_InstruktorId",
                table: "Jazdy",
                newName: "IX_Jazdy_InstruktorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Jazdy",
                table: "Jazdy",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jazdy_Instruktorzy_InstruktorId",
                table: "Jazdy",
                column: "InstruktorId",
                principalTable: "Instruktorzy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jazdy_Kursanci_KursantId",
                table: "Jazdy",
                column: "KursantId",
                principalTable: "Kursanci",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jazdy_Pojazdy_PojazdId",
                table: "Jazdy",
                column: "PojazdId",
                principalTable: "Pojazdy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
