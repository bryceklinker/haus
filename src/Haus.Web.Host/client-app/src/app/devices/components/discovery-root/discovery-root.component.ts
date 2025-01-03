import {Component, OnDestroy, OnInit} from "@angular/core";
import {Store} from "@ngrx/store";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";

import {AppState} from "../../../app.state";
import {DiscoveryActions} from "../../../shared/discovery";
import {DeviceModel, DevicesAssignedToRoomEvent, RoomModel} from "../../../shared/models";
import {DevicesActions, selectAssignedDevices, selectUnassignedDevices} from "../../state";
import {RoomsActions, selectAllRooms} from "../../../rooms/state";

@Component({
  selector: 'discovery-root',
  templateUrl: './discovery-root.component.html',
  styleUrls: ['./discovery-root.component.scss']
})
export class DiscoveryRootComponent implements OnInit, OnDestroy {
  unassignedDevices$: Observable<Array<DeviceModel>>;
  assignedDevices$: Observable<Array<DeviceModel>>;
  rooms$: Observable<Array<RoomModel>>;
  roomIds$: Observable<Array<number>>;

  constructor(private readonly store: Store<AppState>) {
    this.rooms$ = store.select(selectAllRooms);
    this.roomIds$ = this.rooms$.pipe(
      map(rooms => rooms.map(r => r.id))
    );

    this.assignedDevices$ = store.select(selectAssignedDevices)
    this.unassignedDevices$ = store.select(selectUnassignedDevices);
  }

  ngOnInit(): void {
    this.store.dispatch(RoomsActions.loadRooms.request());
    this.store.dispatch(DevicesActions.loadDevices.request());
    this.store.dispatch(DiscoveryActions.startDiscovery.request());
  }

  ngOnDestroy(): void {
    this.store.dispatch(DiscoveryActions.stopDiscovery.request());
  }

  onDeviceAssignedToRoom($event: DevicesAssignedToRoomEvent) {
    this.store.dispatch(RoomsActions.assignDevicesToRoom.request($event));
  }
}
