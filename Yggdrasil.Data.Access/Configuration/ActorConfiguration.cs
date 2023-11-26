using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yggdrasil.Data.Entity;

namespace Yggdrasil.Data.Access.Configuration
{
    internal class ActorConfiguration : IEntityTypeConfiguration<Actor>
    {
        public void Configure(EntityTypeBuilder<Actor> builder)
        {
            builder.HasKey(x => x.Id);
            builder
                .Property(b => b.FirstName)
                .IsRequired();

            builder
              .Property(b => b.LastName)
              .IsRequired();

            builder.HasMany(x => x.Movies).WithMany(x => x.Actors).UsingEntity<MovieActor>();
        }
    }
}
