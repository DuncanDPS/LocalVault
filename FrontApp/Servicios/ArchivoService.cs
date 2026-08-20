using DTOModels.DTOs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Front.Servicios
{
    public class ArchivoService : IArchivoService
    {
        private readonly HttpClient _httpClient;

        //public Action<int>? OnProgresoActualizado;

        public ArchivoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

 
     
        public async Task<List<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? categoriaId = null)
        {
            var url = categoriaId.HasValue
                ? $"api/ArchivoReferencia/obtener-archivos/{categoriaId}"
                : "api/ArchivoReferencia/obtener-archivos";

            return await _httpClient.GetFromJsonAsync<List<ArchivoReferenciaDTO>>(url)
                ?? new List<ArchivoReferenciaDTO>();
        }

        public async Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ArchivoReferenciaDTO>($"api/ArchivoReferencia/obtener-archivo/{id}")
                ?? throw new Exception("Archivo no encontrado");
        }


        public async Task EliminarArchivoAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/ArchivoReferencia/eliminar-archivo/{id}");
            response.EnsureSuccessStatusCode();
        }

        
        public async Task SubirArchivoAsync(IBrowserFile file, Guid IdCategoria, long maxAllowedSize, Action<int> OnProgresoActualizado)
        {
            const int chunkSize = 4 * 1024 * 1024; // 4 MB por chunk
            var IdSubida = Guid.NewGuid().ToString();
            var tamanioTotal = file.Size;
            var chunksTotales = (int)Math.Ceiling((double)tamanioTotal / chunkSize);

            await using var stream = file.OpenReadStream(maxAllowedSize: maxAllowedSize);

            var buffer = new byte[chunkSize];
            int chunkIndex = 0;
            int leer;

            while((leer = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var esUltimo = (chunkIndex + 1) == chunksTotales;
                using var content = new MultipartFormDataContent();
                // usar solo los bytes leidos
                var bytes = leer == buffer.Length ? buffer : buffer[..leer];
                var fileContent = new ByteArrayContent(bytes);
                var mediaType = string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType;
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(mediaType);

                content.Add(fileContent, "chunk", file.Name);
                content.Add(new StringContent(IdSubida),"IdSubida");
                content.Add(new StringContent(chunkIndex.ToString
                    ()),"index");
                content.Add(new StringContent(esUltimo ? "1" : "0"), "ultimo");
                content.Add(new StringContent(IdCategoria.ToString()), "CategoriaId");
                content.Add(new StringContent(file.Name), "filename");

                using var req = new HttpRequestMessage(HttpMethod.Post, "api/ArchivoReferencia/subir-archivo")
                {
                    Content = content
                };

                // importante: pedir que el browser use streaming si esta disponible
                req.SetBrowserRequestStreamingEnabled(true);

                // porcentaje de carga de chunks, para actualizar la barra de progreso
                int porcentaje = (int)((chunkIndex + 1) * 100.0 / chunksTotales);
                OnProgresoActualizado?.Invoke(porcentaje);

                using var resp = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();
                chunkIndex++;
            }          
        }


  

        public string ObtenerUrlDescarga(Guid id)
        {
            return  $"{_httpClient.BaseAddress}api/ArchivoReferencia/descargar-archivo/{id}";
        }

        public string ObtenerUrlThumbnail(string? rutaThumbnail)
        {
            if (string.IsNullOrWhiteSpace(rutaThumbnail))
                return string.Empty;

            return new Uri(new Uri(_httpClient.BaseAddress!.ToString()), rutaThumbnail.TrimStart('/')).ToString();
        }

        // TODO: Continuar con esta implementacion
        public async Task<List<ArchivoReferenciaDTO>> BuscarArchivosAsync(string? q, Guid? categoriaId = null)
        {
            var query = new List<string>();

            if(!string.IsNullOrWhiteSpace(q))
                query.Add($"q={Uri.EscapeDataString(q)}");
            if (categoriaId.HasValue && categoriaId.Value != Guid.Empty)
                query.Add($"categoriaId={categoriaId}");
            var url = "api/ArchivoReferencia/buscar";
            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            return await _httpClient.GetFromJsonAsync<List<ArchivoReferenciaDTO>>(url) ?? new List<ArchivoReferenciaDTO>(); 
        }
    }
}
