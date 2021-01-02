import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {Action} from "@ngrx/store";

import {eventually, ModelFactory, renderFeatureComponent} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsModule} from "../../rooms.module";
import {RoomsActions} from "../../state";

describe('AddRoomDialogComponent', () => {
  it('should notify to save room when room saved', async () => {
    const {store} = await renderAndSaveRoom('five');

    expect(store.dispatchedActions).toContainEqual(RoomsActions.addRoom.request({name: 'five'}));
  })

  it('should close dialog when add room succeeds', async () => {
    const {actionsSubject, matDialogRef} = await renderAndSaveRoom('three');

    actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));
    await eventually(() => {
      expect(matDialogRef.close).toHaveBeenCalled()
    })
  })

  it('should stop waiting for actions when destroyed', async () => {
    const {fixture, actionsSubject, matDialogRef} = await renderDialog();

    fixture.destroy();
    actionsSubject.next(RoomsActions.addRoom.success(ModelFactory.createRoomModel()));

    expect(matDialogRef.close).not.toHaveBeenCalled();
  })

  it('should disable save button when form is invalid', async () => {
    await renderDialog();

    expect(screen.getByTestId('save-room-btn')).toBeDisabled();
  })

  it('should close dialog when cancel is clicked', async () => {
    const {matDialogRef} = await renderDialog();

    userEvent.click(screen.getByTestId('cancel-room-btn'));

    expect(matDialogRef.close).toHaveBeenCalledWith();
  })

  it('should show loading when saving room takes a while', async () => {
    await renderDialog(RoomsActions.addRoom.request(ModelFactory.createRoomModel()));

    expect(await screen.findByTestId('loading-indicator')).toBeInTheDocument();
  })

  async function renderAndSaveRoom(roomName: string) {
    const result = await renderDialog();
    userEvent.type(screen.getByTestId('room-name-field'), roomName);
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
