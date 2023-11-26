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
    public class DirectoryConfiguration : IEntityTypeConfiguration<Director>
    {
        public void Configure(EntityTypeBuilder<Director> builder)
        {
            builder.HasKey(x => x.Id);
            builder
              .Property(b => b.FirstName)
              .IsRequired();

            builder
              .Property(b => b.LastName)
              .IsRequired();

            builder.HasMany(x => x.Movies)
                .WithOne(x => x.Director)
                .HasForeignKey(x => x.DirectorId);
        }
    }
}
