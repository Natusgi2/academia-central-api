// Controllers/BibliotecaProxyController.cs

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Academia_Central.Api.Services;

namespace Academia_Central.Api.Controllers
{
    [ApiController]
    [Route("api/biblioteca")] // <- ruta especifica para el proxy de biblioteca, cambiado al tratar de arreglar un error.
    public class BibliotecaProxyController : ControllerBase
    {
        private readonly IBibliotecaService _bibliotecaService; // Campo para acceder al servicio que se comunica con la API externa “BiblioLink”.

        public BibliotecaProxyController(IBibliotecaService bibliotecaService)
        {
            _bibliotecaService = bibliotecaService;
            Console.WriteLine($"BibliotecaProxyController creado. Service: {_bibliotecaService}"); // Línea de depuración para confirmar la creación del controlador y la inyección del servicio.
        }

        [HttpGet("loans/student/{matricula}")]
        public async Task<IActionResult> GetPrestamos(string matricula)
        {
            var response = await _bibliotecaService.GetPrestamos(matricula);//Recibe una matrícula y llama a 
            // GetPrestamos(matricula) del servicio, que usa HttpClient para consultar la API externa

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content); //Retorna el mismo código de estado HTTP que la 
            // API externa (response.StatusCode) y su contenido.
        }
    }
}