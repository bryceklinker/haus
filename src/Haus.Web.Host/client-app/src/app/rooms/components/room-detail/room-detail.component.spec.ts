import {screen} from "@testing-library/dom";

import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {LightingModel, LightingState} from "../../../shared/models";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import {RoomLightingChangeModel} from "../../models/room-lighting-change.model";

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

    const {matHarness, detectChanges} = await renderComponent({room, lightingChange: emitter});
    const state = await matHarness.getHarness(MatSlideToggleHarness);
    await state.check();

    expect(emitter.emit).toHaveBeenCalledWith({
      roomId: room.id,
      lighting: expect.objectContaining({state: LightingState.On})
    })
  })

  function renderComponent({room = null, devices = [], lightingChange = new TestingEventEmitter<RoomLightingChangeModel>()}: Partial<RoomDetailComponent> = {}) {
    return renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: {room, devices, lightingChange}
    });
  }
});
