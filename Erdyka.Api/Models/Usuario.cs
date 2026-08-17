public class Usuario
{
    public int Id { get; set; } 
    public string NombreUsuario { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    public int RolId { get; set; }
    public Rol? Rol { get; set; }
}