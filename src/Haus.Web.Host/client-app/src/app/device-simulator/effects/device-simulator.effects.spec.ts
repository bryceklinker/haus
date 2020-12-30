import {
  createAppTestingService, eventually, ModelFactory, setupAddSimulatedDevice,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory
} from "../../../testing";
import {DeviceSimulatorEffects} from "./device-simulator.effects";
import {TestBed} from "@angular/core/testing";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../shared/signalr";
import {DeviceSimulatorActions, DeviceSimulatorState} from "../state";
import {CreateSimulatedDeviceModel} from "../models";

describe('DeviceSimulatorEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(DeviceSimulatorEffects);
    actions$ = actionsSubject;
    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.deviceSimulator);
  })

  it('should start hub connection when simulator started', async () => {
    actions$.next(DeviceSimulatorActions.start());

    await eventually(() => {
      signalrHub.triggerStart();
      expect(signalrHub.start).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.connected());
    })
  })

  it('should stop hub connection when simulator stopped', async () => {
    actions$.next(DeviceSimulatorActions.stop());

    await eventually(() => {
      signalrHub.triggerStop();
      expect(signalrHub.stop).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.disconnected());
    })
  })

  it('should notify when state received', async () => {
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

  it('should add simulated device when add simulated device requested', async () => {
    setupAddSimulatedDevice();

    const model: CreateSimulatedDeviceModel = {
      deviceType: 'idk',
      metadata: []
    };
    actions$.next(DeviceSimulatorActions.addSimulatedDevice.request(model));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DeviceSimulatorActions.addSimulatedDevice.success());
    })
  })
})
