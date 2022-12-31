using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class UsuarioService : IUsuarioService
    { 
        private readonly IGenericRepository<Usuario> _repositorio;
        private readonly IFireBaseService  _fireBaseService;
        private readonly IUtilidadesService _utilidadesService;
        private readonly ICorreoService _correoService;

        public UsuarioService(IGenericRepository<Usuario> repositorio, IFireBaseService fireBaseService, IUtilidadesService utilidadesService, ICorreoService correoService)
        {
            _repositorio = repositorio;
            _fireBaseService = fireBaseService;
            _utilidadesService = utilidadesService;
            _correoService = correoService;
        }
        public async Task<List<Usuario>> Lista()
        {
            IQueryable<Usuario> query = await _repositorio.Consultar();
            return query.Include(r => r.IdRolNavigation).ToList();
        }
        public async Task<Usuario> Crear(Usuario entidad, Stream foto = null, string nombreFoto = "", string urlPlantillaCorreo = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");

            try
            {
                string clave_generada = _utilidadesService.GenerarClave();
                entidad.Clave = _utilidadesService.ConvertirSha256(clave_generada);
                entidad.NombreFoto = nombreFoto;

                if(foto != null)
                {
                    string urlFoto = await _fireBaseService.SubirStorage(foto, "carpeta_usuario", nombreFoto);
                    entidad.UrlFoto = urlFoto;
                }

                Usuario usuario_creado = await _repositorio.Crear(entidad);

                if(usuario_creado.IdUsuario == 0)
                    throw new TaskCanceledException("No se pudo crear el usuario");

                if(urlPlantillaCorreo != "")
                {
                    urlPlantillaCorreo = urlPlantillaCorreo.Replace("[correo]", usuario_creado.Correo).Replace("[clave]", clave_generada);
                    string htmlCorreo = "";

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader readerStream = null;
                            if(response.CharacterSet == null)
                            {
                                readerStream = new StreamReader(dataStream);
                            }
                            else
                            {
                                readerStream = new StreamReader(dataStream,Encoding.GetEncoding(response.CharacterSet));
                            }
                            htmlCorreo = readerStream.ReadToEnd();
                            response.Close();
                            readerStream.Close();
                        }
                    }

                    if(htmlCorreo != null)
                    {
                        await _correoService.EnviarCorreo(usuario_creado.Correo,"Cuenta creada", htmlCorreo);
                    }
                
                }

                IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == usuario_creado.IdUsuario);
                usuario_creado = query.Include(r => r.IdRolNavigation).First();
                return usuario_creado;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<Usuario> Editar(Usuario entidad, Stream foto = null, string nombreFoto = "")
        {
            Usuario usuario_existe = await _repositorio.Obtener(u => u.Correo == entidad.Correo && u.IdUsuario != entidad.IdUsuario);

            if (usuario_existe != null)
                throw new TaskCanceledException("El correo ya existe");
            try
            {
                IQueryable<Usuario> queryUsuario = await _repositorio.Consultar(u => u.IdUsuario == entidad.IdUsuario);
                Usuario usuario_editar = queryUsuario.First();

                usuario_editar.Nombre = entidad.Nombre;
                usuario_editar.Correo = entidad.Correo;
                usuario_editar.Telefono = entidad.Telefono;
                usuario_editar.IdRol = entidad.IdRol;
                usuario_editar.EsActivo = entidad.EsActivo;

                if (usuario_editar.NombreFoto=="")
                {
                    usuario_editar.NombreFoto = nombreFoto;
                }

                if(foto != null)
                {
                    string urlFoto = await _fireBaseService.SubirStorage(foto, "carpeta_usuario", usuario_editar.NombreFoto);
                    usuario_editar.UrlFoto = urlFoto;
                }

                bool respuesta = await _repositorio.Editar(usuario_editar);
                if(!respuesta)
                    throw new TaskCanceledException("No se puedo editar el usuario");

                Usuario usuario_editado = queryUsuario.Include(r => r.IdRolNavigation).First();

                return usuario_editado;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> Eliminar(int idUsuario)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                string? nombreFoto = usuario_encontrado.NombreFoto;
                bool respuesta = await _repositorio.Eliminar(usuario_encontrado);

                if (respuesta)
                {
                    await _fireBaseService.EliminarStorage("carpeta_usuario", nombreFoto);
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Usuario> ObtenerPorCredenciales(string correo, string clave)
        {
            string clave_encriptada = _utilidadesService.ConvertirSha256(clave);
            Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo.Equals(correo) && u.Clave.Equals(clave_encriptada));
            return usuario_encontrado;
        }
        public async Task<Usuario> ObtenerPorId(int idUsuario)
        {
            IQueryable<Usuario> query = await _repositorio.Consultar(u => u.IdUsuario == idUsuario);
            Usuario resultado = query.Include(r => r.IdRolNavigation).First();
            return resultado;
        }
        public async Task<bool> GuardarPerfil(Usuario entidad)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == entidad.IdUsuario);

                if(usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                usuario_encontrado.Correo = entidad.Correo;
                usuario_encontrado.Telefono = entidad.Telefono;

                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> CambiarClave(int idUsuario, string claveActual, string claveNueva)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.IdUsuario == idUsuario);

                if (usuario_encontrado == null)
                    throw new TaskCanceledException("El usuario no existe");

                if(usuario_encontrado.Clave != _utilidadesService.ConvertirSha256(claveActual))
                    throw new TaskCanceledException("La contraseña actual es incorrecta");

                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(claveNueva);

                bool respuesta = await _repositorio.Editar(usuario_encontrado);
                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> RestablecerClave(string correo, string urlPlantillaCorreo)
        {
            try
            {
                Usuario usuario_encontrado = await _repositorio.Obtener(u => u.Correo == correo);
                if (usuario_encontrado == null)
                    throw new TaskCanceledException("No encontramos ningun usuario asociado a este correo");

                string clave_generada = _utilidadesService.GenerarClave();
                usuario_encontrado.Clave = _utilidadesService.ConvertirSha256(clave_generada);


                urlPlantillaCorreo = urlPlantillaCorreo.Replace("[clave]", clave_generada);
                string htmlCorreo = "";

                //Solicitud a la plantilla de correo
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlPlantillaCorreo);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader readerStream = null;
                        if (response.CharacterSet == null)
                        {
                            readerStream = new StreamReader(dataStream);
                        }
                        else
                        {
                            readerStream = new StreamReader(dataStream, Encoding.GetEncoding(response.CharacterSet));
                        }
                        htmlCorreo = readerStream.ReadToEnd();
                        response.Close();
                        readerStream.Close();
                    }
                }
                bool correo_enviado = false;

                if (htmlCorreo != null)
                {
                    correo_enviado =  await _correoService.EnviarCorreo(correo, "Contraseña restablecida", htmlCorreo);
                }

                if (!correo_enviado)
                    throw new TaskCanceledException("Error: Intentalo de nuevo o mas tarde");
                bool respuesta = await _repositorio.Editar(usuario_encontrado);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
