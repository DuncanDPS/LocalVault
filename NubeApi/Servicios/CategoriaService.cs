using System;
using DTOModels.DTOs;
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

        // Validar que la categoría padre existe si se especifica
        if(categoriaNueva.CategoriaPadreID.HasValue && 
           categoriaNueva.CategoriaPadreID.Value != Guid.Empty)
        {
            var categoriaPadreExiste = await _appDbContext.categorias
                .AnyAsync(c => c.ID == categoriaNueva.CategoriaPadreID.Value);

            if(!categoriaPadreExiste)
            {
                throw new KeyNotFoundException("La categoría padre no existe");
            }
        }

        // crear la categoria
        var categoria = new Categoria
        {
            ID = Guid.NewGuid(),
            NombreCategoria = categoriaNueva.NombreCategoria,
            CategoriaPadreID = categoriaNueva.CategoriaPadreID
        };
        // añadir a bd
        await _appDbContext.categorias.AddAsync(categoria);
        await _appDbContext.SaveChangesAsync();

        // Cargar el nombre de la categoría padre si existe
        string? nombreCategoriaPadre = null;
        if(categoria.CategoriaPadreID.HasValue)
        {
            var padre = await _appDbContext.categorias
                .FindAsync(categoria.CategoriaPadreID.Value);
            nombreCategoriaPadre = padre?.NombreCategoria;
        }

        var categoriaDTO = new CategoriaDTO
        {
            Id = categoria.ID,
            NombreCategoria = categoria.NombreCategoria,
            CategoriaPadreID = categoria.CategoriaPadreID,
            CategoriaPadreNombre = nombreCategoriaPadre,
            CantidadArchivos = 0,
            FechaDeCreacion = categoria.FechaDeCreacion

        };

        return categoriaDTO;

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

    public async Task<List<CategoriaDTO>> ObtenerCategoriasAsync()
    {
        var categorias = await _appDbContext.categorias
    .Include(c => c.archivosReferencias)
    .ToListAsync();

        return categorias.Select(c => new CategoriaDTO
        {
            Id = c.ID,
            NombreCategoria = c.NombreCategoria,
            CategoriaPadreID = c.CategoriaPadreID,
            CantidadArchivos = c.archivosReferencias?.Count ?? 0,
            FechaDeCreacion = c.FechaDeCreacion

        }).ToList();
    }
}
