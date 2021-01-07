import {ModelFactory} from "../../../testing";
import {DeviceType} from "../models";
import {getDeviceDisplayText} from "./get-device-display-text";

describe('getDeviceDisplayText', () => {
  it('should have device type in text', () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light})

    expect(getDeviceDisplayText(device)).toContain('Light');
  })

  it('should have device name in text', () => {
    const device = ModelFactory.createDeviceModel({name: 'three'});

    expect(getDeviceDisplayText(device)).toContain('three');
  })

  it('should turn device type into title case', () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.MotionSensor});

    expect(getDeviceDisplayText(device)).toContain('Motion Sensor');
  })
})
