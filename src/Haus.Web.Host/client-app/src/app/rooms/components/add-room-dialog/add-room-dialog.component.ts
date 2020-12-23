import {Component} from "@angular/core";
import {FormBuilder, Validators} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {UnsubscribingComponent} from "../../../shared/components/unsubscribing.component";
import {RoomsService} from "../../../shared/rooms";
import {Observable} from "rxjs";
import {map} from "rxjs/operators";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent extends UnsubscribingComponent {
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
    super();
  }

  onSave() {
    this.safeSubscribe(this.service.add(this.form.getRawValue()), () => this.matDialogRef.close());
  }

  onCancel() {
    this.matDialogRef.close();
  }
}
