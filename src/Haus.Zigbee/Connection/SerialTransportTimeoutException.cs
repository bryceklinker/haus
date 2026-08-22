using System;

namespace Haus.Zigbee.Connection;

// Raised when a DeconzChannel round trip doesn't complete within its bounded timeout. Linux's
// System.IO.Ports.SerialPort can leave a write or read against a torn-down port hanging forever
// instead of throwing, so this is the only signal callers get that the transport had to be
// disposed to unstick it.
public class SerialTransportTimeoutException(TimeSpan timeout)
    : Exception($"Serial round-trip did not complete within {timeout}.");
