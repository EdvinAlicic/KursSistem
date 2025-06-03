using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Student> Studenti { get; set; }
        public DbSet<Kurs> Kursevi { get; set; }
        public DbSet<Lekcije> Lekcije { get; set; }
        public DbSet<StudentKurs> StudentKurs { get; set; }
        public DbSet<KursOcjena> KursOcjene { get; set; }
        public DbSet<StudentLekcijaProgress> StudentLekcijaProgress { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<StudentLekcijaProgress>()
                .HasOne(slp => slp.Lekcija)
                .WithMany()
                .HasForeignKey(slp => slp.LekcijaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<StudentLekcijaProgress>()
                .HasOne(slp => slp.Student)
                .WithMany()
                .HasForeignKey(slp => slp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentLekcijaProgress>()
                .HasOne(slp => slp.Kurs)
                .WithMany()
                .HasForeignKey(slp => slp.KursId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentKurs>()
                .HasKey(sc => new { sc.StudentId, sc.KursId });

            modelBuilder.Entity<StudentKurs>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentKursevi)
                .HasForeignKey(sc => sc.StudentId);
            modelBuilder.Entity<StudentKurs>()
                .HasOne(sc => sc.Kurs)
                .WithMany(k => k.StudentKursevi)
                .HasForeignKey(sc => sc.KursId);
        }
    }
}
