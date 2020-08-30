using System;
using Haus.Portal.Web.Common.Storage;
using Microsoft.EntityFrameworkCore;

namespace Haus.Portal.Web.Tests.Support
{
    public static class InMemoryDbContextFactory
    {
        public static HausPortalDbContext CreatePortalContext()
        {
            var options = new DbContextOptionsBuilder<HausPortalDbContext>()
                .UseInMemoryDatabase($"{Guid.NewGuid()}")
                .Options;

            return new HausPortalDbContext(options);
        } 
    }
}