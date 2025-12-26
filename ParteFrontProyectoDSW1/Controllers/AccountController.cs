using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using ParteFrontProyectoDSW1.Contracts.Dtos;
using ParteFrontProyectoDSW1.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        // =====================
        // LOGIN
        // =====================
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
                var resp = await client.PostAsJsonAsync("api/Usuarios/login", model);
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

                // Establecer la información del usuario en la cookie de autenticación
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginApiResp.Usuario.Nombre),
                    new Claim(ClaimTypes.Role, loginApiResp.Usuario.Rol) // Aquí el rol, por ejemplo "ADMIN"
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                HttpContext.Session.SetObject(SessionUserKey, loginApiResp.Usuario);

                // Redirección según el rol
                if (loginApiResp.Usuario.Rol == "ADMIN")
                    return RedirectToAction("Pagos", "Admin");
                else
                    return RedirectToAction("Index", "Productos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Excepción: {ex.Message}");
                return View(model);
            }
        }

        // =====================
        // REGISTER
        // =====================
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
                // ⚡ Ruta correcta de tu API para insertar
                var resp = await client.PostAsJsonAsync("api/Usuarios", model);
                var txt = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, $"Error: {txt}");
                    return View(model);
                }

                // Redirige al login después de registrar
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Excepción: {ex.Message}");
                return View(model);
            }
        }

        // =====================
        // LOGOUT
        // =====================
        public IActionResult Logout()
        {
            HttpContext.Session.RemoveObject(SessionUserKey);
            return RedirectToAction("Index", "Productos");
        }

        // =====================
        // PROPIEDAD PARA USUARIO ACTUAL
        // =====================
        public LoginResponseDto? CurrentUser => HttpContext.Session.GetObject<LoginResponseDto>(SessionUserKey);
    }
}
