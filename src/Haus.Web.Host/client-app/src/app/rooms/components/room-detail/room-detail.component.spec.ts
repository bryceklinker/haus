import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";

import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {LightingState} from "../../../shared/models";
import {RoomLightingChangeModel, RoomModel} from "../../models";

describe('RoomDetailComponent', () => {
  it('should show room name', async () => {
    const room = ModelFactory.createRoomModel({name: 'new hotness'});

    await renderComponent({room});

    expect(screen.getByTestId('room-detail')).toHaveTextContent('new hotness');
  })

  it('should show each device', async () => {
    const devices = [
      ModelFactory.createDeviceModel(),
      ModelFactory.createDeviceModel()
    ];

    await renderComponent({devices});

    expect(screen.queryAllByTestId('room-device-item')).toHaveLength(2);
  })

  it('should show room lighting', async () => {
    const room = ModelFactory.createRoomModel();

    await renderComponent({room});

    expect(await screen.findByTestId('lighting')).toBeInTheDocument();
  })

  it('should notify when lighting changed', async () => {
    const room = ModelFactory.createRoomModel({
      lighting: ModelFactory.createLighting({state: LightingState.Off})
    });
    const emitter = new TestingEventEmitter<RoomLightingChangeModel>();

    const {matHarness} = await renderComponent({room, lightingChange: emitter});
    const state = await matHarness.getHarness(MatSlideToggleHarness);
    await state.check();

    expect(emitter.emit).toHaveBeenCalledWith({
      roomId: room.id,
      lighting: expect.objectContaining({state: LightingState.On})
    })
  })

  it('should notify when assigning devices to room', async () => {
    const room = ModelFactory.createRoomModel();
    const emitter = new TestingEventEmitter<RoomModel>();

    const {detectChanges} = await renderComponent({room, assignDevices: emitter});

    userEvent.click(screen.getByTestId('assign-devices-btn'));
    detectChanges();

    expect(emitter.emit).toHaveBeenCalledWith(room);
  })

  function renderComponent({
                             room = null, devices = [],
                             lightingChange = new TestingEventEmitter<RoomLightingChangeModel>(),
                             assignDevices = new TestingEventEmitter<RoomModel>()
                           }: Partial<RoomDetailComponent> = {}) {
    return renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: {room, devices, lightingChange, assignDevices}
    });
  }
});
