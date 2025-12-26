using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Contracts.Entities;
using System.Net.Http.Json;

namespace ParteFrontProyectoDSW1.Controllers
{
    // Solo permite el acceso si el usuario está autenticado con el rol ADMIN
    [Authorize(Roles = "ADMIN")]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public AdminController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // Vista de lista de pagos (Diseño de tabla limpia)
        public async Task<IActionResult> Pagos()
        {
            try
            {
                var client = _httpFactory.CreateClient("ApiWeb");
                var pagos = await client.GetFromJsonAsync<List<Pago>>("api/Pago");
                return View(pagos);
            }
            catch (Exception)
            {
                return View(new List<Pago>());
            }
        }

        // Vista Detallada de Pagos (Recomendada para gestión)
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

        // Obtener detalle por AJAX para Modal
        [HttpGet]
        public async Task<IActionResult> DetallePago(int idPago)
        {
            try
            {
                var client = _httpFactory.CreateClient("ApiWeb");
                // Obtén todos los pagos detallados
                var detalles = await client.GetFromJsonAsync<List<PagoDetalladoDto>>("api/Pago/detallado");
                // Busca el pago por idPago
                var detalle = detalles?.FirstOrDefault(x => x.IdPago == idPago);
                if (detalle == null)
                    return NotFound();
                return Json(detalle);
            }
            catch (Exception)
            {
                return StatusCode(500, "No se pudo cargar el detalle.");
            }
        }
    }
}