import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {RoomsListComponent} from "./rooms-list.component";
import {RoomsModule} from "../../rooms.module";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

export class RoomsListHarness extends HausComponentHarness<RoomsListComponent> {
  get rooms() {
    return screen.queryAllByTestId('room-item');
  }

  async addRoom() {
    userEvent.click(screen.getByTestId('add-room-btn'));
    await this.whenRenderingDone();
  }

  static async render(props: Partial<RoomsListComponent>) {
    const result = await renderFeatureComponent(RoomsListComponent, {
      imports: [RoomsModule],
      componentProperties: props
    });

    return new RoomsListHarness(result);
  }
}
