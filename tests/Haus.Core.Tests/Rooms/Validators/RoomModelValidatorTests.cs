using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Haus.Core.Common.Storage;
using Haus.Core.Models.Rooms;
using Haus.Core.Rooms.Validators;
using Haus.Testing.Support;
using Xunit;

namespace Haus.Core.Tests.Rooms.Validators
{
    public class RoomModelValidatorTests
    {
        private readonly HausDbContext _context;
        private readonly RoomModelValidator _validator;

        public RoomModelValidatorTests()
        {
            _context = HausDbContextFactory.Create();
            _validator = new RoomModelValidator(_context);
        }

        [Fact]
        public async Task WhenNameIsMissingThenReturnsInvalid()
        {
            var result = await _validator.TestValidateAsync(new RoomModel(Name: null));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task WhenNameIsDuplicateThenReturnsInvalid()
        {
            _context.AddRoom("one");

            var result = await _validator.TestValidateAsync(new RoomModel(Name: "one"));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task WhenNameIsDuplicateWithTheSameIdThenReturnsValid()
        {
            var room = _context.AddRoom("three");

            var result = await _validator.TestValidateAsync(new RoomModel(room.Id, "three"));

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task WhenNameIsUniqueThenReturnsValid()
        {
            _context.AddRoom("one");

            var result = await _validator.TestValidateAsync(new RoomModel(Name: "two"));

            Assert.True(result.IsValid);
        }
    }
}