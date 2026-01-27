using System;

namespace NubeCasera.Dtos;

public class ArchivoReferenciaDTO
{
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaDeSubida { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string TipoHash { get; set; } = string.Empty;
        public string RutaDeAlmacenamiento { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public bool EstaEliminado { get; set; }
        
        public DateTime? FechaDeEliminacion {get; set;}


        // Datos de la carpeta lógica que se mostrarán
        public Guid? CarpetaLogicaId { get; set; }
        public string? CarpetaLogicaNombre { get; set; }
}
