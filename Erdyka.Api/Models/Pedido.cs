using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Erdyka.Api.Models;

public class Pedido
{
    [Key]
    public int PedidoId { get; set; }

    [Required]
    [MaxLength(150)]
    public string NombreCliente { get; set; } = string.Empty;

    public DateTime FechaPedido { get; set; } = DateTime.Now;

    public DateTime FechaEntrega { get; set; }

    [Required]
    [MaxLength(50)]
    public string Estado { get; set; } = "Pendiente"; // Pagado, Abono, Pendiente

    [Required]
    public decimal Total { get; set; }

    public List<DetallePedido> Detalles { get; set; } = new();
}

public class DetallePedido
{
    [Key]
    public int DetalleId { get; set; }

    [ForeignKey("Pedido")]
    public int PedidoId { get; set; }
    public Pedido? Pedido { get; set; }

    [ForeignKey("Producto")]
    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }
}