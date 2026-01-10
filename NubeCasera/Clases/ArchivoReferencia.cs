namespace NubeCasera.Clases
{
    /// <summary>
    /// Esta clase servira como referencia para los archivos almacenados, se puede decir son los blobs que se almacenaran en la BD.
    /// </summary>
    public class ArchivoReferencia
    {
        public Guid ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaDeSubida { get; set; } = DateTime.UtcNow;
        public string Hash { get; set; } = string.Empty;
        public string TipoHash { get; set;  } = string.Empty;
        public string RutaDeAlmacenamiento { get; set;} = string.Empty;
        public string Extension { get; set;  } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public bool EstaEliminado { get; set; } = false;

        public Categoria? carpetaLogica { get; set; } = null;
        public Guid? carpetaLogicaID { get; set; }  // Agregar nullable

    }
}
