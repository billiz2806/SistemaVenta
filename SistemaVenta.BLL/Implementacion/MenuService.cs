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
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<Menu> _repositorioMenu;
        private readonly IGenericRepository<RolMenu> _repositorioRolMenu;
        private readonly IGenericRepository<Usuario> _repositorioUsuario;

        public MenuService(IGenericRepository<Menu> repositorioMenu, IGenericRepository<RolMenu> repositorioRolMenu, IGenericRepository<Usuario> repositorioUsuario)
        {
            _repositorioMenu = repositorioMenu;
            _repositorioRolMenu = repositorioRolMenu;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<List<Menu>> ObtenerMenu(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await _repositorioUsuario.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<RolMenu> tbRolMenu = await _repositorioRolMenu.Consultar();
            IQueryable<Menu> tbMenu = await _repositorioMenu.Consultar();

            IQueryable<Menu> MenuPadre = (from u in tbUsuario
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          join mp in tbMenu on m.IdMenuPadre equals mp.IdMenu
                                          select mp).Distinct().AsQueryable();

            IQueryable<Menu> MenuHijo = (from u in tbUsuario
                                          join rm in tbRolMenu on u.IdRol equals rm.IdRol
                                          join m in tbMenu on rm.IdMenu equals m.IdMenu
                                          where m.IdMenu != m.IdMenuPadre
                                         select m).Distinct().AsQueryable();

            List<Menu> listaMenu = (from mPadre in MenuPadre
                                    select new Menu()
                                    {
                                        Descripcion = mPadre.Descripcion,
                                        Icono = mPadre.Icono,
                                        Controlador = mPadre.Controlador,
                                        PaginaAccion = mPadre.PaginaAccion,
                                        InverseIdMenuPadreNavigation = ( from mHijo in MenuHijo
                                                                         where mHijo.IdMenuPadre == mPadre.IdMenu
                                                                         select mHijo).ToList()
                                    }).ToList();

            return listaMenu;
        }
    }
}
