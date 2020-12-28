import {createTestingService, eventually, ModelFactory, TestingServer} from "../../../../testing";
import {FeatureGuard} from "./feature.guard";
import {SharedModule} from "../../shared.module";
import {FeatureName} from "../feature-name";

describe('FeatureGuard', () => {
  let guard: FeatureGuard;

  beforeEach(() => {
    const {service} = createTestingService(FeatureGuard, {
      imports: [SharedModule]
    });
    guard = service;
  })

  it('should not allow route to be activated when feature is not in enabled features', async () => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult());

    let canActivate = true;
    const snapshot = createSnapshotWithRouteConfigForFeature(FeatureName.DeviceSimulator);
    guard.canActivate(snapshot as any, {} as any).subscribe(c => canActivate = c);

    await eventually(() => {
      expect(canActivate).toEqual(false);
    })
  })

  it('should allow route to be activated when feature is enabled', async () => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult(FeatureName.DeviceSimulator));

    let canActivate = false;
    const snapshot = createSnapshotWithRouteConfigForFeature(FeatureName.DeviceSimulator);
    guard.canActivate(snapshot as any, {} as any).subscribe(c => canActivate = c);

    await eventually(() => {
      expect(canActivate).toEqual(true);
    })
  })

  function createSnapshotWithRouteConfigForFeature(featureName: FeatureName) {
    return {
      routeConfig: {featureName}
    }
  }
})
