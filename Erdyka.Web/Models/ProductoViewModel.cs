namespace Erdyka.Web.Models
{
    public class ProductoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int StockActual { get; set; }
        // Propiedad de compatibilidad usada en la vista (Index.cshtml) como "Stock"
        public int Stock
        {
            get => StockActual;
            set => StockActual = value;
        }
    }
}