using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Haus.Core.Common;
using Haus.Core.Common.Entities;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Rooms;
using Haus.Core.Models.Rooms.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Rooms.Entities
{
    public class RoomEntity : Entity
    {
        public string Name { get; set; } = string.Empty;
        public Lighting Lighting { get; set; } = new Lighting();
        public ICollection<DeviceEntity> Devices { get; set; } = new List<DeviceEntity>();
        public IEnumerable<DeviceEntity> Lights => Devices.Where(d => d.DeviceType == DeviceType.Light);

        public static RoomEntity CreateFromModel(RoomModel model)
        {
            return new RoomEntity {Name = model.Name};
        }

        public void UpdateFromModel(RoomModel roomModel)
        {
            Name = roomModel.Name;
        }

        public void AddDevice(DeviceEntity device)
        {
            if (Devices.Any(d => d.Id == device.Id))
                return;

            Devices.Add(device);
            device.AssignToRoom(this);
        }

        public void AddDevices(IEnumerable<DeviceEntity> devices)
        {
            foreach (var device in devices) AddDevice(device);
        }

        public void RemoveDevice(DeviceEntity device)
        {
            Devices.Remove(device);
            device.UnAssignRoom();
        }

        public void ChangeLighting(Lighting lighting)
        {
            Lighting = lighting;
            foreach (var light in Lights)
                light.ChangeLighting(Lighting);
        }
    }

    public class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
    {
        public void Configure(EntityTypeBuilder<RoomEntity> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired();

            builder.OwnsOne(r => r.Lighting, Lighting.Configure);

            builder.HasMany(r => r.Devices)
                .WithOne(d => d.Room);
        }
    }
}