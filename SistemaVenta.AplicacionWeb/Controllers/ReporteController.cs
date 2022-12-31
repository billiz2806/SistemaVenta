using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVentaService _ventaService;

        public ReporteController(IMapper mapper, IVentaService ventaService)
        {
            _mapper = mapper;
            _ventaService = ventaService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ReporteVenta(string fechaInicio , string fechaFin)
        {
            var lista = await _ventaService.Reporte(fechaInicio, fechaFin);
            List<ReporteVentaDTO> listaReportes = _mapper.Map<List<ReporteVentaDTO>>(lista);
            return StatusCode(StatusCodes.Status200OK,new { data = listaReportes } );
        }
    }
}
