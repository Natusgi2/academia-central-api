// Program.cs - CORREGIDO
using Academia_Central.Api.Data;
using Academia_Central.Api.Services;
using Academia_Central.Api.Services.Implementations;
using Academia_Central.Api.Models;
using Academia_Central.Api.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args); // instancia builder que configura los servicios, configuraciones, logging y host.

// ---- SERVICIOS ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AcademiaCentral API", Version = "v1" });
});
// Swagger genera una interfaz web donde puedes probar los endpoints sin Postman


// Base de datos
var connectionString = 
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=academia.db";  //obtiene la cadena de conexion desde appsettings.json o usa SQLite por defecto
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString)); //registra el contexto de la base de datos (applicationDbContext) para que use SQLite con la cadena de conexion dada

// Servicios
//los servicios se registran para que puedan ser inyectados en controladores mediante constructor
// con scoped para que cada solicitud HTTP tenga su propia instancia    
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<ICarreraService, CarreraService>();
builder.Services.AddScoped<IBibliotecaService, BibliotecaService>();
builder.Services.AddHostedService<MatriculaCleanupService>();

// ⚠️ CORRECCIÓN CRÍTICA: HttpClient para BibliotecaService
builder.Services.AddHttpClient<IBibliotecaService, BibliotecaService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<BibliotecaApiConfig>>().Value;
    // Obtiene la configuración de BibliotecaApi desde appsettings.json
    // IOptions<T> es el patrón de ASP.NET para inyectar configuraciones

    // ✅ IMPORTANTE: BaseAddress NO debe incluir /api
    // La ruta completa se construye en el servicio
    if (!string.IsNullOrEmpty(config.UrlBase))
    {
        // Asegurarse de que termine en /
        // TrimEnd('/')lo que hace es eliminar cualquier / al final, luego se agrega uno solo, no se porque funciona pero lo hace
        //esto arreglo el error al llamar a la api externa de bibliolink... le pedi a claude que me explique porque funciono
        //pero no me dio una respuesta clara

        var baseUrl = config.UrlBase.TrimEnd('/') + "/";
        //probando me di cuenta porque no funcionaba, la urlbase en appsettings no tenia / al final, al llamar los prestamos se formaba 
        // "urlbase"++api (simplificacion del problema) y eso formaba una url invalida. la ia detecto eso y lo corrigio con trimend, pero
        //para evitar errores futuros, agrego el + / despues del trimend, asegurandose que siempre haya una / al final. 

        client.BaseAddress = new Uri(baseUrl);
    }
    
    // Timeout configurado
    client.Timeout = TimeSpan.FromSeconds(30);
    // Si la API externa tarda mas de 30 segundos en responder, se lanza un timeoutexception

})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //Acepta certificados SSL no validos. Sin esto,  errores habria con https://localhost
});

// Configuración de API externa
builder.Services.Configure<BibliotecaApiConfig>(
    builder.Configuration.GetSection("BibliotecaApi"));
// Lee la sección "BibliotecaApi" de appsettings.json y la mapea a BibliotecaApiConfig
// Esto permite inyectar IOptions<BibliotecaApiConfig> en cualquier servicio

// ---- APLICACIÓN ----
var app = builder.Build();
// Construye la aplicación web con todos los servicios configurados

// Migraciones
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Obtiene una instancia del contexto de base de datos

    context.Database.Migrate();
    // Aplica automáticamente las migraciones pendientes

}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// Middleware que redirige automaticamente HTTP -> HTTPS

app.UseAuthorization();
// Verifica si el usuario tiene permisos para acceder a ciertos endpoints

app.MapControllers();
// Busca todas las clases con [ApiController] y registra sus endpoints

app.Run();
// Inicia la aplicación y comienza a escuchar solicitudes HTTP 