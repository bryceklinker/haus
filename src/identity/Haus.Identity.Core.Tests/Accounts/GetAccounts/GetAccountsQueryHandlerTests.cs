using System.Threading.Tasks;
using FluentAssertions;
using Haus.Cqrs;
using Haus.Identity.Core.Accounts.Entities;
using Haus.Identity.Core.Accounts.GetAccounts;
using Haus.Identity.Core.Accounts.Models;
using Haus.Identity.Core.Tests.Support;
using Haus.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Haus.Identity.Core.Tests.Accounts.GetAccounts
{
    public class GetAccountsQueryHandlerTests
    {
        private const string DefaultPassword = "abcXYZ123$";
        private readonly UserManager<HausUser> _userManager;
        private readonly GetAccountsQueryHandler _handler;

        public GetAccountsQueryHandlerTests()
        {
            _userManager = InMemoryUserManagerFactory.Create();
            _handler = new GetAccountsQueryHandler(_userManager);
        }

        [Fact]
        public async Task WhenExecutedThenReturnsAllAccounts()
        {
            await AddUserAsync("bill");
            await AddUserAsync("jack");
            await AddUserAsync("sue");

            var list = await _handler.Handle(new GetAccountsQuery());

            list.Items.Should().HaveCount(3);
        }

        [Fact]
        public async Task WhenSearchTermProvidedThenReturnsMatchesBasedOnUsername()
        {
            await AddUserAsync("one");
            await AddUserAsync("two");
            await AddUserAsync("three");

            var list = await _handler.Handle(new GetAccountsQuery("o"));

            list.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task WhenSearchTermProvidedThenReturnsMatchesWithDifferentCasing()
        {
            await AddUserAsync("one");
            await AddUserAsync("two");
            await AddUserAsync("three");

            var list = await _handler.Handle(new GetAccountsQuery("O"));

            list.Items.Should().HaveCount(2);
        }

        private async Task AddUserAsync(string userName)
        {
            await _userManager.CreateAsync(new HausUser {UserName = userName}, DefaultPassword);
        }

        private async Task<ListModel<HausUserModel>> Handle(GetAccountsQuery query)
        {
            var messageBus = MessageBusFactory.Create(opts =>
            {
                opts.WithUserManager(_userManager);
            });
            return await messageBus.ExecuteQuery(query);
        }
    }
}