using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class NegocioController : Controller
    {

        private readonly IMapper _mapper;
        private readonly INegocioService _negcioService;

        public NegocioController(IMapper mapper, INegocioService negcioService)
        {
            _mapper = mapper;
            _negcioService = negcioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            GenericResponse<NegocioDTO> response = new GenericResponse<NegocioDTO>();

            try
            {
                NegocioDTO negocioDTO = _mapper.Map<NegocioDTO>(await _negcioService.Obtener());
                response.Estado = true;
                response.Objeto = negocioDTO;
            }
            catch (Exception ex)
            {

                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarCambios([FromForm]IFormFile logo, [FromForm]string modelo)
        {
            GenericResponse<NegocioDTO> response = new GenericResponse<NegocioDTO>();

            try
            {
                NegocioDTO negocioDTO = JsonConvert.DeserializeObject<NegocioDTO>(modelo);
                string nombreLogo = "";
                Stream streamLogo = null;

                if(logo != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(logo.FileName);
                    nombreLogo = string.Concat(nombre_en_codigo, extension);
                    streamLogo = logo.OpenReadStream();
                }

                Negocio negocio_editado = await _negcioService.GuardarCambios(_mapper.Map<Negocio>(negocioDTO), streamLogo, nombreLogo);

                negocioDTO  = _mapper.Map<NegocioDTO>(negocio_editado);
                response.Estado = true;
                response.Objeto = negocioDTO;
            }
            catch (Exception ex)
            {

                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}
