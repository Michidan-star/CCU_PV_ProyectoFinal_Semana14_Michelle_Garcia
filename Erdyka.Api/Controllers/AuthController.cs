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

            // Buscamos un rol por defecto
            var rolPorDefecto = await _context.Roles.FirstOrDefaultAsync();
            if (rolPorDefecto == null)
            {
                rolPorDefecto = new Rol { NombreRol = "Administrador" };
                _context.Roles.Add(rolPorDefecto);
                await _context.SaveChangesAsync();
            }

            // Encriptamos la contraseña con BCrypt
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Correo = request.Correo,
                ContrasenaHash = passwordHash,
                RolId = rolPorDefecto.RolId,
                Activo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Correo) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { mensaje = "Ingrese correo y contraseña." });
            }

            // Usamos FirstOrDefaultAsync de forma totalmente asíncrona y segura
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo && u.Activo);

            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El correo no está registrado o el usuario está inactivo." });
            }

            bool passwordValid = false;
            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, usuario.ContrasenaHash);
            }
            catch (Exception)
            {
                passwordValid = false;
            }

            if (!passwordValid)
            {
                return BadRequest(new { mensaje = "La contraseña es incorrecta." });
            }

            return Ok(new
            {
                mensaje = $"¡Bienvenido de nuevo, {usuario.NombreUsuario}!",
                rol = usuario.Rol?.NombreRol ?? "Administrador"
            });
        }
    }
}