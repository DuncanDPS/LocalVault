using System;
using NubeCasera.Dtos;

namespace NubeCasera.Servicios;

/// <summary>
/// Interfaz que define los metodos de la logica para las carpetas logicas
/// </summary>
public interface ICategoriaService
{
    /// <summary>
    /// Crea una categoria nueva
    /// </summary>
    /// <param name="categoriaNueva">Obj categoria nueva</param>
    /// <returns></returns>
    // TODO: IMPLEMENTAR ESTE METODO
    public Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoriaNueva);



}
