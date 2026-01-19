using NubeCasera.Clases;
using NubeCasera.Dtos;
using NubeCasera.Datos;
using Microsoft.EntityFrameworkCore;

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

        public Task<ArchivoReferenciaDTO> ObtenerArchivoReferencia(Guid id)
        {
            throw new NotImplementedException();
        }

        // TODO : IMPLEMENTAR ESTA INTERFAZ SI GUID ES NULO O DIFERENTE A OTRO ID EXISTENTE, ENTONCES MOSTRAMOS LA CATEGORIA POR DEFECTO LLAMADA 'PRINCIPAL'
        public async Task<IEnumerable<ArchivoReferenciaDTO>> ObtenerArchivosReferencia(Guid? id)
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

        public async Task<ArchivoReferenciaDTO> SubirArchivoReferencia(ArchivoReferenciaDTO_Add archivoReferenciaDTO)
        {
            try
            {
                // 3. Validar que el archivo no exista mediante su hash y que no sea null
                if(archivoReferenciaDTO == null)
                {
                    throw new ArgumentNullException(nameof(archivoReferenciaDTO));
                }
                    // validar el hash, buscando en la BD alguno similar
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
                    RutaDeAlmacenamiento = archivoReferenciaDTO.RutaDeAlmacenamiento,
                    Extension = archivoReferenciaDTO.Extension,
                    MimeType = archivoReferenciaDTO.MimeType,
                    TamanioBytes = archivoReferenciaDTO.TamanioBytes,
                    EstaEliminado = false,
                    carpetaLogicaID = archivoReferenciaDTO.CarpetaLogicaId
                };
                _appDBContext.archivoReferencias.Add(nuevoArchivo);
                await _appDBContext.SaveChangesAsync();


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
    }
}
