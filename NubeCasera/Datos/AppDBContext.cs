using Microsoft.EntityFrameworkCore;
using NubeCasera.Clases;
namespace NubeCasera.Datos
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base (options) { }

        // DBSets
        public DbSet<ArchivoReferencia> archivoReferencias { get; set; }
        public DbSet<Categoria> categorias { get; set; }

    }
}
