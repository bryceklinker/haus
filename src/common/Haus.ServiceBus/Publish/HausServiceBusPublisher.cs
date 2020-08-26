using Haus.ServiceBus.Common;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Haus.ServiceBus.Publish
{
    public interface IHausServiceBusPublisher
    {
        void Publish<TPayload>(ServiceBusMessage<TPayload> message);
    }
    
    public class HausServiceBusPublisher : IHausServiceBusPublisher
    {
        private readonly IOptions<ServiceBusOptions> _options;

        private string ExchangeName => _options.Value.ExchangeName;
        
        public HausServiceBusPublisher(IOptions<ServiceBusOptions> options)
        {
            _options = options;
        }
        
        public void Publish<TPayload>(ServiceBusMessage<TPayload> message)
        {
            using var connection = _options.Value.CreateConnection();
            using var model = connection.CreateModel();
            model.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            var properties = model.CreateBasicProperties();
            properties.ContentType = "application/json";
            model.BasicPublish(ExchangeName, "", properties, message.ToBytes());
        }
    }
}