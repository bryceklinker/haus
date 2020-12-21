import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {TurnDeviceOffEffects} from "./turn-device-off.effects";
import {createTestingEffect, eventually, TestingServer} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {DevicesActions} from "../actions";

describe('TurnDeviceOffEffects', () => {
  let effects: TurnDeviceOffEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;

  beforeEach(() => {
    const result = createTestingEffect(TurnDeviceOffEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
  })

  it('should turn device off through the api', async () => {
    TestingServer.setupPost('/api/devices/65/turn-off', null, 204);
    effects.turnOff$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.turnOff.request(65));

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DevicesActions.turnOff.success(65));
    })
  })

  it('should notify of errors if turning device off fails', async () => {
    TestingServer.setupPost('/api/devices/7/turn-off', null, 500);
    effects.turnOff$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.turnOff.request(7));

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.turnOff.failed.type,
        payload: expect.objectContaining({
          deviceId: 7
        })
      }))
    })
  })
})
