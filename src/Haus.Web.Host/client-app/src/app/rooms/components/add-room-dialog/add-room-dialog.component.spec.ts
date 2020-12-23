import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";
import {eventually, ModelFactory, renderFeatureComponent, TestingServer} from "../../../../testing";
import {AddRoomDialogComponent} from "./add-room-dialog.component";
import {RoomsModule} from "../../rooms.module";

describe('AddRoomDialogComponent', () => {
  it('should close dialog when adding room succeeds', async () => {
    TestingServer.setupPost('/api/rooms', {id: 1, name: 'three'});

    const {matDialogRef} = await renderAndSaveRoom('three');

    await eventually(() => {
      expect(matDialogRef.close).toHaveBeenCalledWith();
    })
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
    TestingServer.setupPost('/api/rooms', ModelFactory.createRoomModel(), {delay: 1000});

    const {detectChanges} = await renderAndSaveRoom('something');

    await eventually(async () => {
      detectChanges();
      expect(await screen.findByTestId('loading-indicator')).toBeInTheDocument();
    })
  })

  async function renderAndSaveRoom(roomName: string) {
    const result = await renderDialog();
    userEvent.type(screen.getByTestId('room-name-field'), 'three');
    result.detectChanges();
    userEvent.click(screen.getByTestId('save-room-btn'));
    return result;
  }

  function renderDialog() {
    return renderFeatureComponent(AddRoomDialogComponent, {
      imports: [RoomsModule]
    })
  }
})
