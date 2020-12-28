import {HausApiClient} from "./haus-api-client";
import {createFeatureTestingService, eventually, ModelFactory, TestingServer} from "../../../testing";
import {SharedModule} from "../shared.module";

describe('HausApiClient', () => {
  let api: HausApiClient;
  let isLoading: boolean;

  beforeEach(() => {
    const result = createFeatureTestingService(HausApiClient, {imports: [SharedModule]});

    api = result.service;
    api.isLoading$.subscribe(l => isLoading = l);
  })

  it('should be loading while getting data', async () => {
      TestingServer.setupGet('/api/rooms', ModelFactory.createListResult(), {delay: 3000});

      api.getRooms().subscribe();

      await eventually(() => {
        expect(isLoading).toEqual(true);
      })
  })

  it('should be loading while requests are inflight', async () => {
    TestingServer.setupGet('/api/rooms', ModelFactory.createListResult());
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(), {delay: 200});
    TestingServer.setupPost('/api/rooms', ModelFactory.createRoomModel());

    api.getRooms().subscribe();
    api.getDevices().subscribe();
    api.addRoom(ModelFactory.createRoomModel()).subscribe();

    expect(isLoading).toEqual(true);
  })

  it('should be loading until all requests finish', async () => {
    TestingServer.setupGet('/api/rooms', ModelFactory.createListResult(), {delay: 100});
    TestingServer.setupGet('/api/devices', ModelFactory.createListResult(), {delay: 200});
    TestingServer.setupPost('/api/rooms', ModelFactory.createRoomModel(), {delay: 300});

    api.getRooms().subscribe();
    api.getDevices().subscribe();
    api.addRoom(ModelFactory.createRoomModel()).subscribe();

    expect(api.inflightRequests).toHaveLength(3);
    await eventually(() => {
      expect(api.inflightRequests).toHaveLength(0);
      expect(isLoading).toEqual(false);
    })
  })
})
