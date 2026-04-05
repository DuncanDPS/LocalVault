using System.Net.Http.Json;
using DTOModels.DTOs;

namespace Front.Servicios
{
    public class CategoriaService : ICategoriaService
    {
        public event Action OnCategoriasActualizadas;
        private readonly HttpClient _httpClient;

        public CategoriaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private void NotificarCambios()
        {
            OnCategoriasActualizadas?.Invoke();
        }

        public async Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoria)
        {
            var response = await _httpClient.PostAsJsonAsync("api/categoria/crear-categoria", categoria);
            response.EnsureSuccessStatusCode();
            NotificarCambios();
            return await response.Content.ReadFromJsonAsync<CategoriaDTO>() ?? throw new Exception("Error al crear la categoría");
        }

        public async Task EliminarCategoriaAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/categoria/eliminar-categoria/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<CategoriaDTO>>("api/categoria/obtener-categorias") ?? new List<CategoriaDTO>();
        }
    }
}
