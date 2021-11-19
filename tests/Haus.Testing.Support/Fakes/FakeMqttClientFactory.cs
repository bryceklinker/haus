using System;
using System.Collections.Generic;
using MQTTnet;
using MQTTnet.Adapter;
using MQTTnet.Client;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.LowLevelClient;
using MQTTnet.Server;

namespace Haus.Testing.Support.Fakes
{
    public class FakeMqttClientFactory : IMqttFactory
    {
        public FakeMqttClient Client { get; } = new();

        public IList<Func<IMqttFactory, IMqttServerAdapter>> DefaultServerAdapters { get; }
        public IMqttNetLogger DefaultLogger { get; }
        public IDictionary<object, object> Properties { get; }

        public IMqttClient CreateMqttClient()
        {
            return Client;
        }

        public IMqttClient CreateMqttClient(IMqttNetLogger logger)
        {
            return Client;
        }

        public IMqttClient CreateMqttClient(IMqttClientAdapterFactory adapterFactory)
        {
            return Client;
        }

        public IMqttClient CreateMqttClient(IMqttNetLogger logger, IMqttClientAdapterFactory adapterFactory)
        {
            return Client;
        }

        public IMqttFactory UseClientAdapterFactory(IMqttClientAdapterFactory clientAdapterFactory)
        {
            throw new NotImplementedException();
        }

        public ILowLevelMqttClient CreateLowLevelMqttClient()
        {
            throw new NotImplementedException();
        }

        public ILowLevelMqttClient CreateLowLevelMqttClient(IMqttNetLogger logger)
        {
            throw new NotImplementedException();
        }

        public ILowLevelMqttClient CreateLowLevelMqttClient(IMqttClientAdapterFactory clientAdapterFactory)
        {
            throw new NotImplementedException();
        }

        public ILowLevelMqttClient CreateLowLevelMqttClient(IMqttNetLogger logger, IMqttClientAdapterFactory clientAdapterFactory)
        {
            throw new NotImplementedException();
        }

        public IMqttServer CreateMqttServer()
        {
            throw new NotImplementedException();
        }

        public IMqttServer CreateMqttServer(IMqttNetLogger logger)
        {
            throw new NotImplementedException();
        }

        public IMqttServer CreateMqttServer(IEnumerable<IMqttServerAdapter> adapters)
        {
            throw new NotImplementedException();
        }

        public IMqttServer CreateMqttServer(IEnumerable<IMqttServerAdapter> adapters, IMqttNetLogger logger)
        {
            throw new NotImplementedException();
        }
    }
}