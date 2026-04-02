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
        public async Task<IActionResult> GuardarAsync(IFormFile archivo, Guid? ID_Categoria) // aqui tengo que recibir un id DE CATEGORIA
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
                    hash = await _archivoReferenciaServ.CalcularHashArchivoAsync(stream,"SHA256");
                }

                if(ID_Categoria == null || ID_Categoria == Guid.Empty)
                {
                    ID_Categoria = AppDBContext.CategoriaPrincipalId;
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
                    CarpetaLogicaId = ID_Categoria
                };

                // llamar al servicio
                var resultado = await _archivoReferenciaServ.GuardarArchivoAsync(archivoDTO, archivo); // TODO: AQUI RECIBIR ID DE CATEGORIA
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
            catch(Exception ex)
            {
                return StatusCode(500, new {mensaje = "Error al obtener el archivo ", detalle = ex.Message});
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
            catch(Exception ex)
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
                return Ok(new { mensaje = "Archivo eliminado correctamente9a80b6de-ff00-40cc-abb8-bc50de63f2d6"});
            }
            catch(Exception ex)
            {
                return StatusCode(500, new {mensaje = "Error al eliminar el archivo: ", detalle = ex.Message});
            }
        }

    }
}
