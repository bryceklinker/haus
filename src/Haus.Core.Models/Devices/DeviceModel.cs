using System;
using Haus.Core.Models.Common;
using Haus.Core.Models.Lighting;

namespace Haus.Core.Models.Devices
{
    public record DeviceModel(
        long Id = -1, 
        long? RoomId = null, 
        string ExternalId = null, 
        string Name = null, 
        DeviceType DeviceType = DeviceType.Unknown,
        MetadataModel[] Metadata = null,
        LightingModel Lighting = null) : IdentityModel
    {
        public MetadataModel[] Metadata { get; } = Metadata ?? Array.Empty<MetadataModel>();
        public LightingModel Lighting { get; } = Lighting ?? LightingModel.Default;
    }
}