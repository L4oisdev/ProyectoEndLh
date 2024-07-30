using ProyectoEndLh.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoEndLh.Servicios.Contrato;
using ProyectoEndLh.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Inyección de dependencias
builder.Services.AddDbContext<WebAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebContext"));
});

builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Inicio/IniciarSesion";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

// Configura los servicios para usar controladores con vistas y agrega una configuración específica de caché para las respuestas
builder.Services.AddControllersWithViews(options =>
{
    // Agrega un filtro global de caché de respuesta con las siguientes configuraciones
    options.Filters.Add(
         new ResponseCacheAttribute
         {
             NoStore = true,
             Location = ResponseCacheLocation.None,
         });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//Cookie Authentication
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=IniciarSesion}/{id?}");

app.Run();
