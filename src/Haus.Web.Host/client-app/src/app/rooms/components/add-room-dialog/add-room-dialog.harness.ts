import {HausComponentHarness, ModelFactory, renderFeatureComponent} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsActions} from "../../state";
import {Action} from "@ngrx/store";
import {RoomsModule} from "../../rooms.module";
import userEvent from "@testing-library/user-event";
import {screen} from "@testing-library/dom";

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
    userEvent.type(screen.getByTestId('room-name-field'), name);
    await this.whenRenderingDone();
  }

  async save() {
    userEvent.click(screen.getByTestId('save-room-btn'));
    await this.whenRenderingDone();
  }

  async cancel() {
    userEvent.click(screen.getByTestId('cancel-room-btn'));
    await this.whenRenderingDone();
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    return new AddRoomDialogHarness(result);
  }
}
