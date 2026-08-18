using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class PedidosModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PedidosModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<PedidoViewModel> ListaPedidos { get; set; } = new();

        [BindProperty]
        public PedidoViewModel NuevoPedido { get; set; } = new();

        public string MensajeError { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await CargarPedidosAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");

                // Estructura limpia que empaqueta el pedido y el item para tu API
                var pedidoParaApi = new
                {
                    nombreCliente = NuevoPedido.NombreCliente,
                    telefonoCliente = NuevoPedido.TelefonoCliente ?? "N/A",
                    fechaEntrega = NuevoPedido.FechaEntrega,
                    estado = NuevoPedido.Estado,
                    items = new[]
                    {
                        new
                        {
                            productoId = NuevoPedido.ProductoId,
                            cantidad = NuevoPedido.Cantidad,
                            detallePersonalizado = NuevoPedido.DetallePersonalizado ?? string.Empty
                        }
                    }
                };

                var json = JsonSerializer.Serialize(pedidoParaApi);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/pedidos", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }

                var errorContenido = await response.Content.ReadAsStringAsync();
                MensajeError = $"No se pudo registrar el pedido: {errorContenido}";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al conectar con la API: {ex.Message}";
            }

            await CargarPedidosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                await client.DeleteAsync($"api/pedidos/{id}");
            }
            catch (Exception)
            {
                // Manejo de error de red al eliminar
            }
            return RedirectToPage();
        }

        private async Task CargarPedidosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                var response = await client.GetAsync("api/pedidos");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // Deserialización flexible ignorando mayúsculas/minúsculas
                    var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    ListaPedidos = JsonSerializer.Deserialize<List<PedidoViewModel>>(jsonString, opciones) ?? new();
                }
            }
            catch (Exception)
            {
                MensajeError = "No se pudieron cargar los pedidos de la base de datos.";
            }
        }
    }
}