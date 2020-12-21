import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {createTestingEffect, eventually, TestingServer} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {TurnDeviceOnEffects} from "./turn-device-on.effects";
import {DevicesActions} from "../actions";

describe('TurnDeviceOnEffects', () => {
  let effects: TurnDeviceOnEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;

  beforeEach(() => {
    const result = createTestingEffect(TurnDeviceOnEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];

    effects.turnOn$.subscribe((action: Action) => actionsCollector.push(action));
  })

  it('should turn device on using api', async () => {
    TestingServer.setupPost('/api/devices/76/turn-on', null, 204);
    actions$.next(DevicesActions.turnOn.request(76));

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DevicesActions.turnOn.success(76));
    })
  })

  it('should notify of error when turning device on fails', async () => {
    TestingServer.setupPost('/api/devices/5/turn-on', null, 500);
    actions$.next(DevicesActions.turnOn.request(5));

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.turnOn.failed.type,
        payload: expect.objectContaining({deviceId: 5})
      }))
    })
  })
})
