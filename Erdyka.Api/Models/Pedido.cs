using System;
using System.Collections.Generic;

namespace Erdyka.Api.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string TelefonoCliente { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime FechaEntrega { get; set; }
        public string Estado { get; set; } = "pendiente"; // pagado, abono o pendiente
        public decimal Total { get; set; }

        // Relación de uno a muchos con los detalles del pedido
        public ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();
    }
}