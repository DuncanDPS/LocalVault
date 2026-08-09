using DTOModels.DTOs;
using Microsoft.AspNetCore.Components.Forms;

namespace Front.Servicios
{
    public interface IArchivoService
    {
        Task<ArchivoReferenciaDTO> SubirArchivoAsync(IBrowserFile file, Guid IdCategoria, long maxAllowedSize);
        Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null);
        Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id);
        Task<List<ArchivoReferenciaDTO>> BuscarArchivosAsync(string? q, Guid? categoriaId = null);
        string ObtenerUrlDescarga(Guid id);
        Task EliminarArchivoAsync(Guid id);
        string ObtenerUrlThumbnail(string? rutaThumbnail);
    }
}
