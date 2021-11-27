import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomDetailComponent} from "./room-detail.component";
import {RoomsModule} from "../../rooms.module";

export class RoomDetailHarness extends HausComponentHarness<RoomDetailComponent> {
  get roomDetail() {
    return screen.getByLabelText('room detail');
  }

  get devices() {
    return screen.queryAllByLabelText('room device item');
  }

  get lighting() {
    return screen.queryByLabelText('lighting')
  }

  async turnRoomOn() {
    await this.toggleSlideByLabel('state');
  }

  async assignDevices() {
    await this.clickButtonByLabel('assign devices');
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
