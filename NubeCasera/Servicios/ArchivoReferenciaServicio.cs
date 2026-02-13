using NubeCasera.Clases;
using NubeCasera.Dtos;
using NubeCasera.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace NubeCasera.Servicios
{
    public class ArchivoReferenciaServicio : IArchivoReferenciaServicio
    {

        // inyeccion de la DBContext
        private readonly AppDBContext _appDBContext;
        public ArchivoReferenciaServicio(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id)
        {
            // validamos que el id no sea nulo
            if(id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }
            // buscamos el archivo en la base de datos
           ArchivoReferencia? archivoExistente = await _appDBContext.archivoReferencias.Include(a => a.carpetaLogica).FirstOrDefaultAsync(a => a.ID == id);

            if (archivoExistente == null)
            {
              throw new KeyNotFoundException($"El archivo con ID: {id}, no se encontro ");
            }
            // si existe entonces lo convertimos en DTO y lo retornamos
            // si existe entonces lo convertimos en DTO y lo retornamos
            var archivoDTO = new ArchivoReferenciaDTO
            {
                Id = archivoExistente.ID,
                Nombre = archivoExistente.Nombre,
                FechaDeSubida = archivoExistente.FechaDeSubida,
                Hash = archivoExistente.Hash,
                TipoHash = archivoExistente.TipoHash,
                RutaDeAlmacenamiento = archivoExistente.RutaDeAlmacenamiento,
                Extension = archivoExistente.Extension,
                MimeType = archivoExistente.MimeType,
                TamanioBytes = archivoExistente.TamanioBytes,
                EstaEliminado = archivoExistente.EstaEliminado,
                CarpetaLogicaId = archivoExistente.carpetaLogicaID,
                CarpetaLogicaNombre = archivoExistente.carpetaLogica != null ? archivoExistente.carpetaLogica.NombreCategoria : string.Empty
            };

            return archivoDTO;

        }


        public async Task<IEnumerable<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? id)
        {
            Guid categoriaId = id ?? AppDBContext.CategoriaPrincipalId;

            // verificar si la categoria existe
            var categoriaExiste = await _appDBContext.categorias.AnyAsync(c => c.ID == categoriaId);

            if (!categoriaExiste)
            {
                categoriaId = AppDBContext.CategoriaPrincipalId;
            }

            // obtener los archivos de la categoria especifica
            var archivos = await _appDBContext.archivoReferencias.Include(a => a.carpetaLogica)
            .Where(a => a.carpetaLogicaID == categoriaId && !a.EstaEliminado)
            .Select(a => new ArchivoReferenciaDTO
            {
                Id = a.ID,
                Nombre = a.Nombre,
                FechaDeSubida = a.FechaDeSubida,
                Hash = a.Hash,
                TipoHash = a.TipoHash,
                RutaDeAlmacenamiento = a.RutaDeAlmacenamiento,
                Extension = a.Extension,
                MimeType = a.MimeType,
                TamanioBytes = a.TamanioBytes,
                EstaEliminado = a.EstaEliminado,
                CarpetaLogicaNombre = a.carpetaLogica != null ? a.carpetaLogica.NombreCategoria : string.Empty
            }).ToListAsync();

            return archivos;
        }

        public async Task<ArchivoReferenciaDTO> GuardarArchivoAsync(ArchivoReferenciaDTO_Add archivoReferenciaDTO, IFormFile archivoFisico)
        {
            try
            {
                if(archivoReferenciaDTO == null || archivoFisico == null || archivoFisico.Length == 0)
                {
                    throw new ArgumentNullException(nameof(archivoReferenciaDTO), nameof(archivoFisico));
                }
                    // validar el hash, buscando en la BD algun hash similar
                    var hashExistente = await _appDBContext.archivoReferencias.FirstOrDefaultAsync(a => a.Hash == archivoReferenciaDTO.Hash && !a.EstaEliminado);
                if(hashExistente != null)
                {
                    throw new InvalidOperationException($"Ya existe un archivo con el hash: {archivoReferenciaDTO.Hash}");
                }

                // 4. una vez creado el obj y validado se realiza la persistencia en la bd
                var nuevoArchivo = new ArchivoReferencia
                {
                    ID = Guid.NewGuid(),
                    Nombre = archivoReferenciaDTO.Nombre,
                    FechaDeSubida = (DateTime)archivoReferenciaDTO.FechaDeSubida,
                    Hash = archivoReferenciaDTO.Hash,
                    TipoHash = archivoReferenciaDTO.TipoHash,
                    Extension = archivoReferenciaDTO.Extension,
                    MimeType = archivoReferenciaDTO.MimeType,
                    TamanioBytes = archivoReferenciaDTO.TamanioBytes,
                    EstaEliminado = false,
                    carpetaLogicaID = archivoReferenciaDTO.CarpetaLogicaId
                };

                // establezco la ruta de almacenamiento
                nuevoArchivo.RutaDeAlmacenamiento = RutaDeAlmacenamiento(nuevoArchivo.Extension);



                _appDBContext.archivoReferencias.Add(nuevoArchivo);
                await _appDBContext.SaveChangesAsync();

                // guardamos el archivo fisico en el disco
                using(var stream = archivoFisico.OpenReadStream())
                {
                    // se hace una llamada al metodo que guarda el stream
                    await GuardarEnDisco(stream,nuevoArchivo.RutaDeAlmacenamiento,nuevoArchivo.Nombre);
                }


                // obtener el nombre de la categoria
                var nombreCategoria = await _appDBContext.categorias.FindAsync(AppDBContext.CategoriaPrincipalId);

                if(nombreCategoria == null)
                {
                    throw new InvalidOperationException("La Categoria asignada no existe");
                }

                // MAPEO A DTO PARA RETORNAR
                var resultado = new ArchivoReferenciaDTO
                {
                    Id = nuevoArchivo.ID,
                    Nombre = nuevoArchivo.Nombre,
                    FechaDeSubida = nuevoArchivo.FechaDeSubida,
                    Hash = nuevoArchivo.Hash,
                    TipoHash = nuevoArchivo.TipoHash,
                    RutaDeAlmacenamiento = nuevoArchivo.RutaDeAlmacenamiento,
                    Extension = nuevoArchivo.Extension,
                    MimeType = nuevoArchivo.MimeType,
                    TamanioBytes = nuevoArchivo.TamanioBytes,
                    EstaEliminado = nuevoArchivo.EstaEliminado,
                    CarpetaLogicaNombre = nombreCategoria.NombreCategoria
                };

                return resultado;
            }
            catch
            {
                throw;
            }   
        }

        // metodo que guarda el archivo en el disco
        private async Task GuardarEnDisco(Stream archivo, string rutaRelativa, string nombreArchivo)
        {
            var Rutacompleta = Path.Combine(rutaRelativa,nombreArchivo);

            using var archivoDestino = new FileStream(Rutacompleta,FileMode.Create);
            await archivo.CopyToAsync(archivoDestino);
        }

        // metodo que ayuda a dirigir los archivos a sus rutas
        private string RutaDeAlmacenamiento(string extension)
        {
            // validar que extension no sea null
            if(extension == null)
            {
                throw new ArgumentNullException("La extension pasada como parametro esta vacia o nula");
            }
            
            string miCarpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MisProyectos","ArchivosReference");

            // si no existe entonces se crea el directorio
            if (!Directory.Exists(miCarpeta)) Directory.CreateDirectory(miCarpeta);
            
            // ahora dependiendo de la extension se crea una ruta, si no existe y se devuelve
            // si existe solamente se devuelve la ruta. 
            string rutaExtension = Path.Combine(miCarpeta,extension);
            if (!Directory.Exists(rutaExtension))
            {
                Directory.CreateDirectory(rutaExtension);
                return rutaExtension;
            }
            else
            {
                return rutaExtension;
            }
        }

        
        public async Task<Stream> DescargarAsync(Guid id)
        {
            // 1. validar el id que no sea nulo  y que exista un un archivo con ese id
            if(id == Guid.Empty) throw new ArgumentNullException("El id esta vacio");
            
            var archivoReferencia = await _appDBContext.archivoReferencias.FindAsync(id);

            if(archivoReferencia == null) throw new KeyNotFoundException($"No se encontro el archivo con ID: {id} o este no existe.");

            // 2. Obtener el path de donde se almacena ese archivo
            var rutaCompleta = Path.Combine(archivoReferencia.RutaDeAlmacenamiento,archivoReferencia.Nombre);

            if(!File.Exists(rutaCompleta)) throw new FileNotFoundException($"El archivo con ID: {id}  no se encontro.");

            // 3. Verificar integridad del archivo
            bool hashValido = await VerificarHashAsync(rutaCompleta,archivoReferencia.Hash,archivoReferencia.TipoHash);

            if(!hashValido) throw new InvalidOperationException("El archivo esta corrupto o ha sido modificado");

            // devolver un stream
            return File.OpenRead(rutaCompleta);
        }

        public async Task ELiminarAsync(Guid id)
        {
            // validamos que el id no sea nulo
            if(id == Guid.Empty) throw new ArgumentNullException("El id esta vacio");
            var archivoReferencia = await _appDBContext.archivoReferencias.FindAsync(id);
            if(archivoReferencia == null) throw new KeyNotFoundException($"No se encontro el archivo con ID: {id} o este no existe.");

            
            // eliminamos el archivo fisico del disco
            var rutaCompleta = Path.Combine(archivoReferencia.RutaDeAlmacenamiento, archivoReferencia.Nombre);    
            if (File.Exists(rutaCompleta))
            {
                // comparar hash
                bool esCorrecto = await VerificarHashAsync(rutaCompleta, archivoReferencia.Hash, archivoReferencia.TipoHash);

                if (esCorrecto)
                {
                    File.Delete(rutaCompleta);
                }
            }
            else
            {
             throw new InvalidOperationException("No se pudo eliminar el archivo");   
            }

            archivoReferencia.EstaEliminado = true;
            archivoReferencia.FechaDeEliminacion = DateTime.UtcNow;
            await _appDBContext.SaveChangesAsync();
        }


        // metodo privado para uso interno
        private async Task<string> CalcularHashAsync(Stream stream, string tipoHash = "SHA256")
        {
            using HashAlgorithm hashAlgorithm = tipoHash.ToUpper() switch
            {
                "SHA256" => SHA256.Create(),
                "SHA512" => SHA512.Create(),
                "MD5" => MD5.Create(),
                _ => SHA256.Create()
            };

            stream.Position = 0;
            byte[] hashBytes = await hashAlgorithm.ComputeHashAsync(stream);
            stream.Position = 0;
            
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        // metodo publico para calcular hash en el controlador
        public async Task<string> CalcularHashArchivoAsync(Stream stream, string tipoHash)
        {
            return await CalcularHashAsync(stream, tipoHash);
        }

        private async Task<bool> VerificarHashAsync(string rutaArchivo, string hashEsperado, string tipoHash)
        {
            if(!File.Exists(rutaArchivo)) return false;

            using var stream = File.OpenRead(rutaArchivo);
            string hashCalculado = await CalcularHashAsync(stream,tipoHash);

            return string.Equals(hashCalculado, hashEsperado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
