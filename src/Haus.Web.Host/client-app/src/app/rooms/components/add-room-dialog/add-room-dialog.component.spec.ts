import {renderFeatureComponent, TestingActions} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsModule} from "../../rooms.module";
import {ENTITY_NAMES} from "../../../entity-metadata";

describe('AddRoomDialogComponent', () => {
  it('should add room when dialog is submitted', async () => {
    const {getByTestId, userEvent, store, detectChanges} = await renderDialog();

    userEvent.type(getByTestId('room-name-field'), 'three');
    detectChanges();

    userEvent.click(getByTestId('save-room-btn'));

    expect(store.actions).toContainEntityAction(TestingActions.addOne(ENTITY_NAMES.Room, {name: 'three'}))
  })

  it('should disable save button when form is invalid', async () => {
    const {getByTestId} = await renderDialog();

    expect(getByTestId('save-room-btn')).toBeDisabled();
  })

  function renderDialog() {
    return renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule]
    })
  }
})
