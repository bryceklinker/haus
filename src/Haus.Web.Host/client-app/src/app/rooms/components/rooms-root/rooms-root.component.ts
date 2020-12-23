import {Component, OnDestroy, OnInit} from "@angular/core";
import {Observable} from "rxjs";

import {RoomModel, RoomsService} from "../../../shared/rooms";
import {MatDialog} from "@angular/material/dialog";
import {AddRoomDialogComponent} from "../add-room-dialog/add-room-dialog.component";
import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {takeUntil} from "rxjs/operators";

@Component({
  selector: 'rooms-root',
  templateUrl: './rooms-root.component.html',
  styleUrls: ['./rooms-root.component.scss']
})
export class RoomsRootComponent implements OnInit, OnDestroy {
  private readonly destroyable = new DestroyableSubject();
  rooms$: Observable<RoomModel[]>;

  constructor(private service: RoomsService,
              private dialog: MatDialog) {
    this.rooms$ = service.rooms$;
  }

  ngOnInit(): void {
    this.destroyable.register(this.service.getAll()).subscribe();
  }

  onAddRoom() {
    this.dialog.open(AddRoomDialogComponent);
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
