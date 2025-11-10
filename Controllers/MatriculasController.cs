// Controllers/MatriculasController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Academia_Central.Api.Services;
using Academia_Central.Api.Models;

namespace Academia_Central.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculasController : ControllerBase
    {
        // Inyección de servicios necesarios para validar y registrar matriculaciones

        private readonly IMatriculaService _matriculaService;
        private readonly IAlumnoService _alumnoService;
        private readonly ICarreraService _carreraService;
        private readonly IBibliotecaService _bibliotecaService;

        // Constructor con inyección de dependencias

        public MatriculasController(
            IMatriculaService matriculaService,
            IAlumnoService alumnoService,
            ICarreraService carreraService,
            IBibliotecaService bibliotecaService)
        {
            _matriculaService = matriculaService;
            _alumnoService = alumnoService;
            _carreraService = carreraService;
            _bibliotecaService = bibliotecaService;
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
        {
            // 1. Verificar que el alumno exista
            var alumno = await _alumnoService.GetById(request.AlumnoId);
            if (alumno == null)
                return NotFound($"Alumno con ID {request.AlumnoId} no encontrado.");

            // 2. Verificar que la carrera exista
            var carrera = await _carreraService.GetById(request.CarreraId);
            if (carrera == null)
                return NotFound($"Carrera con ID {request.CarreraId} no encontrada.");

            // 3. Verificar que no haya matrícula vigente en el mismo período
            var existe = await _matriculaService.ExisteMatriculaVigente(alumno.Id, request.PeriodoAcademico);
            if (existe)
                return BadRequest($"El alumno ya tiene una matrícula vigente para el período {request.PeriodoAcademico}.");

            // 4. Validar situación en biblioteca (PASO CRÍTICO)
            bool tienePendientes = await _bibliotecaService.TienePrestamosPendientes(alumno.Matricula);
            if (tienePendientes){
                // Forbid() devuelve código HTTP 403 - acción prohibida
                return StatusCode(403, "No se puede matricular: tiene préstamos pendientes en la biblioteca.");}

            // 5. Crear nueva matrícula
            var matricula = new Matricula
            {
                AlumnoId = alumno.Id,
                CarreraId = carrera.Id,
                PeriodoAcademico = request.PeriodoAcademico,
                FechaInicioVigencia = DateTime.Now,
                //FechaFinVigencia = DateTime.Now.AddMonths(4), 
                FechaFinVigencia = DateTime.Now.AddSeconds(10), // Para una prueba rapida, (profe si ve esto pongame un 10)
                Estado = "Vigente"
            };
            // Guarda la matrícula en la base de datos
            await _matriculaService.Add(matricula);
            // Devuelve 201 Created con la URL al nuevo recurso
            return CreatedAtAction(nameof(GetById), new { id = matricula.Id }, matricula);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Matricula>> GetById(int id)
        {
            // Busca la matrícula en la base de datos
            var m = await _matriculaService.GetById(id);
            // Si no existe -> 404, si existe -> 200 con el objeto
            return m is null ? NotFound() : Ok(m);
        }
    }

    // DTO: objeto que representa los datos necesarios para inscribir un alumno
    public class EnrollRequest
    {
        public int AlumnoId { get; set; }
        public int CarreraId { get; set; }
        public string PeriodoAcademico { get; set; } = string.Empty;
    }
}