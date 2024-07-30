using Microsoft.EntityFrameworkCore;
using ProyectoEndLh.Models;

namespace ProyectoEndLh.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuarios(string correo, string clave);

        Task<Usuario> SaveUsuario(Usuario modelo);
    }
}
