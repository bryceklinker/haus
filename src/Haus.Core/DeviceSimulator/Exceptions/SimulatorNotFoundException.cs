using System;

namespace Haus.Core.DeviceSimulator.Exceptions;

public class SimulatorNotFoundException : Exception
{
    public SimulatorNotFoundException(string id)
        : base($"Could not find simulator with id {id}.")
    {
    }
}