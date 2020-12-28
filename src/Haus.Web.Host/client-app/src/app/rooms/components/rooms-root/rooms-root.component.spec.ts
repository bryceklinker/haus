import {screen} from "@testing-library/dom";

import {
  eventually,
  ModelFactory,
  renderFeatureComponent,
  setupAllRoomsApis,
  setupGetAllRooms,
  TestingServer
} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {RoomsRootComponent} from "./rooms-root.component";
import userEvent from "@testing-library/user-event";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";

describe('RoomsRootComponent', () => {
  it('should get rooms when rendered', async () => {
    const rooms = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    ];
    setupGetAllRooms(rooms);

    const {detectChanges} = await renderRoot();

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('room-item')).toHaveLength(3);
    })
  })

  it('should open add dialog when add room clicked', async () => {
    setupAllRoomsApis();

    const {matDialog} = await renderRoot();

    userEvent.click(screen.getByTestId('add-room-btn'));

    await eventually(() => {
      expect(matDialog.open).toHaveBeenCalledWith(AddRoomDialogComponent);
    })
  })

  function renderRoot() {
    return renderFeatureComponent(RoomsRootComponent, {
      imports: [RoomsModule]
    })
  }
})
