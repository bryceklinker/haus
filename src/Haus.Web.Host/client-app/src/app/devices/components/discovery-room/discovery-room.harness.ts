import {CdkDropList} from "@angular/cdk/drag-drop";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {DiscoveryRoomComponent} from "./discovery-room.component";
import {DevicesModule} from "../../devices.module";
import {DeviceModel} from "../../../shared/models";
import {screen} from "@testing-library/dom";

export class DiscoveryRoomHarness extends HausComponentHarness<DiscoveryRoomComponent> {

  get room() {
    return this.container.querySelector(`[id="${this.componentInstance.roomId}"]`)
  }

  get assignedDevices() {
    return screen.queryAllByLabelText('assigned device item');
  }

  async dropDeviceOnRoom(device: DeviceModel) {
    await this.triggerEventHandler(CdkDropList, 'cdkDropListDropped', {item: {data: device}});
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new DiscoveryRoomHarness(result);
  }

  static async render(props: Partial<DiscoveryRoomComponent>) {
    const result = await renderFeatureComponent(DiscoveryRoomComponent, {
      imports: [DevicesModule],
      componentProperties: props
    });

    return new DiscoveryRoomHarness(result);
  }
}
