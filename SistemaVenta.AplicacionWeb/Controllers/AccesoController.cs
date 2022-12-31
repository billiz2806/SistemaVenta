using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly IUsuarioService _usuarioServicio;
        private readonly INegocioService _negocioService;
        public AccesoController(IUsuarioService usuarioServicio, INegocioService negocioService)
        {
            _usuarioServicio = usuarioServicio;
            _negocioService = negocioService;
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDTO modelo)
        {
            Usuario usuario_encontrado = await _usuarioServicio.ObtenerPorCredenciales(modelo.Correo, modelo.Clave);
            Negocio negocio = await _negocioService.Obtener();

            if(usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "Usuario o clave incorrecta, no se encontraron considencias";
                return View();
            }
            else
            {

                ViewData["Mensaje"] = null;

                //Creando lista de reclamación 
                List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role, usuario_encontrado.IdRol.ToString()),
                new Claim("UrlFoto", usuario_encontrado.UrlFoto),
                new Claim("urlLogo", negocio.UrlLogo),
            };

                //registrando los claims a la autenticacion por coockies
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                //configurar propiedades a la auntenticacion
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true, //refescar la auntenticacion
                    IsPersistent = modelo.MantenerSesion,  //persistir la auntenticacion
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties
                    );

                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult RestablecerContraseña()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestablecerContraseña(UsuarioLoginDTO modelo)
        {
            try
            {
                string urlPLantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/RestablecerClave?clave=[clave]";
                bool resultado = await _usuarioServicio.RestablecerClave(modelo.Correo, urlPLantillaCorreo);
                if (resultado)
                {
                    ViewData["Mensaje"] = "Listo, su contraseña fue reestablecida, revise su correo";
                    ViewData["MensajeError"] = "";
                }
                else
                {
                    ViewData["Mensaje"] = "";
                    ViewData["MensajeError"] = "Tenemos problemas, inténtelo de nuevo más tarde";
                }
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "";
                ViewData["MensajeError"] = ex.Message;
            }

            return View();
        }
    }
}
