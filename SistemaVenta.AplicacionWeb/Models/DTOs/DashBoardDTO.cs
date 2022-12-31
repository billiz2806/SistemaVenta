namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class DashBoardDTO
    {
        public int TotalVentas { get; set; }
        public string? TotalIngresos { get; set; }
        public int TotalProductos { get; set; }
        public int TotalCategorias { get; set; }

        public List<VentasSemanaDTO> VentasUltimaSemana { get; set; }
        public List<ProductoSemanaDTO> ProductosTopUltimaSemana { get; set; }



    }
}
