using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuramos la autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

// 2. Rutas protegidas y carpeta Auth pública
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToFolder("/Auth");
});

// 3. Registrar el HttpClient apuntando al puerto EXACTO de tu API (7168)
builder.Services.AddHttpClient("ErdykaApi", client =>
{
    // Si tus controladores en la API usan [Route("api/[controller]")]:
    client.BaseAddress = new Uri("https://localhost:7168/api/");

    // NOTA: Si en tu AuthController la ruta NO lleva "api/", cambia la línea anterior por:
    // client.BaseAddress = new Uri("https://localhost:7168/");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();