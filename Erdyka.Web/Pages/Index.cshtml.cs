using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ProductoViewModel> Productos { get; set; } = new();
        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");

                var response = await client.GetAsync("api/productos");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
                }
                else
                {
                    MensajeError = "No se pudieron cargar los productos desde la API.";
                }
            }
            catch (Exception)
            {
                MensajeError = "Ocurrió un error de conexión con la API. ¿Está encendida?";
            }
        }
    }
}