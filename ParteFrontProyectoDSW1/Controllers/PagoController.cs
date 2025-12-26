using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Helpers;

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
        public async Task<IActionResult> Registrar(int idOrden)
        {
            // Si no se recibe idOrden, buscar la última orden pendiente del usuario
            if (idOrden <= 0)
            {
                var user = HttpContext.Session.GetObject<UsuarioLoginDto>("CurrentUser");
                if (user == null || user.IdUsuario <= 0)
                    return RedirectToAction("Login", "Account");

                var client = _httpFactory.CreateClient("ApiWeb");
                var ordenes = await client.GetFromJsonAsync<List<Orden>>(
                    $"api/orden/por-usuario/{user.IdUsuario}"
                );

                var ultimaPendiente = ordenes?
                    .Where(o => o.Estado == "Pendiente" || o.Estado == "PENDIENTE")
                    .OrderByDescending(o => o.FechaOrden)
                    .FirstOrDefault();

                if (ultimaPendiente == null)
                    return RedirectToAction("Index", "Orders");

                idOrden = ultimaPendiente.IdOrden;
            }

            var client2 = _httpFactory.CreateClient("ApiWeb");
            var productos = await client2.GetFromJsonAsync<List<PagoDetalladoDto>>(
                $"api/orden/detalle/{idOrden}"
            );

            if (productos == null || !productos.Any())
                return RedirectToAction("Index", "Orders");

            ViewBag.IdOrden = idOrden;
            return View(productos);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(int idOrden, decimal monto, string metodo)
        {
            if (idOrden <= 0)
                return RedirectToAction("Index", "Productos");

            var client = _httpFactory.CreateClient("ApiWeb");
            var resp = await client.PostAsync(
                $"api/pago/registrar?idOrden={idOrden}&monto={monto}&metodo={metodo}",
                null
            );

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al registrar el pago";
                return RedirectToAction(nameof(Registrar), new { idOrden });
            }

            // Mensaje de éxito
            TempData["Success"] = "Tu pago se ha registrado correctamente.";
            return RedirectToAction("Index", "Productos"); // redirige al Index de Productos
        }
    }
}