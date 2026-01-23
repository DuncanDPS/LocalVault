using NubeCasera.Clases;
using NubeCasera.Datos;
using NubeCasera.Dtos;

namespace NubeCasera.Servicios
{
    /// <summary>
    /// Aqui se definen los metodos para el manejo de archivos de referencia
    /// </summary>
    public interface IArchivoReferenciaServicio
    {
       
        /// <summary>
        /// Devuelve los archivos por referencia de una categoria en especifico, si no se menciona un id, entonces
        /// se muestran los archivos de la categoria principal
        /// </summary>
        /// <param name="id">id de la categoria especifica opcional</param>
        /// <returns></returns>
        public Task<IEnumerable<ArchivoReferenciaDTO>> ObtenerArchivosAsync(Guid? id);
        /// <summary>
        /// Obtenemos un archivo a traves de su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Devuelve un archivo segun su ID especifico</returns>
        public Task<ArchivoReferenciaDTO> ObtenerArchivoAsync(Guid id);
        
        /// <summary>
        /// Sube un archivo de referencia
        /// </summary>
        /// <param name="archivoReferenciaDTO"></param>
        /// <returns></returns>
        public Task<ArchivoReferenciaDTO> SubirArchivoAsync(ArchivoReferenciaDTO_Add archivoReferenciaDTO);
        
        

    }
}
