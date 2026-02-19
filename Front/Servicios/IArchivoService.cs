using DTOModels.DTOs;

namespace Front.Servicios
{
    public interface IArchivoService
    {
        Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null);
        Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id);
        Task<Stream> DescargarArchivoAsync(Guid id);
        Task EliminarArchivoAsync(Guid id);
    }
}
