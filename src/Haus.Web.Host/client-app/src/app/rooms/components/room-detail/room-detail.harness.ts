import {screen} from "@testing-library/dom";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomDetailComponent} from "./room-detail.component";
import {RoomsModule} from "../../rooms.module";
import userEvent from '@testing-library/user-event';

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

  turnRoomOn() {
    this.toggleSlideByLabel('state');
  }

  assignDevices() {
    this.clickButtonByLabel('assign devices');
  }

  enterOccupancyTimeout(timeout: number) {
    userEvent.clear(screen.getByRole('spinbutton', {name: 'occupancy timeout'}));
    userEvent.type(screen.getByRole('spinbutton', {name: 'occupancy timeout'}), `${timeout}`);
  }

  enterRoomName(name: string) {
    userEvent.clear(screen.getByRole('textbox', {name: 'room name'}));
    userEvent.type(screen.getByRole('textbox', {name: 'room name'}), name);
  }

  saveRoom() {
    userEvent.click(screen.getByRole('button', {name: 'save room'}));
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
