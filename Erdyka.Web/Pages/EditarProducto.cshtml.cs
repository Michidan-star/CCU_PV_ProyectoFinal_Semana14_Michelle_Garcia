using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Erdyka.Web.Models;

namespace Erdyka.Web.Pages
{
    public class EditarProductoModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditarProductoModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ProductoViewModel Producto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("ErdykaApi");
            var response = await client.GetAsync($"api/productos/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Productos");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Producto = JsonSerializer.Deserialize<ProductoViewModel>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient("ErdykaApi");
            var json = JsonSerializer.Serialize(Producto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"api/productos/{Producto.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Productos");
            }

            return Page();
        }
    }
}