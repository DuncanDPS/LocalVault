using Microsoft.EntityFrameworkCore;

namespace NubeCasera.Datos
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base (options) { }

    }
}
