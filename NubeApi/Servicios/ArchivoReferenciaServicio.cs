using NubeCasera.Clases;
using DTOModels.DTOs;
using NubeCasera.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Linq.Expressions;

namespace NubeCasera.Servicios
{
    public class ArchivoReferenciaServicio : IArchivoReferenciaServicio
    {

        // inyeccion de la DBContext
        private readonly AppDBContext _appDBContext;
        // recibimos el id de la categoria principal
        Guid idCatPrincipal = AppDBContext.CategoriaPrincipalId;

        // constructor con IThumnailServicio
        private readonly IThumbnailServicio _thumbnailServicio;
        public ArchivoReferenciaServicio(AppDBContext appDBContext, IThumbnailServicio thumbnailServicio)
        {
            _appDBContext = appDBContext;
            _thumbnailServicio = thumbnailServicio;
        }

        // Expression usada por EF Core en .Select(...) - traducible a sql
        private static readonly Expression<Func<ArchivoReferencia, ArchivoReferenciaDTO>>
            ToDtoExpression = a => new ArchivoReferenciaDTO
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
                FechaDeEliminacion = a.FechaDeEliminacion,
                CarpetaLogicaId = a.carpetaLogicaID,
                CarpetaLogicaNombre = a.carpetaLogica != null ? a.carpetaLogica.NombreCategoria : null,
                TieneThumbnail = a.Extension != null &&
                    (a.Extension.ToLower() == ".jpg" || a.Extension.ToLower() == ".jpeg" ||
             a.Extension.ToLower() == ".png" || a.Extension.ToLower() == ".gif" ||
             a.Extension.ToLower() == ".webp" || a.Extension.ToLower() == ".bmp"),
                RutaThumbnail = a.Extension != null &&
            (a.Extension.ToLower() == ".jpg" || a.Extension.ToLower() == ".jpeg" ||
             a.Extension.ToLower() == ".png" || a.Extension.ToLower() == ".gif" ||
             a.Extension.ToLower() == ".webp" || a.Extension.ToLower() == ".bmp")
            ? ("/thumbnails/" + a.Hash + "_thumb.webp")
            : null
            };

