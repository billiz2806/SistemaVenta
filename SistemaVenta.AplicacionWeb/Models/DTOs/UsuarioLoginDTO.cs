namespace SistemaVenta.AplicacionWeb.Models.DTOs
{
    public class UsuarioLoginDTO
    {
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public bool MantenerSesion { get; set; }
    }
}
