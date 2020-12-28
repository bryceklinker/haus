import {HausApiClient} from "./haus-api-client";
import {
  createFeatureTestingService,
  eventually,
  ModelFactory,
  setupAddRoom,
  setupGetAllDevices,
  setupGetAllRooms
} from "../../../testing";
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
    setupGetAllRooms([], {delay: 3000});

    api.getRooms().subscribe();

    await eventually(() => {
      expect(isLoading).toEqual(true);
    })
  })

  it('should be loading while requests are inflight', async () => {
    setupGetAllRooms([]);
    setupGetAllDevices([], {delay: 200});
    setupAddRoom();

    api.getRooms().subscribe();
    api.getDevices().subscribe();
    api.addRoom(ModelFactory.createRoomModel()).subscribe();

    expect(isLoading).toEqual(true);
  })

  it('should be loading until all requests finish', async () => {
    setupGetAllRooms([], {delay: 100});
    setupGetAllDevices([], {delay: 200});
    setupAddRoom(ModelFactory.createRoomModel(), {delay: 300});

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
