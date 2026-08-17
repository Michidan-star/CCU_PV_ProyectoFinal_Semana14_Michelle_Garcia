namespace Erdyka.Web.Models
{
    public class UsuarioDtoModel
    {
        public int Id { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
    }
}