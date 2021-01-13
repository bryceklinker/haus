import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomDetailComponent} from "./room-detail.component";
import {RoomsModule} from "../../rooms.module";

export class RoomDetailHarness extends HausComponentHarness<RoomDetailComponent> {
  get roomDetail() {
    return screen.getByTestId('room-detail');
  }

  get devices() {
    return screen.queryAllByTestId('room-device-item');
  }

  get lighting() {
    return screen.queryByTestId('lighting')
  }

  async turnRoomOn() {
    await this.checkSlideToggleByTestId('state-input');
  }

  async assignDevices() {
    await this.clickButtonByTestId('assign-devices-btn');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new RoomDetailHarness(result);
  }

  static async render(props: Partial<RoomDetailComponent>) {
    const result = await renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: props
    });

    return new RoomDetailHarness(result);
  }
}
