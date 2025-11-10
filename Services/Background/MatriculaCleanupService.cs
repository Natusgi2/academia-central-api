using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Academia_Central.Api.Services.Background
{
    public class MatriculaCleanupService : BackgroundService // Servicio en segundo plano para limpiar matrículas vencidas
    {
        // Necesitamos un ScopeFactory porque el DbContext es "Scoped" y este servicio es "Singleton"
        private readonly IServiceScopeFactory _scopeFactory; //scope factory para crear scopes
        private readonly ILogger<MatriculaCleanupService> _logger; //logger para registrar eventos

        public MatriculaCleanupService(IServiceScopeFactory scopeFactory, ILogger<MatriculaCleanupService> logger) //inyecta dependencias
        {
            _scopeFactory = scopeFactory; //inyecta scope factory
            _logger = logger; //inyecta logger
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de limpieza de matrículas iniciado."); //log de inicio

            while (!stoppingToken.IsCancellationRequested) // Bucle infinito hasta que se cancele el token
            {
                try
                {
                    await CerrarMatriculasVencidas(stoppingToken); // Llama al método que cierra matrículas vencidas
                }   
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la limpieza de matrículas."); 
                }

                // Esperar 24 horas antes de la siguiente ejecución (ajustable para pruebas)
                // Para probarlo rápido, puedes cambiarlo a: TimeSpan.FromMinutes(1)
                //await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // PARA PRUEBAS RÁPIDAS
            }
        }

        private async Task CerrarMatriculasVencidas(CancellationToken token)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); //obtiene el dbcontext
                var hoy = DateTime.Now; //se utiliza datetime.now, para facilitar pruebas con periodos cortos, se deberia usar .Today para mejores practicas

                // Busca matrículas que deben cerrarse
                // Criterio: Estado 'Vigente' Y FechaFinVigencia es anterior a hoy
                var matriculasVencidas = await context.Matriculas 
                    .Where(m => m.Estado == "Vigente" && m.FechaFinVigencia < hoy) //matriculas vencidas
                    .ToListAsync(token);

                if (matriculasVencidas.Any()) //si hay matriculas vencidas
                {
                    _logger.LogInformation($"Se encontraron {matriculasVencidas.Count} matrículas vencidas. Cerrando..."); //log de informacion

                    foreach (var matricula in matriculasVencidas) //por cada matricula vencida
                    {
                        matricula.Estado = "Finalizada";
                    }

                    await context.SaveChangesAsync(token); //guarda los cambios en la bd
                    _logger.LogInformation("Matrículas actualizadas correctamente."); //log de exito
                }
                else
                {
                     _logger.LogDebug("No se encontraron matrículas para cerrar hoy."); //log de debug
                }
            }
        }
    }
}