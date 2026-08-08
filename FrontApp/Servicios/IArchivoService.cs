using DTOModels.DTOs;

namespace Front.Servicios
{
    public interface IArchivoService
    {
        Task<ArchivoReferenciaDTO> SubirArchivoAsync(Stream contenido, string nombreArchivo,string contentType, Guid IdCategoria);
        Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null);
        Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id);
        Task<List<ArchivoReferenciaDTO>> BuscarArchivosAsync(string? q, Guid? categoriaId = null);
        string ObtenerUrlDescarga(Guid id);
        Task EliminarArchivoAsync(Guid id);
        string ObtenerUrlThumbnail(string? rutaThumbnail);
    }
}
