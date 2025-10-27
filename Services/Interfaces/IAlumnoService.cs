// Services/Interfaces/IAlumnoService.cs

using Academia_Central.Api.Models;
using System.Threading.Tasks;

namespace Academia_Central.Api.Services
{
    // Interfaz que define el contrato del servicio de alumnos.
    // Establece qué operaciones puede realizar la capa de negocio sobre los alumnos.
    public interface IAlumnoService
    {
        // Obtiene la lista completa de alumnos
        Task<List<Alumno>> GetAll();
        // Busca un alumno por su ID (clave primaria)
        Task<Alumno?> GetById(int id);
        
        // Agrega un nuevo alumno a la base de datos
        Task<Alumno> Add(Alumno alumno);
        // Actualiza un alumno existente
        Task<Alumno?> Update(int id, Alumno alumno);
        // Elimina un alumno por su ID
        Task<bool> Delete(int id);
    }
}