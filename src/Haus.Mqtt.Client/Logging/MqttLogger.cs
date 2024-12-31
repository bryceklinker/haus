using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics;

namespace Haus.Mqtt.Client.Logging;

public class MqttLogger : IMqttNetLogger
{
    private readonly ILogger<MqttLogger> _logger;

    public MqttLogger(ILogger<MqttLogger> logger)
    {
        _logger = logger;
    }

    public void Publish(MqttNetLogLevel level, string source, string message, object[] parameters, Exception exception)
    {
        var convertedLogLevel = ConvertLogLevel(level);
        _logger.Log(convertedLogLevel, exception, message, parameters);

        var logMessagePublishedEvent = LogMessagePublished;
        if (logMessagePublishedEvent != null)
        {
            var logMessage = new MqttNetLogMessage
            {
                Timestamp = DateTime.UtcNow,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source,
                Level = level,
                Message = message,
                Exception = exception
            };

            logMessagePublishedEvent.Invoke(this, new MqttNetLogMessagePublishedEventArgs(logMessage));
        }
    }

    public bool IsEnabled { get; } = true;

    public event EventHandler<MqttNetLogMessagePublishedEventArgs> LogMessagePublished;

    private static LogLevel ConvertLogLevel(MqttNetLogLevel logLevel)
    {
        return logLevel switch
        {
            MqttNetLogLevel.Error => LogLevel.Error,
            MqttNetLogLevel.Warning => LogLevel.Warning,
            MqttNetLogLevel.Info => LogLevel.Information,
            MqttNetLogLevel.Verbose => LogLevel.Debug,
            _ => LogLevel.Debug
        };
    }
}