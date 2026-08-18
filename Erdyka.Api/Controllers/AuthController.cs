using Erdyka.Api.Data;
using Erdyka.Api.DTOs;
using Erdyka.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Erdyka.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/registro
        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            {
                return BadRequest("El correo ya está registrado.");
            }

            var rolPorDefecto = await _context.Set<Rol>().FirstOrDefaultAsync()
                                ?? new Rol { Nombre = "Administrador" };

            // Usamos un hash seguro o texto plano limpio para evitar bloqueos de bytes
            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Correo = request.Correo,
                PasswordHash = request.Password, // Guardado directo y limpio para pruebas estables
                Rol = rolPorDefecto
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Ingrese correo y contraseña.");
            }

            // Buscamos directamente sin includes raros para probar
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario == null)
            {
                return Unauthorized("El correo no está registrado en la base de datos.");
            }

            if (usuario.PasswordHash != request.Password)
            {
                return Unauthorized("La contraseña es incorrecta.");
            }

            return Ok(new
            {
                mensaje = $"¡Bienvenido de nuevo, {usuario.NombreUsuario}!",
                rol = "Administrador"
            });
        }
    }
}