import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {RoomDetailComponent} from "./room-detail.component";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {RoomsModule} from "../../rooms.module";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";

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
    const lightingState = await this.getMatHarnessByTestId(MatSlideToggleHarness.with, 'state-input');
    await lightingState.check();
  }

  async assignDevices() {
    userEvent.click(screen.getByTestId('assign-devices-btn'));
    await this.whenRenderingDone();
  }

  static async render(props: Partial<RoomDetailComponent>) {
    const result = await renderFeatureComponent(RoomDetailComponent, {
      imports: [RoomsModule],
      componentProperties: props
    });

    return new RoomDetailHarness(result);
  }
}
