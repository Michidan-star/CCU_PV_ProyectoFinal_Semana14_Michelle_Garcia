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

        // GET: api/pedidos/5 (Método necesario para consultar un pedido individual)
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDto>> GetPedido(int id)
        {
            var p = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return NotFound();

            var dto = new PedidoDto
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
            };

            return Ok(dto);
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
                        await transaction.RollbackAsync();
                        return BadRequest($"El producto con ID {itemDto.ProductoId} no existe en el catálogo.");
                    }

                    // 2. Validar que haya suficiente stock
                    if (producto.StockActual < itemDto.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.StockActual}, solicitado: {itemDto.Cantidad}.");
                    }

                    // 3. REBAJAR EL INVENTARIO AUTOMÁTICAMENTE
                    producto.StockActual -= itemDto.Cantidad;
                    _context.Entry(producto).State = EntityState.Modified;

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



        // DELETE: api/pedidos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            // Usamos una transacción para asegurarnos de que se borren los detalles y el pedido correctamente
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.DetallePedidos)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pedido == null)
                {
                    return NotFound(new { mensaje = "El pedido no fue encontrado." });
                }

                // 1. Eliminar los detalles asociados al pedido primero
                if (pedido.DetallePedidos != null && pedido.DetallePedidos.Any())
                {
                    _context.DetallePedidos.RemoveRange(pedido.DetallePedidos);
                }

                // 2. Eliminar el pedido principal
                _context.Pedidos.Remove(pedido);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "¡Pedido eliminado con éxito!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno al eliminar el pedido: {ex.Message}");
            }
        }
    }
}