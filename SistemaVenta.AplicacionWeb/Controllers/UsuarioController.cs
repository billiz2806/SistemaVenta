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
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly IMapper _mapper;

        public UsuarioController(IUsuarioService usuarioService, IRolService rolService, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _rolService = rolService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaRoles()
        {
            var lista = await _rolService.Lista();
            List<RolDTO> listaRolesDto = _mapper.Map<List<RolDTO>>(lista);

            return StatusCode(StatusCodes.Status200OK, listaRolesDto);
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var lista = await _usuarioService.Lista();
            List<UsuarioDTO> listaUsuariosDto = _mapper.Map<List<UsuarioDTO>>(lista);

            return StatusCode(StatusCodes.Status200OK,new {data = listaUsuariosDto });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo )
        {
            GenericResponse<UsuarioDTO> response = new GenericResponse<UsuarioDTO>();
            try
            {
                UsuarioDTO dtoUsuario = JsonConvert.DeserializeObject<UsuarioDTO>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto!= null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                string urlPlantillaCorreo = $"{this.Request.Scheme}://{this.Request.Host}/Plantilla/EnviarClave?correo=[correo]&clave=[clave]";
                Usuario usuario_creado = await _usuarioService.Crear(_mapper.Map<Usuario>(dtoUsuario), fotoStream, nombreFoto, urlPlantillaCorreo);

                dtoUsuario = _mapper.Map<UsuarioDTO>(usuario_creado);

                response.Estado = true;
                response.Objeto = dtoUsuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<UsuarioDTO> response = new GenericResponse<UsuarioDTO>();
            try
            {
                UsuarioDTO dtoUsuario = JsonConvert.DeserializeObject<UsuarioDTO>(modelo);
                string nombreFoto = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreFoto = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Usuario usuario_editado = await _usuarioService.Editar(_mapper.Map<Usuario>(dtoUsuario), fotoStream, nombreFoto);
                dtoUsuario = _mapper.Map<UsuarioDTO>(usuario_editado);
                response.Estado = true;
                response.Objeto = dtoUsuario;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _usuarioService.Eliminar(idUsuario);
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
