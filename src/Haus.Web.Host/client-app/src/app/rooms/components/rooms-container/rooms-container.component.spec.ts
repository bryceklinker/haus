import {Action} from "@ngrx/store";

import {ModelFactory, renderFeatureComponent, TestingActions} from "../../../../testing";
import {RoomsContainerComponent} from "./rooms-container.component";
import {RoomsModule} from "../../rooms.module";
import {ENTITY_NAMES} from "../../../entity-metadata";

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

  function renderContainer(...actions: Action[]){
    return renderFeatureComponent(RoomsContainerComponent, {
      imports: [RoomsModule],
      actions: [...actions]
    })
  }
})
