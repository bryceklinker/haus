import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

import {ModelFactory, renderFeatureComponent} from "../../../../testing";
import {DiscoveryRootComponent} from "./discovery-root.component";
import {DevicesModule} from "../../devices.module";
import {DiscoveryActions} from "../../../shared/discovery";
import {EventsActions} from "../../../shared/events";
import {RoomsActions} from "../../../rooms/state";
import {DevicesActions} from "../../state";
import {DiscoveryRoomComponent} from "../discovery-room/discovery-room.component";

describe('DiscoveryRootComponent', () => {
  it('should start discovery when rendered', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(DiscoveryActions.startDiscovery.request());
  })

  it('should load rooms when rendered', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.loadRooms.request());
  })

  it('should load devices when rendered', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(DevicesActions.loadDevices.request());
  })

  it('should stop discovery when destroyed', async () => {
    const {store, fixture} = await renderRoot();

    fixture.destroy();

    expect(store.dispatchedActions).toContainEqual(DiscoveryActions.stopDiscovery.request());
  })

  it('should show unassigned devices', async () => {
    await renderRoot(
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: undefined})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: 1})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: 4})}),
      EventsActions.deviceCreated({device: ModelFactory.createDeviceModel({roomId: undefined})}),
    );

    expect(screen.queryAllByTestId('unassigned-device-item')).toHaveLength(2);
  })

  it('should show all rooms', async () => {
    await renderRoot(
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
      EventsActions.roomCreated({room: ModelFactory.createRoomModel()}),
    );

    expect(screen.queryAllByTestId('discovery-room')).toHaveLength(3);
  })

  it('should show assigned devices', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel({roomId: room.id});
    await renderRoot(
      EventsActions.roomCreated({room}),
      EventsActions.deviceCreated({device})
    );

    expect(screen.queryAllByTestId('unassigned-device-item')).toHaveLength(0);
    expect(screen.getByTestId('discovery-room')).toHaveTextContent(device.name);
  })

  it('should notify when device is assigned to room', async () => {
    const room = ModelFactory.createRoomModel();
    const device = ModelFactory.createDeviceModel();
    const {triggerEventHandler, detectChanges, store} = await renderRoot(
      EventsActions.roomCreated({room}),
      EventsActions.deviceCreated({device})
    );

    triggerEventHandler(DiscoveryRoomComponent, 'assignDevice', {roomId: room.id, deviceIds: [device.id]});
    detectChanges();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({roomId: room.id, deviceIds: [device.id]}))
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(DiscoveryRootComponent, {
      imports: [DevicesModule],
      actions: actions
    })
  }
})
