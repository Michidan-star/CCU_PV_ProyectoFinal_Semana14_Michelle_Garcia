using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public LoginViewModel Credenciales { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var json = JsonSerializer.Serialize(Credenciales);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Llama al endpoint de login de tu API
                var response = await client.PostAsync("api/auth/login", content);
                var resultadoTexto = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    Mensaje = "¡Bienvenido! Has iniciado sesión correctamente.";
                    // Aquí después podrías redirigir a una página principal (Index)
                }
                else
                {
                    Mensaje = $"Error de acceso: {resultadoTexto}";
                }
            }
            catch (Exception)
            {
                Mensaje = "No se pudo conectar con la API.";
            }

            return Page();
        }
    }
}