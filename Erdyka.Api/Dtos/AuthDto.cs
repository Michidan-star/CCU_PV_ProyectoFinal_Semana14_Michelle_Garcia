using System.Text.Json.Serialization;

namespace Erdyka.Api.DTOs
{
    public class RegistroDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Cambiado a string
    }
    public class LoginDto
    {
        [JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}