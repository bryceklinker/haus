import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";

import {HausComponentHarness, ModelFactory, renderFeatureComponent} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsActions} from "../../state";
import {RoomsModule} from "../../rooms.module";

export class AddRoomDialogHarness extends HausComponentHarness<AddRoomDialogComponent> {
  get loadingIndicator() {
    return screen.queryByTestId('loading-indicator');
  }

  get saveElement() {
    return screen.getByTestId('save-room-btn');
  }

  simulateAddSuccess() {
    this.actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));
  }

  async enterName(name: string) {
    await this.changeInputByTestId(name, 'room-name-field');
  }

  async save() {
    await this.clickButtonByTestId('save-room-btn');
  }

  async cancel() {
    await this.clickButtonByTestId('cancel-room-btn');
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    return new AddRoomDialogHarness(result);
  }
}
