using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Utilidades.ViewComponents
{
    public class LogoViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //contexto del usuario logeado
            ClaimsPrincipal claimUser = HttpContext.User;

            string urlLogo = "";

            if (claimUser.Identity.IsAuthenticated)
            {

                urlLogo = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlLogo").Value;
            }

            ViewData["urlLogo"] = urlLogo;

            return View();
        }
    }
}
