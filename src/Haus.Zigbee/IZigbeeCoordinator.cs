using System.Threading;
using System.Threading.Tasks;
using Haus.Zigbee.Coordinator;

namespace Haus.Zigbee;

// The single entry point the Haus.Zigbee library exposes to the outside world. Everything else in
// the assembly is an internal collaborator this façade composes; a consumer only programs against
// this contract.
public interface IZigbeeCoordinator
{
    bool IsConnected { get; }

    NetworkConfig? NetworkConfig { get; }

    Task ConnectAsync(CancellationToken token);
}
