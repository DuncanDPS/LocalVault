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

        public async Task<ArchivoReferenciaDTO> SubirArchivoAsync(byte[] contenido, string nombreArchivo)
        {
            // Crear el contenido multipart/form-data
            using var content = new MultipartFormDataContent();

            // Crear el  archivo como ByteArrayContent
            var fileContent = new ByteArrayContent(contenido);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            // Agregar el archivo con el nombre "archivo" (debe coincidir con lo que espera el backend)
            content.Add(fileContent, "archivo", nombreArchivo);

            // Hacer POST a la API
            var response = await _httpClient.PostAsync("api/ArchivoReferencia/subir-archivo", content);
            response.EnsureSuccessStatusCode();

            // Deserializar y retornar el resultado
            return await response.Content.ReadFromJsonAsync<ArchivoReferenciaDTO>()
                ?? throw new Exception("Error al procesar respuesta del servidor");





        }
    }
}
