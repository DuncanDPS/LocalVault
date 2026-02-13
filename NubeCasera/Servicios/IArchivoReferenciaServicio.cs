using NubeCasera.Clases;
using NubeCasera.Datos;
using DTOModels.DTOs;

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
        public Task<ArchivoReferenciaDTO> GuardarArchivoAsync(ArchivoReferenciaDTO_Add archivoReferenciaDTO, IFormFile archivoFisico);

        /// <summary>
        /// Este metodo sirve para calcular el hash de los archivos
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="tipoHash"></param>
        /// <returns></returns>
        public Task<string> CalcularHashArchivoAsync(Stream stream, string tipoHash);

        
        /// <summary>
        /// Metodo que devuelve el stream del archivo, para luego ser utilizado para construir el File que se retornara al usuario
        /// </summary>
        /// <param name="id">Id de la referencia del archivo almacenado en la base de datos</param>
        /// <returns></returns>
        public Task<Stream> DescargarAsync(Guid id);


        /// <summary>
        /// Elimina un archivo segun su id especifico
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task ELiminarAsync(Guid id);
    }
}
