// Services/Implementations/BibliotecaService.cs - VERSIÓN FINAL CORREGIDA
using Microsoft.Extensions.Options;
using System.Text.Json;
using Academia_Central.Api.Models;
using Academia_Central.Api.Services;

namespace Academia_Central.Api.Services.Implementations
{
    public class BibliotecaService : IBibliotecaService
    {
        private readonly HttpClient _httpClient; // Cliente HTTP para conectarse a la API externa
        private readonly BibliotecaApiConfig _config; // Configuración de la API (URL base, API key)
        private readonly ILogger<BibliotecaService> _logger; // Para registrar logs en la consola o archivos

        // Constructor: recibe dependencias mediante inyección (config, logger y HttpClient)
        public BibliotecaService(
            HttpClient httpClient, 
            IOptions<BibliotecaApiConfig> config,
            ILogger<BibliotecaService> logger)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _logger = logger;
        }
        // Verifica si un alumno tiene préstamos pendientes en la biblioteca
        public async Task<bool> TienePrestamosPendientes(string matricula)
        {
            // Si la matrícula es inválida, se niega por seguridad
            if (string.IsNullOrWhiteSpace(matricula))
            {
                _logger.LogWarning("Matrícula vacía o nula");
                return true; // Denegar por seguridad
            }

            try
            {
                // Construye la URL relativa para la API externa
                var requestUrl = $"api/loans/student/{matricula}";
                
                _logger.LogInformation($"Consultando préstamos en: {_httpClient.BaseAddress}{requestUrl}"); 
                // Crea una solicitud GET con cabecera de autenticación
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl); 
                request.Headers.Add("X-API-Key", _config.ApiKey); 

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); // Timeout de 10 segundos
                var response = await _httpClient.SendAsync(request, cts.Token); // Envía la solicitud con timeout

                _logger.LogInformation($"Respuesta de BiblioLink: {response.StatusCode}"); // Registra el código de estado HTTP recibido

                // Si la API responde con error (404, 500, etc.), asumimos que hay pendientes
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error al consultar biblioteca: {response.StatusCode}");
                    return true; // Por seguridad, denegar si hay error
                }
                // Lee y deserializa el JSON de la respuesta
                var json = await response.Content.ReadAsStringAsync();
                _logger.LogDebug($"JSON recibido: {json}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };// Ignora mayúsculas/minúsculas en nombres de propiedades
                var prestamos = JsonSerializer.Deserialize<List<PrestamoDto>>(json, options); // Deserializa a lista de DTOs

                // Devuelve true si hay préstamos pendientes (Estado != "Devuelto")
                bool tienePendientes = prestamos?.Any(p => p.Estado != "Devuelto") == true;
                
                _logger.LogInformation($"Alumno {matricula} - Tiene pendientes: {tienePendientes}"); 
                
                return tienePendientes;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Timeout al consultar la API de biblioteca");
                return true; // Denegar por timeout
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de red al consultar biblioteca");
                return true; // Denegar por error de red
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al consultar biblioteca");
                return true; // Denegar por error inesperado
            }
        }
        // Método proxy que reenvía solicitudes GET a la API externa
        public async Task<HttpResponseMessage> GetPrestamos(string matricula)
        {
            if (string.IsNullOrWhiteSpace(matricula))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Matrícula no válida.")
                };
            }

            try
            {
                // Construir la URL correctamente
                var requestUrl = $"api/loans/student/{matricula}";
                
                _logger.LogInformation($"Proxy - Consultando: {_httpClient.BaseAddress}{requestUrl}");
                
                // IMPORTANTE: Crear el objeto HttpRequestMessage
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Add("X-API-Key", _config.ApiKey);

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.SendAsync(request, cts.Token);

                return response;
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Timeout en proxy de biblioteca");
                return new HttpResponseMessage(System.Net.HttpStatusCode.GatewayTimeout)
                {
                    Content = new StringContent("La API de biblioteca no respondió a tiempo.")
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error de conexión en proxy de biblioteca");
                return new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent($"Error al conectar con la biblioteca: {ex.Message}")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno en proxy de biblioteca");
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Error interno: {ex.Message}")
                };
            }
        }
    }
    // DTO (Data Transfer Object) usado para deserializar la respuesta de la API externa
    public class PrestamoDto
    {
        public int Id { get; set; }
        public string LibroTitulo { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionEstimada { get; set; }
        public DateTime? FechaDevolucionReal { get; set; }
        public string Estado { get; set; } = "Pendiente";
    }
}