using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Helpers;

namespace ParteFrontProyectoDSW1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private const string SessionUserKey = "CurrentUser";

        public OrdersController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public IActionResult Index()
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PagoHistorialAjax()
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null)
                return Json(new { success = false, message = "Sesión expirada" });

            var client = _httpFactory.CreateClient("ApiWeb");

            try
            {
                var pagos = await client.GetFromJsonAsync<List<PagoDetalladoDto>>(
                    $"api/Pago/usuario/{user.IdUsuario}/historial");

                return Json(new { success = true, data = pagos });
            }
            catch
            {
                return Json(new { success = false, message = "Error al obtener pagos" });
            }
        }
    }
}
