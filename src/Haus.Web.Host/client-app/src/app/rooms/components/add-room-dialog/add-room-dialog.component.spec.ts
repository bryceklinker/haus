import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {eventually, ModelFactory, renderFeatureComponent, setupAddRoom} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsModule} from "../../rooms.module";
import {Action} from "@ngrx/store";
import {RoomsActions} from "../../state";

describe('AddRoomDialogComponent', () => {
  it('should close dialog when adding room succeeds', async () => {
    const {actionsSubject, matDialogRef, detectChanges} = await renderAndSaveRoom('five');
    actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));

    await eventually(() => {
      detectChanges();
      expect(matDialogRef.close).toHaveBeenCalledWith();
    })
  })

  it('should disable save button when form is invalid', async () => {
    await renderDialog();

    expect(screen.getByTestId('save-room-btn')).toBeDisabled();
  })

  it('should close dialog when cancel is clicked', async () => {
    const {matDialogRef, store} = await renderDialog();

    userEvent.click(screen.getByTestId('cancel-room-btn'));

    expect(matDialogRef.close).toHaveBeenCalledWith();
    expect(store.dispatchedActions).toContainEqual(RoomsActions.addRoom.cancel());
  })

  it('should show loading when saving room takes a while', async () => {
    await renderDialog(RoomsActions.addRoom.request(ModelFactory.createRoomModel()));

    expect(await screen.findByTestId('loading-indicator')).toBeInTheDocument();
  })

  it('should stop listening for actions when destroyed', async () => {
    const {fixture, actionsSubject, matDialogRef} = await renderDialog()

    fixture.destroy();
    actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));

    expect(matDialogRef.close).not.toHaveBeenCalled();
  })

  async function renderAndSaveRoom(roomName: string) {
    const result = await renderDialog(RoomsActions.addRoom.begin());
    userEvent.type(screen.getByTestId('room-name-field'), 'three');
    result.detectChanges();
    userEvent.click(screen.getByTestId('save-room-btn'));
    return result;
  }

  function renderDialog(...actions: Array<Action>) {
    return renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions
    })
  }
})
