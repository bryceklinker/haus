import {eventually, ModelFactory} from '../../../../testing';
import {DeviceDetailRootComponent} from './device-detail-root.component';
import {DevicesActions, LightTypesActions} from '../../state';
import {EventsActions} from '../../../shared/events';
import {DeviceType, LightType} from '../../../shared/models';
import {DeviceDetailRootHarness} from './device-detail-root.harness';

describe('DeviceDetailRootComponent', () => {
  it('should show device detail when rendered', async () => {
    const device = ModelFactory.createDeviceModel();
    const action = DevicesActions.loadDevices.success(ModelFactory.createListResult(device));

    const page = await DeviceDetailRootHarness.render(device, action);

    await eventually(() => {
      expect(page.deviceDetail).toHaveTextContent(device.name);
    });
  });

  it('should load light types when rendered', async () => {
    const device = ModelFactory.createDeviceModel();
    const action = DevicesActions.loadDevices.success(ModelFactory.createListResult(device));

    const page = await DeviceDetailRootHarness.render(device, action);

    await eventually(() => {
      expect(page.dispatchedActions).toContainEqual(LightTypesActions.loadLightTypes.request());
    });
  });

  it('should show light types when rendered', async () => {
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light});
    const actions = [
      LightTypesActions.loadLightTypes.success(ModelFactory.createListResult(LightType.Level, LightType.Temperature)),
      EventsActions.deviceCreated({device})
    ];

    const page = await DeviceDetailRootHarness.render(device, ...actions);

    await eventually(async () => {
      expect(await page.getLightTypes()).toHaveLength(2);
    });
  });

  it('should notify change device lighting constraints', async () => {
    const lighting = ModelFactory.createLighting();
    const device = ModelFactory.createDeviceModel({deviceType: DeviceType.Light, lighting});
    const action = EventsActions.deviceCreated({device});

    const page = await DeviceDetailRootHarness.render(device, action);

    page.saveConstraints();

    const expectedAction = DevicesActions.changeDeviceLightingConstraints.request({
      device,
      constraints: {
        minLevel: lighting.level.min,
        maxLevel: lighting.level.max
      }
    });
    expect(page.dispatchedActions).toContainEqual(expectedAction);
  });

  it('should notify update device when device is saved', async () => {
    const device = ModelFactory.createDeviceModel();
    const action = EventsActions.deviceCreated({device});

    const page = await DeviceDetailRootHarness.render(device, action);

    page.saveDevice();

    expect(page.dispatchedActions).toContainEqual(DevicesActions.updateDevice.request(device));
  });
});

