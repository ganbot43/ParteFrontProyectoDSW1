using Microsoft.AspNetCore.Authentication.Cookies; // Agrega este namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. CONFIGURAR AUTENTICACIÓN POR COOKIES (Añade esto)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Productos/Index"; // Si un usuario normal intenta entrar a Admin
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpClient para consumir la API backend
var apiBase = builder.Configuration["ApiSettings:ApiBaseUrl"]?.TrimEnd('/') ?? "https://localhost:7011";
builder.Services.AddHttpClient("ApiWeb", client =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront",
        policy => policy
            .WithOrigins("https://localhost:7295")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

// 2. EL ORDEN ES VITAL: Authentication debe ir antes de Authorization
app.UseAuthentication(); // <-- AGREGAR ESTO
app.UseAuthorization();

app.UseCors("AllowFront");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();