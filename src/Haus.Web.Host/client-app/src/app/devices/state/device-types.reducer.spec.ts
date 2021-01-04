import {generateStateFromActions} from "../../../testing/app-state-generator";
import {deviceTypesReducer} from "./device-types.reducer";
import {DeviceTypesActions} from "./actions";
import {ModelFactory} from "../../../testing";
import {DeviceType} from "../../shared/models";

describe('deviceTypesReducer', () => {
  it('should populate device types in ascending order', () => {
    const result = ModelFactory.createListResult(
      DeviceType.LightSensor,
      DeviceType.MotionSensor,
      DeviceType.Light
    )
    const state = generateStateFromActions(deviceTypesReducer,
      DeviceTypesActions.loadDeviceTypes.success(result)
    );

    expect(state.deviceTypes[0]).toEqual(DeviceType.Light);
    expect(state.deviceTypes[1]).toEqual(DeviceType.LightSensor);
    expect(state.deviceTypes[2]).toEqual(DeviceType.MotionSensor);
  })
})
