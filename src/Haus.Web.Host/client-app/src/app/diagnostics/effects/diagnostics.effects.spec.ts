import {createTestingEffect,TestingAuthService, TestingHub, initAction, eventually} from "../../../testing";
import {DiagnosticsEffects} from "./diagnostics.effects";
import {DiagnosticsModule} from "../diagnostics.module";
import {Action} from "@ngrx/store";
import {createHub, createSignalRHub, signalrHubUnstarted} from "ngrx-signalr-core";
import {AuthService} from "@auth0/auth0-angular";
import {DIAGNOSTICS_HUB} from "./diagnostics-hub";
import {MqttDiagnosticsMessageModel} from "../models/mqtt-diagnostics-message.model";
import {DiagnosticsActions} from "../actions";
import {Subject} from "rxjs";
import {TestBed} from "@angular/core/testing";

describe('DiagnosticsEffects', () => {
  let effects: DiagnosticsEffects;
  let actions$: Subject<Action>;
  let authService: TestingAuthService;
  let testingHub: TestingHub;
  let actionsCollector: Array<Action>;

  beforeAll(() => {
    testingHub = createHub(DIAGNOSTICS_HUB.hubName, DIAGNOSTICS_HUB.url) as TestingHub;
  })

  beforeEach(() => {
    const result = createTestingEffect(DiagnosticsEffects, {imports: [DiagnosticsModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];

    // @ts-ignore
    authService = TestBed.inject(AuthService) as TestingAuthService;
  })

  it('should create hub when initialized', async () => {
    effects.init$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(initAction());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        ...DIAGNOSTICS_HUB,
        type: createSignalRHub.type,
        options: expect.anything()
      }))
    })
  })

  it('should start hub when hub has not been started', async () => {
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));
    actions$.next(signalrHubUnstarted(DIAGNOSTICS_HUB));

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        ...DIAGNOSTICS_HUB
      }))
    })
  })

  it('should notify when message is received from hub', async () => {
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(signalrHubUnstarted(DIAGNOSTICS_HUB));
    testingHub.publish<MqttDiagnosticsMessageModel>('OnMqttMessage', {
      payload: 'this is data',
      topic: 'sometopic'
    })

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DiagnosticsActions.messageReceived({
        payload: 'this is data',
        topic: 'sometopic'
      }))
    })
  })
})
