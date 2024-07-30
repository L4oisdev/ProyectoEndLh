using Microsoft.AspNetCore.Mvc;

using ProyectoEndLh.Models;
using ProyectoEndLh.Recursos;
using ProyectoEndLh.Servicios.Contrato;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoEndLh.Servicios.Implementacion;

namespace ProyectoEndLh.Controllers
{
    public class InicioController : Controller
    {
        // Inyección de dependencia del servicio de usuario
        private readonly IUsuarioService _usuarioService;

        // Constructor que recibe el servicio de usuario
        public InicioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }


        public IActionResult Registrarse()
        {
            return View();
        }

        // Acción que maneja el envío del formulario de registro
        [HttpPost]
        public async Task <IActionResult> Registrarse(Usuario modelo)
        {
            modelo.Clave = Utilities.Encriptar(modelo.Clave);

            Usuario userCreate = await _usuarioService.SaveUsuario(modelo);

            // Verifica si el usuario fue creado exitosamente
            if (userCreate.IdUsuario > 0) return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }
        
        public IActionResult IniciarSesion()
        {
            return View();
        }

        // Acción que maneja el envío del formulario de inicio de sesión
        [HttpPost]
        public async Task <IActionResult> IniciarSesion( string correo, string clave)
        {
            // Busca al usuario en la base de datos por correo y contraseña encriptada
            Usuario userFound = await _usuarioService.GetUsuarios(correo, Utilities.Encriptar(clave));

            if (userFound == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            // Crea una lista de 'claims' con la información del usuario
            List<Claim> claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, userFound.NombreUsuario)
            };

            // Crea una identidad de 'claims' usando el esquema de autenticación de cookies
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() 
            { 
                AllowRefresh = true
            };

            // Firma al usuario en la aplicación usando el esquema de autenticación de cookies
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "Home");
        }
    }
}
