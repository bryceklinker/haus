import {Component, OnDestroy, OnInit} from "@angular/core";
import {FormBuilder, Validators} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {ofType} from "@ngrx/effects";
import {ActionsSubject, Store} from "@ngrx/store";

import {DestroyableSubject} from "../../../shared/destroyable-subject";
import {AppState} from "../../../app.state";
import {RoomsActions} from "../../state";
import {selectIsLoading} from "../../../shared/loading/loading.reducer";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent implements OnInit, OnDestroy {
  private readonly destroyable: DestroyableSubject;
  isLoading$: Observable<boolean>;

  get isNotLoading$(): Observable<boolean> {
    return this.isLoading$.pipe(
      map(isLoading => !isLoading)
    )
  }

  form = new FormBuilder().group({
    name: ['', Validators.required]
  });

  constructor(private matDialogRef: MatDialogRef<AddRoomDialogComponent>,
              private store: Store<AppState>,
              private actionsSubject: ActionsSubject) {
    this.destroyable = new DestroyableSubject();
    this.isLoading$ = this.store.select(selectIsLoading(RoomsActions.addRoom.request));
  }

  onSave() {
    const room = this.form.getRawValue();
    this.store.dispatch(RoomsActions.addRoom.request(room));
  }

  onCancel() {
    this.store.dispatch(RoomsActions.addRoom.cancel());
    this.matDialogRef.close();
  }

  ngOnInit(): void {
    this.destroyable.register(this.actionsSubject.pipe(ofType(RoomsActions.addRoom.success)))
      .subscribe(() => this.matDialogRef.close());
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
