// Controllers/AlumnosController.cs
using Microsoft.AspNetCore.Mvc;
using Academia_Central.Api.Models;
using Academia_Central.Api.Services;
using System.Threading.Tasks;

namespace Academia_Central.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlumnosController : ControllerBase
    {
        private readonly IAlumnoService _alumnoService;

        public AlumnosController(IAlumnoService alumnoService)
        {
            _alumnoService = alumnoService;
        }
        //automáticamente inyecta una instancia de AlumnoService (registrada en Program.cs como Scoped).

        // GET: api/alumnos
        [HttpGet]
        public async Task<ActionResult<List<Alumno>>> GetAll()
        {
            var alumnos = await _alumnoService.GetAll();
            return Ok(alumnos);//devuelve un codigo 200 con la lista de alumnos
        }

        // GET: api/alumnos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alumno>> GetById(int id)
        {
            var alumno = await _alumnoService.GetById(id);
            if (alumno == null)
            {
                return NotFound($"No se encontró el alumno con ID {id}");
            }
            return Ok(alumno);
        }

        // POST: api/alumnos
        [HttpPost]
        public async Task<ActionResult<Alumno>> Create([FromBody] Alumno alumno) //toma el cuerpo JSON de la solicitud y lo convierte en un objeto Alumno.
        {
            if (alumno == null)
            {
                return BadRequest("El alumno no puede ser nulo.");
            }

            try
            {
                var nuevoAlumno = await _alumnoService.Add(alumno);
                return CreatedAtAction(nameof(GetById), new { id = nuevoAlumno.Id }, nuevoAlumno);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Por ejemplo, matrícula duplicada
            }
        }

        // PUT: api/alumnos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Alumno>> Update(int id, [FromBody] Alumno alumno)
        {
            if (alumno == null)
            {
                return BadRequest("El alumno no puede ser nulo.");
            }

            var alumnoActualizado = await _alumnoService.Update(id, alumno);
            if (alumnoActualizado == null)
            {
                return NotFound($"No se encontró el alumno con ID {id}");
            }

            return Ok(alumnoActualizado);
        }

        // DELETE: api/alumnos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existe = await _alumnoService.GetById(id) != null; //Primero valida si existe.
            if (!existe)
            {
                return NotFound($"No se encontró el alumno con ID {id}");
            }

            var eliminado = await _alumnoService.Delete(id);
            if (!eliminado)
            {
                return StatusCode(500, "Error al eliminar el alumno."); //control de errores internos basico
            }

            return NoContent(); // 204
        }
    }
}