import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";

import {RoomModel} from "../../../shared/rooms";
import {AppState} from "../../../app.state";
import {RoomsActions, selectAllRooms} from "../../state";
import {Store} from "@ngrx/store";

@Component({
  selector: 'rooms-root',
  templateUrl: './rooms-root.component.html',
  styleUrls: ['./rooms-root.component.scss']
})
export class RoomsRootComponent implements OnInit {
  rooms$: Observable<RoomModel[]>;

  constructor(private store: Store<AppState>) {
    this.rooms$ = this.store.select(selectAllRooms);
  }

  ngOnInit(): void {
    this.store.dispatch(RoomsActions.loadRooms.request());
  }

  onAddRoom() {
    this.store.dispatch(RoomsActions.addRoom.begin());
  }
}
