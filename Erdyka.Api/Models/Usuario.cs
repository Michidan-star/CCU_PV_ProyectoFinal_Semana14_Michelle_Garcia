namespace Erdyka.Api.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        // Debe ser string para evitar el conflicto de tipos
        public string PasswordHash { get; set; } = string.Empty;

        public int RolId { get; set; }
        public Rol? Rol { get; set; }
    }
}