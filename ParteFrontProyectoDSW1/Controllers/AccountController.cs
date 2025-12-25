using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Helpers;

namespace ParteFrontProyectoDSW1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        private const string SessionUserKey = "CurrentUser";

        public AccountController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpFactory.CreateClient("ApiWeb");
            try
            {
                var resp = await client.PostAsJsonAsync("api/usuarios/login", model);
                var txt = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {txt}");
                    return View(model);
                }

                var loginApiResp = await resp.Content.ReadFromJsonAsync<LoginApiResponseDto>();
                if (loginApiResp == null || loginApiResp.Usuario == null)
                {
                    ModelState.AddModelError(string.Empty, "Respuesta inesperada del servidor.");
                    return View(model);
                }

                HttpContext.Session.SetObject("CurrentUser", loginApiResp.Usuario);
                Console.WriteLine($"[DEBUG] Usuario guardado en sesión: {loginApiResp.Usuario.IdUsuario}");
                return RedirectToAction("Index", "Productos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Excepción: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new UsuarioLoginDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsuarioLoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpFactory.CreateClient("ApiWeb");
            try
            {
                var resp = await client.PostAsJsonAsync("api/usuarios/insertar", model);
                var txt = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {txt}");
                    return View(model);
                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Excepción: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.RemoveObject(SessionUserKey);
            return RedirectToAction("Index", "Productos");
        }

        public LoginResponseDto? CurrentUser => HttpContext.Session.GetObject<LoginResponseDto>(SessionUserKey);
    }
}