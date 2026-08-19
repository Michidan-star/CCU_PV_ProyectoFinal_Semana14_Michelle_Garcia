using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace Erdyka.Web.Pages.Admin
{
    public class CrearUsuarioModel : PageModel
    {
        private readonly HttpClient _http;
        public CrearUsuarioModel(HttpClient http) => _http = http;

        [BindProperty]
        public UsuarioInput Input { get; set; } = new();

        // Esta parte es la que carga los datos al entrar
        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var usuario = await _http.GetFromJsonAsync<UsuarioInput>($"https://localhost:7168/api/admin/usuarios/{id.Value}");
                if (usuario != null)
                {
                    Input = usuario;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            HttpResponseMessage response;
            if (Input.UsuarioId > 0)
            {
                response = await _http.PutAsJsonAsync($"https://localhost:7168/api/admin/usuarios/{Input.UsuarioId}", Input);
            }
            else
            {
                response = await _http.PostAsJsonAsync("https://localhost:7168/api/admin/usuarios", Input);
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./GestionUsuarios");
            }
            return Page();
        }
    }

    public class UsuarioInput
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Contrasena { get; set; } = "";
        public int RolId { get; set; } = 2;
    }
}