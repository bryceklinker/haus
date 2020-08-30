using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Haus.ServiceBus.Publish
{
    public interface IHausServiceBusPublisher
    {
        Task PublishAsync<TPayload>(TPayload payload);
    }
    
    public class HausServiceBusPublisher : IHausServiceBusPublisher
    {
        private readonly ILogger _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public HausServiceBusPublisher(ILoggerFactory loggerFactory, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _logger = loggerFactory.CreateLogger<HausServiceBusPublisher>();
        }
        
        public async Task PublishAsync<TPayload>(TPayload payload)
        {
            _logger.LogInformation("Publishing message to service bus...");
            await _publishEndpoint.Publish(payload);
            _logger.LogInformation("Published message to service bus.");
        }
    }
}