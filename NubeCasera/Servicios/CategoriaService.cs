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

    public async Task<bool> InsertarArchivo(Guid ID_archivo_referencia, Guid ID_Categoria)
    {
        // como funciona este metodo ?

        // 1. validar que los GUID no sean nulos
        if(ID_archivo_referencia == Guid.Empty || ID_Categoria == Guid.Empty ) throw new KeyNotFoundException("Los ID son invalidos");

        // buscar el archivoReferencia segun su ID y Buscar la Categoria segun su ID
        var archivoRef = await _appDbContext.archivoReferencias.FindAsync(ID_archivo_referencia);
        var categoria = await _appDbContext.categorias.FindAsync(ID_Categoria);

        if(archivoRef == null || categoria == null)
        {
            throw new InvalidOperationException("No existe archivo referencia o categoria");
        }

        // TODO : CONTINUAR CON LA IMPLEMENTACION
        archivoRef.carpetaLogicaID = categoria.ID; // ASIGNAR LA CATEGORIA AL ARCHIVO REFERENCIA
        await _appDbContext.SaveChangesAsync();
        
        return true;
    }
}
