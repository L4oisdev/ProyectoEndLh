using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProyectoEndLh.Models;
using ProyectoEndLh.Servicios.Contrato;

namespace ProyectoEndLh.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly WebAppDbContext _webAppDbContext;

        public UsuarioService(WebAppDbContext webAppDbContext)
        {
            _webAppDbContext = webAppDbContext;
        }

        public async Task<Usuario> GetUsuarios(string correo, string clave)
        {
            Usuario userFound = await _webAppDbContext.Usuarios.Where(u => u.Correo == correo && u.Clave == clave)
                .FirstOrDefaultAsync();

            return userFound;
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _webAppDbContext.Usuarios.Add(modelo);

            await _webAppDbContext.SaveChangesAsync();  

            return modelo;
        }
    }
}