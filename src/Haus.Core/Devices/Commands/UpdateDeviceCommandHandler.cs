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
    public class UpdateDeviceCommand : UpdateEntityCommand<DeviceModel>
    {
        public UpdateDeviceCommand(long id, DeviceModel model)
            : base(id, model)
        {
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
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
                .ConfigureAwait(false);

            var device = await _context.FindByIdAsync<DeviceEntity>(request.Id, cancellationToken)
                .ConfigureAwait(false);
            if (device == null)
                throw new EntityNotFoundException<DeviceEntity>(request.Id);
            
            device.UpdateFromModel(request.Model);
            await _context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}