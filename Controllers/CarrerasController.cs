// Controllers/CarrerasController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Academia_Central.Api.Services;
using Academia_Central.Api.Models;

namespace Academia_Central.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrerasController : ControllerBase
    {
        private readonly ICarreraService _service;

        public CarrerasController(ICarreraService service)
        {
            _service = service; // Guarda la referencia del servicio para usar en los endpoints
        }

        // GET: api/carreras
        [HttpGet]
        public async Task<ActionResult<List<Carrera>>> GetAll()
        {
            var carreras = await _service.GetAll();// Llama al servicio que consulta la base de datos de forma asincrónica

            return Ok(carreras);
        }

        // GET: api/carreras/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Carrera>> GetById(int id)
        {
            var carrera = await _service.GetById(id);
            // Busca la carrera por ID en la base de datos mediante el servicio

            if (carrera == null)
            {
                return NotFound($"No se encontró la carrera con ID {id}");
            }
            return Ok(carrera);
        }

        // POST: api/carreras
        [HttpPost]
        public async Task<ActionResult<Carrera>> Create([FromBody] Carrera carrera)
        {
            if (carrera == null)
            {
                return BadRequest("La carrera no puede ser nula.");
            }

            var nueva = await _service.Add(carrera);
            // Llama al servicio para agregar la nueva carrera en la base de datos
            // CreatedAtAction incluye la URL de la carrera recién creada (api/carreras/{id})
            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
        }

        // PUT: api/carreras/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Carrera carrera)
        {
            if (carrera == null)
            {
                return BadRequest("La carrera no puede ser nula.");
            }

            // Verifica que el ID de la URL coincida con el ID del objeto recibido

            if (id != carrera.Id)
            {
                return BadRequest("El ID de la URL no coincide con el ID del objeto.");
            }
         
            // Llama al servicio para actualizar la carrera

            var carreraActualizada = await _service.Update(id, carrera);
            if (carreraActualizada == null)
            {
                return NotFound($"No se encontró la carrera con ID {id}");
            }

            return NoContent(); // 204 - Actualizado correctamente
        }

        // DELETE: api/carreras/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Comprueba si existe la carrera antes de eliminar

            var existe = await _service.GetById(id) != null;
            if (!existe)
            {
                return NotFound($"No se encontró la carrera con ID {id}");
            }
            // Llama al servicio para eliminarla

            var eliminado = await _service.Delete(id);
            if (!eliminado)
            {
                return StatusCode(500, "Error interno al intentar eliminar la carrera.");
            }

            return NoContent(); // 204 - Eliminado correctamente
        }
    }
}