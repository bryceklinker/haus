import {LoadDevicesEffects} from "./load-devices.effects";
import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {HttpTestingController} from "@angular/common/http/testing";
import {TestBed} from "@angular/core/testing";

import {createTestingEffect, eventually} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {DevicesActions} from "../actions";
import {ModelFactory} from "../../../testing/model-factory";

describe('LoadDevicesEffects', () => {
  let effects: LoadDevicesEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;
  let httpController: HttpTestingController;

  beforeEach(() => {
    const result = createTestingEffect(LoadDevicesEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
    httpController = TestBed.inject(HttpTestingController);
  })

  it('should get devices from api', async () => {
    const result = ModelFactory.createListResult(
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    )
    effects.load$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.load.request());

    await eventually(() => {
      httpController.match('/api/devices').forEach(r => r.flush(result));
      expect(actionsCollector).toContainEqual(DevicesActions.load.success(result));
    })
  })

  it('should notify that loading devices failed', async () => {
    effects.load$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.load.request());

    await eventually(() => {
      httpController.match('/api/devices').forEach(r => r.error(new ErrorEvent('not good')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.load.failed.type
      }));
    })
  })
})
