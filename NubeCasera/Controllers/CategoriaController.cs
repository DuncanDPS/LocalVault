using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NubeCasera.Clases;
using NubeCasera.Servicios;
using NubeCasera.Dtos;

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
                return StatusCode(500, new {Mensaje = "Error al crear categoria ", e.Message});
            }
        }

        

    }
}
