using System;
using NubeCasera.Dtos;
using NubeCasera.Datos;
using NubeCasera.Clases;
namespace NubeCasera.Servicios;

public class CategoriaService : ICategoriaService
{

    private readonly AppDBContext _appDbContext;

    public CategoriaService(AppDBContext appDBContext)
    {
        _appDbContext = appDBContext;
    }

    public async Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoriaNueva)
    {
        // valida que la categoria no este vacia
        if(categoriaNueva == null)
        {
            throw new ArgumentNullException(nameof(categoriaNueva));
        }

        // crear la categoria
        var categoria = new Categoria
        {
            ID = Guid.NewGuid(),
            NombreCategoria = categoriaNueva.NombreCategoria, 
        };
        // añadir a bd
        await _appDbContext.categorias.AddAsync(categoria);
        await _appDbContext.SaveChangesAsync();

        var CategoriaDTO = new CategoriaDTO
        {
            Id = categoria.ID,
            NombreCategoria = categoria.NombreCategoria
        };

        return CategoriaDTO;

    }
}
