import {screen} from "@testing-library/dom";

import {
  ModelFactory,
  renderFeatureComponent,
} from "../../../../testing";
import {RoomsModule} from "../../rooms.module";
import {RoomsRootComponent} from "./rooms-root.component";
import userEvent from "@testing-library/user-event";
import {Action} from "@ngrx/store";
import {RoomsActions} from "../../state";

describe('RoomsRootComponent', () => {
  it('should load rooms when rendered', async () => {
    const {store} = await renderRoot();

    expect(store.dispatchedActions).toContainEqual(RoomsActions.loadRooms.request());
  })

  it('should show rooms when rendered', async () => {
    const rooms = [
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel(),
      ModelFactory.createRoomModel()
    ];

    await renderRoot(RoomsActions.loadRooms.success(ModelFactory.createListResult(...rooms)));

    expect(screen.queryAllByTestId('room-item')).toHaveLength(3);
  })

  it('should open add dialog when add room clicked', async () => {
    const {store} = await renderRoot();

    userEvent.click(screen.getByTestId('add-room-btn'));

    expect(store.dispatchedActions).toContainEqual(RoomsActions.addRoom.begin());
  })

  function renderRoot(...actions: Array<Action>) {
    return renderFeatureComponent(RoomsRootComponent, {
      imports: [RoomsModule],
      actions: actions
    })
  }
})
