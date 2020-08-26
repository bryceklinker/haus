using System;
using RabbitMQ.Client;

namespace Haus.ServiceBus.Common
{
    public class ServiceBusOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Hostname { get; set; }
        internal string ExchangeName { get; set; } = "haus-service-bus";

        internal IConnection CreateConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = Hostname, 
                Password = Password, 
                UserName = Username, 
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            return connectionFactory.CreateConnection();
        }
    }
}