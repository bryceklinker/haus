import {DeviceModel, DeviceType} from "../../../devices/models";

export class DeviceModelFactory {
  static create(model: any): DeviceModel {
    const deviceTypes = typeof model.deviceType == 'string'
      ? model.deviceType.split(',').map((type: DeviceType) => type.trim())
      : model.deviceType;
    return {
      ...model,
      deviceType: deviceTypes
    };
  }
}
