using System;
using Haus.Identity.Core.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Identity.Core.Tests.Support
{
    public static class MessageBusFactory
    {
        public static IMessageBus Create(Action<ServiceProviderOptionsBuilder> configureProvider = null)
        {
            return ServiceProviderFactory.CreateProvider(configureProvider).GetRequiredService<IMessageBus>();
        }
    }
}