using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Helpers;

namespace ParteFrontProyectoDSW1.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private const string SessionUserKey = "CurrentUser";

        public CartController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        // GET: show active cart
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return RedirectToAction("Login", "Account");

            var client = _httpFactory.CreateClient("ApiWeb");
            int idCarrito = 0;
            IEnumerable<CarritoDetalle> detalles = Enumerable.Empty<CarritoDetalle>();

            try
            {
                // Obtener carrito activo
                idCarrito = await client.GetFromJsonAsync<int>($"api/carrito/obtener-activo/{user.IdUsuario}");
                HttpContext.Session.SetInt32("IdCarrito", idCarrito);

                // Obtener los detalles del carrito
                detalles = await client.GetFromJsonAsync<IEnumerable<CarritoDetalle>>($"api/carrito/ver/{idCarrito}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el carrito: {ex.Message}";
            }

            var model = new CarritoViewModel
            {
                IdCarrito = idCarrito,
                Items = detalles ?? Enumerable.Empty<CarritoDetalle>()
            };

            return View(model);
        }

        // POST: add/update/remove product in cart (AJAX y normal)
        [HttpPost]
        public async Task<IActionResult> Add(int idProducto, int cantidad = 1)
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return RedirectToAction("Login", "Account");

            var idCarrito = HttpContext.Session.GetInt32("IdCarrito") ?? 0;
            if (idCarrito == 0)
            {
                TempData["Error"] = "No se pudo obtener el carrito activo.";
                return RedirectToAction("Index");
            }

            var client = _httpFactory.CreateClient("ApiWeb");
            var resp = await client.PostAsync($"api/carrito/agregar-producto?idCarrito={idCarrito}&idProducto={idProducto}&cantidad={cantidad}", null);
            var txt = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Error API: {txt}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddAjax(int idProducto, int cantidad = 1)
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return Json(new { success = false, total = 0 });

            var idCarrito = HttpContext.Session.GetInt32("IdCarrito") ?? 0;
            if (idCarrito == 0)
                return Json(new { success = false, total = 0 });

            var client = _httpFactory.CreateClient("ApiWeb");
            var resp = await client.PostAsync(
                $"api/carrito/agregar-producto?idCarrito={idCarrito}&idProducto={idProducto}&cantidad={cantidad}", null);

            int total = 0;
            if (resp.IsSuccessStatusCode)
            {
                var detalles = await client.GetFromJsonAsync<IEnumerable<CarritoDetalle>>($"api/carrito/ver/{idCarrito}");
                total = detalles?.Sum(x => x.Cantidad) ?? 0;
            }

            return Json(new { success = resp.IsSuccessStatusCode, total });
        }

        // GET: get cart count (para badge del layout)
        [HttpGet]
        public async Task<IActionResult> TotalItems()
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return Content("0");

            var client = _httpFactory.CreateClient("ApiWeb");
            int idCarrito = 0;
            try
            {
                idCarrito = await client.GetFromJsonAsync<int>($"api/carrito/obtener-activo/{user.IdUsuario}");
                var detalles = await client.GetFromJsonAsync<IEnumerable<CarritoDetalle>>($"api/carrito/ver/{idCarrito}");
                int count = detalles?.Sum(x => x.Cantidad) ?? 0;
                return Content(count.ToString());
            }
            catch
            {
                return Content("0");
            }
        }

        // POST: checkout/generate order y redirige a pago
        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return RedirectToAction("Login", "Account");

            var client = _httpFactory.CreateClient("ApiWeb");
            var resp = await client.PostAsync(
                $"api/orden/generar?idUsuario={user.IdUsuario}",
                null
            );

            if (!resp.IsSuccessStatusCode)
            {
                TempData["ErrorCheckout"] = $"Error generando la orden: {resp.StatusCode}";
                return RedirectToAction("Index");
            }

            // Buscar la última orden pendiente del usuario
            var ordenes = await client.GetFromJsonAsync<List<Orden>>(
                $"api/orden/por-usuario/{user.IdUsuario}"
            );
            var ultimaPendiente = ordenes?
                .Where(o => o.Estado == "Pendiente" || o.Estado == "PENDIENTE")
                .OrderByDescending(o => o.FechaOrden)
                .FirstOrDefault();

            if (ultimaPendiente == null)
            {
                TempData["ErrorCheckout"] = "No se pudo encontrar la orden generada";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Registrar", "Pago", new { idOrden = ultimaPendiente.IdOrden });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int idProducto)
        {
            var user = HttpContext.Session.GetObject<UsuarioLoginDto>(SessionUserKey);
            if (user == null || user.IdUsuario <= 0)
                return Unauthorized();

            var idCarrito = HttpContext.Session.GetInt32("IdCarrito") ?? 0;
            if (idCarrito == 0)
                return BadRequest("No se pudo obtener el carrito activo.");

            var client = _httpFactory.CreateClient("ApiWeb");
            var resp = await client.PostAsync(
                $"api/carrito/eliminar-producto?idCarrito={idCarrito}&idProducto={idProducto}", null);

            var txt = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                return BadRequest(txt);

            return Ok();
        }

        public class CarritoViewModel
        {
            public int IdCarrito { get; set; }
            public IEnumerable<CarritoDetalle> Items { get; set; } = Enumerable.Empty<CarritoDetalle>();
        }
    }
}