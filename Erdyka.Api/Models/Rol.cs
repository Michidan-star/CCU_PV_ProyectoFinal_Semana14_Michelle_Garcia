namespace Erdyka.Api.Models
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ejemplo: "Administrador", "Vendedor"

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}