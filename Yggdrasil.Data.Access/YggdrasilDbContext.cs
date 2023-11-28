using Microsoft.EntityFrameworkCore;
using Yggdrasil.Data.Access.Configuration;
using Yggdrasil.Data.Entity;

namespace Yggdrasil.Data.Access
{
    public class YggdrasilDbContext : DbContext
    {
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Director> Director { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new ActorConfiguration());
            modelBuilder.ApplyConfiguration(new DirectoryConfiguration());
        }
        public YggdrasilDbContext(DbContextOptions<YggdrasilDbContext> options):base(options) { 
        }
    }
}
