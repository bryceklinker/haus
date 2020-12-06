using System.Collections.Generic;
using Haus.Core.Common.Entities;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Haus.Core.Rooms.Entities
{
    public class RoomEntity : Entity
    {
        public string Name { get; set; }

        public ICollection<DeviceEntity> Devices { get; set; } = new List<DeviceEntity>();

        public static RoomEntity CreateFromModel(RoomModel model)
        {
            return new RoomEntity { Name = model.Name };   
        }

        public void UpdateFromModel(RoomModel roomModel)
        {
            Name = roomModel.Name;
        }

        public void AddDevice(DeviceEntity device)
        {
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
    }

    public class RoomEntityConfiguration : IEntityTypeConfiguration<RoomEntity>
    {
        public void Configure(EntityTypeBuilder<RoomEntity> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired();

            builder.HasMany(r => r.Devices)
                .WithOne(d => d.Room);
        }
    }
}