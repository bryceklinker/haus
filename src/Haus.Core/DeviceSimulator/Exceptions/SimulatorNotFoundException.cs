using System;

namespace Haus.Core.DeviceSimulator.Exceptions;

public class SimulatorNotFoundException(string id) : Exception($"Could not find simulator with id {id}.");
