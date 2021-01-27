import {TestBed} from "@angular/core/testing";

import {
  createAppTestingService, eventually, ModelFactory,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory,
  TestingSnackBar,
  setupGetLogs,
  setupGetLogsFailure
} from "../../../testing";
import {HealthEffects} from "./health.effects";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../shared/signalr";
import {HealthActions} from "../state";
import {MatSnackBar} from "@angular/material/snack-bar";

describe('HealthEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection;
  let snackBar: TestingSnackBar;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(HealthEffects);
    actions$ = actionsSubject;

    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.health);
    snackBar = TestBed.inject(MatSnackBar) as TestingSnackBar;
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

  it('should notify when loading logs fails', async () => {
    setupGetLogsFailure();

    actions$.next(HealthActions.loadRecentLogs.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(expect.objectContaining({
        type: HealthActions.loadRecentLogs.failed.type
      }))
    })
  })

  it('should show snack bar when logs fail to load', async () => {
    setupGetLogs();

    actions$.next(HealthActions.loadRecentLogs.failed(new Error('Bad Things')));

    await eventually(() => {
      expect(snackBar.open).toHaveBeenCalled();
    })
  })
})
