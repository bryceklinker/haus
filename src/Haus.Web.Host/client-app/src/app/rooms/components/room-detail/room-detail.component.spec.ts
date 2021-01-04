import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";

import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {LightingState, RoomLightingChangedEvent, RoomModel} from "../../../shared/models";

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

  it('should show each device\'s info', async () => {
    const devices = [
      ModelFactory.createDeviceModel({name: 'bob', deviceType: 'Light'})
    ];

    const {container} = await renderComponent({devices});

    expect(container).toHaveTextContent(/bob/g);
    expect(container).toHaveTextContent(/Light/g);
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
    const emitter = new TestingEventEmitter<RoomLightingChangedEvent>();

    const {matHarness} = await renderComponent({room, lightingChange: emitter});
    const state = await matHarness.getHarness(MatSlideToggleHarness);
    await state.check();

    expect(emitter.emit).toHaveBeenCalledWith({
      room: room,
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
                             lightingChange = new TestingEventEmitter<RoomLightingChangedEvent>(),
                             assignDevices = new TestingEventEmitter<RoomModel>()
                           }: Partial<RoomDetailComponent> = {}) {
    return renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: {room, devices, lightingChange, assignDevices}
    });
  }
});
