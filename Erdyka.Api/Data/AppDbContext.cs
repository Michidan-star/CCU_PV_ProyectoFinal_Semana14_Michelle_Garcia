using Erdyka.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Erdyka.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Roles por defecto
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolId = 1, NombreRol = "Administrador" },
                new Rol { RolId = 2, NombreRol = "Usuario" }
            );

            // Usuario Administrador por defecto (con valor fijo para evitar errores)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioId = 1,
                    NombreUsuario = "ErdykaAdmin",
                    Correo = "admin@erdyka.com",
                    ContrasenaHash = "Admin123_Temp",
                    RolId = 1,
                    Activo = true
                }
            );

            // Para configurar la precisión de los decimales
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}