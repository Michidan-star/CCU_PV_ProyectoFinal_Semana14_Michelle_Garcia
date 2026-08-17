using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class ProductosModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<ProductoViewModel> ListaProductos { get; set; } = new();

        [BindProperty]
        public ProductoViewModel NuevoProducto { get; set; } = new();

        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await CargarProductosAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var json = JsonSerializer.Serialize(NuevoProducto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/productos", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }

                MensajeError = "No se pudo registrar el producto.";
            }
            catch (Exception)
            {
                MensajeError = "Error de conexión con la API.";
            }

            await CargarProductosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ErdykaApi");
            await client.DeleteAsync($"api/productos/{id}");
            return RedirectToPage();
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var response = await client.GetAsync("api/productos");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    ListaProductos = JsonSerializer.Deserialize<List<ProductoViewModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
                }
            }
            catch (Exception)
            {
                MensajeError = "No se pudieron cargar los productos.";
            }
        }
    }
}