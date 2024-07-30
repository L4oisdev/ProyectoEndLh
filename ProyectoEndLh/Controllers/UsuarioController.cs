using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoEndLh.Models;
using Microsoft.AspNetCore.Authorization;
using ProyectoEndLh.Recursos;

namespace ProyectoEndLh.Controllers

{
    // Aplica el atributo [Authorize] para requerir autenticación en todas las acciones del controlador
    [Authorize]

    public class UsuarioController : Controller
    {
        private readonly WebAppDbContext _WebAppDbContext;

        public UsuarioController(WebAppDbContext webAppDbContext)
        {
            _WebAppDbContext = webAppDbContext;
        }

        // Acción que lista todos los usuarios
        public async Task<IActionResult> Lista()
        {
            List<Usuario> usuarios = await _WebAppDbContext.Usuarios.ToListAsync();
            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Nuevo()
        {
            return View();
        }

        // Acción que maneja el envío del formulario para crear un nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Nuevo(Usuario usuario)
        {
            if (!string.IsNullOrEmpty(usuario.Clave))
            {
                usuario.Clave = Utilities.Encriptar(usuario.Clave);
            }
            await _WebAppDbContext.Usuarios.AddAsync(usuario);
            await _WebAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            Usuario usuario = await _WebAppDbContext.Usuarios.FirstAsync(e => e.IdUsuario == id);
            return View(usuario);
        }

        // Acción que maneja el envío del formulario para editar un usuario existente
        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            var existingUser = await _WebAppDbContext.Usuarios.FindAsync(usuario.IdUsuario);
            if (existingUser != null)
            {
                existingUser.NombreUsuario = usuario.NombreUsuario;
                existingUser.Correo = usuario.Correo;

                // Solo actualizar la contraseña si se ha ingresado una nueva
                if (!string.IsNullOrEmpty(usuario.Clave))
                {
                    string newHashedPassword = Utilities.Encriptar(usuario.Clave);
                    // Compara la contraseña ingresada encriptada con la actual
                    if (newHashedPassword != existingUser.Clave)
                    {
                        existingUser.Clave = newHashedPassword;
                    }
                    else
                    {
                        // Manejar el caso donde la nueva contraseña es igual a la actual
                        ModelState.AddModelError(string.Empty, "La nueva contraseña no puede ser igual a la actual.");
                        return View(usuario); // Devuelve la vista de edición con el modelo usuario y los errores de validación
                    }
                }

                _WebAppDbContext.Usuarios.Update(existingUser);
                await _WebAppDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Lista));
            }
            return View(usuario); // Devuelve la vista de edición si el usuario no se encuentra
        }

        // Acción que maneja la eliminación de un usuario
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            Usuario usuario = await _WebAppDbContext.Usuarios.FirstAsync(e => e.IdUsuario == id);

            _WebAppDbContext.Usuarios.Remove(usuario);
            await _WebAppDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Lista));
        }

        // Acción que maneja el cierre de sesión del usuario
        public async Task<IActionResult> CerrarSesion()
        {
            // Cierra la sesión del usuario utilizando el esquema de autenticación de cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("IniciarSesion", "Inicio");
        }
        
    }
}

