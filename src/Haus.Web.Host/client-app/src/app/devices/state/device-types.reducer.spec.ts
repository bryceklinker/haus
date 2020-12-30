import {generateStateFromActions} from "../../../testing/app-state-generator";
import {deviceTypesReducer} from "./device-types.reducer";
import {DeviceTypesActions} from "./actions";
import {ModelFactory} from "../../../testing";

describe('deviceTypesReducer', () => {
  it('should populate device types in ascending order', () => {
    const result = ModelFactory.createListResult(
      'b',
      'c',
      'a'
    )
    const state = generateStateFromActions(deviceTypesReducer,
      DeviceTypesActions.loadDeviceTypes.success(result)
    );

    expect(state.deviceTypes[0]).toContainEqual('a');
    expect(state.deviceTypes[1]).toContainEqual('b');
    expect(state.deviceTypes[2]).toContainEqual('c');
  })
})
