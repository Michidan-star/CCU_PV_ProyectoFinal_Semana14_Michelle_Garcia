using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Erdyka.Api.Data;
using Erdyka.Api.Models;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/admin/usuarios
    [HttpGet("usuarios")]
    public async Task<IActionResult> ObtenerUsuarios()
    {
        try
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new {
                    u.UsuarioId,
                    u.NombreUsuario,
                    u.Correo,
                    Rol = u.Rol != null ? u.Rol.NombreRol : "Sin Rol"
                })
                .ToListAsync();

            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error al obtener usuarios", error = ex.Message });
        }
    }

    // GET: api/admin/usuarios/{id} (Para cargar los datos al editar)
    [HttpGet("usuarios/{id}")]
    public async Task<IActionResult> ObtenerUsuarioPorId(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado" });

        var dto = new
        {
            usuarioId = usuario.UsuarioId,
            nombreUsuario = usuario.NombreUsuario,
            correo = usuario.Correo,
            contrasena = usuario.ContrasenaHash,
            rolId = usuario.RolId
        };

        return Ok(dto);
    }

    // POST: api/admin/usuarios (Para agregar nuevos usuarios desde el panel)
    [HttpPost("usuarios")]
    public async Task<IActionResult> CrearUsuario([FromBody] CreacionUsuarioDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Validar si el correo ya existe
        var existe = await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo);
        if (existe)
            return BadRequest(new { mensaje = "Ya existe un usuario registrado con este correo." });

        var nuevoUsuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            Correo = dto.Correo,
            ContrasenaHash = dto.Contrasena,
            RolId = dto.RolId
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario creado correctamente" });
    }

    // PUT: api/admin/usuarios/{id} (Para actualizar el usuario existente)
    [HttpPut("usuarios/{id}")]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] CreacionUsuarioDto dto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado" });

        usuario.NombreUsuario = dto.NombreUsuario;
        usuario.Correo = dto.Correo;
        usuario.ContrasenaHash = dto.Contrasena;
        usuario.RolId = dto.RolId;

        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario actualizado correctamente" });
    }

    // DELETE: api/admin/usuarios/{id}
    [HttpDelete("usuarios/{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
            return NotFound(new { mensaje = "Usuario no encontrado" });

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Usuario eliminado correctamente" });
    }
}

// DTO para recibir los datos al crear o actualizar un usuario
public class CreacionUsuarioDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public int RolId { get; set; }
}