import {eventually, ModelFactory, renderFeatureComponent, TestingActivatedRoute} from "../../../../testing";
import {RoomDetailRootComponent} from "./room-detail-root.component";
import {RoomsModule} from "../../rooms.module";
import {RoomModel} from "../../models";
import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";
import {RoomsActions} from "../../state";
import {DevicesActions} from "../../../devices/state";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {LightingState} from "../../../shared/models";

describe('DeviceDetailRootComponent', () => {
  let room: RoomModel;

  beforeEach(() => {
    room = ModelFactory.createRoomModel({
      lighting: ModelFactory.createLighting({state: LightingState.Off})
    });
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

  it('should request lighting change when room lighting changed', async () => {
    const {activatedRoute, store, matHarness} = await renderRoot(RoomsActions.loadRooms.success(ModelFactory.createListResult(room)));
    triggerRoomChanged(activatedRoute, room.id);

    const state = await matHarness.getHarness(MatSlideToggleHarness);
    await state.check();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.changeRoomLighting.request({
      roomId: room.id,
      lighting: expect.objectContaining({state: LightingState.On})
    }));
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
