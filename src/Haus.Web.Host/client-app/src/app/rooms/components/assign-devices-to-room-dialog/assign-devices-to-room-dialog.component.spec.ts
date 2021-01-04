import {Action} from "@ngrx/store";
import {screen} from "@testing-library/dom";
import userEvent from "@testing-library/user-event";

import {eventually, ModelFactory, renderFeatureComponent, TestingActionsSubject} from "../../../../testing";
import {AssignDevicesToRoomDialogComponent} from "./assign-devices-to-room-dialog.component";
import {RoomsModule} from "../../rooms.module";
import {DevicesActions} from "../../../devices/state";
import {AppState} from "../../../app.state";
import {TestingStore} from "../../../../testing/fakes/testing-store";
import {RoomsActions} from "../../state";
import {TestingMatDialogRef} from "../../../../testing/fakes/testing-mat-dialog-ref";
import {DeviceModel, RoomModel} from "../../../shared/models";

describe('AssignDevicesToRoomDialogComponent', () => {
  let unassignedDevices: Array<DeviceModel>;
  let loadDevicesSuccessAction: Action;
  let detectChanges: () => void;
  let store: TestingStore<AppState>;
  let room: RoomModel;
  let dialogRef: TestingMatDialogRef;
  let actionsSubject: TestingActionsSubject;

  beforeEach(async () => {
    room = ModelFactory.createRoomModel();
    unassignedDevices = [
      ModelFactory.createDeviceModel({roomId: undefined, name: 'one'}),
      ModelFactory.createDeviceModel({roomId: undefined, name: 'two'}),
      ModelFactory.createDeviceModel({roomId: undefined, name: 'three'}),
    ]
    const devices = [
      ModelFactory.createDeviceModel({roomId: 1}),
      ...unassignedDevices,
      ModelFactory.createDeviceModel({roomId: 4}),
    ];

    loadDevicesSuccessAction = DevicesActions.loadDevices.success(ModelFactory.createListResult(...devices));
    const result = await renderDialog(loadDevicesSuccessAction);
    detectChanges = result.detectChanges;
    store = result.store;
    dialogRef = result.matDialogRef;
    actionsSubject = result.actionsSubject;
  })

  it('should show all unassigned devices', () => {
    expect(screen.queryAllByTestId('unassigned-device-item')).toHaveLength(3);
  })

  it('should disallow assignment when no devices have been selected', () => {
    expect(screen.getByTestId('assign-devices-to-room-btn')).toBeDisabled();
  })

  it('should request to assign devices to room when assign triggered', () => {
    selectDevice(unassignedDevices[0]);

    assignDevices();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({
      roomId: room.id,
      deviceIds: [unassignedDevices[0].id]
    }))
  })

  it('should only add devices that have been selected when assigning devices is triggered', () => {
    selectDevice(unassignedDevices[1]);
    selectDevice(unassignedDevices[0]);
    selectDevice(unassignedDevices[1]);
    selectDevice(unassignedDevices[2]);

    assignDevices();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.assignDevicesToRoom.request({
      roomId: room.id,
      deviceIds: [unassignedDevices[0].id, unassignedDevices[2].id]
    }))
  })

  it('should close dialog when assignment is successful', async () => {
    actionsSubject.next(RoomsActions.assignDevicesToRoom.success({roomId: 54, deviceIds: []}));

    await eventually(() => {
      expect(dialogRef.close).toHaveBeenCalled();
    })
  })

  it('should close dialog when assignment is cancelled', () => {
    userEvent.click(screen.getByTestId('cancel-assign-devices-btn'));

    expect(dialogRef.close).toHaveBeenCalled();
  })

  function renderDialog(...actions: Array<Action>) {
    return renderFeatureComponent(AssignDevicesToRoomDialogComponent, {
      imports: [RoomsModule],
      actions: actions,
      dialogData: room,
    })
  }

  function selectDevice(device: DeviceModel) {
    userEvent.click(screen.getByText(new RegExp(device.name, 'g')));
    detectChanges();
  }

  function assignDevices() {
    userEvent.click(screen.getByTestId('assign-devices-to-room-btn'));
    detectChanges();
  }
})
