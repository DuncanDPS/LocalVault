using NubeCasera.Clases;
using NubeCasera.Dtos;

namespace NubeCasera.Servicios
{
    /// <summary>
    /// Aqui se definen los metodos para el manejo de archivos de referencia
    /// </summary>
    public interface IArchivoReferenciaServicio
    {
        /// <summary>
        /// Invoca a todos los archivos de referencia
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ArchivoReferenciaDTO>> ObtenerArchivosReferencia();
        /// <summary>
        /// Obtenemos un archivo a traves de su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Devuelve un archivo segun su ID especifico</returns>
        public Task<ArchivoReferenciaDTO> ObtenerArchivoReferencia(Guid id);
        
        /// <summary>
        /// Sube un archivo de referencia
        /// </summary>
        /// <param name="archivoReferenciaDTO"></param>
        /// <returns></returns>
        public Task<ArchivoReferenciaDTO> SubirArchivoReferencia(ArchivoReferenciaDTO_Add archivoReferenciaDTO);
        
    }
}
