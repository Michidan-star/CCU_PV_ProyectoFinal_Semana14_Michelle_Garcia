using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace Erdyka.Web.Pages.Admin
{
    public class GestionUsuariosModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GestionUsuariosModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<UsuarioDto> Usuarios { get; set; } = new List<UsuarioDto>();

        public async Task OnGetAsync()
        {
            try
            {
                // URL absoluta apuntando a tu API en el puerto 7168
                string urlApi = "https://localhost:7168/api/admin/usuarios";
                Usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioDto>>(urlApi) ?? new List<UsuarioDto>();
            }
            catch
            {
                Usuarios = new List<UsuarioDto>();
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                string urlApi = $"https://localhost:7168/api/admin/usuarios/{id}";
                var response = await _httpClient.DeleteAsync(urlApi);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Usuario eliminado con éxito.";
                }
            }
            catch
            {
                // Manejo de errores opcional
            }

            return RedirectToPage();
        }
    }

    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}