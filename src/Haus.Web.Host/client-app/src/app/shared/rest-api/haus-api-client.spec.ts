import {TestBed} from "@angular/core/testing";

import {HausApiClient} from "./haus-api-client";
import {
  createFeatureTestingService,
  eventually,
  ModelFactory,
  setupAddRoom,
  setupDownloadPackage,
  setupGetAllDevices,
  setupGetAllRooms,
  TestingSaveFileService
} from "../../../testing";
import {SharedModule} from "../shared.module";
import {SaveFileService} from "../save-file.service";

describe('HausApiClient', () => {
  let api: HausApiClient;
  let isLoading: boolean;
  let saveFileService: TestingSaveFileService;

  beforeEach(() => {
    const result = createFeatureTestingService(HausApiClient, {imports: [SharedModule]});

    saveFileService = TestBed.inject(SaveFileService) as TestingSaveFileService;
    api = result.service;
    api.isLoading$.subscribe(l => isLoading = l);
  })

  test('should be loading while getting data', async () => {
    setupGetAllRooms([], {delay: 3000});

    api.getRooms().subscribe();

    await eventually(() => {
      expect(isLoading).toEqual(true);
    })
  })

  test('should be loading while requests are inflight', async () => {
    setupGetAllRooms([]);
    setupGetAllDevices([], {delay: 200});
    setupAddRoom();

    api.getRooms().subscribe();
    api.getDevices().subscribe();
    api.addRoom(ModelFactory.createRoomModel()).subscribe();

    expect(isLoading).toEqual(true);
  })

  test('should be loading until all requests finish', async () => {
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

  test('should save blob from api to package name file', async () => {
    const blob = new Blob();
    setupDownloadPackage(44, blob);

    api.downloadPackage(ModelFactory.createApplicationPackage({id: 44, name: 'something.zip'}))
      .subscribe();

    await eventually(() => {
      expect(saveFileService.saveAs).toHaveBeenCalledWith(blob, 'something.zip');
    })
  })
})
