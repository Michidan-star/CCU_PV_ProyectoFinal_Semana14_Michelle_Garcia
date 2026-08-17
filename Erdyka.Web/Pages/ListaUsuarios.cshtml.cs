using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class ListaUsuariosModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ListaUsuariosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<UsuarioDtoModel> ListaUsuarios { get; set; } = new();
        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");

                // Llama al endpoint GET de la API
                var response = await client.GetAsync("api/usuarios");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Convierte el JSON de la API a la lista de C# ignorando mayúsculas/minúsculas
                    ListaUsuarios = JsonSerializer.Deserialize<List<UsuarioDtoModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
                }
                else
                {
                    MensajeError = "No se pudieron cargar los usuarios de la API.";
                }
            }
            catch (Exception)
            {
                MensajeError = "Error de conexión con el servidor.";
            }

        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ErdykaApi");

            // Enviamos la petición DELETE a la API
            var response = await client.DeleteAsync($"api/usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Si borra bien, recargamos la página para que la tabla se actualice
                return RedirectToPage();
            }

            MensajeError = "Error al intentar borrar el usuario.";
            return Page();
        }
    }
}