using DTOModels.DTOs;

namespace Front.Servicios
{
    public interface IArchivoService
    {
        Task<ArchivoReferenciaDTO> SubirArchivoAsync(byte[] contenido, string nombreArchivo, Guid IdCategoria);
        Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null);
        Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id);
        string ObtenerUrlDescarga(Guid id);
        Task EliminarArchivoAsync(Guid id);
    }
}
