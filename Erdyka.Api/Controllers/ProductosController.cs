using Erdyka.Api.Data;
using Erdyka.Api.DTOs;
using Erdyka.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erdyka.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Productos
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    StockActual = p.StockActual
                })
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var p = await _context.Productos.FindAsync(id);
            if (p == null) return NotFound();

            var dto = new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Precio = p.Precio,
                StockActual = p.StockActual
            };

            return Ok(dto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto([FromBody] ProductoCrearDto productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Descripcion = productoDto.Descripcion,
                Precio = productoDto.Precio,
                StockActual = productoDto.StockActual
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var dtoRespuesta = new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                StockActual = producto.StockActual
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, dtoRespuesta);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, [FromBody] ProductoDto productoDto)
        {
            if (id != productoDto.Id) return BadRequest();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            producto.Nombre = productoDto.Nombre;
            producto.Descripcion = productoDto.Descripcion;
            producto.Precio = productoDto.Precio;
            producto.StockActual = productoDto.StockActual;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}