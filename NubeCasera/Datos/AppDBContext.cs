using Microsoft.EntityFrameworkCore;
using NubeCasera.Clases;
namespace NubeCasera.Datos
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        // DBSets
        public DbSet<ArchivoReferencia> archivoReferencias { get; set; }
        public DbSet<Categoria> categorias { get; set; }

        // ID para la categoria Principal
        public static readonly Guid CategoriaPrincipalId = Guid.Parse("00000000-0000-0000-0000-000000000001");


        // seed en categoria
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { ID = CategoriaPrincipalId, NombreCategoria = "Principal",
                FechaDeCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
                );

            // filtro para excluir los archivos eliminados
            modelBuilder.Entity<ArchivoReferencia>().HasQueryFilter(f => !f.EstaEliminado);

            // Índices para mejorar performance
            modelBuilder.Entity<ArchivoReferencia>()
                .HasIndex(f => f.EstaEliminado);

            modelBuilder.Entity<ArchivoReferencia>()
                .HasIndex(f => f.FechaDeEliminacion);
        }



    }
}
