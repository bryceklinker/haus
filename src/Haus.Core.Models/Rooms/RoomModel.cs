using Haus.Core.Models.Common;

namespace Haus.Core.Models.Rooms
{
    public class RoomModel : IModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        public LightingModel Lighting { get; set; }
    }
}