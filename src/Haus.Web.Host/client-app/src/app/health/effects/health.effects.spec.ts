import {TestBed} from "@angular/core/testing";

import {
  createAppTestingService, eventually, ModelFactory,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory
} from "../../../testing";
import {HealthEffects} from "./health.effects";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../shared/signalr";
import {HealthActions} from "../state";
import {setupGetLogs} from "../../../testing/fakes/testing-server/setup-logs-api";

describe('HealthEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(HealthEffects);
    actions$ = actionsSubject;
    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.health);
  })

  it('should start listening for health updates', async () => {
    actions$.next(HealthActions.start());

    await eventually(() => {
      signalrHub.triggerStart();
      expect(signalrHub.start).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(HealthActions.connected());
    })
  })

  it('should stop listening for health updates', async () => {
    actions$.next(HealthActions.stop());

    await eventually(() => {
      signalrHub.triggerStop();

      expect(signalrHub.stop).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(HealthActions.disconnected());
    })
  })

  it('should notify when health received', async () => {
    const report = ModelFactory.createHealthReportModel({
      checks: [
        ModelFactory.createHealthCheckModel(),
        ModelFactory.createHealthCheckModel(),
      ]
    })
    actions$.next(HealthActions.start());

    await eventually(() => {
      signalrHub.triggerMessage("OnHealth", report);
      expect(actions$.publishedActions).toContainEqual(HealthActions.healthReceived(report));
    })
  })

  it('should load logs when started', async () => {
    const log = ModelFactory.createLogEntry();
    setupGetLogs([log]);

    actions$.next(HealthActions.start());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(HealthActions.loadRecentLogs.success(
        ModelFactory.createListResult(log)
      ))
    })
  })
})
