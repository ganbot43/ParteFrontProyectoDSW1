using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Contracts.Entities;
using System.Net.Http.Json;

namespace ParteFrontProyectoDSW1.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public AdminController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public async Task<IActionResult> Pagos()
        {
            try
            {
                var client = _httpFactory.CreateClient("ApiWeb");

                var pagosTask = client.GetFromJsonAsync<List<Pago>>("api/Pago");
                var detallesTask = client.GetFromJsonAsync<List<PagoDetalladoDto>>("api/Pago/detallado");

                await Task.WhenAll(pagosTask, detallesTask);

                var pagos = await pagosTask ?? new List<Pago>();
                var detalles = await detallesTask ?? new List<PagoDetalladoDto>();

                var productoEstrella = detalles
                    .GroupBy(d => d.Producto)
                    .OrderByDescending(g => g.Sum(x => x.Cantidad))
                    .Select(g => g.Key)
                    .FirstOrDefault();

                ViewBag.ProductoMasVendido = productoEstrella ?? "Sin ventas";

                return View(pagos);
            }
            catch (Exception)
            {
                ViewBag.ProductoMasVendido = "Error al cargar";
                return View(new List<Pago>());
            }
        }

        public async Task<IActionResult> PagosDetallado()
        {
            try
            {
                var client = _httpFactory.CreateClient("ApiWeb");
                var pagos = await client.GetFromJsonAsync<List<PagoDetalladoDto>>("api/Pago/detallado");
                return View(pagos);
            }
            catch (Exception)
            {
                return View(new List<PagoDetalladoDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetallePago(int idPago)
        {
            try
            {
                var client = _httpFactory.CreateClient("ApiWeb");
                var detalles = await client.GetFromJsonAsync<List<PagoDetalladoDto>>("api/Pago/detallado");
                var detallesPago = detalles?.Where(x => x.IdPago == idPago).ToList();

                if (detallesPago == null || !detallesPago.Any())
                    return NotFound();

                return Json(detallesPago);
            }
            catch (Exception)
            {
                return StatusCode(500, "No se pudo cargar el detalle.");
            }
        }
    }
}