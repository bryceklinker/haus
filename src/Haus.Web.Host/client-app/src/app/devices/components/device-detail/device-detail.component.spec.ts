import {ModelFactory, TestingEventEmitter} from "../../../../testing";
import {DeviceDetailComponent} from "./device-detail.component";
import {DeviceModel, DeviceType, LightType} from "../../../shared/models";
import {DeviceLightingConstraintsModel} from "../../models";
import {DeviceDetailHarness} from "./device-detail.harness";

describe('DeviceDetailComponent', () => {
  it('should show device information', async () => {
    const device = ModelFactory.createDeviceModel({
      deviceType: DeviceType.MotionSensor
    });

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.nameField).toHaveValue(device.name);
    expect(harness.externalIdField).toHaveValue(device.externalId);
    expect(harness.deviceTypeField).toHaveValue(DeviceType.MotionSensor);
  })

  it('should show not available when no device provided', async () => {
    const harness = await DeviceDetailHarness.render();

    expect(harness.nameField).toHaveValue('N/A');
    expect(harness.externalIdField).toHaveValue('N/A');
  })

  it('should keep readonly data inputs disabled', async () => {
    const device = ModelFactory.createDeviceModel();

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.externalIdField).toBeDisabled();
    expect(harness.deviceTypeField).toBeDisabled();
  })

  it('should be readonly when no device provided', async () => {
    const harness = await DeviceDetailHarness.render();

    expect(harness.nameField).toBeDisabled();
    expect(harness.externalIdField).toBeDisabled();
  })

  it('should show device metadata', async () => {
    const device = ModelFactory.createDeviceModel({
      metadata: [
        {key: 'Vendor', value: 'Philips'},
        {key: 'Model', value: '2342'}
      ]
    });

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.metadata).toHaveLength(2);
    expect(harness.container).toHaveTextContent('Philips');
    expect(harness.container).toHaveTextContent('2342');
  })

  it('should show lighting when device is a light', async () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light});

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.lighting).toBeInTheDocument();
    expect(harness.lightingConstraints).toBeInTheDocument();
  })

  it('should hide lighting when device is not a light', async () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.MotionSensor});

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.lighting).not.toBeInTheDocument();
    expect(harness.lightingConstraints).not.toBeInTheDocument();
  })

  it('should notify when lighting constraints are saved', async () => {
    const device = ModelFactory.createDeviceModel({
      deviceType: DeviceType.Light,
      lighting: ModelFactory.createLighting({
        level: ModelFactory.createLevelLighting({min: 12, max: 90})
      })
    });
    const emitter = new TestingEventEmitter<DeviceLightingConstraintsModel>();

    const harness = await DeviceDetailHarness.render({device, saveConstraints: emitter});
    harness.saveConstraints();

    expect(emitter.emit).toHaveBeenCalledWith({
      device,
      constraints: {
        minLevel: 12,
        maxLevel: 90
      }
    })
  })

  it('should show light types', async () => {
    const lightTypes = [LightType.Color, LightType.Level];
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light});

    const harness = await DeviceDetailHarness.render({device, lightTypes});

    expect(await harness.getLightTypesOptions()).toHaveLength(2);
  })

  it('should save using light type', async () => {
    const lightTypes = [LightType.Color, LightType.Level];
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light});
    const emitter = new TestingEventEmitter<DeviceModel>();

    const harness = await DeviceDetailHarness.render({device, lightTypes, saveDevice: emitter});
    await harness.selectLightType(LightType.Level);
    harness.saveDevice();

    expect(emitter.emit).toHaveBeenCalledWith(expect.objectContaining({
      lightType: LightType.Level
    }))
  })

  it('should notify when device is saved', async () => {
    const lightTypes = [LightType.Color, LightType.Level];
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light, lightType: LightType.Level});
    const emitter = new TestingEventEmitter<DeviceModel>();

    const harness = await DeviceDetailHarness.render({device, lightTypes, saveDevice: emitter});
    harness.saveDevice();

    expect(emitter.emit).toHaveBeenCalledWith(device);
  })

  it('should hide light type when device is not a light', async () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Switch});

    const harness = await DeviceDetailHarness.render({device});

    expect(harness.lightTypeSelect).not.toBeInTheDocument();
  })
})
