using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Erdyka.Api.Services
{
    public class PdfReportService
    {
        public byte[] GenerarReporte(string? filtro)
        {
            // Licencia comunitaria gratuita requerida por QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("Sistema Erdyka - Reporte de Gestión")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}");
                            column.Item().Text($"Filtro aplicado: {(string.IsNullOrEmpty(filtro) ? "Ninguno (General)" : filtro)}");
                            column.Item().Text("Este reporte PDF ha sido generado dinámicamente cumpliendo con el componente de investigación del proyecto final.");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}