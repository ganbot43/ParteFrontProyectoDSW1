using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Dtos;

namespace ParteFrontProyectoDSW1.Controllers
{
    public class PagoController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public PagoController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // 🟢 PANTALLA DE PAGO
        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            var idOrden = HttpContext.Session.GetInt32("ID_ORDEN");

            if (idOrden == null || idOrden <= 0)
                return RedirectToAction("Index", "Orders");

            var client = _httpFactory.CreateClient("ApiWeb");

            var orden = await client.GetFromJsonAsync<List<PagoDetalladoDto>>(
                $"api/orden/detalle/{idOrden}"
            );

            if (orden == null || !orden.Any())
                return RedirectToAction("Index", "Orders");

            ViewBag.IdOrden = idOrden;
            return View(orden);
        }

        // 🟢 REGISTRA EL PAGO
        [HttpPost]
        public async Task<IActionResult> Registrar(decimal monto, string metodo)
        {
            var idOrden = HttpContext.Session.GetInt32("ID_ORDEN");

            if (idOrden == null || idOrden <= 0)
                return RedirectToAction("Index", "Orders");

            var client = _httpFactory.CreateClient("ApiWeb");

            var resp = await client.PostAsync(
                $"api/pago/registrar?idOrden={idOrden}&monto={monto}&metodo={metodo}",
                null
            );

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al registrar el pago";
                return RedirectToAction(nameof(Registrar));
            }

            // Limpio sesión
            HttpContext.Session.Remove("ID_ORDEN");

            return RedirectToAction("Index", "Orders");
        }
    }

}
