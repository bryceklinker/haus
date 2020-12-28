import {FeaturesService} from "./features.service";
import {createTestingService, eventually, ModelFactory, TestingServer} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {FeatureName} from "../feature-name";
import {fromObservableToPromise} from "../../observable-extensions";

describe('FeaturesService', () => {
  let service: FeaturesService;

  beforeEach(() => {
    TestingServer.setupGet('/api/features', ModelFactory.createListResult(FeatureName.DeviceSimulator));
    const result = createTestingService(FeaturesService, {imports: [SharedModule]});

    service = result.service;
  })

  it('should load features when initialized', async () => {
    let enabledFeatures: FeatureName[] = [];
    service.enabledFeatures$.subscribe(f => enabledFeatures = f);

    service.load().subscribe();
    await eventually(() => {
      expect(enabledFeatures).toEqual([FeatureName.DeviceSimulator]);
    })
  })

  it('should only load features once', async () => {
    await fromObservableToPromise(service.load())
    await fromObservableToPromise(service.load())
    await fromObservableToPromise(service.load())

    expect(TestingServer.requests).toHaveLength(1);
  })
})
