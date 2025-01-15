using Fluxor;
using Haus.Core.Models.Devices;

namespace Haus.Site.Host.Shared.State.DeviceTypes;



[FeatureState]
public record DeviceTypesState : ListState<DeviceType>;


public static class DeviceTypesReducer
{
    [ReducerMethod(typeof(LoadListRequestAction<DeviceType>))]
    public static DeviceTypesState ReduceLoadRequest(DeviceTypesState state)
    {
        return state with {IsLoading = true, Error = null};
    }

    [ReducerMethod]
    public static DeviceTypesState ReduceLoadSuccess(DeviceTypesState state, LoadListSuccessAction<DeviceType> action)
    {
        return state with
        {
            IsLoading = false,
            Error = null,
            Items = state.Items
        };
    }

    [ReducerMethod]
    public static DeviceTypesState ReduceLoadFailed(DeviceTypesState state, LoadListFailedAction<DeviceType> action)
    {
        return state with
        {
            IsLoading = false,
            Error = action.Error,
        };
    }
}