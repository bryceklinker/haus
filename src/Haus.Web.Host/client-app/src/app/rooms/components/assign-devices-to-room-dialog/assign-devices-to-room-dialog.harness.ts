import {HausComponentHarness, renderFeatureComponent} from "../../../../testing";
import {AssignDevicesToRoomDialogComponent} from "./assign-devices-to-room-dialog.component";
import {screen} from "@testing-library/dom";
import {DeviceModel, RoomModel} from "../../../shared/models";
import userEvent from "@testing-library/user-event";
import {Action} from "@ngrx/store";
import {RoomsModule} from "../../rooms.module";
import {RoomsActions} from "../../state";

export class AssignDevicesToRoomDialogHarness extends HausComponentHarness<AssignDevicesToRoomDialogComponent> {
  get unassignedDevices() {
    return screen.queryAllByTestId('unassigned-device-item');
  }

  get assignDevicesElement() {
    return screen.getByTestId('assign-devices-to-room-btn')
  }

  async selectDevice(device: DeviceModel) {
    userEvent.click(screen.getByText(new RegExp(device.name, 'g')));
    await this.whenRenderingDone();
  }

  async assignDevices() {
    await this.clickButtonByTestId('assign-devices-to-room-btn')
  }

  async cancel() {
    await this.clickButtonByTestId('cancel-assign-devices-btn');
  }

  simulateAssignDevicesSuccess(roomId: number) {
    this.actionsSubject.next(RoomsActions.assignDevicesToRoom.success({roomId, deviceIds: []}))
  }

  static async render(room: RoomModel, ...actions: Action[]) {
    const result = await renderFeatureComponent(AssignDevicesToRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions,
      dialogData: room,
    })
    return new AssignDevicesToRoomDialogHarness(result);
  }
}
