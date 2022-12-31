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
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductoController(IMapper mapper, IProductoService productoService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<ProductoDTO> productoDTOLista = _mapper.Map<List<ProductoDTO>>(await _productoService.Listar());
            return StatusCode(StatusCodes.Status200OK, new { data = productoDTOLista }); // El DataTable funciona recibiendo un objeto 'data' . 
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen,[FromForm]string modelo)
        {
            GenericResponse<ProductoDTO> response = new GenericResponse<ProductoDTO>();
            try
            {
                ProductoDTO productoDto = JsonConvert.DeserializeObject<ProductoDTO>(modelo);
                string nombreImagen = "";
                Stream streamImagen = null;

                if (imagen != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombre_en_codigo,extension);
                    streamImagen = imagen.OpenReadStream();
                }

                Producto producto_creado = await _productoService.Crear(_mapper.Map<Producto>(productoDto), streamImagen, nombreImagen);

                productoDto = _mapper.Map<ProductoDTO>(producto_creado);
                response.Estado = true;
                response.Objeto = productoDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string modelo)
        {
            GenericResponse<ProductoDTO> response = new GenericResponse<ProductoDTO>();
            try
            {
                ProductoDTO productoDto = JsonConvert.DeserializeObject<ProductoDTO>(modelo);
                Stream streamImagen = null;

                if (imagen != null)
                {
                    streamImagen = imagen.OpenReadStream();
                }

                Producto producto_editado = await _productoService.Editar(_mapper.Map<Producto>(productoDto), streamImagen);

                productoDto = _mapper.Map<ProductoDTO>(producto_editado);
                response.Estado = true;
                response.Objeto = productoDto;
            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _productoService.Eliminar(idProducto);
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
