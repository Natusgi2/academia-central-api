// Services/Interfaces/ICarreraService.cs

using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;
namespace Academia_Central.Api.Services
{
    // Interfaz que define las operaciones de negocio para la entidad Carrera
    public interface ICarreraService
    {
        // Obtiene la lista completa de carreras
        Task<List<Carrera>> GetAll();
        // Busca una carrera por su ID (clave primaria)
        Task<Carrera?> GetById(int id);
        // Agrega una nueva carrera a la base de datos
        Task<Carrera> Add(Carrera carrera);
        // Actualiza una carrera existente
        Task<Carrera?> Update(int id, Carrera carrera);
        // Elimina una carrera por su ID
        Task<bool> Delete(int id);
    }
}