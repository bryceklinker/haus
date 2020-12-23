import {Component, OnInit} from "@angular/core";
import {Observable} from "rxjs";

import {RoomModel, RoomsService} from "../../../shared/rooms";
import {MatDialog} from "@angular/material/dialog";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";

@Component({
  selector: 'rooms-root',
  templateUrl: './rooms-root.component.html',
  styleUrls: ['./rooms-root.component.scss']
})
export class RoomsRootComponent implements OnInit {
  rooms$: Observable<RoomModel[]>;

  constructor(private service: RoomsService,
              private dialog: MatDialog) {
    this.rooms$ = service.rooms$;
  }

  ngOnInit(): void {
    this.service.getAll();
  }

  onAddRoom() {
    this.dialog.open(AddRoomDialogComponent);
  }
}
