using SistemaVenta.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesService : IUtilidadesService
    {
        public string GenerarClave()
        {
            string clave = Guid.NewGuid().ToString("N").Substring(0,6);
            return clave;
        }
        public string ConvertirSha256(string texto)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding encode = Encoding.UTF8;
                byte[] result = hash.ComputeHash(encode.GetBytes(texto));

                foreach (byte item in result)
                {
                    stringBuilder.Append(item.ToString("x2"));
                }
            }
            return stringBuilder.ToString();
        }

    }
}
