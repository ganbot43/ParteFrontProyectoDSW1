using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Entities;
using ParteFrontProyectoDSW1.Contracts.Dtos;

namespace ParteFrontProyectoDSW1.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public ProductosController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        public async Task<IActionResult> Index(int? idCategoria = null, string search = "")
        {
            var client = _httpFactory.CreateClient("ApiWeb");

            IEnumerable<Producto>? productos = Enumerable.Empty<Producto>();

            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    productos = await client.GetFromJsonAsync<IEnumerable<Producto>>(
                        $"api/productos/buscar?nombre={Uri.EscapeDataString(search)}"
                    );
                }
                else if (idCategoria.HasValue)
                {
                    productos = await client.GetFromJsonAsync<IEnumerable<Producto>>(
                        $"api/productos/por-categoria/{idCategoria.Value}"
                    );
                }
                else
                {

                    productos = await client.GetFromJsonAsync<IEnumerable<Producto>>("api/productos");
                }

                var categorias = await client.GetFromJsonAsync<IEnumerable<Categoria>>("api/categorias") ?? Enumerable.Empty<Categoria>();
                ViewBag.Categorias = categorias;
            }
            catch
            {
                TempData["Error"] = "No se pudieron cargar los productos.";
            }

            return View(productos ?? Enumerable.Empty<Producto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = _httpFactory.CreateClient("ApiWeb");
            var producto = await client.GetFromJsonAsync<Producto>($"api/productos/{id}");
            if (producto == null) return NotFound();
            return View(producto);
        }


        [HttpGet]
        public IActionResult UploadImage(int id)
        {
            ViewBag.ProductId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un archivo");
                ViewBag.ProductId = id;
                return View();
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError(string.Empty, "Formato no permitido");
                ViewBag.ProductId = id;
                return View();
            }
            if (file.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(string.Empty, "Archivo excede 5MB");
                ViewBag.ProductId = id;
                return View();
            }

            var client = _httpFactory.CreateClient("ApiWeb");
            using var content = new MultipartFormDataContent();
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;
            var fileContent = new StreamContent(ms);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
            content.Add(fileContent, "file", file.FileName);

            var resp = await client.PostAsync($"api/productos/{id}/upload-image", content);
            if (!resp.IsSuccessStatusCode)
            {
                var txt = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Error subiendo imagen: " + txt);
                ViewBag.ProductId = id;
                return View();
            }

            // Suponiendo que el API devuelve el nombre final del archivo
            var fileName = file.FileName; // O genera un nombre único si es necesario
            var relativePath = $"/images/productos/{fileName}";
            // Guarda 'relativePath' en el campo ImagenUrl del producto en la BD si es necesario

            return RedirectToAction("Details", new { id });
        }
    }
}
