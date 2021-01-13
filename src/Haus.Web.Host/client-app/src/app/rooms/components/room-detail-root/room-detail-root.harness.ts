import {Action} from "@ngrx/store";

import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {RoomDetailRootComponent} from "./room-detail-root.component";
import {RoomsModule} from "../../rooms.module";
import {screen} from "@testing-library/dom";
import {MatSlideToggleHarness} from "@angular/material/slide-toggle/testing";
import userEvent from "@testing-library/user-event";

export class RoomDetailRootHarness extends HausComponentHarness<RoomDetailRootComponent> {
  get devices() {
    return screen.queryAllByTestId('room-device-item');
  }

  async turnRoomOn() {
    const state = await this.getMatHarnessByTestId(MatSlideToggleHarness.with, 'state-input');
    await state.check();
  }

  async assignDevices() {
    userEvent.click(screen.getByTestId('assign-devices-btn'));
    await this.whenRenderingDone();
  }

  static async render(roomId?: number, ...actions: Action[]) {
    const result = await renderFeatureComponent(RoomDetailRootComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    if (roomId) {
      result.activatedRoute.triggerParamsChange({roomId: `${roomId}`});
      result.detectChanges();
      await result.fixture.whenRenderingDone();
    }

    return new RoomDetailRootHarness(result);
  }
}
