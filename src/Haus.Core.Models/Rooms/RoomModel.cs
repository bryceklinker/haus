using Haus.Core.Models.Common;

namespace Haus.Core.Models.Rooms
{
    public record RoomModel : IModel
    {
        public long Id { get; set; }
        public string Name { get; }
        
        public LightingModel Lighting { get; }

        public RoomModel(long id = -1, string name = null, LightingModel lighting = null)
        {
            Id = id;
            Name = name;
            Lighting = lighting ?? new LightingModel();
        }
    }
}