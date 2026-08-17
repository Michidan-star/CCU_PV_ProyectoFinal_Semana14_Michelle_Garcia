namespace Erdyka.Api.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Total { get; set; }

        // Relación con los detalles
        public List<DetallePedido> DetallePedidos { get; set; } = new();
    }
}