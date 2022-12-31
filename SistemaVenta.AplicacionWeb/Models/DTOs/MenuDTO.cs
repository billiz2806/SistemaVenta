using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class MenuDTO
    {
        public string? Descripcion { get; set; }

        public string? Icono { get; set; }

        public string? Controlador { get; set; }

        public string? PaginaAccion { get; set; }

        public virtual ICollection<MenuDTO> SubMenus { get; set; }


    }
}
