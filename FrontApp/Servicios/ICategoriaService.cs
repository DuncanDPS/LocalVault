using DTOModels.DTOs;

namespace Front.Servicios
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDTO>> ObtenerCategoriasAsync();
        Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoria);
        Task EliminarCategoriaAsync(Guid id);

        event Action? OnCategoriasActualizadas;
        
    }
}
