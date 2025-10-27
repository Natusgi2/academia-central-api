// Services/Interfaces/IMatriculaService.cs

using Academia_Central.Api.Models;
using System.Threading.Tasks;

namespace Academia_Central.Api.Services
{
    // Interfaz que define la lógica de negocio relacionada con las matrículas

    public interface IMatriculaService
    {
        // Obtiene todas las matrículas (con sus relaciones)
        Task<List<Matricula>> GetAll();
        // Busca una matrícula por ID
        Task<Matricula?> GetById(int id);
        // Crea una nueva matrícula
        Task<Matricula> Add(Matricula matricula);
        // Actualiza una matrícula existente
        Task<Matricula?> Update(int id, Matricula matricula);
        // Elimina una matrícula por ID
        Task<bool> Delete(int id);

        // Verifica si un alumno ya tiene una matrícula vigente en un período específico
        Task<bool> ExisteMatriculaVigente(int alumnoId, string periodoAcademico);
    }
}