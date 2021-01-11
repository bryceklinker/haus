import {CdkDropList} from "@angular/cdk/drag-drop";

import {DiscoveryRoomComponent} from "./discovery-room.component";
import {ModelFactory, renderFeatureComponent, TestingEventEmitter} from "../../../../testing";
import {DevicesModule} from "../../devices.module";
import {DevicesAssignedToRoomEvent} from "../../../shared/models";

describe('DiscoveryRoomComponent', () => {
  it('should show room name', async () => {
    const room = ModelFactory.createRoomModel();

    const {container} = await renderRoom({room});

    expect(container).toHaveTextContent(room.name);
  })

  it('should set the id of drop zone', async () => {
    const room = ModelFactory.createRoomModel();

    const {container} = await renderRoom({room});

    expect(container.querySelector(`[id="${room.id}"]`)).toBeInTheDocument();
  })

  it('should show devices in room', async () => {
    const room = ModelFactory.createRoomModel();
    const devices = [
      ModelFactory.createDeviceModel({roomId: room.id}),
      ModelFactory.createDeviceModel({roomId: undefined}),
      ModelFactory.createDeviceModel({roomId: room.id}),
    ];

    const {container} = await renderRoom({room, devices});

    expect(container).toHaveTextContent(devices[0].name);
    expect(container).toHaveTextContent(devices[2].name);
    expect(container).not.toHaveTextContent(devices[1].name);
  })

  it('should notify when device is assigned to room', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel();
    const eventEmitter = new TestingEventEmitter<DevicesAssignedToRoomEvent>()

    const {triggerEventHandler, detectChanges} = await renderRoom({room, assignDevice: eventEmitter});
    triggerEventHandler(CdkDropList, 'cdkDropListDropped', {item: {data: device}});
    detectChanges();

    expect(eventEmitter.emit).toHaveBeenCalledWith({roomId: room.id, deviceIds: [device.id]});
  })

  function renderRoom(props: Partial<DiscoveryRoomComponent> = {}) {
    return renderFeatureComponent(DiscoveryRoomComponent, {
      imports: [DevicesModule],
      componentProperties: props
    })
  }
})
