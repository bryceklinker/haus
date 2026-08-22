using System;

namespace Haus.Zigbee.Connection;

// A caller catching this far from DeconzChannel needs to know: the underlying transport has
// already been disposed to unstick the hung round trip, not just abandoned.
public class SerialTransportTimeoutException(TimeSpan timeout)
    : Exception($"Serial round-trip did not complete within {timeout}.");
