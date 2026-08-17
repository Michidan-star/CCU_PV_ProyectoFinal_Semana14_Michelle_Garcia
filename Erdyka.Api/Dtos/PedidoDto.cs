namespace Erdyka.Api.DTOs
{
    public class PedidoCrearDto
    {
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; } = "pendiente"; // pagado, abono o pendiente
        public List<DetallePedidoCrearDto> Items { get; set; } = new();
    }

    public class DetallePedidoCrearDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? DetallePersonalizado { get; set; } // Ej: "Texto en taza: Feliz Cumpleaños"
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<DetallePedidoDto> DetallePedidos { get; set; } = new();
    }

    public class DetallePedidoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? DetallePersonalizado { get; set; }
    }
}