import {
  createAppTestingService,
  eventually,
  ModelFactory,
  setupGetDiscovery,
  setupStartDiscovery,
  setupStopDiscovery,
  setupSyncDiscovery,
  TestingActionsSubject
} from "../../../../testing";
import {DevicesEffects} from "../../../devices/effects/devices.effects";
import {DiscoveryActions} from "../state";
import {DiscoveryState} from "../../models";

describe('DiscoveryEffects', () => {
  let actions$: TestingActionsSubject;

  beforeEach(() => {
    const {actionsSubject} = createAppTestingService(DevicesEffects);
    actions$ = actionsSubject;
  })

  it('should start discovery when start discovery is requested', async () => {
    setupStartDiscovery();

    actions$.next(DiscoveryActions.startDiscovery.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiscoveryActions.startDiscovery.success());
    })
  })

  it('should stop discovery when stop discovery is requested', async () => {
    setupStopDiscovery();

    actions$.next(DiscoveryActions.stopDiscovery.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiscoveryActions.stopDiscovery.success());
    })
  })

  it('should sync discovery when sync discovery is requested', async () => {
    setupSyncDiscovery();

    actions$.next(DiscoveryActions.syncDiscovery.request());

    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiscoveryActions.syncDiscovery.success());
    })
  })

  it('should notify that get discovery is successful', async () => {
    const model = ModelFactory.createDiscovery({state: DiscoveryState.Enabled});
    setupGetDiscovery(model);

    actions$.next(DiscoveryActions.getDiscovery.request());
    await eventually(() => {
      expect(actions$.publishedActions).toContainEqual(DiscoveryActions.getDiscovery.success(model));
    })
  })
})
