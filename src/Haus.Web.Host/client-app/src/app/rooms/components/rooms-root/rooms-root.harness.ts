import {Action} from "@ngrx/store";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomsRootComponent} from "./rooms-root.component";
import {RoomsModule} from "../../rooms.module";
import {RoomsListHarness} from "../rooms-list/rooms-list.harness";

export class RoomsRootHarness extends HausComponentHarness<RoomsRootComponent> {
  private _roomsListHarness: RoomsListHarness;

  get rooms() {
      return this._roomsListHarness.rooms;
  }

  private constructor(result: RenderComponentResult<RoomsRootComponent>) {
    super(result);

    this._roomsListHarness = RoomsListHarness.fromResult(result);
  }

  async addRoom() {
    await this._roomsListHarness.addRoom();
  }

  static async render(...actions: Action[]) {
    const result = await renderFeatureComponent(RoomsRootComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    return new RoomsRootHarness(result);
  }
}
