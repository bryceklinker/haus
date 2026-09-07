using System;

namespace Haus.Zigbee.Coordinator;

public class CommandDeliveryFailedException : Exception
{
    public CommandDeliveryFailedException(byte lastConfirmStatus, int attemptCount)
        : base($"Command delivery failed after {attemptCount} attempts. Last confirm status: 0x{lastConfirmStatus:x2}")
    {
        LastConfirmStatus = lastConfirmStatus;
        AttemptCount = attemptCount;
    }

    public byte LastConfirmStatus { get; }

    public int AttemptCount { get; }
}
