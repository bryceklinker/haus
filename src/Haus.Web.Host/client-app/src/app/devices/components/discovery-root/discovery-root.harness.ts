import {screen} from "@testing-library/dom";
import {Action} from "@ngrx/store";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DiscoveryRootComponent} from "./discovery-root.component";
import {DevicesModule} from "../../devices.module";
import {DeviceModel} from "../../../shared/models";
import {DiscoveryRoomHarness} from "../discovery-room/discovery-room.harness";
import {DiscoveryUnassignedDevicesHarness} from "../discovery-unassigned-devices/discovery-unassigned-devices.harness";

export class DiscoveryRootHarness extends HausComponentHarness<DiscoveryRootComponent> {
  private _discoveryRoomHarness: DiscoveryRoomHarness;
  private _discoveryUnassignedDevicesHarness: DiscoveryUnassignedDevicesHarness;

  get unassignedDevices() {
    return this._discoveryUnassignedDevicesHarness.unassignedDevices;
  }

  get assignedDevices() {
    return this._discoveryRoomHarness.assignedDevices;
  }

  get rooms() {
    return screen.queryAllByLabelText('discovery room');
  }

  private constructor(result: RenderComponentResult<DiscoveryRootComponent>) {
    super(result);

    this._discoveryRoomHarness = DiscoveryRoomHarness.fromResult(result);
    this._discoveryUnassignedDevicesHarness = DiscoveryUnassignedDevicesHarness.fromResult(result);
  }

  async assignDevice(device: DeviceModel) {
    await this._discoveryRoomHarness.dropDeviceOnRoom(device);
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(DiscoveryRootComponent, {
      imports: [DevicesModule],
      actions: actions
    });

    return new DiscoveryRootHarness(result);
  }
}
