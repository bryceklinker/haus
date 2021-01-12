import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {DiscoveryRootComponent} from "./discovery-root.component";
import {Action} from "@ngrx/store";
import {DevicesModule} from "../../devices.module";
import {screen} from "@testing-library/dom";
import {DeviceModel, RoomModel} from "../../../shared/models";
import {DiscoveryRoomComponent} from "../discovery-room/discovery-room.component";

export class DiscoveryRootHarness extends HausComponentHarness<DiscoveryRootComponent> {
  get unassignedDevices() {
    return screen.queryAllByTestId('unassigned-device-item');
  }

  get assignedDevices() {
    return screen.queryAllByTestId('assigned-device-item');
  }

  get rooms() {
    return screen.queryAllByTestId('discovery-room');
  }

  async assignDevice(room: RoomModel, device: DeviceModel) {
    await this.triggerEventHandler(DiscoveryRoomComponent, 'assignDevice', {roomId: room.id, deviceIds: [device.id]});
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DiscoveryRootComponent, {
      imports: [DevicesModule],
      actions: actions
    });

    return new DiscoveryRootHarness(result);
  }
}
