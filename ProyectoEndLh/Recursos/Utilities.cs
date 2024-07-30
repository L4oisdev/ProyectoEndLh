using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ProyectoEndLh.Recursos
{
    public class Utilities
    {
        public static string Encriptar(string clave)
        {
            StringBuilder hashCreate = new StringBuilder();

            using (SHA256 has = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;

                byte[] result = has.ComputeHash(enc.GetBytes(clave));

                foreach (byte b in result) hashCreate.Append(b.ToString("x2"));

                return hashCreate.ToString();   
            }
        }
    }
}
