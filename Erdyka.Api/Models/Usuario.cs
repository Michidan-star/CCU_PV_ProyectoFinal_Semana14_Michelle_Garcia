using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erdyka.Api.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string ContrasenaHash { get; set; } = string.Empty;

        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol? Rol { get; set; }

        public bool Activo { get; set; } = true;
    }
}