using DTOModels.DTOs;
using System.Net.Http.Json;

namespace Front.Servicios
{
    public class ArchivoService : IArchivoService
    {
        private readonly HttpClient _httpClient;
        public ArchivoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null)
        {
            // ✅ CORREGIR LA RUTA
            var url = categoriaId.HasValue
                ? $"api/ArchivoReferencia/obtener-archivos/{categoriaId}"
                : "api/ArchivoReferencia/obtener-archivos";

            return await _httpClient.GetFromJsonAsync<List<ArchivoReferenciaDTO>>(url)
                ?? new List<ArchivoReferenciaDTO>();
        }

        public async Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id)
        {
            // ✅ CORREGIR LA RUTA
            return await _httpClient.GetFromJsonAsync<ArchivoReferenciaDTO>($"api/ArchivoReferencia/obtener-archivo/{id}")
                ?? throw new Exception("Archivo no encontrado");
        }

        public async Task<Stream> DescargarArchivoAsync(Guid id)
        {
            return await _httpClient.GetStreamAsync($"api/ArchivoReferencia/{id}/descargar");
        }

        public async Task EliminarArchivoAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/ArchivoReferencia/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
