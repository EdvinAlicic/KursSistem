using Microsoft.EntityFrameworkCore;

namespace KursSistemDiplomskiRad.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Entities.Student> Studenti { get; set; }
        public DbSet<Entities.Kurs> Kursevi { get; set; }
        public DbSet<Entities.Lekcije> Lekcije { get; set; }
        public DbSet<Entities.StudentKurs> StudentKurs { get; set; }
        public DbSet<Entities.KursOcjena> KursOcjene { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.StudentKurs>()
                .HasKey(sc => new { sc.StudentId, sc.KursId });
            modelBuilder.Entity<Entities.StudentKurs>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentKursevi)
                .HasForeignKey(sc => sc.StudentId);
            modelBuilder.Entity<Entities.StudentKurs>()
                .HasOne(sc => sc.Kurs)
                .WithMany(k => k.StudentKursevi)
                .HasForeignKey(sc => sc.KursId);
        }
    }
}
