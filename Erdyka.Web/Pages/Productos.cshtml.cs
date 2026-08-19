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

        // Modificado para aceptar el clic de "Editar" desde la tabla y cargar los datos en el formulario
        public async Task OnGetAsync(int? idParaEditar)
        {
            await CargarProductosAsync();

            if (idParaEditar.HasValue)
            {
                var productoEncontrado = ListaProductos.FirstOrDefault(p => p.Id == idParaEditar.Value);
                if (productoEncontrado != null)
                {
                    NuevoProducto = productoEncontrado;
                }
            }
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var json = JsonSerializer.Serialize(NuevoProducto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("productos", content);

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
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                await client.DeleteAsync($"productos/{id}");
            }
            catch (Exception)
            {
                // Manejo básico de error por si falla la red al borrar
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditarAsync()
        {
            try
            {
                // Aseguramos que usamos el ID que viene en el modelo enlazado
                int id = NuevoProducto.Id;

                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var json = JsonSerializer.Serialize(NuevoProducto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"productos/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }

                var errorDetalle = await response.Content.ReadAsStringAsync();
                MensajeError = $"No se pudo actualizar el producto. (Detalle: {errorDetalle})";
            }
            catch (Exception)
            {
                MensajeError = "Error de conexión al actualizar.";
            }

            await CargarProductosAsync();
            return Page();
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var response = await client.GetAsync("productos");

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