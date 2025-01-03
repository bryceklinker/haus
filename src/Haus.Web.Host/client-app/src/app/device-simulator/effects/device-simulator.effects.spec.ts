import {TestBed} from "@angular/core/testing";

import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupAddSimulatedDevice,
  setupTriggerSimulatedDeviceOccupancyChange,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory
} from "../../../testing";
import {DeviceSimulatorEffects} from "./device-simulator.effects";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../shared/signalr";
import {DeviceSimulatorActions, DeviceSimulatorState} from "../state";
import {DeviceType, SimulatedDeviceModel} from "../../shared/models";

describe('DeviceSimulatorEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(DeviceSimulatorEffects);
    actions$ = actionsSubject;
    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.deviceSimulator);
  })

  test('should start hub connection when simulator started', async () => {
    actions$.next(DeviceSimulatorActions.start());

    await eventually(() => {
      signalrHub.triggerStart();
      expect(signalrHub.start).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.connected());
    })
  })

  test('should stop hub connection when simulator stopped', async () => {
    actions$.next(DeviceSimulatorActions.stop());

    await eventually(() => {
      signalrHub.triggerStop();
      expect(signalrHub.stop).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.disconnected());
    })
  })

  test('should notify when state received', async () => {
    const expected: DeviceSimulatorState = {
      devices: [
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice(),
        ModelFactory.createSimulatedDevice()
      ]
    };

    actions$.next(DeviceSimulatorActions.start());

    await eventually(() => {
      signalrHub.triggerMessage('OnState', expected);
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.stateReceived(expected));
    })
  })

  test('should add simulated device when add simulated device requested', async () => {
    setupAddSimulatedDevice();

    const model: Partial<SimulatedDeviceModel> = {
      deviceType: DeviceType.Light,
      metadata: []
    };
    actions$.next(DeviceSimulatorActions.addSimulatedDevice.request(model));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.addSimulatedDevice.success());
    })
  })

  test('should trigger occupancy change for simulated device when requested', async () => {
    setupTriggerSimulatedDeviceOccupancyChange('my-super-device');
    const model = ModelFactory.createSimulatedDevice({deviceType: DeviceType.MotionSensor, id: 'my-super-device'});

    actions$.next(DeviceSimulatorActions.triggerOccupancyChange.request(model));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.triggerOccupancyChange.success());
    })
  })
})
