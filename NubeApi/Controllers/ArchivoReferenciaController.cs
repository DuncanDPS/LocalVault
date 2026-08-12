using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NubeCasera.Datos;
using DTOModels.DTOs;
using NubeCasera.Servicios;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using NubeCasera.Clases;

namespace NubeCasera.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivoReferenciaController : ControllerBase
    {
        private readonly IArchivoReferenciaServicio _archivoReferenciaServ;

        public ArchivoReferenciaController(IArchivoReferenciaServicio archivoReferenciaServ)
        {
            _archivoReferenciaServ = archivoReferenciaServ;
        }

        [HttpPost("subir-archivo")]
        public async Task<IActionResult> GuardarAsync(
            [FromForm] IFormFile chunk,
            [FromForm] string Idsubida,
            [FromForm] int index,
            [FromForm] string ultimo, 
            [FromForm] Guid? CategoriaId, 
            [FromForm] string filename) // aqui tengo que recibir un id DE CATEGORIA
        {
            if (chunk == null) return BadRequest("Chunk Vacio");

            // Ruta temporal para ir uniendo los chunks
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Idsubida}_{filename}");
            
            using(var stream = new FileStream(tempPath, FileMode.Append))
            {
                await chunk.CopyToAsync(stream);
            }
            // Si no es el ultimo, regresamos OK para que continue subiendo
            if(ultimo != "1")
            {
                return Ok();
            }

            // Procesamiento del archivo Finalizado
            try
            {
                // calcular el hash del archivo
                string hash;
                using (var stream = System.IO.File.OpenRead(tempPath))
                {
                    hash = await _archivoReferenciaServ.CalcularHashArchivoAsync(stream, "SHA256");
                }

                if (CategoriaId == null || CategoriaId == Guid.Empty)
                {
                    CategoriaId = AppDBContext.CategoriaPrincipalId;
                }

                var fileInfo = new FileInfo(tempPath);

                // Crear el DTO
                var archivoDTO = new ArchivoReferenciaDTO_Add
                {
                    Nombre = filename,
                    Hash = hash,
                    TipoHash = "SHA256",
                    Extension = Path.GetExtension(filename),
                    MimeType = chunk.ContentType,
                    TamanioBytes = fileInfo.Length,
                    FechaDeSubida = DateTime.UtcNow,
                    CarpetaLogicaId = CategoriaId
                };

                // Para simular el IFormFile puedes abrir un stream u otro mecanismo de tu servicio
                // Depende de cómo `_archivoReferenciaServ.GuardarArchivoAsync` consuma el archivo original
                using var finalStream = System.IO.File.OpenRead(tempPath);
                var formFile = new FormFile(finalStream, 0, finalStream.Length, "archivo", filename)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = chunk.ContentType
                };

                // llamar al servicio
                var resultado = await _archivoReferenciaServ.GuardarArchivoAsync(archivoDTO, formFile); // TODO: AQUI RECIBIR ID DE CATEGORIA

                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al subir archivo", detalle = ex.Message });
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                {
                    System.IO.File.Delete(tempPath);
                }
            }
        }

        [HttpGet("obtener-archivos/{id?}")]
        public async Task<IActionResult> ObtenerArchivosAsync(Guid? id)
        {
            try
            {
                var archivos = await _archivoReferenciaServ.ObtenerArchivosAsync(id);
                return Ok(archivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener archivos", detalle = ex.Message });
            }
        }

        [HttpGet("obtener-archivo/{id}")]
        public async Task<IActionResult> ObtenerArchivoAsync(Guid id)
        {
            try
            {
                var archivo = await _archivoReferenciaServ.ObtenerArchivoAsync(id);
                return Ok(archivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el archivo ", detalle = ex.Message });
            }
        }


        [HttpGet("descargar-archivo/{id}")]
        public async Task<IActionResult> DescargarArchivoAsync(Guid id)
        {
            try
            {
                // Desestructuramos la tupla recibida
                var (contenido, nombreArchivo) = await _archivoReferenciaServ.DescargarAsync(id);
                // Agregamos el nombre como tercer parámetro
                return File(contenido, "application/octet-stream", nombreArchivo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al descargar el archivo", detalle = ex.Message });
            }
        }

        [HttpDelete("eliminar-archivo/{id}")]
        public async Task<IActionResult> EliminarArchivoAsync(Guid id)
        {
            try
            {
                await _archivoReferenciaServ.ELiminarAsync(id);
                return Ok(new { mensaje = "Archivo eliminado correctamente9a80b6de-ff00-40cc-abb8-bc50de63f2d6" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el archivo: ", detalle = ex.Message });
            }
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarArchivosAsync([FromQuery] string? q, [FromQuery] Guid? categoriaId)
        {
            try
            {
                var resultados = await _archivoReferenciaServ.BuscarArchivoAsync(q, categoriaId);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al buscar archivos", detalle = ex.Message });
            }

        }
    }
}
