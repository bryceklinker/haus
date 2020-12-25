import {FeaturesService} from "./features.service";
import {createTestingService, eventually, ModelFactory, TestingServer} from "../../../testing";
import {SharedModule} from "../shared.module";
import {FeatureNames} from "./feature-names";

describe('FeaturesService', () => {
  let service: FeaturesService;

  beforeEach(() => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult(FeatureNames.DeviceSimulator));
    const result = createTestingService(FeaturesService, {imports: [SharedModule]});

    service = result.service;
  })

  it('should load features when initialized', async () => {
    let enabledFeatures: FeatureNames[] = [];
    service.enabledFeatures$.subscribe(f => enabledFeatures = f);

    service.load().subscribe();
    await eventually(() => {
      expect(enabledFeatures).toEqual([FeatureNames.DeviceSimulator]);
    })
  })
})
