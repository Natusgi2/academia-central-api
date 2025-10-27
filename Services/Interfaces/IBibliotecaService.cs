// Services/Interfaces/IBibliotecaService.cs

using System.Threading.Tasks;
using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;
namespace Academia_Central.Api.Services
{
    // Interfaz que define la comunicación con la API externa de la biblioteca (BiblioLink)
    public interface IBibliotecaService
    {
        // Verifica si un alumno tiene préstamos pendientes en la biblioteca
        // Devuelve true si hay deudas o si ocurre algún error de conexión
        Task<bool> TienePrestamosPendientes(string matricula);
        // Envía una solicitud GET hacia la API externa y devuelve la respuesta HTTP
        // Actúa como proxy entre la API local y la API de biblioteca
        Task<HttpResponseMessage> GetPrestamos(string matricula);
    }
}