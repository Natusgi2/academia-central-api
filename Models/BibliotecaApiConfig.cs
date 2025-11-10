// Models/BibliotecaApiConfig.cs

namespace Academia_Central.Api.Models
{
    public class BibliotecaApiConfig
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "BiblioLink";
        public string UrlBase { get; set; } = "";
        public string ApiKey { get; set; } = "";
    }
}