using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Storage;
using Haus.Core.Devices.Entities;
using Haus.Core.Models.Devices;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public class UpdateDeviceCommand : UpdateEntityCommand<DeviceModel>
    {
        public UpdateDeviceCommand(DeviceModel model)
            : base(model)
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

            var device = await _context.FindByIdOrThrowAsync<DeviceEntity>(request.Id, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            
            device.UpdateFromModel(request.Model);
            await _context.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}