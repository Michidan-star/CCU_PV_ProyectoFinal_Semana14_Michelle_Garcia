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

        // Listas para mostrar en la interfaz
        public List<PedidoViewModel> ListaPedidos { get; set; } = new();
        public List<ProductoViewModel> ListaProductosDisponibles { get; set; } = new();

        [BindProperty]
        public PedidoViewModel NuevoPedido { get; set; } = new();

        public string MensajeError { get; set; } = string.Empty;
        public string MensajeExito { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            await CargarDatosAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");

                // Estructura que espera el PedidoCrearDto de tu API
                var payloadDto = new
                {
                    nombreCliente = NuevoPedido.NombreCliente,
                    telefonoCliente = NuevoPedido.TelefonoCliente,
                    fechaEntrega = NuevoPedido.FechaEntrega,
                    estado = string.IsNullOrEmpty(NuevoPedido.Estado) ? "Pendiente" : NuevoPedido.Estado,
                    items = new[]
                    {
                        new
                        {
                            productoId = NuevoPedido.ProductoId,
                            cantidad = NuevoPedido.Cantidad <= 0 ? 1 : NuevoPedido.Cantidad,
                            detallePersonalizado = NuevoPedido.DetallePersonalizado ?? string.Empty
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payloadDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("pedidos", content);

                if (response.IsSuccessStatusCode)
                {
                    MensajeExito = "¡Pedido registrado con éxito!";
                    return RedirectToPage();
                }

                var errorContenido = await response.Content.ReadAsStringAsync();
                MensajeError = $"No se pudo registrar el pedido: {errorContenido}";
            }
            catch (Exception ex)
            {
                MensajeError = $"Error de conexión con la API: {ex.Message}";
            }

            await CargarDatosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");
                await client.DeleteAsync($"pedidos/{id}");
            }
            catch (Exception)
            {
                // Manejo de error silencioso o básico de red al borrar
            }
            return RedirectToPage();
        }

        private async Task CargarDatosAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ErdykaApi");

                // 1. Cargar Pedidos
                var responsePedidos = await client.GetAsync("pedidos");
                if (responsePedidos.IsSuccessStatusCode)
                {
                    var jsonString = await responsePedidos.Content.ReadAsStringAsync();
                    ListaPedidos = JsonSerializer.Deserialize<List<PedidoViewModel>>(jsonString, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
                }

                // 2. Cargar Productos para el selector del formulario
                var responseProductos = await client.GetAsync("productos");
                if (responseProductos.IsSuccessStatusCode)
                {
                    var jsonProductos = await responseProductos.Content.ReadAsStringAsync();
                    ListaProductosDisponibles = JsonSerializer.Deserialize<List<ProductoViewModel>>(jsonProductos, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new();
                }
            }
            catch (Exception)
            {
                MensajeError = "No se pudieron cargar los datos necesarios.";
            }
        }
    }
}