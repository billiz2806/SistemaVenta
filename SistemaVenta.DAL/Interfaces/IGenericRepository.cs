using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Interfaces
{
    public interface IGenericRepository<TEntiry> where TEntiry : class
    {
        Task<TEntiry> Obtener(Expression<Func<TEntiry,bool>> filtro);
        Task<TEntiry> Crear(TEntiry entidad);
        Task<bool> Editar(TEntiry entidad);
        Task<bool> Eliminar(TEntiry entidad);
        Task<IQueryable<TEntiry>> Consultar(Expression<Func<TEntiry, bool>> filtro= null);
    }
}
