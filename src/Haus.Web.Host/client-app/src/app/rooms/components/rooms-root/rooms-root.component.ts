import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";

import {RoomModel} from "../../models";
import {AppState} from "../../../app.state";
import {RoomsActions, selectAllRooms} from "../../state";
import {Store} from "@ngrx/store";
import {MatDialog} from "@angular/material/dialog";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";

@Component({
  selector: 'rooms-root',
  templateUrl: './rooms-root.component.html',
  styleUrls: ['./rooms-root.component.scss']
})
export class RoomsRootComponent implements OnInit {
  rooms$: Observable<RoomModel[]>;

  constructor(private store: Store<AppState>,
              private readonly dialog: MatDialog) {
    this.rooms$ = this.store.select(selectAllRooms);
  }

  ngOnInit(): void {
    this.store.dispatch(RoomsActions.loadRooms.request());
  }

  onAddRoom() {
    this.dialog.open(AddRoomDialogComponent);
  }
}
