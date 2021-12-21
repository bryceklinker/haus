import {TestBed} from "@angular/core/testing";

import {
  createAppTestingService, eventually, ModelFactory, setupDiagnosticsReplay,
  TestingActionsSubject,
  TestingSignalrHubConnection,
  TestingSignalrHubConnectionFactory
} from "../../../testing";
import {DiagnosticsEffects} from "./diagnostics.effects";
import {KNOWN_HUB_NAMES, SignalrHubConnectionFactory} from "../../shared/signalr";
import {DiagnosticsActions} from "../state";

describe('DiagnosticsEffects', () => {
  let actions$: TestingActionsSubject;
  let signalrHub: TestingSignalrHubConnection

  beforeEach(() => {
    const result = createAppTestingService(DiagnosticsEffects);
    actions$ = result.actionsSubject;
    signalrHub = (TestBed.inject(SignalrHubConnectionFactory) as TestingSignalrHubConnectionFactory)
      .getTestingHub(KNOWN_HUB_NAMES.diagnostics);
  })

  test('should start signalr connection when diagnostics is started', async () => {
    actions$.next(DiagnosticsActions.start());

    await eventually(() => {
      signalrHub.triggerStart();
      expect(signalrHub.start).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.connected());
    })
  })

  test('should stop signalr connection when diagnostics is stopped', async () => {
    actions$.next(DiagnosticsActions.stop());

    await eventually(() => {
      signalrHub.triggerStop();
      expect(signalrHub.stop).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.disconnected());
    })
  })

  test('should notify when message received', async () => {
    const expected = ModelFactory.createMqttDiagnosticsMessage();
    actions$.next(DiagnosticsActions.start());

    await eventually(() => {
      signalrHub.triggerMqttMessage(expected);
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.messageReceived(expected));
    })
  })

  test('should replay message through api when replay message is requested', async () => {
    setupDiagnosticsReplay();

    const expected = ModelFactory.createMqttDiagnosticsMessage();

    actions$.next(DiagnosticsActions.replayMessage.request(expected));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.replayMessage.success(expected));
    })
  })
})
