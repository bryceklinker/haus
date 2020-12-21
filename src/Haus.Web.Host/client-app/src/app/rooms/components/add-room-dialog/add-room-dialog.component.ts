import {Component, OnDestroy} from "@angular/core";
import {EntityCollectionService, EntityCollectionServiceFactory} from "@ngrx/data";
import {RoomModel} from "../../models/room.model";
import {ENTITY_NAMES} from "../../../entity-metadata";
import {FormBuilder, Validators} from "@angular/forms";
import {MatDialogRef} from "@angular/material/dialog";
import {UnsubscribingComponent} from "../../../shared/components/unsubscribing.component";

@Component({
  selector: 'add-room-dialog',
  templateUrl: './add-room-dialog.component.html',
  styleUrls: ['./add-room-dialog.component.scss']
})
export class AddRoomDialogComponent extends UnsubscribingComponent {
  private readonly service: EntityCollectionService<RoomModel>
  form = new FormBuilder().group({
    name: ['', Validators.required]
  });

  constructor(private factory: EntityCollectionServiceFactory,
              private matDialogRef: MatDialogRef<AddRoomDialogComponent>) {
    super();

    this.service = factory.create(ENTITY_NAMES.Room);
  }

  onSave() {
    this.safeSubscribe(this.service.add(this.form.getRawValue()), () => {
      this.matDialogRef.close()
    });
  }

  onCancel() {
    this.matDialogRef.close();
  }
}
