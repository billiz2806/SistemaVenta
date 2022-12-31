using Firebase.Auth;
using Firebase.Storage;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IGenericRepository<Configuracion> _repositorio;

        public FireBaseService(IGenericRepository<Configuracion> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<string> SubirStorage(Stream streamArchivo, string carpetaDestino, string nombreArchivo)
        {
            string urlImagen = "";
            string error = "";

            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Recurso.Equals("FireBase_Storage"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var singIn = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(singIn.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(config[carpetaDestino])
                    .Child(nombreArchivo)
                    .PutAsync(streamArchivo, cancellation.Token);

                urlImagen = await task;

            }
            catch (Exception ex)
            {
                error = ex.Message;
                urlImagen = "";
            }

            return urlImagen;
        }
        public async Task<bool> EliminarStorage(string carpetaDestino, string nombreArchivo)
        {
            try
            {
                IQueryable<Configuracion> query = await _repositorio.Consultar(c => c.Equals("FireBase_Storage"));
                Dictionary<string, string> config = query.ToDictionary(keySelector: c => c.Propiedad, elementSelector: c => c.Valor);

                var auth = new FirebaseAuthProvider(new FirebaseConfig(config["api_key"]));
                var singIn = await auth.SignInWithEmailAndPasswordAsync(config["email"], config["clave"]);

                var cancellation = new CancellationTokenSource();
                var task = new FirebaseStorage(
                    config["ruta"],
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(singIn.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(config[carpetaDestino])
                    .Child(nombreArchivo)
                    .DeleteAsync();

                await task;
                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}
