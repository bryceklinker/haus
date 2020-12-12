import {Subject} from "rxjs";
import {Action} from "@ngrx/store";
import {HttpTestingController} from "@angular/common/http/testing";
import {TestBed} from "@angular/core/testing";

import {createTestingEffect, eventually} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {DiscoveryEffects} from "./discovery.effects";
import {DevicesActions} from "../actions";

describe('DiscoveryEffects', () => {
  let effects: DiscoveryEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;
  let httpController: HttpTestingController;

  beforeEach(() => {
    const result = createTestingEffect(DiscoveryEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
    httpController = TestBed.inject(HttpTestingController);
  })

  it('should start discovery on the api', async () => {
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.startDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/start-discovery').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DevicesActions.startDiscovery.success());
    })
  })

  it('should notify that starting discovery failed', async () => {
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.startDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/start-discovery').forEach(r => r.error(new ErrorEvent('broken')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.startDiscovery.failed.type
      }))
    })
  })

  it('should stop discovery on the api', async () => {
    effects.stop$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.stopDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/stop-discovery').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DevicesActions.stopDiscovery.success());
    })
  })

  it('should notify that stopping discovery failed', async () => {
    effects.stop$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.stopDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/stop-discovery').forEach(r => r.error(new ErrorEvent('broken')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.stopDiscovery.failed.type
      }))
    })
  })

  it('should sync discovery on the api', async () => {
    effects.sync$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.syncDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/sync-discovery').forEach(r => r.flush(204));
      expect(actionsCollector).toContainEqual(DevicesActions.syncDiscovery.success());
    })
  })

  it('should notify that syncing discovery failed', async () => {
    effects.sync$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.syncDiscovery.request());

    await eventually(() => {
      httpController.match('/api/devices/sync-discovery').forEach(r => r.error(new ErrorEvent('broken')));
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.syncDiscovery.failed.type
      }))
    })
  })
})
