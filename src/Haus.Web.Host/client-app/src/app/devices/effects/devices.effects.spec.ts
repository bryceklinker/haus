import {DevicesEffects} from "./devices.effects";
import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupChangeDeviceLightingConstraints,
  setupDeviceTurnOff,
  setupDeviceTurnOn,
  setupGetAllDevices, setupUpdateDevice,
  TestingActionsSubject
} from "../../../testing";
import {DevicesActions} from "../state";

describe('DevicesEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(DevicesEffects);
    actions$ = actionsSubject;
  })

  it('should get devices from api when load devices requested', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    ]
    setupGetAllDevices(devices);

    actions$.next(DevicesActions.loadDevices.request());
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DevicesActions.loadDevices.success(ModelFactory.createListResult(...devices)));
    })
  })

  it('should turn device off when turn off device requested', async () => {
    setupDeviceTurnOff(4);

    actions$.next(DevicesActions.turnOffDevice.request(4));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DevicesActions.turnOffDevice.success(4));
    })
  })

  it('should turn device on when turn on device requested', async () => {
    setupDeviceTurnOn(7);

    actions$.next(DevicesActions.turnOnDevice.request(7));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DevicesActions.turnOnDevice.success(7));
    })
  })

  it('should change lighting constraints when change lighting constraints requested', async () => {
    const device = ModelFactory.createDeviceModel();
    const constraints = ModelFactory.createLightingConstraints();
    setupChangeDeviceLightingConstraints(device.id);

    actions$.next(DevicesActions.changeDeviceLightingConstraints.request({device, constraints}));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DevicesActions.changeDeviceLightingConstraints.success({device, constraints}));
    })
  })

  it('should update device when device update requested', async () => {
    const device = ModelFactory.createDeviceModel();
    setupUpdateDevice(device.id);

    actions$.next(DevicesActions.updateDevice.request(device));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DevicesActions.updateDevice.success(device));
    })
  })
})
