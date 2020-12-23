import {Component} from "@angular/core";
import {FormBuilder, Validators} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";
import {RoomsService} from "../../../shared/rooms";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent {
  get isLoading$():Observable<boolean> {
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
    this.service.add(room).subscribe(() => this.matDialogRef.close());
  }

  onCancel() {
    this.matDialogRef.close();
  }
}
