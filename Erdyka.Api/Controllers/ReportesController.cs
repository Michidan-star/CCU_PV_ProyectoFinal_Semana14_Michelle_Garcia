using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Erdyka.Api.Services;

namespace Erdyka.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly PdfReportService _pdfReportService;

        public ReportesController()
        {
            _pdfReportService = new PdfReportService();
        }

        [HttpGet("pdf")]
        public IActionResult DescargarPdf([FromQuery] string? filtro)
        {
            try
            {
                byte[] pdfBytes = _pdfReportService.GenerarReporte(filtro);

                // Retorna el archivo PDF como un FileResult tal como exige la rúbrica
                return File(pdfBytes, "application/pdf", $"ReporteErdyka_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al generar el reporte en PDF", detalle = ex.Message });
            }
        }
    }
}