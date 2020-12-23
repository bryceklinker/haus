import {screen} from "@testing-library/dom";

import {eventually, ModelFactory, renderFeatureComponent, TestingServer} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {RoomsRootComponent} from "./rooms-root.component";
import userEvent from "@testing-library/user-event";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";

describe('RoomsRootComponent', () => {
  it('should get rooms when rendered', async () => {
    const rooms = ModelFactory.createListResult(
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    );
    TestingServer.setupGet('/api/rooms', rooms);

    const {detectChanges} = await renderRoot();

    await eventually(() => {
      detectChanges();
      expect(screen.queryAllByTestId('room-item')).toHaveLength(3);
    })
  })

  it('should open add dialog when add room clicked', async () => {
    TestingServer.setupRoomsEndpoints();

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
