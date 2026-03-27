using System;
using System.Collections.Generic;

namespace DTOModels.DTOs
{
    public class CategoriaDTO
    {
        public Guid Id { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public Guid? CategoriaPadreID { get; set; }
        public string? CategoriaPadreNombre { get; set; }
        public List<CategoriaDTO>? SubCategorias { get; set; }
        public int CantidadArchivos { get; set; }
        public DateTime FechaDeCreacion { get; init; }
    }
}
