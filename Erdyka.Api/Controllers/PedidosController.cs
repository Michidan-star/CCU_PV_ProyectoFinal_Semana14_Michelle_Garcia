using Erdyka.Api.Data;
using Erdyka.Api.DTOs;
using Erdyka.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erdyka.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDto>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.Producto)
                .Select(p => new PedidoDto
                {
                    Id = p.Id,
                    NombreCliente = p.NombreCliente,
                    TelefonoCliente = p.TelefonoCliente,
                    FechaCreacion = p.FechaCreacion,
                    FechaEntrega = p.FechaEntrega,
                    Estado = p.Estado,
                    Total = p.Total,
                    DetallePedidos = p.DetallePedidos.Select(d => new DetallePedidoDto
                    {
                        Id = d.Id,
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto != null ? d.Producto.Nombre : "Desconocido",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        DetallePersonalizado = d.DetallePersonalizado
                    }).ToList()
                })
                .ToListAsync();

            return Ok(pedidos);
        }

        // POST: api/pedidos
        [HttpPost]
        public async Task<IActionResult> PostPedido([FromBody] PedidoCrearDto dto)
        {
            // Usamos una transacción para asegurarnos de que si algo falla, no se descuente stock por error
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoPedido = new Pedido
                {
                    NombreCliente = dto.NombreCliente,
                    TelefonoCliente = dto.TelefonoCliente,
                    FechaCreacion = DateTime.Now,
                    FechaEntrega = dto.FechaEntrega,
                    Estado = dto.Estado,
                    DetallePedidos = new List<DetallePedido>()
                };

                decimal totalPedido = 0;

                foreach (var itemDto in dto.Items)
                {
                    // 1. Buscar el producto en la BD
                    var producto = await _context.Productos.FindAsync(itemDto.ProductoId);
                    if (producto == null)
                    {
                        return BadRequest($"El producto con ID {itemDto.ProductoId} no existe en el catálogo.");
                    }

                    // 2. Validar que haya suficiente stock
                    if (producto.StockActual < itemDto.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}, solicitado: {itemDto.Cantidad}.");
                    }

                    // 3. REBAJAR EL INVENTARIO AUTOMÁTICAMENTE
                    producto.StockActual -= itemDto.Cantidad;

                    // 4. Calcular subtotal y armar el detalle
                    var subtotal = producto.Precio * itemDto.Cantidad;
                    totalPedido += subtotal;

                    var detalle = new DetallePedido
                    {
                        ProductoId = producto.Id,
                        Cantidad = itemDto.Cantidad,
                        PrecioUnitario = producto.Precio,
                        DetallePersonalizado = itemDto.DetallePersonalizado
                    };

                    nuevoPedido.DetallePedidos.Add(detalle);
                }

                nuevoPedido.Total = totalPedido;

                _context.Pedidos.Add(nuevoPedido);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "¡Pedido registrado con éxito y stock actualizado!", idPedido = nuevoPedido.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno al registrar el pedido: {ex.Message}");
            }
        }
    }
}