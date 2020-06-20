using System.Threading.Tasks;
using Haus.Identity.Core.Accounts;
using Haus.Identity.Core.Common.Storage;
using Haus.Identity.Core.Tests.Support;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts
{
    public class GetAllAccountsQueryHandlerTests
    {
        private readonly HausIdentityDbContext _context;
        private readonly GetAccountsQueryHandler _handler;

        public GetAllAccountsQueryHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateIdentityDbContext();
            _handler = new GetAccountsQueryHandler();
        }

        [Fact]
        public async Task WhenExecutedThenReturnsAllAccounts()
        {
            
        }
    }
}