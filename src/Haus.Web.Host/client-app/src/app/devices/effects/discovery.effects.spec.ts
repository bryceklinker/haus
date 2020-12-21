import {Subject} from "rxjs";
import {Action} from "@ngrx/store";

import {createTestingEffect, eventually, TestingServer} from "../../../testing";
import {DevicesModule} from "../devices.module";
import {DiscoveryEffects} from "./discovery.effects";
import {DevicesActions} from "../actions";

describe('DiscoveryEffects', () => {
  let effects: DiscoveryEffects;
  let actions$: Subject<Action>;
  let actionsCollector: Array<Action>;

  beforeEach(() => {
    const result = createTestingEffect(DiscoveryEffects, {imports: [DevicesModule]});
    effects = result.effects;
    actions$ = result.actions$;
    actionsCollector = [];
  })

  it('should start discovery on the api', async () => {
    TestingServer.setupPost('/api/devices/start-discovery', null, 204);
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.startDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DevicesActions.startDiscovery.success());
    })
  })

  it('should notify that starting discovery failed', async () => {
    TestingServer.setupPost('/api/devices/start-discovery', null, 500);
    effects.start$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.startDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.startDiscovery.failed.type
      }))
    })
  })

  it('should stop discovery on the api', async () => {
    TestingServer.setupPost('/api/devices/stop-discovery', null, 204);
    effects.stop$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.stopDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DevicesActions.stopDiscovery.success());
    })
  })

  it('should notify that stopping discovery failed', async () => {
    TestingServer.setupPost('/api/devices/stop-discovery', null, 500);
    effects.stop$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.stopDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.stopDiscovery.failed.type
      }))
    })
  })

  it('should sync discovery on the api', async () => {
    TestingServer.setupPost('/api/devices/sync-discovery', null, 204);
    effects.sync$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.syncDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(DevicesActions.syncDiscovery.success());
    })
  })

  it('should notify that syncing discovery failed', async () => {
    TestingServer.setupPost('/api/devices/sync-discovery', null, 500);
    effects.sync$.subscribe((action: Action) => actionsCollector.push(action));

    actions$.next(DevicesActions.syncDiscovery.request());

    await eventually(() => {
      expect(actionsCollector).toContainEqual(expect.objectContaining({
        type: DevicesActions.syncDiscovery.failed.type
      }))
    })
  })
})
