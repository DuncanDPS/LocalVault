using System;

namespace DTOModels.DTOs
{
    public class ArchivoReferenciaDTO_Add
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime? FechaDeSubida { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string TipoHash { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public Guid? CarpetaLogicaId { get; set; }
    }
}
