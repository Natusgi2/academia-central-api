// Services/Implementations/CarreraService.cs

using Academia_Central.Api.Data;
using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using Microsoft.EntityFrameworkCore;


namespace Academia_Central.Api.Services
{
    public class CarreraService : ICarreraService
    {
        private readonly ApplicationDbContext _context;

        public CarreraService(ApplicationDbContext context)
        {
            _context = context; // Inyección de dependencia del contexto de la base de datos
        }

        public async Task<List<Carrera>> GetAll() => await _context.Carreras.ToListAsync(); // Devuelve todas las carreras

        public async Task<Carrera?> GetById(int id) => await _context.Carreras.FindAsync(id); // Busca una carrera por ID

        public async Task<Carrera> Add(Carrera carrera) // Agrega una nueva carrera
        {
            _context.Carreras.Add(carrera); // Agrega la nueva carrera al contexto
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return carrera;
        }

        public async Task<Carrera?> Update(int id, Carrera carrera) // Actualiza una carrera existente
        {
            var existente = await _context.Carreras.FindAsync(id); // Busca la carrera por ID
            if (existente == null) return null; // Si no existe, devuelve null

            existente.Nombre = carrera.Nombre;
            existente.DuracionAnios = carrera.DuracionAnios;
            existente.CodigoUnico = carrera.CodigoUnico;

            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return existente; // Devuelve la carrera actualizada
        }

        public async Task<bool> Delete(int id) // Elimina una carrera por ID
        {
            var carrera = await _context.Carreras.FindAsync(id); // Busca la carrera por ID
            if (carrera == null) return false; // Si no existe, devuelve false

            _context.Carreras.Remove(carrera); // Elimina la carrera del contexto si existe
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return true;
        }
    }
}