import {Action} from "@ngrx/store";

import {HausComponentHarness, RenderComponentResult, renderFeatureComponent} from "../../../../testing";
import {RoomDetailRootComponent} from "./room-detail-root.component";
import {RoomsModule} from "../../rooms.module";
import {RoomDetailHarness} from "../room-detail/room-detail.harness";

export class RoomDetailRootHarness extends HausComponentHarness<RoomDetailRootComponent> {
  private _roomDetailHarness: RoomDetailHarness;

  get devices() {
    return this._roomDetailHarness.devices;
  }

  private constructor(result: RenderComponentResult<RoomDetailRootComponent>) {
    super(result);

    this._roomDetailHarness = RoomDetailHarness.fromResult(result);
  }

  async turnRoomOn() {
    await this._roomDetailHarness.turnRoomOn();
  }

  async assignDevices() {
    await this._roomDetailHarness.assignDevices();
  }

  static async render(roomId?: number, ...actions: Action[]) {
    const result = await renderFeatureComponent(RoomDetailRootComponent, {
      imports: [RoomsModule],
      actions: actions
    });

    if (roomId) {
      result.activatedRoute.triggerParamsChange({roomId: `${roomId}`});
      result.detectChanges();
      await result.fixture.whenRenderingDone();
    }

    return new RoomDetailRootHarness(result);
  }
}
