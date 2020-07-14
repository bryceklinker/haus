using System.Threading.Tasks;
using FluentAssertions;
using Haus.Identity.Core.Tests.Support;
using Haus.Identity.Core.Users.Entities;
using Haus.Identity.Core.Users.GetUsers;
using Haus.Identity.Core.Users.Models;
using Haus.Models;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Haus.Identity.Core.Tests.Users.GetUsers
{
    public class GetUsersQueryHandlerTests
    {
        private const string DefaultPassword = "abcXYZ123$";
        private readonly UserManager<HausUser> _userManager;
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryHandlerTests()
        {
            _userManager = InMemoryUserManagerFactory.Create();
            _handler = new GetUsersQueryHandler(_userManager);
        }

        [Fact]
        public async Task WhenExecutedThenReturnsAllAccounts()
        {
            await AddUserAsync("bill");
            await AddUserAsync("jack");
            await AddUserAsync("sue");

            var list = await _handler.Handle(new GetUsersQuery());

            list.Items.Should().HaveCount(3);
        }

        [Fact]
        public async Task WhenSearchTermProvidedThenReturnsMatchesBasedOnUsername()
        {
            await AddUserAsync("one");
            await AddUserAsync("two");
            await AddUserAsync("three");

            var list = await _handler.Handle(new GetUsersQuery("o"));

            list.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task WhenSearchTermProvidedThenReturnsMatchesWithDifferentCasing()
        {
            await AddUserAsync("one");
            await AddUserAsync("two");
            await AddUserAsync("three");

            var list = await _handler.Handle(new GetUsersQuery("O"));

            list.Items.Should().HaveCount(2);
        }

        private async Task AddUserAsync(string userName)
        {
            await _userManager.CreateAsync(new HausUser {UserName = userName}, DefaultPassword);
        }

        private async Task<ListModel<HausUserModel>> Handle(GetUsersQuery query)
        {
            var messageBus = MessageBusFactory.Create(opts =>
            {
                opts.WithUserManager(_userManager);
            });
            return await messageBus.ExecuteQuery(query);
        }
    }
}