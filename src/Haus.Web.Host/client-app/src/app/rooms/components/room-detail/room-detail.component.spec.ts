import {screen} from "@testing-library/dom";

import {RoomDetailComponent} from './room-detail.component';
import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";

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

  function renderComponent({room = null, devices = []}: Partial<RoomDetailComponent> = {}) {
    return renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: {room, devices}
    });
  }
});
