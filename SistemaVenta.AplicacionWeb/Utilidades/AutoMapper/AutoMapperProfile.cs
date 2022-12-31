using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.DTOs;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol,RolDTO>().ReverseMap();
            #endregion

            #region Usuario
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true?1:0))
                .ForMember(destino => destino.nombreRol,
                    opt => opt.MapFrom(origen => origen.IdRolNavigation.Descripcion));

            CreateMap<UsuarioDTO, Usuario>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdRolNavigation,
                    opt => opt.Ignore());
            #endregion

            #region Negocio
            CreateMap<Negocio, NegocioDTO>()
                .ForMember(destino => destino.PorcentajeImpuesto,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.PorcentajeImpuesto.Value,new CultureInfo("es-PE"))));

            CreateMap<NegocioDTO , Negocio>()
               .ForMember(destino => destino.PorcentajeImpuesto,
                   opt => opt.MapFrom(origen => Convert.ToDecimal(origen.PorcentajeImpuesto.Value, new CultureInfo("es-PE"))));
            #endregion

            #region Categoria
            CreateMap<Categoria, CategoriaDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)); // convertir boolean en entero

            CreateMap<CategoriaDTO, Categoria>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false));// convertir entero en boolean
            #endregion

            #region Producto
            CreateMap<Producto, ProductoDTO>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0))
                .ForMember(destino => destino.NombreCategoria,
                    opt => opt.MapFrom(origen => origen.IdCategoriaNavigation.Descripcion))
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-PE"))));

            CreateMap<ProductoDTO, Producto>()
                .ForMember(destino => destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false))
                .ForMember(destino => destino.IdCategoriaNavigation,
                    opt => opt.Ignore())
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-PE"))));
            #endregion

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, TipoDocumentoVentaDTO>().ReverseMap();
            #endregion

            #region Venta
            CreateMap<Venta, VentaDTO>()
                .ForMember(destino => destino.TipoDocumentoVenta,
                    opt => opt.MapFrom(origen => origen.IdTipoDocumentoVentaNavigation.Descripcion))
                .ForMember(destino => destino.Usuario,
                    opt => opt.MapFrom(origen => origen.IdUsuarioNavigation.Nombre))
                .ForMember(destino => destino.SubTotal,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.SubTotal.Value, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.ImpuestoTotal,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.ImpuestoTotal.Value, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.FechaRegistro,
                    opt => opt.MapFrom(origen => origen.FechaRegistro.Value.ToString("dd/MM/yyyy")));

            CreateMap<VentaDTO, Venta>()
                .ForMember(destino => destino.SubTotal,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.SubTotal, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.ImpuestoTotal,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ImpuestoTotal, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-PE"))));
            #endregion

            #region DetalleVenta
            CreateMap<DetalleVenta, DetalleVentaDTO>()
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-PE"))));

            CreateMap<DetalleVentaDTO, DetalleVenta>()
                .ForMember(destino => destino.Precio,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Precio, new CultureInfo("es-PE"))))
                .ForMember(destino => destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, new CultureInfo("es-PE"))));

            CreateMap<DetalleVenta, ReporteVentaDTO>()
               .ForMember(destino => destino.FechaRegistro,
                   opt => opt.MapFrom(origen => origen.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy")))
               .ForMember(destino => destino.NumeroVenta,
                   opt => opt.MapFrom(origen => origen.IdVentaNavigation.NumeroVenta))
               .ForMember(destino => destino.TipoDocumento,
                   opt => opt.MapFrom(origen => origen.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion))
               .ForMember(destino => destino.TipoDocumento,
                   opt => opt.MapFrom(origen => origen.IdVentaNavigation.DocumentoCliente))
               .ForMember(destino => destino.NombreCliente,
                   opt => opt.MapFrom(origen => origen.IdVentaNavigation.NombreCliente))
               .ForMember(destino => destino.SubTotalVenta,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-PE"))))
               .ForMember(destino => destino.ImpuestoTotalVenta,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-PE"))))
               .ForMember(destino => destino.TotalVenta,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.IdVentaNavigation.Total.Value, new CultureInfo("es-PE"))))
               .ForMember(destino => destino.Producto,
                   opt => opt.MapFrom(origen => origen.DescripcionProducto))
               .ForMember(destino => destino.Precio,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.Precio.Value, new CultureInfo("es-PE"))))
               .ForMember(destino => destino.Total,
                   opt => opt.MapFrom(origen => Convert.ToString(origen.Total.Value, new CultureInfo("es-PE"))));
            #endregion

            #region Menu
            CreateMap<Menu, MenuDTO>()
                .ForMember(destino => destino.SubMenus,
                opt => opt.MapFrom(origen => origen.InverseIdMenuPadreNavigation));
            #endregion
        }
    }
}
