using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMenuService _menuServicio;
        private readonly IMapper _mapper;

        public MenuViewComponent(IMenuService menuServicio, IMapper mapper)
        {
            _menuServicio = menuServicio;
            _mapper = mapper;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            List<MenuDTO> listaMenus;
            if (claimUser.Identity.IsAuthenticated)
            {
                string idUsuario = claimUser.Claims
                     .Where(c => c.Type == ClaimTypes.NameIdentifier)
                     .Select(c => c.Value).SingleOrDefault();

                listaMenus = _mapper.Map<List<MenuDTO>>(await _menuServicio.ObtenerMenu(int.Parse(idUsuario)));
            }
            else
            {
                listaMenus = new List<MenuDTO>();
            }

            return View(listaMenus);

        }

    }
}
