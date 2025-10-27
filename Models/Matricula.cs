// Models/Matricula.cs
namespace Academia_Central.Api.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int CarreraId { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty; // ej: "2025-1"

        public DateTime FechaInicioVigencia { get; set; }
        public DateTime FechaFinVigencia { get; set; }

        public string Estado { get; set; } = "Vigente"; // Vigente, Finalizada, Inactiva

        // Relaciones
        public Alumno? Alumno { get; set; }
        public Carrera? Carrera { get; set; }
    }
}