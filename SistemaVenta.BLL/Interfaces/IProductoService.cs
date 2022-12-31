using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IProductoService
    {
        Task<List<Producto>> Listar();
        Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "" );
        Task<Producto> Editar(Producto entidad, Stream imagen = null);
        Task<bool> Eliminar(int idProducto);

    }
}
