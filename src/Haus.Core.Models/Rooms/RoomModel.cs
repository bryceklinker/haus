using Haus.Core.Models.Common;

namespace Haus.Core.Models.Rooms
{
    public record RoomModel(long Id = -1, string Name = null, LightingModel Lighting = null) : IdentityModel
    {
        public LightingModel Lighting { get; } = Lighting ?? LightingModel.Default;
    }
}