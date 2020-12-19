import {Action} from "@ngrx/store";

import {ModelFactory, renderFeatureComponent, TestingActions} from "../../../../testing";
import {RoomsContainerComponent} from "./rooms-container.component";
import {RoomsModule} from "../../rooms.module";
import {ENTITY_NAMES} from "../../../entity-metadata";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";

describe('RoomsContainerComponent', () => {
  it('should show each room in list', async () => {
    const queryAllAction = TestingActions.createQueryAllSuccess(ENTITY_NAMES.Room, [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    ])

    const {queryAllByTestId} = await renderContainer(queryAllAction);

    expect(queryAllByTestId('room-item')).toHaveLength(3);
  })

  it('should get rooms when rendered', async () => {
    const {store} = await renderContainer();

    expect(store.actions).toContainEntityAction(TestingActions.createQueryAll(ENTITY_NAMES.Room));
  })

  it('should open add room dialog', async () => {
    const {getByTestId, fireEvent, matDialog} = await renderContainer();

    fireEvent.click(getByTestId('add-room-btn'));

    expect(matDialog.open).toHaveBeenCalledWith(AddRoomDialogComponent);
  })

  function renderContainer(...actions: Action[]){
    return renderFeatureComponent(RoomsContainerComponent, {
      imports: [RoomsModule],
      actions: [...actions]
    })
  }
})
