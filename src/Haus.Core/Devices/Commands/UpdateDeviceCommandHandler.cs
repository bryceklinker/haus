using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public class UpdateDeviceCommand : ICommand
    {
        public long Id { get; }
        public DeviceModel Model { get; }

        public UpdateDeviceCommand(long id, DeviceModel model)
        {
            Id = id;
            Model = model;
        }
    }

    internal class UpdateDeviceCommandHandler : AsyncRequestHandler<UpdateDeviceCommand>, ICommandHandler<UpdateDeviceCommand>
    {
        private readonly HausDbContext _context;
        private readonly IValidator<DeviceModel> _validator;

        public UpdateDeviceCommandHandler(HausDbContext context, IValidator<DeviceModel> validator)
        {
            _context = context;
            _validator = validator;
        }

        protected override async Task Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _context.FindByIdAsync<DeviceEntity>(request.Id, cancellationToken);
            if (device == null)
                throw new EntityNotFoundException<DeviceEntity>(request.Id);

            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken);
            device.UpdateFromModel(request.Model);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}