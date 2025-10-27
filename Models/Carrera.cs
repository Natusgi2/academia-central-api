// Models/Carrera.cs
namespace Academia_Central.Api.Models
{
    public class Carrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int DuracionAnios { get; set; }
        public string CodigoUnico { get; set; } = string.Empty;
    }
}