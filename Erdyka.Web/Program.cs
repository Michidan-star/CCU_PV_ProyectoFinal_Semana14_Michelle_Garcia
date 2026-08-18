var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Registrar HttpClient para consumir la API
builder.Services.AddHttpClient("ErdykaApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7168"); // URL donde corre la API
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// NUEVO: Redirigir la raíz del sitio web directamente al Login/Registro
app.MapGet("/", async context =>
{
    context.Response.Redirect("/Auth/Login");
    await Task.CompletedTask;
});

app.MapRazorPages()
   .WithStaticAssets();

app.Run();