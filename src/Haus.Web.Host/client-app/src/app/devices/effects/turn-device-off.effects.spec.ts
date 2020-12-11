import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {HttpTestingController} from "@angular/common/http/testing";
import {TurnDeviceOffEffects} from "./turn-device-off.effects";
import {createTestingEffect, eventually} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {TestBed} from "@angular/core/testing";
import {DevicesActions} from "../actions";

describe('TurnDeviceOffEffects', () => {
  let effects: TurnDeviceOffEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;
  let httpController: HttpTestingController;

  beforeEach(() => {
    const result = createTestingEffect(TurnDeviceOffEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
    httpController = TestBed.inject(HttpTestingController);
  })

  it('should turn device off through the api', async () => {
    effects.turnOff$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.turnOff.request(65));

    await eventually(() => {
      httpController.match('/api/devices/65/turn-off').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DevicesActions.turnOff.success(65));
    })
  })

  it('should notify of errors if turning device off fails', async () => {
    effects.turnOff$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.turnOff.request(7));

    await eventually(() => {
      httpController.match('/api/devices/7/turn-off').forEach(r => r.error(new ErrorEvent('whoops')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.turnOff.failed.type,
        payload: expect.objectContaining({
          deviceId: 7
        })
      }))
    })
  })
})
