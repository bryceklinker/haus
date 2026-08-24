using System;
using Haus.Zigbee.Connection;
using Haus.Zigbee.Transport;

namespace Haus.Zigbee.Coordinator;

// Bundles every collaborator that depends on the coordinator's current transport instance.
// ZigbeeCoordinator holds exactly one mutable reference to one of these and swaps it in a single
// atomic assignment on reconnect, so a concurrent caller reading that one field always sees a
// fully-built, internally-consistent set (e.g. never a rebuilt DeconzChannel paired with the
// previous ApsSender) instead of a torn mix of old and new collaborators.
internal sealed class TransportComponents(
    ISerialTransport transport,
    DeconzChannel channel,
    DeconzConnection connection,
    ApsPollLoop pollLoop,
    PermitJoinController permitJoinController,
    ApsSender sender,
    CommandSender commandSender,
    AttributeReportListener attributeReportListener,
    DeviceInterview deviceInterview,
    NetworkAddressResolver networkAddressResolver
) : IDisposable
{
    public ISerialTransport Transport { get; } = transport;
    public DeconzChannel Channel { get; } = channel;
    public DeconzConnection Connection { get; } = connection;
    public ApsPollLoop PollLoop { get; } = pollLoop;
    public PermitJoinController PermitJoinController { get; } = permitJoinController;
    public ApsSender Sender { get; } = sender;
    public CommandSender CommandSender { get; } = commandSender;
    public AttributeReportListener AttributeReportListener { get; } = attributeReportListener;
    public DeviceInterview DeviceInterview { get; } = deviceInterview;
    public NetworkAddressResolver NetworkAddressResolver { get; } = networkAddressResolver;

    // Channel, Connection, PollLoop, PermitJoinController, and CommandSender hold no unmanaged
    // resources and subscribe to nothing on their own -- only the members disposed below do, so
    // only those need tearing down here.
    public void Dispose()
    {
        NetworkAddressResolver.Dispose();
        DeviceInterview.Dispose();
        AttributeReportListener.Dispose();
        Sender.Dispose();
        Transport.Dispose();
    }
}
