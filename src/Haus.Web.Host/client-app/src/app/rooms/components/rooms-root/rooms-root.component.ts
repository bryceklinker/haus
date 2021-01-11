import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";
import {Store} from "@ngrx/store";
import {MatDialog} from "@angular/material/dialog";

import {AppState} from "../../../app.state";
import {RoomsActions, selectAllRooms} from "../../state";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";
import {RoomModel} from "../../../shared/models";

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
