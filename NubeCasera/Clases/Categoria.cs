namespace NubeCasera.Clases
{
    public class Categoria
    {
        public Guid ID { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public string Ruta { get; set;  } = string.Empty;
        public Guid? CategoriaPadreID { get; set;} = Guid.Empty; 
        public Categoria? CategoriaPadre { get; set; }
        public List<Categoria>? SubCategorias { get; set; }
    }
}
