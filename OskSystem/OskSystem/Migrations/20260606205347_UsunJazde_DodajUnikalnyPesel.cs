using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OskSystem.Migrations
{
    public partial class UsunJazde_DodajUnikalnyPesel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jazda");

            migrationBuilder.AlterColumn<string>(
                name: "Pesel",
                table: "Kursanci",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Kursanci",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Instruktorzy",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Administratorzy",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Kursanci_Login",
                table: "Kursanci",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kursanci_Pesel",
                table: "Kursanci",
                column: "Pesel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instruktorzy_Login",
                table: "Instruktorzy",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administratorzy_Login",
                table: "Administratorzy",
                column: "Login",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kursanci_Login",
                table: "Kursanci");

            migrationBuilder.DropIndex(
                name: "IX_Kursanci_Pesel",
                table: "Kursanci");

            migrationBuilder.DropIndex(
                name: "IX_Instruktorzy_Login",
                table: "Instruktorzy");

            migrationBuilder.DropIndex(
                name: "IX_Administratorzy_Login",
                table: "Administratorzy");

            migrationBuilder.AlterColumn<string>(
                name: "Pesel",
                table: "Kursanci",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Kursanci",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Instruktorzy",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Administratorzy",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Jazda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstruktorId = table.Column<int>(type: "int", nullable: false),
                    KursantId = table.Column<int>(type: "int", nullable: false),
                    PojazdId = table.Column<int>(type: "int", nullable: false),
                    CzasTrwaniaGodziny = table.Column<int>(type: "int", nullable: false),
                    DataRozpoczecia = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jazda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jazda_Instruktorzy_InstruktorId",
                        column: x => x.InstruktorId,
                        principalTable: "Instruktorzy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jazda_Kursanci_KursantId",
                        column: x => x.KursantId,
                        principalTable: "Kursanci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Jazda_Pojazdy_PojazdId",
                        column: x => x.PojazdId,
                        principalTable: "Pojazdy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jazda_InstruktorId",
                table: "Jazda",
                column: "InstruktorId");

            migrationBuilder.CreateIndex(
                name: "IX_Jazda_KursantId",
                table: "Jazda",
                column: "KursantId");

            migrationBuilder.CreateIndex(
                name: "IX_Jazda_PojazdId",
                table: "Jazda",
                column: "PojazdId");
        }
    }
}
