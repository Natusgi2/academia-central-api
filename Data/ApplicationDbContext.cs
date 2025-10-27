// Data/ApplicationDbContext.cs
using Academia_Central.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Academia_Central.Api.Data
{
    // Es la clase central que define la conexión con la base de datos y el mapeo de las entidades
    
    public class ApplicationDbContext : DbContext
    {
        // Constructor que recibe las opciones del contexto (cadena de conexión, proveedor, etc.)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        // EF Core genera las tablas a partir de estas propiedades
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        // Método que se ejecuta cuando EF Core construye el modelo (antes de generar la base de datos)
        // Aquí se definen relaciones, restricciones, índices y datos iniciales
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Una matrícula tiene un alumno asociado (1 a muchos)
            // Matricula.AlumnoId es clave foránea de Alumno.Id
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Alumno)
                .WithMany()
                .HasForeignKey(m => m.AlumnoId);
            // Una matrícula también tiene una carrera asociada (1 a muchos)
            // Matricula.CarreraId es clave foránea de Carrera.Id
            modelBuilder.Entity<Matricula>()
                .HasOne(m => m.Carrera)
                .WithMany()
                .HasForeignKey(m => m.CarreraId);
            // un mismo alumno no puede tener dos matrículas activas para el mismo período académico
            modelBuilder.Entity<Matricula>()
                .HasIndex(m => new { m.AlumnoId, m.PeriodoAcademico })
                .IsUnique();

            // Datos iniciales (seed data) para pruebas rapidas.
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
}