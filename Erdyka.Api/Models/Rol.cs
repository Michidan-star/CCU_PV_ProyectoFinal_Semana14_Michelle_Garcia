using System.ComponentModel.DataAnnotations;

namespace Erdyka.Api.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreRol { get; set; } = string.Empty; // Ejemplo: "Administrador", "Usuario"

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
