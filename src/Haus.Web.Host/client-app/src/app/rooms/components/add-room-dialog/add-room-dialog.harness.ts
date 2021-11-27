import {Action} from '@ngrx/store';
import {screen} from '@testing-library/dom';

import {HausComponentHarness, ModelFactory, renderFeatureComponent} from '../../../../testing';
import {AddRoomDialogComponent} from './add-room-dialog.component';
import {RoomsActions} from '../../state';
import {RoomsModule} from '../../rooms.module';

export class AddRoomDialogHarness extends HausComponentHarness<AddRoomDialogComponent> {
  get loadingIndicator() {
    return screen.queryByLabelText('loading indicator');
  }

  get saveElement() {
    return screen.getByRole('button', {name: 'save room'});
  }

  simulateAddSuccess() {
    this.actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));
  }

  async enterName(name: string) {
    await this.changeInputByLabel(name, 'room name');
  }

  async save() {
    await this.clickButtonByLabel('save room');
  }

  async cancel() {
    await this.clickButtonByLabel('cancel room');
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    return new AddRoomDialogHarness(result);
  }
}
