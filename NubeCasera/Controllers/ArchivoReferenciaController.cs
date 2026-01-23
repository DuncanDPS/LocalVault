using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NubeCasera.Datos;
using NubeCasera.Dtos;
using NubeCasera.Servicios;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;

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
        public async Task<IActionResult> SubirArchivo(IFormFile archivo)
        {
            // validar que no sea null
            if(archivo == null)
            {
                return BadRequest("No se ha proporcionado ningun archivo o esta vacio");
            }
            try
            {
                // calcular el hash del archivo
                string hash;
                using (var stream = archivo.OpenReadStream())
                {
                    using (var sha256 = SHA256.Create())
                    {
                        var hashBytes = await sha256.ComputeHashAsync(stream);
                        hash = BitConverter.ToString(hashBytes).Replace("-", "");
                    }
                }

                // Crear el DTO
                var archivoDTO = new ArchivoReferenciaDTO_Add
                {
                    Nombre = archivo.FileName,
                    Hash = hash,
                    TipoHash = "SHA256",
                    Extension = Path.GetExtension(archivo.FileName),
                    MimeType = archivo.ContentType,
                    TamanioBytes = archivo.Length,
                    FechaDeSubida = DateTime.UtcNow,
                    CarpetaLogicaId = AppDBContext.CategoriaPrincipalId
                };

                var carpeta = Path.Combine("ArchivosSubidos", DateTime.UtcNow.ToString("yyyy"), DateTime.UtcNow.ToString("MM"));
                Directory.CreateDirectory(carpeta); // Crear si no existe
                archivoDTO.RutaDeAlmacenamiento = Path.Combine(carpeta, $"{hash}{Path.GetExtension(archivo.FileName)}");

                // llamar al servicio
                var resultado = await _archivoReferenciaServ.SubirArchivoAsync(archivoDTO);
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new {mensaje = "Error al subir archivo", detalle = ex.Message});
            }   
        }

        [HttpGet("obtener-archivos/{id?}")]
        public async Task<IActionResult> ObtenerArchivosReferencia(Guid? id)
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
        public async Task<IActionResult> ObtenerArchivoReferencia(Guid id)
        {
            try
            {
                var archivo = await _archivoReferenciaServ.ObtenerArchivoAsync(id);
                return Ok(archivo);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new {mensaje = "Error al obtener el archivo ", detalle = ex.Message});
            }
        }
    }
}
