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
    public Task<CategoriaDTO> CrearCategoriaAsync(CategoriaDTO_Add categoriaNueva);

    /// <summary>
    /// Agrega o inserta un archivo en una categoria/carpeta logica
    /// </summary>
    /// <param name="ID_archivo_referencia">Id del archivo referencia. apartir de este id se construira su instancia, 
    /// y se añade la relacion con la categoria</param>
    /// <param name="ID_Categoria">Id de la categoria donde se quiere insertar el archivo</param>
    /// <returns>true si el archivo se inserto correctamente, de lo contrario false</returns>
    public Task<bool> InsertarArchivo(Guid ID_archivo_referencia, Guid ID_Categoria);

    // TODO: Implementar este metodo, tener en cuenta que Eliminar Categoria por su ID, sin envargo el usuario debe aceptar que se eliminaran todos los archivos referencia relacionados a esta categoria, es decir, se eliminara la carpeta logica y su contenido.
    public Task<bool> EliminarCategoria(Guid ID_Categoria);
    


}
