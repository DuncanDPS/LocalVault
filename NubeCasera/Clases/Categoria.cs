namespace NubeCasera.Clases
{
    public class Categoria
    {
        public Guid ID { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public Guid? CategoriaPadreID { get; set;} = null; 
        public Categoria? CategoriaPadre { get; set; }
        public List<Categoria>? SubCategorias { get; set; }
        public List<ArchivoReferencia>? archivosReferencias { get; set; }
    }
}
