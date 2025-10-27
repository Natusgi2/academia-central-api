// Models/BibliotecaApiConfig.cs

namespace Academia_Central.Api.Models
{
    public class BibliotecaApiConfig
    {
        // Nombre descriptivo de la API externa (por defecto "BiblioLink")
        public string Nombre { get; set; } = "BiblioLink";
        public string UrlBase { get; set; } = "";
        public string ApiKey { get; set; } = "";
    }
}