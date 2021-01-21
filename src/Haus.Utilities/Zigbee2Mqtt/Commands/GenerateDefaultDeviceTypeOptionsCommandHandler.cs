using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Haus.Core.Models;
using Haus.Cqrs.Commands;
using Haus.Utilities.Common.Cli;
using Haus.Utilities.Zigbee2Mqtt.GenerateDefaultDeviceTypeOptions;
using Haus.Zigbee.Host.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Haus.Utilities.Zigbee2Mqtt.Commands
{
    [Command("zigbee2mqtt", "generate-device-type-defaults")]
    public record GenerateDefaultDeviceTypeOptionsCommand : ICommand;
    
    public class GenerateDefaultDeviceTypeOptionsCommandHandler : AsyncRequestHandler<GenerateDefaultDeviceTypeOptionsCommand>, ICommandHandler<GenerateDefaultDeviceTypeOptionsCommand>
    {
        private const string SupportedDevicesPage = "https://www.zigbee2mqtt.io/information/supported_devices.html";
        private static readonly string DefaultDeviceTypeOptionsPath = Path.GetFullPath(Path.Combine("..", "Haus.Zigbee.Host", "Zigbee2Mqtt", "Resolvers", "DefaultDeviceTypeOptions.json"));
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GenerateDefaultDeviceTypeOptionsCommandHandler> _logger;
        private readonly IDeviceTypeOptionsHtmlParser _parser;
        private readonly IDeviceTypeOptionsMerger _merger;

        public GenerateDefaultDeviceTypeOptionsCommandHandler(
            IHttpClientFactory httpClientFactory, 
            ILogger<GenerateDefaultDeviceTypeOptionsCommandHandler> logger, 
            IDeviceTypeOptionsHtmlParser parser,
            IDeviceTypeOptionsMerger merger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _parser = parser;
            _merger = merger;
        }

        protected override async Task Handle(GenerateDefaultDeviceTypeOptionsCommand request, CancellationToken cancellationToken)
        {
            var html = await GetHtml(cancellationToken);
            var existingOptions = await ReadExistingOptions();
            var options = _parser.Parse(html).ToArray();
            _logger.LogInformation("Generated {number} device type options from html", options.Length);
            
            var merged = _merger.Merge(existingOptions, options).ToArray();
            _logger.LogInformation("Merged {number} device type options", merged.Length);

            await WriteDefaultsOptions(merged);
        }

        private async Task<string> GetHtml(CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient();
            return await client.GetStringAsync(SupportedDevicesPage, cancellationToken);
        }

        private async Task<DeviceTypeOptions[]> ReadExistingOptions()
        {
            _logger.LogInformation("Reading defaults from {filePath}", DefaultDeviceTypeOptionsPath);
            if (!File.Exists(DefaultDeviceTypeOptionsPath))
            {
                _logger.LogInformation("Could not find defaults file at {filePath}", DefaultDeviceTypeOptionsPath);
                return Array.Empty<DeviceTypeOptions>();
            }
            var defaultsJson = await File.ReadAllTextAsync(DefaultDeviceTypeOptionsPath);
            return  HausJsonSerializer.Deserialize<DeviceTypeOptions[]>(defaultsJson);
        }

        private async Task WriteDefaultsOptions(IEnumerable<DeviceTypeOptions> options)
        {
            _logger.LogInformation("Writing defaults to {filePath}", DefaultDeviceTypeOptionsPath);
            var json = HausJsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(DefaultDeviceTypeOptionsPath, json);
        }
    }
}