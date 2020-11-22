import {createTestingEffect, TestingHub, eventually, TestingAuthService} from "../../../testing";
import {DiagnosticsEffects} from "./diagnostics.effects";
import {DiagnosticsModule} from "../diagnostics.module";
import {Action} from "@ngrx/store";
import {createHub, createSignalRHub, signalrHubUnstarted} from "ngrx-signalr-core";
import {DIAGNOSTICS_HUB} from "./diagnostics-hub";
import {DiagnosticsActions} from "../actions";
import {Subject} from "rxjs";
import {TestBed} from "@angular/core/testing";
import {HttpTestingController} from "@angular/common/http/testing";
import {ModelFactory} from "../../../testing/model-factory";
import {AuthService} from "@auth0/auth0-angular";
import {DiagnosticsMessageModel} from "../models";

describe('DiagnosticsEffects', () => {
  let effects: DiagnosticsEffects;
  let actions$: Subject<Action>;
  let testingHub: TestingHub;
  let actionsCollector: Array<Action>;
  let httpController: HttpTestingController;
  let authService: TestingAuthService;

  beforeAll(() => {
    testingHub = createHub(DIAGNOSTICS_HUB.hubName, DIAGNOSTICS_HUB.url) as TestingHub;
  })

  beforeEach(() => {
    const result = createTestingEffect(DiagnosticsEffects, {imports: [DiagnosticsModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];

    httpController = TestBed.inject(HttpTestingController);
    authService = TestBed.inject(AuthService) as TestingAuthService;
  })

  it('should create hub when initialized', async () => {
    effects.init$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DiagnosticsActions.initHub());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        ...DIAGNOSTICS_HUB,
        type: createSignalRHub.type,
        options: expect.anything()
      }))
    })
  })

  it('should setup hub to get access token from auth service', async () => {
    authService.setAccessToken('this.is.my.token');
    let createHubAction: ReturnType<typeof createSignalRHub> | null = null;
    effects.init$.subscribe((action: Action) => createHubAction = action.type === createSignalRHub.type ? action as ReturnType<typeof createSignalRHub> : null);

    actions$.next(DiagnosticsActions.initHub());

    await eventually(async () => {
      // @ts-ignore
      expect(await createHubAction.options.accessTokenFactory()).toEqual('this.is.my.token');
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
    const model = ModelFactory.createMqttDiagnosticsMessage();
    testingHub.publish<DiagnosticsMessageModel>('OnMqttMessage', model)

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DiagnosticsActions.messageReceived(model));
    })
  })

  it('should send message to be replayed', async () => {
    effects.replayMessage$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DiagnosticsActions.replayMessageRequest(ModelFactory.createMqttDiagnosticsMessage()));
    await eventually(() => {
      httpController.match('/api/diagnostics/replay').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DiagnosticsActions.replayMessageSuccess());
    })
  })

  it('should notify that replaying message failed', async () => {
    effects.replayMessage$.subscribe((action: Action) => actionsCollector.push(action));

    let model = ModelFactory.createMqttDiagnosticsMessage();
    actions$.next(DiagnosticsActions.replayMessageRequest(model));
    await eventually(() => {
      httpController.match('/api/diagnostics/replay').forEach(r => r.error(new ErrorEvent('bad things')));
      expect(actionsCollector).toContainEqual({
        type: DiagnosticsActions.replayMessageFailed.type,
        payload: {
          message: model,
          error: expect.anything()
        }
      });
    })
  })
})
