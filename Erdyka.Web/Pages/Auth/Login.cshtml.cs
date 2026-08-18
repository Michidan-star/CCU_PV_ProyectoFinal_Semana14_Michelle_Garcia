using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Erdyka.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ErdykaApi");
        }

        [BindProperty]
        public LoginDto LoginInput { get; set; } = new();

        [BindProperty]
        public RegistroDto RegistroInput { get; set; } = new();

        [TempData]
        public string? MensajeError { get; set; }

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeErrorRegistro { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostRegistro()
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/registro", RegistroInput);
                if (response.IsSuccessStatusCode)
                {
                    MensajeExito = "¡Usuario registrado con éxito! Ya puedes iniciar sesión.";
                    return Page();
                }
                else
                {
                    var errorContenido = await response.Content.ReadAsStringAsync();
                    MensajeErrorRegistro = string.IsNullOrWhiteSpace(errorContenido)
                        ? "No se pudo registrar el usuario."
                        : errorContenido.Trim('"');
                    return Page();
                }
            }
            catch (Exception ex)
            {
                MensajeErrorRegistro = $"Error de conexión: {ex.Message}";
                return Page();
            }
        }
    }

    public class LoginDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegistroDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}