using System;
using NubeCasera.Dtos;
using NubeCasera.Datos;
using NubeCasera.Clases;
using Microsoft.EntityFrameworkCore;
namespace NubeCasera.Servicios;

public class CategoriaService : ICategoriaService
{

    private readonly AppDBContext _appDbContext;
    private readonly IArchivoReferenciaServicio _archivoReferenciaServicio;

    public CategoriaService(AppDBContext appDBContext, IArchivoReferenciaServicio archivoReferenciaServicio)
    {
        _appDbContext = appDBContext;
        _archivoReferenciaServicio = archivoReferenciaServicio;
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

    public async Task<bool> EliminarCategoria(Guid ID_Categoria)
    {
        // validar que el ID no sea nulo
        if (ID_Categoria == Guid.Empty) throw new KeyNotFoundException("El ID es invalido");

        // No permitir eliminar la categoria principal
        if(ID_Categoria == AppDBContext.CategoriaPrincipalId)
        {
            throw new InvalidOperationException("No se puede eliminar la categoria Principal");
        }

        // se busca la categoria
        var categoria = await _appDbContext.categorias.Include(c => c.archivosReferencias).Include(c => c.SubCategorias).FirstOrDefaultAsync(c => c.ID == ID_Categoria);

        if(categoria == null)
        {
            throw new KeyNotFoundException($"No existe la categoria con ID: {ID_Categoria}");
        }

        // VALIDAR SI TIENE SUBCATEGORIAS
        if(categoria.SubCategorias != null && categoria.SubCategorias.Any())
        {
            throw new InvalidOperationException("No se puede eliminar una categoría que tiene subcategorías. Elimínelas primero.");
        }

        // Eliminar todos los archivos asociados usando el servicio
        if (categoria.archivosReferencias != null && categoria.archivosReferencias.Any())
        {
            foreach (var archivo in categoria.archivosReferencias.ToList())
            {
                await _archivoReferenciaServicio.ELiminarAsync(archivo.ID);
            }
        }

        // Eliminar la categoría
        _appDbContext.categorias.Remove(categoria);
        await _appDbContext.SaveChangesAsync();

        return true;

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

        
        archivoRef.carpetaLogicaID = categoria.ID; // ASIGNAR LA CATEGORIA AL ARCHIVO REFERENCIA
        await _appDbContext.SaveChangesAsync();
        
        return true;
    }


}
