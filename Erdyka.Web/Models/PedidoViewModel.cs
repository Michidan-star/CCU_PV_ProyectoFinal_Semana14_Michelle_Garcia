namespace Erdyka.Web.Models
{
    public class PedidoViewModel
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaEntrega { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";
        public decimal Total { get; set; }

        // Propiedades auxiliares para la interfaz visual
        public int ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
        public string DetallePersonalizado { get; set; } = string.Empty;
    }
}