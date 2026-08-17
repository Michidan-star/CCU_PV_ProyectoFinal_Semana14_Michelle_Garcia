using System.Security.Cryptography;
using System.Text;
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
        public async Task<ActionResult<UsuarioDto>> Registrar([FromBody] RegistroDto request)
        {
            // Asegúrate de que request.Correo sea un string explícito
            if (await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo))
            {
                return BadRequest("El correo ya está registrado.");
            }

            CrearPasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var rolPorDefecto = await _context.Set<Rol>().FirstOrDefaultAsync() ?? new Rol { Nombre = "Administrador" };

            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Correo = request.Correo,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Rol = rolPorDefecto
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado con éxito" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas (Correo no encontrado).");
            }

            if (!VerificarPasswordHash(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                return Unauthorized("Credenciales inválidas (Contraseña incorrecta).");
            }

            return Ok(new { mensaje = $"¡Bienvenido de nuevo, {usuario.NombreUsuario}!", rol = usuario.Rol?.Nombre });
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}