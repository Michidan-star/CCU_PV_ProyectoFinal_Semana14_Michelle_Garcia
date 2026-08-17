using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegistroModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public RegistroViewModel Usuario { get; set; } = new();

        public string Mensaje { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var json = JsonSerializer.Serialize(Usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Apunta a la ruta exacta de la API con 'registro'
                var response = await client.PostAsync("api/auth/registro", content);

                if (response.IsSuccessStatusCode)
                {
                    Mensaje = "¡Usuario registrado con éxito! Ya puedes iniciar sesión.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Mensaje = $"Error al registrar: {errorContent}";
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