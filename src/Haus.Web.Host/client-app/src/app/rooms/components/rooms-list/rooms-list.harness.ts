import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomsListComponent} from "./rooms-list.component";
import {RoomsModule} from "../../rooms.module";

export class RoomsListHarness extends HausComponentHarness<RoomsListComponent> {
  get rooms() {
    return screen.queryAllByTestId('room-item');
  }

  async addRoom() {
    await this.clickButtonByTestId('add-room-btn');
  }

  static fromResult(result: RenderComponentResult<any>) {
    return new RoomsListHarness(result);
  }

  static async render(props: Partial<RoomsListComponent>) {
    const result = await renderFeatureComponent(RoomsListComponent, {
      imports: [RoomsModule],
      componentProperties: props
    });

    return new RoomsListHarness(result);
  }
}
