using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class TipoDocumentoVentaService :ITipoDocumentoVentaService
    {
        private readonly IGenericRepository<TipoDocumentoVenta> _repository;

        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repository)
        {
            _repository = repository;
        }

        public async Task<List<TipoDocumentoVenta>> Lista()
        {
           IQueryable<TipoDocumentoVenta> query = await _repository.Consultar();
            return query.ToList();
        }
    }
}
