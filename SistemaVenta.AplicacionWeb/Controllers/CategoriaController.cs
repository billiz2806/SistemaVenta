using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaService categoriaService, IMapper mapper)
        {
            _categoriaService = categoriaService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<CategoriaDTO> categoriaDTOLista = _mapper.Map<List<CategoriaDTO>>(await _categoriaService.Lista());
            return StatusCode(StatusCodes.Status200OK, new { data = categoriaDTOLista }); // El DataTable funciona recibiendo un objeto 'data' . 
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CategoriaDTO modelo)
        {
            GenericResponse<CategoriaDTO> response = new GenericResponse<CategoriaDTO>();
            try
            {
                Categoria categoria_creada = await _categoriaService.Crear(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<CategoriaDTO>(categoria_creada);

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] CategoriaDTO modelo)
        {
            GenericResponse<CategoriaDTO> response = new GenericResponse<CategoriaDTO>();
            try
            {
                Categoria categoria_editada = await _categoriaService.Editar(_mapper.Map<Categoria>(modelo));
                modelo = _mapper.Map<CategoriaDTO>(categoria_editada);

                response.Estado = true;
                response.Objeto = modelo;

            }
            catch (Exception ex)
            {
                response.Estado = false;
                response.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, response);

        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCategoria)
        {
            GenericResponse<string> response = new GenericResponse<string>();
            try
            {
                response.Estado = await _categoriaService.Eliminar(idCategoria);
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
