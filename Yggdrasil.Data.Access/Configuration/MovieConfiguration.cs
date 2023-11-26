using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yggdrasil.Data.Entity;

namespace Yggdrasil.Data.Access.Configuration
{
    internal class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Director).WithMany(x => x.Movies)
                .HasForeignKey(x=>x.DirectorId).IsRequired();

            builder.HasMany(x => x.Actors).WithMany(x => x.Movies).UsingEntity<MovieActor>();
        }
    }
}
