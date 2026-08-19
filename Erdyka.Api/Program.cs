using Erdyka.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración del DbContext con la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Agregar servicios de controladores y configurar la respuesta de errores de validación (ModelState 400) en un solo bloque
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Esto tomará el error real de validación y lo mandará como texto al cliente
            var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return new BadRequestObjectResult(new { mensaje = string.Join(" | ", errors) });
        };
    });

// 3. Configuración para Swagger (para probar tu API visualmente)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4. Configuración del pipeline de solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 5. Inicialización y creación automática de la base de datos por seguridad
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Erdyka.Api.Data.AppDbContext>();
    context.Database.EnsureCreated(); // Crea la base de datos y todas las tablas si no existen
}

app.Run();