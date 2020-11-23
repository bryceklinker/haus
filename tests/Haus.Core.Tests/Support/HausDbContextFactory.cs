using System;
using Haus.Core.Common.Storage;
using Microsoft.EntityFrameworkCore;

namespace Haus.Core.Tests.Support
{
    public static class HausDbContextFactory
    {
        public static HausDbContext Create()
        {
            var options = new DbContextOptionsBuilder<HausDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;

            return new HausDbContext(options);
        }
    }
}