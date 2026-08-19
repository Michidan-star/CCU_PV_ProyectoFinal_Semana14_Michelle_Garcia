using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Erdyka.Web.Pages.Admin
{
    public class CrearUsuarioModel : PageModel
    {
        private readonly HttpClient _http;
        public CrearUsuarioModel(HttpClient http) => _http = http;

        [BindProperty]
        public UsuarioInput Input { get; set; } = new();

        [TempData]
        public string? MensajeError { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                try
                {
                    var usuario = await _http.GetFromJsonAsync<UsuarioInput>($"https://localhost:7168/api/admin/usuarios/{id.Value}");
                    if (usuario != null)
                    {
                        Input = usuario;
                        // Limpiamos la contraseña al cargar para que no se envíe un hash viejo alterado
                        Input.Contrasena = string.Empty;
                    }
                }
                catch (Exception)
                {
                    // Manejo silencioso si falla la carga inicial
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            int usuarioIdReal = id ?? Input.UsuarioId;

            // Si la contraseña viene vacía al editar, le asignamos un texto temporal 
            // para que la API no reciba un string nulo y reviente.
            if (usuarioIdReal > 0 && string.IsNullOrWhiteSpace(Input.Contrasena))
            {
                Input.Contrasena = "Temporal123*";
            }

            HttpResponseMessage response;

            if (usuarioIdReal > 0)
            {
                Input.UsuarioId = usuarioIdReal;
                response = await _http.PutAsJsonAsync($"https://localhost:7168/api/admin/usuarios/{usuarioIdReal}", Input);
            }
            else
            {
                response = await _http.PostAsJsonAsync("https://localhost:7168/api/admin/usuarios", Input);
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./GestionUsuarios");
            }
            else
            {
                var errorContenido = await response.Content.ReadAsStringAsync();
                MensajeError = $"Error al guardar: {errorContenido}";
                return Page();
            }
        }
    }

    public class UsuarioInput
    {
        [JsonPropertyName("usuarioId")]
        public int UsuarioId { get; set; }

        [JsonPropertyName("nombreUsuario")]
        public string NombreUsuario { get; set; } = "";

        [JsonPropertyName("correo")]
        public string Correo { get; set; } = "";

        [JsonPropertyName("contrasena")]
        public string Contrasena { get; set; } = "";

        [JsonPropertyName("rolId")]
        public int RolId { get; set; } = 2;
    }
}