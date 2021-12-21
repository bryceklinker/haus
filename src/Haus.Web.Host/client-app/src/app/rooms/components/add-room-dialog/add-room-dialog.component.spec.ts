import {eventually, ModelFactory} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsActions} from "../../state";
import {AddRoomDialogHarness} from "./add-room-dialog.harness";

describe('AddRoomDialogComponent', () => {
  test('should notify to save room when room saved', async () => {
    const harness = await AddRoomDialogHarness.render();

    await harness.enterName('five');
    await harness.save();

    expect(harness.dispatchedActions).toContainEqual(RoomsActions.addRoom.request({name: 'five'}));
  })

  test('should close dialog when add room succeeds', async () => {
    const harness = await AddRoomDialogHarness.render();

    await harness.enterName('three');
    await harness.save();
    harness.simulateAddSuccess();

    await eventually(() => {
      expect(harness.dialogRef.close).toHaveBeenCalled()
    })
  })

  test('should stop waiting for actions when destroyed', async () => {
    const harness = await AddRoomDialogHarness.render();

    harness.destroy();
    harness.simulateAddSuccess();

    expect(harness.dialogRef.close).not.toHaveBeenCalled();
  })

  test('should disable save button when form is invalid', async () => {
    const harness = await AddRoomDialogHarness.render();

    expect(harness.saveElement).toBeDisabled();
  })

  test('should close dialog when cancel is clicked', async () => {
    const harness = await AddRoomDialogHarness.render();

    await harness.cancel();

    expect(harness.dialogRef.close).toHaveBeenCalledWith();
  })

  test('should show loading when saving room takes a while', async () => {
    const harness = await AddRoomDialogHarness.render(
      RoomsActions.addRoom.request(ModelFactory.createRoomModel())
    );

    expect(harness.loadingIndicator).toBeInTheDocument();
  })
})
