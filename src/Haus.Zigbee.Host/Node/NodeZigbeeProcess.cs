using System;
using System.Diagnostics;
using System.IO;
using Haus.Zigbee.Host.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Haus.Zigbee.Host.Node
{
    public interface INodeZigbeeProcess : IDisposable
    {
        void Start();
        void Stop();
    }

    public class NodeZigbeeProcess : INodeZigbeeProcess
    {
        private readonly IOptions<ZigbeeOptions> _options;
        private readonly ILogger<NodeZigbeeProcess> _logger;
        private readonly Process _process;

        private ZigbeeOptions Options => _options.Value;

        public NodeZigbeeProcess(IOptions<ZigbeeOptions> options, ILogger<NodeZigbeeProcess> logger)
        {
            _options = options;
            _logger = logger;
            _process = new Process
            {
                StartInfo = CreateStartInfo(Options)
            };
            _process.OutputDataReceived += OnOutputReceived;
            _process.ErrorDataReceived += OnErrorReceived;
        }

        public void Start()
        {
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
        }

        public void Stop()
        {
            _process.OutputDataReceived -= OnOutputReceived;
            _process.ErrorDataReceived -= OnErrorReceived;
            if (_process.HasExited)
                return;
            
            _process.Kill(true);
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogError($"[ZIGBEE] {e.Data}");
        }

        private void OnOutputReceived(object sender, DataReceivedEventArgs e)
        {
            _logger.LogInformation($"[ZIGBEE] {e.Data}");
        }

        private static ProcessStartInfo CreateStartInfo(ZigbeeOptions options)
        {
            var startInfo = new ProcessStartInfo("npm")
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = "run start"
            };
            startInfo.Environment["ZIGBEE2MQTT_CONFIG"] = Path.GetFullPath(options.ConfigFile);
            startInfo.Environment["ZIGBEE2MQTT_DATA"] = Path.GetFullPath(options.DataDirectory);
            return startInfo;
        }

        public void Dispose()
        {
            _process.Dispose();
        }
    }
}