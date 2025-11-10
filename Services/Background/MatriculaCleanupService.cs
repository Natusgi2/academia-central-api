using Academia_Central.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Academia_Central.Api.Services.Background
{
    public class MatriculaCleanupService : BackgroundService
    {
        // Necesitamos un ScopeFactory porque el DbContext es "Scoped" y este servicio es "Singleton"
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MatriculaCleanupService> _logger;

        public MatriculaCleanupService(IServiceScopeFactory scopeFactory, ILogger<MatriculaCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de limpieza de matrículas iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CerrarMatriculasVencidas(stoppingToken);
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
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var hoy = DateTime.Today;

                // Busca matrículas que deben cerrarse
                // Criterio: Estado 'Vigente' Y FechaFinVigencia es anterior a hoy
                var matriculasVencidas = await context.Matriculas
                    .Where(m => m.Estado == "Vigente" && m.FechaFinVigencia < hoy)
                    .ToListAsync(token);

                if (matriculasVencidas.Any())
                {
                    _logger.LogInformation($"Se encontraron {matriculasVencidas.Count} matrículas vencidas. Cerrando...");

                    foreach (var matricula in matriculasVencidas)
                    {
                        matricula.Estado = "Finalizada";
                    }

                    await context.SaveChangesAsync(token);
                    _logger.LogInformation("Matrículas actualizadas correctamente.");
                }
                else
                {
                     _logger.LogDebug("No se encontraron matrículas para cerrar hoy.");
                }
            }
        }
    }
}