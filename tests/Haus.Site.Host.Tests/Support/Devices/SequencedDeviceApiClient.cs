using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Haus.Api.Client.Devices;
using Haus.Core.Models.Common;
using Haus.Core.Models.Devices;
using Haus.Core.Models.Lighting;

namespace Haus.Site.Host.Tests.Support.Devices;

public class SequencedDeviceApiClient : IDeviceApiClient
{
    private readonly Queue<TaskCompletionSource<ListResult<DeviceModel>>> _getDevicesResponses = new();

    public string ApiBaseUrl => "";
    public string BaseUrl => "";

    public TaskCompletionSource<ListResult<DeviceModel>> EnqueueGetDevicesResponse()
    {
        var response = new TaskCompletionSource<ListResult<DeviceModel>>();
        _getDevicesResponses.Enqueue(response);
        return response;
    }

    public Task<ListResult<DeviceModel>> GetDevicesAsync(string? externalId = null)
    {
        return _getDevicesResponses.Dequeue().Task;
    }

    public Task<DeviceModel?> GetDeviceAsync(long id) => throw new NotSupportedException();

    public Task<ListResult<DeviceType>> GetDeviceTypesAsync() => throw new NotSupportedException();

    public Task<ListResult<LightType>> GetLightTypesAsync() => throw new NotSupportedException();

    public Task UpdateDeviceAsync(long deviceId, DeviceModel model) => throw new NotSupportedException();

    public Task DeleteDeviceAsync(long deviceId) => throw new NotSupportedException();

    public Task<HttpResponseMessage> ChangeDeviceLightingAsync(long deviceId, LightingModel model) =>
        throw new NotSupportedException();

    public Task<HttpResponseMessage> ChangeDeviceLightingConstraintsAsync(
        long deviceId,
        LightingConstraintsModel model
    ) => throw new NotSupportedException();

    public Task<HttpResponseMessage> TurnLightOffAsync(long deviceId) => throw new NotSupportedException();

    public Task<HttpResponseMessage> TurnLightOnAsync(long deviceId) => throw new NotSupportedException();
}
