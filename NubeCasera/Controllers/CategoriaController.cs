using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NubeCasera.Clases;
using NubeCasera.Servicios;
using DTOModels.DTOs;

namespace NubeCasera.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }


        [HttpPost("crear-categoria")]
        public async Task<IActionResult> CrearCategoriaAsync(CategoriaDTO_Add categoriaDto)
        {
            try
            {
                var categoriaCreada = await _categoriaService.CrearCategoriaAsync(categoriaDto);
                return Ok(categoriaCreada);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Mensaje = "Error al crear categoria ", e.Message });
            }
        }

        [HttpPatch("insertar-archivo-en-categoria/{ID_archivo_referencia}/{ID_Categoria}")]
        public async Task<IActionResult> InsertarArchivoEnCategoria(Guid ID_archivo_referencia, Guid ID_Categoria)
        {
            try
            {
                var resultado = await _categoriaService.InsertarArchivo(ID_archivo_referencia, ID_Categoria);
                return Ok(new { Mensaje = "Archivo insertado en categoria exitosamente", Exito = resultado });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Mensaje = "Error al insertar archivo en categoria", e.Message });
            }
        }

        [HttpDelete("eliminar-categoria/{ID_Categoria}")]
        public async Task<IActionResult> EliminarCategoria(Guid ID_Categoria)
        {
            try
            {
                var resultado = await _categoriaService.EliminarCategoria(ID_Categoria);
                return Ok(new { Mensaje = "Categoria eliminada exitosamente", Exito = resultado });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Mensaje = "Error al eliminar categoria", e.Message });
            }

        }
    }
}
