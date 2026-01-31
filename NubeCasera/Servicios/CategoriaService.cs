using System;
using NubeCasera.Dtos;
using NubeCasera.Datos;
namespace NubeCasera.Servicios;

public class CategoriaService : ICategoriaService
{

    private readonly AppDBContext _appDbContext;

    public CategoriaService(AppDBContext appDBContext)
    {
        _appDbContext = appDBContext;
    }

    public Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoriaNueva)
    {
        // valida que la categoria no este vacia
        if(categoriaNueva == null)
        {
            throw new ArgumentNullException(nameof(categoriaNueva));
        }




    }
}
