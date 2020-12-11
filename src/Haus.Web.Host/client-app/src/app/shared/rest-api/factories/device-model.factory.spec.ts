import {DeviceModelFactory} from "./device-model.factory";
import {ModelFactory} from "../../../../testing/model-factory";

describe('DeviceModelFactory', () => {
  it('should return device model with one device type', () => {
    const original = {
      ...ModelFactory.createDeviceModel(),
      deviceType: 'Light'
    };

    const actual = DeviceModelFactory.create(original);

    expect(actual.deviceType).toEqual(['Light']);
  })

  it('should return device model with multiple device types', () => {
    const original = {
      ...ModelFactory.createDeviceModel(),
      deviceType: 'LightSensor, MotionSensor, TemperatureSensor'
    };

    const actual = DeviceModelFactory.create(original);

    expect(actual.deviceType).toEqual(['LightSensor', 'MotionSensor', 'TemperatureSensor']);
  })
})
