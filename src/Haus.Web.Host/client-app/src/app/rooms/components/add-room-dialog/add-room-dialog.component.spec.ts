import {eventually, renderFeatureComponent, TestingActions, TestingServer} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsModule} from "../../rooms.module";
import {ENTITY_NAMES} from "../../../entity-metadata";

describe('AddRoomDialogComponent', () => {
  it('should close dialog when adding room succeeds', async () => {
    TestingServer.setupPost('/api/rooms', {id: 1, name: 'three'});

    const {getByTestId, userEvent, detectChanges, matDialogRef, store} = await renderDialog();
    userEvent.type(getByTestId('room-name-field'), 'three');
    detectChanges();
    userEvent.click(getByTestId('save-room-btn'));

    await eventually(() => {
      expect(matDialogRef.close).toHaveBeenCalledWith();
      expect(store.actions).toContainEntityAction(TestingActions.addOne(ENTITY_NAMES.Room, {name: 'three'}))
    })
  })

  it('should disable save button when form is invalid', async () => {
    const {getByTestId} = await renderDialog();

    expect(getByTestId('save-room-btn')).toBeDisabled();
  })

  it('should close dialog when cancel is clicked', async () => {
    const {getByTestId, userEvent, matDialogRef} = await renderDialog();

    userEvent.click(getByTestId('cancel-room-btn'));

    expect(matDialogRef.close).toHaveBeenCalledWith();
  })



  function renderDialog() {
    return renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule]
    })
  }
})
