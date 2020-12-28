import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
  setupGetAllRooms,
  setupGetDevicesForRoom, TestingActivatedRoute
} from "../../../../testing";
import {RoomDetailRootComponent} from "./room-detail-root.component";
import {RoomsModule} from "../../rooms.module";
import {RoomModel, RoomsService} from "../../../shared/rooms";
import {DeviceModel} from "../../../shared/devices";
import {screen} from "@testing-library/dom";
import {TestBed} from "@angular/core/testing";

describe('DeviceDetailRootComponent', () => {
  let room: RoomModel;
  let roomDevices: Array<DeviceModel>;
  let activatedRoute: TestingActivatedRoute;
  let detectChanges: () => void;
  let container: Element;

  beforeEach(async () => {
    room = ModelFactory.createRoomModel();
    roomDevices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel(),
    ];

    setupGetAllRooms([room]);
    setupGetDevicesForRoom(room.id, roomDevices);

    const result = await renderRoot();
    const service = TestBed.inject(RoomsService);
    service.ngOnInit();
    service.getAll().subscribe();

    activatedRoute = result.activatedRoute;
    detectChanges = result.detectChanges;
    container = result.container;
  })

  it('should show room name', async () => {
    triggerRoomChanged(room.id);

    await eventually(() => {
      detectChanges();
      expect(container).toHaveTextContent(room.name);
    })
  })

  it('should show devices in room', async () => {
    triggerRoomChanged(room.id);

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('room-device-item')).toHaveLength(2);
    })
  })

  function renderRoot() {
    return renderFeatureComponent(RoomDetailRootComponent, {
      imports: [RoomsModule]
    })
  }

  function triggerRoomChanged(roomId?: number) {
    activatedRoute.triggerParamsChange({roomId: roomId ? `${roomId}` : null});
  }
})
