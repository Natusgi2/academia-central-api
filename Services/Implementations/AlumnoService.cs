// Services/Implementations/AlumnoService.cs
using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Academia_Central.Api.Services.Implementations
{
    public class AlumnoService : IAlumnoService
    {
        private readonly ApplicationDbContext _context; // Contexto EF para acceder a la BD

        public AlumnoService(ApplicationDbContext context)
        {
            _context = context;
        }
        // Devuelve todos los alumnos de la base de datos (SELECT * FROM Alumnos)
        public async Task<List<Alumno>> GetAll() => await _context.Alumnos.ToListAsync();
        // Busca un alumno por su ID (clave primaria)
        public async Task<Alumno?> GetById(int id) => await _context.Alumnos.FindAsync(id);
        // Agrega un nuevo alumno a la base de datos
        public async Task<Alumno> Add(Alumno alumno)
        {
            // Valida que la matrícula sea única antes de insertar
            if (await _context.Alumnos.AnyAsync(a => a.Matricula == alumno.Matricula))
                throw new InvalidOperationException($"Ya existe un alumno con matrícula {alumno.Matricula}");

            _context.Alumnos.Add(alumno);// Agrega el nuevo objeto al contexto
            await _context.SaveChangesAsync();// Guarda los cambios en la BD
            return alumno;
        }

        public async Task<Alumno?> Update(int id, Alumno alumno)
        {
            var existente = await _context.Alumnos.FindAsync(id);
            if (existente == null) return null; // Si no existe, devuelve null

            // No permitir cambiar la matrícula si ya existe otra con ese valor
            if (alumno.Matricula != existente.Matricula &&
                await _context.Alumnos.AnyAsync(a => a.Matricula == alumno.Matricula))
                throw new InvalidOperationException($"La matrícula {alumno.Matricula} ya está en uso.");

            existente.Nombre = alumno.Nombre;
            existente.Apellido = alumno.Apellido;
            existente.DNI = alumno.DNI;
            existente.Email = alumno.Email;
            existente.Matricula = alumno.Matricula;

            await _context.SaveChangesAsync(); //guarda los cambios en la BD
            return existente; // Devuelve el alumno actualizado
        }
        // Elimina un alumno por ID
        public async Task<bool> Delete(int id)
        {
            var alumno = await _context.Alumnos.FindAsync(id); //busca el alumno por el id,
            if (alumno == null) return false; //si no existe devuelve false

            _context.Alumnos.Remove(alumno); //elimina el alumno del contexto
            await _context.SaveChangesAsync(); //guarda los cambios en la BD
            return true; //devuelve true indicando que se eliminó correctamente
        }
    }
}