using System.ComponentModel.DataAnnotations;

namespace Erdyka.Api.Models;

public class Producto
{
    [Key]
    public int ProductoId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1000000)]
    public decimal Precio { get; set; }

    [Required]
    [Range(0, 10000)]
    public int StockDisponible { get; set; }
}