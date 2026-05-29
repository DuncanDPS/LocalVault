using System;

namespace DTOModels.DTOs
{
    public class CategoriaDTO_Add
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public Guid? CategoriaPadreID { get; set; } = null;
    }
}
