using NubeCasera.Clases;

namespace NubeCasera.Dtos
{
    public class CategoriaDTO_Add
    {
        public string NombreCategoria { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty;
        public Guid? CategoriaPadreID { get; set; } = Guid.Empty;
        public Categoria? CategoriaPadre { get; set; } = null;
        public List<Categoria>? SubCategorias { get; set; } = null;
    }
}
