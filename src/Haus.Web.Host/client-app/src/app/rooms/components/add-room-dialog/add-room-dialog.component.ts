import {Component, OnDestroy} from "@angular/core";
import {FormBuilder, Validators} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {Observable} from "rxjs";
import {map, takeUntil} from "rxjs/operators";
import {RoomsService} from "../../../shared/rooms";
import {DestroyableSubject} from "../../../shared/destroyable-subject";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent implements OnDestroy {
  private readonly destroyable = new DestroyableSubject();

  get isLoading$(): Observable<boolean> {
    return this.service.isLoading$;
  }

  get isNotLoading$(): Observable<boolean> {
    return this.isLoading$.pipe(
      map(isLoading => !isLoading)
    )
  }

  form = new FormBuilder().group({
    name: ['', Validators.required]
  });

  constructor(private matDialogRef: MatDialogRef<AddRoomDialogComponent>,
              private service: RoomsService) {
  }

  onSave() {
    const room = this.form.getRawValue();
    this.destroyable.register(this.service.add(room))
      .subscribe(() => this.matDialogRef.close());
  }

  onCancel() {
    this.matDialogRef.close();
  }

  ngOnDestroy(): void {
    this.destroyable.destroy();
  }
}
