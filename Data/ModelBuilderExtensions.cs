// Data/ModelBuilderExtensions.cs
using Academia_Central.Api.Models;
using Microsoft.EntityFrameworkCore;

public static class ModelBuilderExtensions
{
    // Método de extensión Seed() que inicializa datos predeterminados en el modelo
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Carrera>().HasData(
            new Carrera { Id = 1, Nombre = "Ingeniería en Sistemas", DuracionAnios = 5, CodigoUnico = "IS-2025" },
            new Carrera { Id = 2, Nombre = "Licenciatura en Psicología", DuracionAnios = 4, CodigoUnico = "PS-2025" }
        );

        modelBuilder.Entity<Alumno>().HasData(
            new Alumno { Id = 1, Matricula = "ALU001", Nombre = "Juan", Apellido = "Pérez", DNI = "12345678A", Email = "juan@univ.edu" },
            new Alumno { Id = 2, Matricula = "ALU002", Nombre = "Ana", Apellido = "Gómez", DNI = "87654321B", Email = "ana@univ.edu" }
        );
    }
}