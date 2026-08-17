using Microsoft.EntityFrameworkCore;
using Erdyka.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración del DbContext con la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Agregar servicios de controladores
builder.Services.AddControllers();

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

app.Run();