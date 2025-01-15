using System;
using System.Threading.Tasks;
using Fluxor;
using Haus.Api.Client.Devices;
using Haus.Core.Models.Devices;

namespace Haus.Site.Host.Shared.State.DeviceTypes;

public class DeviceTypesEffects(IDeviceApiClient client)
{
    [EffectMethod(typeof(LoadListRequestAction<DeviceType>))]
    public async Task LoadDeviceTypesAsync(IDispatcher dispatcher)
    {
        try
        {
            var deviceTypes = await client.GetDeviceTypesAsync();
            dispatcher.Dispatch(new LoadListSuccessAction<DeviceType>(deviceTypes));
        }
        catch (Exception e)
        {
            dispatcher.Dispatch(new LoadListFailedAction<DeviceType>(e));
        }
        
    }
}