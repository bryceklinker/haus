import {TestBed} from "@angular/core/testing";
import {ActivatedRoute} from "@angular/router";

import {RoomsService} from "./rooms.service";
import {
  createFeatureTestingService,
  eventually,
  ModelFactory, setupAddRoom, setupAllRoomsApis, setupGetAllRooms, setupGetDevicesForRoom,
  TestingActivatedRoute
} from "../../../../testing";
import {SharedModule} from "../../shared.module";
import {RoomModel} from "../models";
import {DeviceModel} from "../../devices";

describe('RoomsService', () => {
  let activatedRoute: TestingActivatedRoute;
  let service: RoomsService;
  let rooms: Array<RoomModel> | null;
  let selectedRoom: RoomModel | null;
  let selectedRoomDevices: Array<DeviceModel> | null;

  beforeEach(() => {
    const result = createFeatureTestingService(RoomsService, {imports: [SharedModule]});
    service = result.service;
    activatedRoute = <TestingActivatedRoute>TestBed.inject(ActivatedRoute);

    rooms = null;
    selectedRoom = null;
    selectedRoomDevices = null;
    service.rooms$.subscribe(r => rooms = r);
    service.selectedRoom$.subscribe(r => selectedRoom = r);
    service.selectedRoomDevices$.subscribe(r => {
      console.log('DEVICES', r);
      selectedRoomDevices = r;
    })
  })

  it('should have empty rooms', async () => {
    await eventually(() => {
      expect(rooms).toEqual([]);
    })
  })

  it('should notify when rooms are loaded', async () => {
    const expected = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    ];
    setupGetAllRooms(expected);

    service.getAll().subscribe();

    await eventually(() => {
      expect(rooms).toContainEqual(expected[0]);
      expect(rooms).toContainEqual(expected[1]);
      expect(rooms).toContainEqual(expected[2]);
    })
  })

  it('should return rooms to get all caller', async () => {
    const expected = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
    ];
    setupGetAllRooms(expected);

    let actual: RoomModel[]
    service.getAll().subscribe(r => actual = r);

    await eventually(() => {
      expect(actual).toContainEqual(expected[0]);
      expect(actual).toContainEqual(expected[1]);
      expect(actual).toContainEqual(expected[2]);
    })
  })

  it('should return room after room is created', async () => {
    const expected = ModelFactory.createRoomModel();
    setupAddRoom(expected);

    const roomToAdd = ModelFactory.createRoomModel({name: expected.name});

    let actual: RoomModel | null = null;
    service.add(roomToAdd).subscribe(r => actual = r);

    await eventually(() => {
      expect(actual).toEqual(expected);
    })
  })

  it('should notify rooms subscribers when room is added', async () => {
    const expected = ModelFactory.createRoomModel();
    setupAddRoom(expected);

    service.add(expected).subscribe();

    await eventually(() => {
      expect(rooms).toContainEqual(expected);
    })
  })

  it('should sort rooms by name', async () => {
    const second = ModelFactory.createRoomModel({name: 'B'});
    const third = ModelFactory.createRoomModel({name: 'C'});
    const first = ModelFactory.createRoomModel({name: 'A'});
    setupGetAllRooms([second, third, first]);

    let actual: RoomModel[] = [];
    service.getAll().subscribe(r => actual = r);

    await eventually(() => {
      if (rooms == null) {
        throw new Error('Rooms is still null');
      }
      expect(rooms[0]).toEqual(first);
      expect(rooms[1]).toEqual(second);
      expect(rooms[2]).toEqual(third);

      expect(actual[0]).toEqual(first);
      expect(actual[1]).toEqual(second);
      expect(actual[2]).toEqual(third);
    })
  })

  it('should be loading while adding room', async () => {
    setupAddRoom(ModelFactory.createRoomModel(), {delay: 500});
    let isLoading = false;
    service.isLoading$.subscribe(l => isLoading = l);

    service.add(ModelFactory.createRoomModel()).subscribe();

    expect(isLoading).toEqual(true);
  })

  it('should have null selected room', async () => {
    setupAllRoomsApis();

    service.getAll();

    await eventually(() => {
      expect(selectedRoom).toBeNull();
    })
  })

  it('should notify that selected room changed when route changes', async () => {
    const room = ModelFactory.createRoomModel();
    setupGetAllRooms([room]);
    setupGetDevicesForRoom(room.id);

    service.getAll().subscribe();
    activatedRoute.triggerParamsChange({'roomId': `${room.id}`});

    await eventually(() => {
      expect(selectedRoom).toEqual(room);
    })
  })

  it('should get devices for room when selected', async () => {
    const room = ModelFactory.createRoomModel();
    setupGetAllRooms([room]);
    setupGetDevicesForRoom(room.id, [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    ])

    service.getAll().subscribe();
    activatedRoute.triggerParamsChange({'roomId': `${room.id}`});

    await eventually(() => {
      expect(selectedRoomDevices).toHaveLength(2);
    })
  })

  afterEach(() => {
    service.ngOnDestroy();
  })
})
