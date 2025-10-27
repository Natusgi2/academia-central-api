using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Academia_Central.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Matricula = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", nullable: false),
                    DNI = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Carreras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    DuracionAnios = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoUnico = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carreras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AlumnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CarreraId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeriodoAcademico = table.Column<string>(type: "TEXT", nullable: false),
                    FechaInicioVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFinVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matriculas_Carreras_CarreraId",
                        column: x => x.CarreraId,
                        principalTable: "Carreras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alumnos",
                columns: new[] { "Id", "Apellido", "DNI", "Email", "Matricula", "Nombre" },
                values: new object[,]
                {
                    { 1, "Pérez", "12345678A", "juan@univ.edu", "ALU001", "Juan" },
                    { 2, "Gómez", "87654321B", "ana@univ.edu", "ALU002", "Ana" }
                });

            migrationBuilder.InsertData(
                table: "Carreras",
                columns: new[] { "Id", "CodigoUnico", "DuracionAnios", "Nombre" },
                values: new object[,]
                {
                    { 1, "IS-2025", 5, "Ingeniería en Sistemas" },
                    { 2, "PS-2025", 4, "Licenciatura en Psicología" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_AlumnoId_PeriodoAcademico",
                table: "Matriculas",
                columns: new[] { "AlumnoId", "PeriodoAcademico" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CarreraId",
                table: "Matriculas",
                column: "CarreraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Carreras");
        }
    }
}
