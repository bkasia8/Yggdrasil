using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Yggdrasil.Data.Access;

namespace Yggdrasil.Tests
{
    internal static class TestUtils
    {
        public static YggdrasilDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<YggdrasilDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging().Options;

            var context = new YggdrasilDbContext(options);

            context.Database.EnsureCreated();
            context.Database.EnsureDeleted();

            return context;
        }
    }
}