        // Metodo auxiliar para entidades ya cargadas en memoria
        private static ArchivoReferenciaDTO ToDto(ArchivoReferencia a)
        {
            return new ArchivoReferenciaDTO
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
                FechaDeEliminacion = a.FechaDeEliminacion,
                CarpetaLogicaId = a.carpetaLogicaID,
                CarpetaLogicaNombre = a.carpetaLogica != null ? a.carpetaLogica.NombreCategoria : null,
                TieneThumbnail = a.Extension != null &&
            (a.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)),
                RutaThumbnail = (a.Extension != null &&
            (a.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".gif", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".webp", StringComparison.OrdinalIgnoreCase) ||
             a.Extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase)))
            ? $"/thumbnails/{a.Hash}_thumb.webp"
            : null
            };
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
            .Where(a => a.carpetaLogicaID == categoriaId && !a.EstaEliminado).AsNoTracking()
            .Select(ToDtoExpression).ToListAsync();

            return archivos;
        }

        
        public async Task<ArchivoReferenciaDTO> GuardarArchivoAsync(ArchivoReferenciaDTO_Add archivoReferenciaDTO, IFormFile archivoFisico)
        {
            if (archivoReferenciaDTO == null || archivoFisico == null || archivoFisico.Length == 0)
                throw new ArgumentNullException(nameof(archivoReferenciaDTO));

            var hashExistente = await _appDBContext.archivoReferencias
                .FirstOrDefaultAsync(a => a.Hash == archivoReferenciaDTO.Hash && !a.EstaEliminado);
            if (hashExistente != null)
                throw new InvalidOperationException($"Ya existe un archivo con el hash: {archivoReferenciaDTO.Hash}");

            var nuevoArchivo = new ArchivoReferencia
            {
                ID = Guid.NewGuid(),
                Nombre = archivoReferenciaDTO.Nombre,
                FechaDeSubida = archivoReferenciaDTO.FechaDeSubida ?? DateTime.UtcNow,
                Hash = archivoReferenciaDTO.Hash,
                TipoHash = archivoReferenciaDTO.TipoHash,
                Extension = archivoReferenciaDTO.Extension,
                MimeType = archivoReferenciaDTO.MimeType,
                TamanioBytes = archivoReferenciaDTO.TamanioBytes,
                EstaEliminado = false,
                carpetaLogicaID = archivoReferenciaDTO.CarpetaLogicaId
            };

            nuevoArchivo.RutaDeAlmacenamiento = RutaDeAlmacenamiento(nuevoArchivo.Extension);

            _appDBContext.archivoReferencias.Add(nuevoArchivo);
            await _appDBContext.SaveChangesAsync();

            // Guardar archivo físico y generar thumbnail
            string rutaThumbnail = string.Empty;
            bool tieneThumbmail = false;

            using (var stream = archivoFisico.OpenReadStream())
            {
                await GuardarEnDisco(stream, nuevoArchivo.RutaDeAlmacenamiento, nuevoArchivo.Nombre);

                stream.Position = 0;
                var (exito, ruta) = await _thumbnailServicio.GenerarThumbnailAsync(
                    stream,
                    nuevoArchivo.Extension,
                    nuevoArchivo.Hash);

                tieneThumbmail = exito;
                rutaThumbnail = ruta;
            }

            // Cargar la categoría asignada (usar la carpetaLogicaID del nuevo archivo; fallback a principal)
            var categoriaId = nuevoArchivo.carpetaLogicaID ?? AppDBContext.CategoriaPrincipalId;
            var nombreCategoria = await _appDBContext.categorias.FindAsync(categoriaId);
            if (nombreCategoria == null)
                throw new InvalidOperationException("La Categoria asignada no existe");

            // asignar la navegación en memoria para que ToDto pueda leer el nombre
            nuevoArchivo.carpetaLogica = nombreCategoria;


            // mapear consistentemente usando el helper ToDto
            var resultado = ToDto(nuevoArchivo);
            return resultado;
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

        
        public async Task<(Stream archivo, string nombre)> DescargarAsync(Guid id)
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
            // abrimos el stream
            var stream = File.OpenRead(rutaCompleta);
            // devolver un stream
            return (stream, archivoReferencia.Nombre);
        }

        // Modificar ELiminarAsync para también eliminar thumbnail
        public async Task ELiminarAsync(Guid id)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("El id esta vacio");
            var archivoReferencia = await _appDBContext.archivoReferencias.FindAsync(id);
            if (archivoReferencia == null) throw new KeyNotFoundException($"No se encontro el archivo con ID: {id} o este no existe.");

            var rutaCompleta = Path.Combine(archivoReferencia.RutaDeAlmacenamiento, archivoReferencia.Nombre);
            if (File.Exists(rutaCompleta))
            {
                bool esCorrecto = await VerificarHashAsync(rutaCompleta, archivoReferencia.Hash, archivoReferencia.TipoHash);

                if (esCorrecto)
                {
                    File.Delete(rutaCompleta);

                    // Eliminar thumbnail asociado
                    if (!string.IsNullOrEmpty(archivoReferencia.RutaThumbnail))
                    {
                        await _thumbnailServicio.EliminarThumbnailAsync(archivoReferencia.RutaThumbnail);
                    }
                }
            }

            // Marcar como eliminado en BD
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

        public async Task<List<ArchivoReferenciaDTO>> BuscarArchivoAsync(string? q, Guid? categoriaId)
        {
            var query = _appDBContext.archivoReferencias.Include(a => a.carpetaLogica)
                .Where(a => !a.EstaEliminado).AsQueryable();

            if(categoriaId.HasValue && categoriaId.Value != Guid.Empty)
            {
                query = query.Where(a => a.carpetaLogicaID == categoriaId.Value);
            }

            if (!string.IsNullOrEmpty(q))
            {
                q = q.Trim().ToLower();

                query = query.Where(a =>
                    a.Nombre.ToLower().Contains(q) ||
                    a.Extension.ToLower().Contains(q) ||
                    a.Hash.ToLower().Contains(q));
            }
            var resultados = await query.AsNoTracking().Select(ToDtoExpression).ToListAsync();
            return resultados;
        }



    }
}
