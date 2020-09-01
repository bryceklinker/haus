using System;
using System.Threading.Tasks;
using Haus.Identity.Web.Common.Storage;
using Haus.Identity.Web.Tests.Support;
using Haus.Identity.Web.Users.Commands;
using Haus.Identity.Web.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Haus.Identity.Web.Tests.Users.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly HausIdentityDbContext _context;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _context = InMemoryDbContextFactory.CreateIdentityContext();
            var userManager = new UserManager<HausUser>(
                new UserStore<HausUser>(_context),
                Options.Create(new IdentityOptions()),
                new PasswordHasher<HausUser>(Options.Create(new PasswordHasherOptions())),
                new []{new UserValidator<HausUser>()},
                new []{new PasswordValidator<HausUser>(), },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                new ServiceCollection().BuildServiceProvider(),
                new NullLogger<UserManager<HausUser>>());
            _handler = new CreateUserCommandHandler(userManager);
        }

        [Fact]
        public async Task WhenUserCreatedThenUserIsAddedToDatabase()
        {
            await _handler.Handle(new CreateUserCommand("admin", "##123abCD"));

            Assert.Single(_context.Users);
        }

        [Fact]
        public async Task WhenUserPasswordIsTooSimpleThenErrorIsReturned()
        {
            var result = await _handler.Handle(new CreateUserCommand("admin", "idk"));
            
            Assert.False(result.WasSuccessful);
            Assert.NotEmpty(result.Errors);
        }
    }
}