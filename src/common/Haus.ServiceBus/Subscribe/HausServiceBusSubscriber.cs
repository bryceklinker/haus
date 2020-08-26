using System;
using System.Threading.Tasks;
using Haus.ServiceBus.Common;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Haus.ServiceBus.Subscribe
{
    public interface IHausServiceBusSubscriber : IDisposable
    {
        void Subscribe(string queueName, Func<ReadOnlyMemory<byte>, Task> handler);
    }
    public class HausServiceBusSubscriber : IHausServiceBusSubscriber
    {
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly Lazy<IConnection> _lazyConnection;
        private readonly Lazy<IModel> _lazyModel;
        private AsyncEventingBasicConsumer _consumer;

        private IConnection Connection => _lazyConnection.Value;
        private IModel Model => _lazyModel.Value;
        
        public HausServiceBusSubscriber(IOptions<ServiceBusOptions> options)
        {
            _options = options;
            _lazyConnection = new Lazy<IConnection>(_options.Value.CreateConnection);
            _lazyModel = new Lazy<IModel>(CreateModel);
        }

        public void Subscribe(string queueName, Func<ReadOnlyMemory<byte>, Task> handler)
        {
            _consumer = new AsyncEventingBasicConsumer(Model);
            _consumer.Received += async (ch, ea) =>
            {
                await handler.Invoke(ea.Body);
            };
            Model.QueueDeclare(queueName);
            Model.QueueBind(queueName, _options.Value.ExchangeName, null); 
            Model.BasicConsume(queueName, true, _consumer);
        }

        public void Dispose()
        {
            CancelConsumers();
            if (_lazyModel.IsValueCreated) Model.Dispose();
            if (_lazyConnection.IsValueCreated) Connection.Dispose();
        }

        private IModel CreateModel()
        {
            return Connection.CreateModel();
        }

        private void CancelConsumers()
        {
            if (_consumer == null)
                return;

            foreach (var tag in _consumer.ConsumerTags) 
                Model.BasicCancel(tag);
        }
    }
}