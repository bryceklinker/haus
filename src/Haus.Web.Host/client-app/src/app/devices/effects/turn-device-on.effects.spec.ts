import {TurnDeviceOffEffects} from "./turn-device-off.effects";
import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {HttpTestingController} from "@angular/common/http/testing";
import {createTestingEffect, eventually} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {TestBed} from "@angular/core/testing";
import {TurnDeviceOnEffects} from "./turn-device-on.effects";
import {DevicesActions} from "../actions";

describe('TurnDeviceOnEffects', () => {
  let effects: TurnDeviceOnEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;
  let httpController: HttpTestingController;

  beforeEach(() => {
    const result = createTestingEffect(TurnDeviceOnEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
    httpController = TestBed.inject(HttpTestingController);

    effects.turnOn$.subscribe((action: Action) => actionsCollector.push(action));
  })

  it('should turn device on using api', async () => {
    actions$.next(DevicesActions.turnOn.request(76));

    await eventually(() => {
      httpController.match('/api/devices/76/turn-on').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DevicesActions.turnOn.success(76));
    })
  })

  it('should notify of error when turning device on fails', async () => {
    actions$.next(DevicesActions.turnOn.request(5));

    await eventually(() => {
      httpController.match('/api/devices/5/turn-on').forEach(r => r.error(new ErrorEvent('not today')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.turnOn.failed.type,
        payload: expect.objectContaining({deviceId: 5})
      }))
    })
  })
})
