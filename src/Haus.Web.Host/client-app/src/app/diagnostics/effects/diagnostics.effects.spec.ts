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

  it('should start signalr connection when diagnostics is started', async () => {
    actions$.next(DiagnosticsActions.start());

    await eventually(() => {
      signalrHub.triggerStart();
      expect(signalrHub.start).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.connected());
    })
  })

  it('should stop signalr connection when diagnostics is stopped', async () => {
    actions$.next(DiagnosticsActions.stop());

    await eventually(() => {
      signalrHub.triggerStop();
      expect(signalrHub.stop).toHaveBeenCalled();
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.disconnected());
    })
  })

  it('should notify when message received', async () => {
    const expected = ModelFactory.createMqttDiagnosticsMessage();
    actions$.next(DiagnosticsActions.start());

    await eventually(() => {
      signalrHub.triggerMqttMessage(expected);
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.messageReceived(expected));
    })
  })

  it('should replay message through api when replay message is requested', async () => {
    setupDiagnosticsReplay();

    const expected = ModelFactory.createMqttDiagnosticsMessage();

    actions$.next(DiagnosticsActions.replayMessage.request(expected));

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiagnosticsActions.replayMessage.success(expected));
    })
  })
})
