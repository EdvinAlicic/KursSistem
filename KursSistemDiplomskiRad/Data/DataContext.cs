using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Entities.Student> Student { get; set; }
        public DbSet<Entities.Kurs> Kurs { get; set; }
        public DbSet<Entities.Lekcije> Lekcije { get; set; }
    }
}
