using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Tests.Support
{
    public class OptionsMonitorFake<T> : IOptionsMonitor<T>
    {
        private readonly List<OptionsListenerFake<T>> _listeners;
        public T CurrentValue { get; private set; }

        public OptionsMonitorFake(T value)
        {
            CurrentValue = value;
            _listeners = new List<OptionsListenerFake<T>>();
        }
        
        public T Get(string name)
        {
            return CurrentValue;
        }

        public IDisposable OnChange(Action<T, string> listener)
        {
            var handler = new OptionsListenerFake<T>(listener, obj => _listeners.Remove(obj));
            _listeners.Add(handler);
            return handler;
        }

        public void TriggerChange(T value, string name = null)
        {
            CurrentValue = value;
            foreach (var listener in _listeners) listener.Trigger(value, name);
        }
    }
}