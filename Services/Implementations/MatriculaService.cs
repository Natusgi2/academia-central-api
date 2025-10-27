// Services/Implementations/MatriculaService.cs

using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Academia_Central.Api.Services.Implementations
{
    // Servicio que maneja las matrículas de alumnos en carreras
    public class MatriculaService : IMatriculaService
    {
        private readonly ApplicationDbContext _context; // Contexto EF para acceder a la BD

        public MatriculaService(ApplicationDbContext context) 
        {
            _context = context; // Inyección de dependencia del contexto de la base de datos
        }
        // Obtiene todas las matrículas, incluyendo datos de alumno y carrera
        public async Task<List<Matricula>> GetAll() => await _context.Matriculas
            .Include(m => m.Alumno)
            .Include(m => m.Carrera)
            .ToListAsync();
        // Busca una matrícula por ID e incluye relaciones
        public async Task<Matricula?> GetById(int id) => await _context.Matriculas
            .Include(m => m.Alumno)
            .Include(m => m.Carrera)
            .FirstOrDefaultAsync(m => m.Id == id);
        // Agrega una nueva matrícula
        public async Task<Matricula> Add(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }
        // Actualiza una matrícula existente
        public async Task<Matricula?> Update(int id, Matricula matricula) 
        {
            var existente = await _context.Matriculas.FindAsync(id); // Busca la matrícula por ID
            if (existente == null) return null; // Si no existe, devuelve null

            existente.PeriodoAcademico = matricula.PeriodoAcademico;
            existente.FechaInicioVigencia = matricula.FechaInicioVigencia;
            existente.FechaFinVigencia = matricula.FechaFinVigencia;
            existente.Estado = matricula.Estado;

            await _context.SaveChangesAsync(); // Guarda los cambios en la BD
            return existente;
        }
// Elimina una matrícula por ID
        public async Task<bool> Delete(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id); //busca la matrícula por el id,
            if (matricula == null) return false; //si no existe devuelve false

            _context.Matriculas.Remove(matricula);  //elimina la matrícula del contexto
            await _context.SaveChangesAsync(); //guarda los cambios en la BD
            return true;
        }
// Verifica si existe una matrícula vigente para un alumno en un período académico dado
        public async Task<bool> ExisteMatriculaVigente(int alumnoId, string periodoAcademico)
        {
            return await _context.Matriculas // Consulta la tabla de matrículas
                .AnyAsync(m =>
                    m.AlumnoId == alumnoId && 
                    m.PeriodoAcademico == periodoAcademico &&
                    m.Estado == "Vigente"); // Devuelve true si existe al menos una matrícula que cumpla las condiciones
        }
    }
}