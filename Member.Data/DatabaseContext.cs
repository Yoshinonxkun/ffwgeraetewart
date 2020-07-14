using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Member.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<Psa> Psas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=data.db",
                options => { options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName); });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>().ToTable("Members");
            modelBuilder.Entity<Member>(entity => { entity.HasKey(e => e.MemberId); });
            modelBuilder.Entity<Psa>().ToTable("Psa");
            modelBuilder.Entity<Psa>(entity => { entity.HasKey(e => e.PsaId); });
            base.OnModelCreating(modelBuilder);
        }
    }
}