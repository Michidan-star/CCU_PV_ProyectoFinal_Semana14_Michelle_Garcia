using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Erdyka.Web.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // AQUÍ ESTÁ EL CAMBIO: Usamos IHttpClientFactory para inyectar el cliente configurado
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

        public async Task<IActionResult> OnPostLogin()
        {
            try
            {
                var content = JsonContent.Create(LoginInput);
                var response = await _httpClient.PostAsync("auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<LoginRespuestaDto>();

                    var claims = new List<System.Security.Claims.Claim>
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, LoginInput.Correo),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, resultado?.Rol ?? "Administrador")
                    };

                    var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
                        new System.Security.Claims.ClaimsPrincipal(claimsIdentity));

                    return RedirectToPage("/Index");
                }
                else
                {
                    try
                    {
                        var errorObj = await response.Content.ReadFromJsonAsync<ErrorRespuestaDto>();
                        MensajeError = errorObj?.mensaje ?? $"Error HTTP Status: {(int)response.StatusCode}";
                    }
                    catch
                    {
                        var errorCrudo = await response.Content.ReadAsStringAsync();
                        MensajeError = string.IsNullOrWhiteSpace(errorCrudo)
                            ? $"Error HTTP Status: {(int)response.StatusCode}"
                            : $"API: {errorCrudo}";
                    }
                    return Page();
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error de conexión con la API: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostRegistro()
        {
            try
            {
                var content = JsonContent.Create(RegistroInput);
                var response = await _httpClient.PostAsync("auth/registro", content);

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
        [System.Text.Json.Serialization.JsonPropertyName("correo")]
        public string Correo { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegistroDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRespuestaDto
    {
        public string Mensaje { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class ErrorRespuestaDto
    {
        public string mensaje { get; set; } = string.Empty;
    }
}