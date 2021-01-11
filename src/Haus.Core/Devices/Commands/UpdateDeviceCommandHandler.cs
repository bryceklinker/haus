using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Haus.Core.Common.Commands;
using Haus.Core.Common.Events;
using Haus.Core.Devices.Repositories;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Devices.Events;
using Haus.Cqrs;
using Haus.Cqrs.Commands;
using MediatR;

namespace Haus.Core.Devices.Commands
{
    public record UpdateDeviceCommand(DeviceModel Model) : UpdateEntityCommand<DeviceModel>(Model);

    internal class UpdateDeviceCommandHandler : AsyncRequestHandler<UpdateDeviceCommand>, ICommandHandler<UpdateDeviceCommand>
    {
        private readonly IDeviceCommandRepository _repository;
        private readonly IValidator<DeviceModel> _validator;
        private readonly IHausBus _hausBus;

        public UpdateDeviceCommandHandler(IValidator<DeviceModel> validator, IHausBus hausBus, IDeviceCommandRepository repository)
        {
            _validator = validator;
            _hausBus = hausBus;
            _repository = repository;
        }

        protected override async Task Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
        {
            await _validator.HausValidateAndThrowAsync(request.Model, cancellationToken)
                .ConfigureAwait(false);

            var device = await _repository.GetById(request.Id, cancellationToken)
                .ConfigureAwait(false);
            
            device.UpdateFromModel(request.Model, _hausBus);
            await _repository.SaveAsync(device, cancellationToken).ConfigureAwait(false);

            await _hausBus.PublishAsync(RoutableEvent.FromEvent(new DeviceUpdatedEvent(device.ToModel())), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}