import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {RoomsRootComponent} from "./rooms-root.component";
import {Action} from "@ngrx/store";
import {RoomsModule} from "../../rooms.module";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

export class RoomsRootHarness extends HausComponentHarness<RoomsRootComponent> {
  get rooms() {
      return screen.queryAllByTestId('room-item');
  }

  async addRoom() {
    userEvent.click(screen.getByTestId('add-room-btn'));
    await this.whenRenderingDone();
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(RoomsRootComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    return new RoomsRootHarness(result);
  }
}
