import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
  TestingActivatedRoute
} from "../../../../testing";
import {RoomDetailRootComponent} from "./room-detail-root.component";
import {RoomsModule} from "../../rooms.module";
import {RoomModel} from "../../models";
import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";
import {RoomsActions} from "../../state";
import {DevicesActions} from "../../../devices/state";

describe('DeviceDetailRootComponent', () => {
  let room: RoomModel;

  beforeEach(() => {
    room = ModelFactory.createRoomModel();
  })


  it('should show room name', async () => {
    const {activatedRoute, detectChanges, container} = await renderRoot(RoomsActions.loadRooms.success(ModelFactory.createListResult(room)));
    triggerRoomChanged(activatedRoute, room.id);

    await eventually(() => {
      detectChanges();
      expect(container).toHaveTextContent(room.name);
    })
  })

  it('should show devices in room', async () => {
    const {detectChanges, activatedRoute} = await renderRoot(
      RoomsActions.loadRooms.success(ModelFactory.createListResult(room)),
      DevicesActions.loadDevices.success(ModelFactory.createListResult(
        ModelFactory.createDeviceModel({roomId: room.id}),
        ModelFactory.createDeviceModel({roomId: room.id}),
      ))
    );
    triggerRoomChanged(activatedRoute, room.id);

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('room-device-item')).toHaveLength(2);
    })
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(RoomDetailRootComponent, {
      imports: [RoomsModule],
      actions: actions
    })
  }

  function triggerRoomChanged(activatedRoute: TestingActivatedRoute, roomId?: number) {
    activatedRoute.triggerParamsChange({roomId: roomId ? `${roomId}` : null});
  }
})
